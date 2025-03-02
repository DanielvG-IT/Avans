using UnityEngine;

public class Player : MonoBehaviour
{
    private const float speed = 2f;
    private float xMin, xMax, yMin, yMax;

    public void Start()
    {
        ResetMovementValues();
    }

    public void Update()
    {
        float movementX = Input.GetAxisRaw("Horizontal");
        float movementY = Input.GetAxisRaw("Vertical");
        transform.position += speed * Time.deltaTime * new Vector3(movementX, movementY);

        float yPos = Mathf.Clamp(transform.position.y, yMin, yMax);
        float xPos = Mathf.Clamp(transform.position.x, xMin, xMax);
        transform.position = new Vector3(xPos, yPos);
    }

    public void ResetMovementValues()
    {
        xMin = -(GameManager.Instance.SelectedEnvironment.maxLength / 2);
        xMax = GameManager.Instance.SelectedEnvironment.maxLength / 2;

        yMin = -(GameManager.Instance.SelectedEnvironment.maxHeight / 2);
        yMax = GameManager.Instance.SelectedEnvironment.maxHeight / 2;
    }
}
