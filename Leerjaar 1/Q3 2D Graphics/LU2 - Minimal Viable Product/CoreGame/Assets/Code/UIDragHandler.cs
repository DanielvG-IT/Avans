using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject prefabToInstantiate;
    private GameObject draggedObject;
    private Environment2D loadedEnvironment;

    public void OnBeginDrag(PointerEventData eventData)
    {
        loadedEnvironment = GameManager.Instance.SelectedEnvironment;
        draggedObject = Instantiate(prefabToInstantiate);
        draggedObject.transform.position = Camera.main.ScreenToWorldPoint(eventData.position);
        draggedObject.transform.position = new Vector3(draggedObject.transform.position.x, draggedObject.transform.position.y, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedObject != null)
        {
            draggedObject.transform.position = Camera.main.ScreenToWorldPoint(eventData.position);
            draggedObject.transform.position = new Vector3(draggedObject.transform.position.x, draggedObject.transform.position.y, 0);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedObject != null)
        {
            EnvironmentSystem environmentSystem = FindFirstObjectByType<EnvironmentSystem>();
            Object2D newObjectData = new()
            {
                environmentId = loadedEnvironment.id,
                positionX = draggedObject.transform.position.x,
                positionY = draggedObject.transform.position.y,
                prefabId = prefabToInstantiate.name
            };

            environmentSystem.CreateObject2D(newObjectData);

            DraggableObject draggable = draggedObject.AddComponent<DraggableObject>();
            draggable.Initialize(newObjectData);
        }
    }
}
