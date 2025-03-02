using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject prefabToInstantiate;
    private GameObject dragPreview;
    private Environment2D loadedEnvironment;
    private EnvironmentSystem environmentSystem;

    void Start()
    {
        environmentSystem = FindFirstObjectByType<EnvironmentSystem>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        loadedEnvironment = GameManager.Instance.SelectedEnvironment;

        // Create a semi-transparent preview object
        dragPreview = Instantiate(prefabToInstantiate);
        dragPreview.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        UpdatePosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragPreview != null)
        {
            UpdatePosition(eventData);
        }
    }

    public async void OnEndDrag(PointerEventData eventData)
    {
        if (dragPreview != null)
        {
            Vector3 finalPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            finalPosition.z = 0; // Keep it on the same layer

            // Prepare object data for API
            Object2D newObjectData = new()
            {
                environmentId = loadedEnvironment.id,
                positionX = finalPosition.x,
                positionY = finalPosition.y,
                scaleX = prefabToInstantiate.transform.localScale.x,
                scaleY = prefabToInstantiate.transform.localScale.y,
                prefabId = prefabToInstantiate.name
            };

            // Call API to create the object and wait for response
            Object2D createdObject = await environmentSystem.CreateObject2D(newObjectData);

            if (createdObject != null)
            {
                // Spawn real object after getting ID
                environmentSystem.SpawnObject2D(createdObject);
            }
            else
            {
                Debug.LogError("Failed to create object on server.");
            }

            // Remove preview object
            Destroy(dragPreview);
        }
    }

    private void UpdatePosition(PointerEventData eventData)
    {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(eventData.position);
        newPos.z = 0;
        dragPreview.transform.position = newPos;
    }
}
