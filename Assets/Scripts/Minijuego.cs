using UnityEngine;

public class FollowOnClick : MonoBehaviour
{
    public Transform player;         // Referencia al jugador
    public float followSpeed = 100f;
    public GameObject noMolestar;

    public GameObject targetObject;
    private Collider colision;
    private Rigidbody noGravedad;
    private bool playerInside = false;
    private bool isFollowing = false;

    void Start()
    {
        colision = GetComponent<Collider>();
        noGravedad = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Si el jugador está dentro y hace click izquierdo
        if (playerInside && Input.GetMouseButtonDown(0))
        {
            isFollowing = !isFollowing; // alternar seguir / dejar de seguir
            colision.isTrigger = isFollowing;
            noGravedad.useGravity = !isFollowing;
        }
        // Si debe seguir al jugador
        if (isFollowing && targetObject != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                followSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}