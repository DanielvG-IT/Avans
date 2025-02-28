using UnityEngine;

public class Player : MonoBehaviour
{
private readonly float speed = 2f;
    private float xMin = -200f, xMax = 200f;
    private float yMin = -100f, yMax = 100f;

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
