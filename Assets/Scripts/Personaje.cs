using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;           // Velocidad de movimiento
    public float rotationSpeed = 10f;  // Velocidad de rotaci�n hacia la direcci�n del movimiento

    private Rigidbody rb;
    private Vector3 movement;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Evita que la c�psula se vuelque
    }

    void Update()
    {
        // Captura input WASD o flechas
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movement = new Vector3(horizontal, 0f, vertical).normalized;
    }

    void FixedUpdate()
    {
        // Movimiento
        Vector3 velocity = movement * speed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z); // Conserva la velocidad vertical

        // Rotaci�n hacia la direcci�n del movimiento
        if (movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }
}
