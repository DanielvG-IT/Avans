using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private EnvironmentSystem environmentSystem;
    private Object2D objectData;
    private Vector3 initialPosition;
    private bool isDragging = false;

    public void Initialize(Object2D data)
    {
        objectData = data;
        environmentSystem = FindFirstObjectByType<EnvironmentSystem>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (objectData.id == null)
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
        newPos.z = 0;
        transform.position = newPos;
    }

    public async void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        Vector3 finalPosition = transform.position;

        // Prepare object update data
        Object2D updatedData = new Object2D
        {
            id = objectData.id,
            environmentId = objectData.environmentId,
            positionX = finalPosition.x,
            positionY = finalPosition.y,
            scaleX = transform.localScale.x,
            scaleY = transform.localScale.y,
            prefabId = objectData.prefabId
        };

        // Call API to validate position
        Object2D validatedData = await environmentSystem.UpdateObject2D(updatedData);

        if (validatedData != null)
        {
            // Apply server-validated position
            transform.position = new Vector3(validatedData.positionX, validatedData.positionY, 0);
        }
        else
        {
            // Revert to initial position if update fails
            Debug.LogError("Failed to update object on server. Reverting position.");
            transform.position = initialPosition;
        }
    }
}
