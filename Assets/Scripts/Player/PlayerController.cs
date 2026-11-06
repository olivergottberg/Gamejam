using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float jumpVelocity = 7f;

    [Header("Ground check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool jumpRequested;
    private bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;

        Vector2 velocity = rb.linearVelocity;
        velocity.x = forwardSpeed;

        if (jumpRequested)
        {
            if (IsGrounded())
                velocity.y = jumpVelocity;

            jumpRequested = false;
        }

        rb.linearVelocity = velocity;
    }

    // PlayerInput calls this because the Input Action is named "Jump".
    public void OnJump()
    {
        if (!isDead)
            jumpRequested = true;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        ) != null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || !other.CompareTag("Hazards"))
            return;

        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Debug.Log("Britney hit a hazard.");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}