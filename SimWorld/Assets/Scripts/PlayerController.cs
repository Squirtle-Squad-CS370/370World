using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private MouseManager mouseManager;

    private const float MOVE_SPEED = 5F;
    private float moveSpeed = MOVE_SPEED;
    [SerializeField]
    private float runSpeed = 8f;

    [SerializeField]
    private Rigidbody2D rb;
    // [SerializeField]
    // private Animator animator;

    private Vector2 moveInput; // will be used to store player input between Update & Fixed

    // Start is called before the first frame update
    void Start()
    {
        this.rb = transform.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    // We collect player input in this method
    void Update()
    {
        // Handle shift-to-sprint input
        if (Input.GetKey(KeyCode.LeftShift)) 
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = MOVE_SPEED;
        }
        
        // by default, getAxisRaw() responds to both arrow keys & WASD
        moveInput.x = Input.GetAxisRaw("Horizontal"); // (left = -1) (right = 1)
        moveInput.y = Input.GetAxisRaw("Vertical"); // (up = 1) (down = -1);

        // these will be used for animating our character with a blend tree
        // animator.SetFloat("Horizontal", movement.x);
        // animator.SetFloat("Vertical", movement.y);

        // Toolbar hotkey functionality - may be encapsulated later?
        // Press 1 to shoot, 2 to build walls
        if( Input.GetKeyDown(KeyCode.Alpha1) )
        {
            mouseManager.SetCrosshairCursor();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mouseManager.SetTileSelectCursor();
        }
    }

    // FixedUpdate is used because it is unaffected by framerate
    // that means physics (movement) will have consistent speed
    void FixedUpdate()
    {
        // move to the location that is the current position plus 
        // (input direction vector * moveSpeed * [utility for consistent speed] )
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }


}
