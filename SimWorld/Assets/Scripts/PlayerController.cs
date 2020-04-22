using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Singleton

    public static PlayerController Instance { get; protected set; }

    void Awake()
    {
        // Make sure this is the only instance of AudioController
        if (Instance != null)
        {
            Debug.LogError("PlayerController - Another PC already exists.");
            return;
        }

        Instance = this;
    }

    #endregion

    [Header("Movement Settings")]
    [SerializeField]
    private const float runSpeed = 8f;
    [SerializeField]
    private const float walkSpeed = 5f;

    [Header("SFX Settings")]
    public AudioClip ac_footstep;
    [SerializeField]
    private float walkSFXTime;  // time between footstep sfx when walking
    [SerializeField]
    private float sprintSFXTime;    // time between footstep sfx when sprinting

    // Internal use only
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    // private Animator animator;
    private Vector2 moveInput; // will be used to store player input between Update & Fixed
    private float currentMoveSpeed;
    private float footstepSFXTimer = 0f;

    private InventoryItem equippedItem = null;

    // Start is called before the first frame update
    void Start()
    {
        this.rb = transform.GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    // We collect player input in this method
    void Update()
    {
        // Handle shift-to-sprint input
        if (Input.GetKey(KeyCode.LeftShift)) 
        {
            currentMoveSpeed = runSpeed;
        }
        else
        {
            currentMoveSpeed = walkSpeed;
        }
        
        // by default, getAxisRaw() responds to both arrow keys & WASD
        moveInput.x = Input.GetAxisRaw("Horizontal"); // (left = -1) (right = 1)
        moveInput.y = Input.GetAxisRaw("Vertical"); // (up = 1) (down = -1);

        // these will be used for animating our character with a blend tree
        // animator.SetFloat("Horizontal", movement.x);
        // animator.SetFloat("Vertical", movement.y);

        if( Input.GetMouseButtonDown(0) )
        {
            if( equippedItem != null )
            {
                if( equippedItem.GetComponent<Installable>() != null )
                {
                    Installable tmp;
                    if (equippedItem.quantity > 1)
                    {
                        tmp = Instantiate(equippedItem).GetComponent<Installable>();
                    }
                    else
                    {
                        tmp = equippedItem.GetComponent<Installable>();
                    }
                    Unequip();
                    Inventory.Instance.RemoveItem();
                    tmp.gameObject.SetActive(true);
                    MouseManager.Instance.GetTileUnderMouse().Install(tmp);
                }
            }
        }
        
        spriteRenderer.sortingOrder = (int)rb.position.y;
    }

    // FixedUpdate is used because it is unaffected by framerate
    // that means physics (movement) will have consistent speed
    void FixedUpdate()
    {
        if( moveInput != Vector2.zero )
        {
            // move to the location that is the current position plus 
            // (input direction vector * moveSpeed * [utility for consistent speed] )
            Vector2 newPosition = rb.position + moveInput * currentMoveSpeed * Time.fixedDeltaTime;
            //Tile t = WorldController.Instance.World.GetTileAt((int)newPosition.x, (int)newPosition.y);
            
            //TODO(Skyler): This only works in the first chunk. The player can't leave the first chunk.
            //if (t.isWalkable)
            //{
                rb.MovePosition(newPosition);
            //}
            
            // Play sound effects
            switch( currentMoveSpeed )
            {
                case walkSpeed:
                    if( footstepSFXTimer <= 0 )
                    {
                        AudioController.Instance.PlaySound(ac_footstep, transform.position, true);
                        footstepSFXTimer = walkSFXTime;
                    }
                    footstepSFXTimer -= Time.deltaTime;
                    break;
                case runSpeed:
                    if( footstepSFXTimer <= 0 )
                    {
                        AudioController.Instance.PlaySound(ac_footstep, transform.position, true);
                        footstepSFXTimer = sprintSFXTime;
                    }
                    footstepSFXTimer -= Time.deltaTime;
                    break;
                default:
                    break;
            }
        }
        else
        {
            footstepSFXTimer = 0f;
        }
    }

    public void Equip(InventoryItem item)
    {
        if( equippedItem == item )
        {
            Unequip();
            return;
        }

        equippedItem = item;

        if( equippedItem.GetComponent<Installable>() != null )
        {
            MouseManager.Instance.SetTileSelectCursor();
        }
        else
        {
            MouseManager.Instance.SetCrosshairCursor();
        }

    }
    public void Unequip()
    {
        equippedItem = null;
        MouseManager.Instance.SetCrosshairCursor();
    }
}
