using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private bool isPlaying;

    private enum MovementMode
    {
        Runner,
        Ship
    }

    [Header("Runner")]
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float jumpVelocity = 11.2f;
    [SerializeField] private float jumpBufferSeconds = 0.15f;

    [Header("Ground check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Ship")]
    [SerializeField] private Sprite shipSprite;
    [SerializeField] private Vector2 shipColliderSize = new Vector2(1.8f, 0.8f);
    [SerializeField] private float shipGravityScale = 2f;
    [SerializeField] private float shipThrust = 45f;
    [SerializeField] private float shipMaxVerticalSpeed = 7f;

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private InputAction jumpAction;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D bodyCollider;

    private Sprite runnerSprite;
    private Vector2 runnerColliderSize;

    private MovementMode movementMode = MovementMode.Runner;
    private float jumpBufferRemaining;
    private bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        jumpAction = playerInput.actions["Jump"];
        spriteRenderer = GetComponent<SpriteRenderer>();
        bodyCollider = GetComponent<BoxCollider2D>();

        runnerSprite = spriteRenderer.sprite;
        runnerColliderSize = bodyCollider.size;
    }

    private void FixedUpdate()
    {
        if (isDead || !isPlaying)
            return;

        Vector2 velocity = rb.linearVelocity;
        velocity.x = forwardSpeed;

        if (movementMode == MovementMode.Runner)
        {
            jumpBufferRemaining =
                Mathf.Max(0f, jumpBufferRemaining - Time.fixedDeltaTime);

            if (jumpBufferRemaining > 0f && IsGrounded())
            {
                velocity.y = jumpVelocity;
                jumpBufferRemaining = 0f;
            }

            rb.linearVelocity = velocity;
            return;
        }

        // Ship mode: holding Jump adds upward thrust; release lets gravity pull Britney down.
        if (jumpAction.IsPressed())
            rb.AddForce(Vector2.up * shipThrust, ForceMode2D.Force);

        velocity.y = Mathf.Clamp(
            velocity.y,
            -shipMaxVerticalSpeed,
            shipMaxVerticalSpeed
        );

        rb.linearVelocity = velocity;
    }

    // PlayerInput calls this because the Input Action is named "Jump".
    public void OnJump()
    {
        if (isDead || !isPlaying || movementMode != MovementMode.Runner)
            return;

        jumpBufferRemaining = jumpBufferSeconds;
    }

    public void EnterShipMode()
    {
        if (isDead || movementMode == MovementMode.Ship)
            return;

        movementMode = MovementMode.Ship;
        rb.gravityScale = shipGravityScale;
        jumpBufferRemaining = 0f;

        if (shipSprite != null)
            spriteRenderer.sprite = shipSprite;

        bodyCollider.size = shipColliderSize;

        Debug.Log("Entered Toxic ship mode.");
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

        Debug.Log("Britney hit a hazard.");
        gameManager.PlayerDied();
    }

    public void BeginRun()
    {
        isDead = false;
        isPlaying = true;
        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;
    }

    public void StopForEnd()
    {
        isDead = true;
        isPlaying = false;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}