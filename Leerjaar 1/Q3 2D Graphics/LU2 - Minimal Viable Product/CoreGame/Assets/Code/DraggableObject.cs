using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private EnvironmentSystem environmentSystem;
    private Object2D data;
    private Vector3 initialPosition;
    private bool isDragging = false;
    private bool isSelected = false;
    private Outline outlineEffect;

    void Start()
    {
        // Ensure the physics raycaster is attached to the main camera for 2D raycasting
        if (Camera.main.GetComponent<Physics2DRaycaster>() == null)
        {
            Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
        }

        // Ensure outline effect is present
        outlineEffect = GetComponent<Outline>();
        if (outlineEffect != null)
        {
            outlineEffect.enabled = false; // Start with outline disabled
        }
        else
        {
            Debug.LogWarning("Outline component is missing on " + gameObject.name);
        }
    }

    public void Initialize(Object2D data)
    {
        this.data = data;
        environmentSystem = FindFirstObjectByType<EnvironmentSystem>();
    }

    void Update()
    {
        if (isSelected && Input.GetKeyDown(KeyCode.Delete))
        {
            Debug.Log("Deleting selected object with ID: " + data.id);
            DeleteObject();
        }
    }

    #region UpdateObject

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (data.id == null)
        {
            Debug.LogWarning("Cannot drag an object that has no ID assigned.");
            return;
        }

        initialPosition = transform.position;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 newPos = Camera.main.ScreenToWorldPoint(eventData.position);
        newPos.z = 0; // Ensure the object stays in 2D space

        // Add raycast to avoid overlapping UI elements blocking drag
        RaycastHit2D hit = Physics2D.Raycast(newPos, Vector2.zero);
        if (hit.collider == null || hit.collider.gameObject == gameObject)
        {
            transform.position = newPos;
        }
    }

    public async void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;

        Vector3 finalPosition = transform.position;

        // Prepare object update data
        Object2D updatedData = new Object2D
        {
            id = data.id,
            environmentId = data.environmentId,
            positionX = finalPosition.x,
            positionY = finalPosition.y,
            scaleX = transform.localScale.x,
            scaleY = transform.localScale.y,
            prefabId = data.prefabId
        };

        // Call API to validate position
        Object2D validatedData = await environmentSystem.UpdateObject2D(updatedData);

        if (validatedData != null)
        {
            // Apply server-validated position
            transform.position = new Vector3(validatedData.positionX, validatedData.positionY, 0);
            Debug.Log("Updated object with id: " + data.id);
        }
        else
        {
            // Revert to initial position if update fails
            Debug.LogError("Failed to update object on server. Reverting position.");
            transform.position = initialPosition;
        }
    }

    #endregion

    #region DeleteObject

    public void OnPointerClick(PointerEventData eventData)
    {
        // Deselect all objects before selecting the clicked one
        DeselectAllObjects();

        // Toggle selection state and update visual selection
        isSelected = !isSelected;

        UpdateVisualSelection();
    }

    private void DeselectAllObjects()
    {
        // Deselect all draggable objects
        DraggableObject[] allObjects = FindObjectsByType<DraggableObject>(FindObjectsSortMode.None);
        foreach (DraggableObject obj in allObjects)
        {
            obj.isSelected = false;
            obj.UpdateVisualSelection();
        }
    }

    private void UpdateVisualSelection()
    {
        if (outlineEffect != null)
        {
            outlineEffect.enabled = isSelected;
        }
    }

    private async void DeleteObject()
    {
        if (data.id == null)
        {
            Debug.LogWarning("Cannot delete an object without an ID.");
            return;
        }

        // Send request to the server to delete object
        bool success = await environmentSystem.DeleteObject2D(data);
        if (success) 
        { 
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Failed to delete object from the server.");
        }
    }

    #endregion
}
