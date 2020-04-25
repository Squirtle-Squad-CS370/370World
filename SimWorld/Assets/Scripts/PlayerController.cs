﻿using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private const float runSpeed = 8f;
    [SerializeField]
    private const float walkSpeed = 5f;

    [Header("SFX Settings")]
    public AudioClip ac_footstep;
    public AudioClip ac_shoot;
    [SerializeField]
    private float walkSFXTime;  // time between footstep sfx when walking
    [SerializeField]
    private float sprintSFXTime;    // time between footstep sfx when sprinting
    
    [SerializeField]
    private GameObject bulletPrefab;

    // Internal use only
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    // private Animator animator;
    private Vector2 moveInput; // will be used to store player input between Update & Fixed
    private float currentMoveSpeed;
    private float footstepSFXTimer = 0f;
    private float bulletSpeed = 2;
    private bool canShoot = true;
    private float timeBetweenShots = 1.5f;
    private float shootTimer = 1.5f;

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

        // Toolbar hotkey functionality - may be encapsulated later?
        // Press 1 to shoot, 2 to build walls
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MouseManager.Instance.SetCrosshairCursor();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MouseManager.Instance.SetTileSelectCursor();
        }
        
        //shoot on left click
        //TODO(Skyler): For testing. Needs to check if gun is equiped/has ammo.
        //TODO(Skyler): Don't shoot when accessing game UI. DONE
        if (Input.GetMouseButtonDown(0) && canShoot && !Inventory.Instance.beingInteractedWith())
        {
            shoot();
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
        
        if (timeBetweenShots <= 0)
        {
            canShoot = true;
        }
        else
        {
            timeBetweenShots -= Time.deltaTime;
        }
    }
    
    private void shoot()
    {
        canShoot = false;
        shootTimer = timeBetweenShots;
        
        //For some reason if you subtract the vectors the spped depends on the distance. That's why x and y are done separately.
        Vector3 dir = new Vector3();
        dir.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - rb.transform.position.x;
        dir.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - rb.transform.position.y;
        dir = dir.normalized;
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        // Create a bullet
        GameObject bullet = Instantiate(bulletPrefab, rb.transform.position, rot);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        bullet.transform.SetParent(transform);
        // Play sfx
        AudioController.Instance.PlaySound(ac_shoot, rb.transform.position);
        Rigidbody2D rbb = bullet.GetComponent<Rigidbody2D>();
        //TODO(Skyler): Bug: The closer the mouse is to the player the slower the bullet is.
        rbb.AddForce(dir * bulletSpeed, ForceMode2D.Impulse);
    }
}
