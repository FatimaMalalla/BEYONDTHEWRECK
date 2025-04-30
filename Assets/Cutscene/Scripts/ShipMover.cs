using UnityEngine;

public class ShipMover : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Move the ship forward constantly
        rb.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);
    }
}
