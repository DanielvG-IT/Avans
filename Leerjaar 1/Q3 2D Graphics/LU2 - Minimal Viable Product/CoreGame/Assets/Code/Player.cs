using UnityEngine;

public class Player : MonoBehaviour
{
    private const float speed = 2f;

    private readonly float xMin = -(GameManager.Instance.SelectedEnvironment.maxLength / 2);
    private readonly float xMax = GameManager.Instance.SelectedEnvironment.maxLength / 2;

    private readonly float yMin = -(GameManager.Instance.SelectedEnvironment.maxHeight / 2);
    private readonly float yMax = GameManager.Instance.SelectedEnvironment.maxHeight / 2;

    void Update()
    {
        float movementX = Input.GetAxisRaw("Horizontal");
        float movementY = Input.GetAxisRaw("Vertical");
        transform.position += speed * Time.deltaTime * new Vector3(movementX, movementY);

        float yPos = Mathf.Clamp(transform.position.y, yMin, yMax);
        float xPos = Mathf.Clamp(transform.position.x, xMin, xMax);
        transform.position = new Vector3(xPos, yPos);
    }
}
