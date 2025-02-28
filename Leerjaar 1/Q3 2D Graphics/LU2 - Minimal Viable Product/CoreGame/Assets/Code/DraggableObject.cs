using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 initialPosition;
    private bool isDragging = false;
    private Object2D objectData;
    private EnvironmentSystem environmentSystem;

    void Start()
    {
        environmentSystem = FindFirstObjectByType<EnvironmentSystem>();
    }

    public void Initialize(Object2D data)
    {
        objectData = data;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPosition = transform.position;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Camera.main.ScreenToWorldPoint(eventData.position);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        objectData.positionX = transform.position.x;
        objectData.positionY = transform.position.y;

        environmentSystem.UpdateObject2D(objectData);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && environmentSystem.SelectedObject == this.gameObject)
        {
            environmentSystem.DeleteObject2D(objectData);
            Destroy(gameObject);
        }
    }
}
