using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 4;
    [SerializeField]
    private bool isRanged = true;
    [SerializeField]
    private float range = 5; //How close the enemy will get to the player before shooting.
    
    private Vector3 targetPosition;
    private GameObject player;
    private Vector3 playerPosition;
    private bool isAttacking = false;
    private bool canShoot;
    private float timeBetweenShots = 1.5f;
    private float attackTimer = 0;
    private Transform target; //Could be the player or a turret.
    private float bulletSpeed = 6;
    
    [SerializeField]
    private GameObject bulletPrefab;
    
    [Header("Sound Settings")]
    public AudioClip ac_shoot;
    
    // Start is called before the first frame update
    void Start()
    {
        //ac_shoot = AudioController.Instance.ac_shoot;
        //bulletPrefab = (GameObject)Resources.Load("prefabs/TEST_Bullet", typeof(GameObject));
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = player.transform.position;
        target = player.transform; //TODO(Skyler): Make this target other things as well.
        
        if (Vector3.Distance(transform.position, playerPosition) <= range) 
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
            targetPosition = playerPosition;
        }
    }
    
    void FixedUpdate()
    {
        if (!isAttacking)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else 
        {
            if (canShoot)
            {
                Shoot();
                attackTimer = timeBetweenShots;
                canShoot = false;
            }
            else
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0)
                {
                    canShoot = true;
                }
            }
        }
    }
    
    private void Shoot()
    {
        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            // Create a bullet
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rot);
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            bullet.transform.SetParent(transform);
            // Play sfx
            AudioController.Instance.PlaySound(ac_shoot, transform.position);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(dir * bulletSpeed, ForceMode2D.Impulse);
        }
    }
}
