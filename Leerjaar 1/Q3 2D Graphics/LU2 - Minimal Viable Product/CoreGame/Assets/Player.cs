using UnityEngine;

public class Player : MonoBehaviour
{
private float speed = 2f;
    private float xMin = 0f, xMax = 0f;
    private float yMin = 0f, yMax = 0f;

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
