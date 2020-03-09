using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    // Like with animal, I have left these variables blank
    // so that we can set them to different values for
    // different types of turrets.
    [Header("Attack Settings")]
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private float timeToLockTarget;
    private float lockOnTimer = 0f;
    [SerializeField]
    private float timeBetweenAttacks;
    private float attackTimer = 0f;
    [SerializeField]
    // bulletSpeed will tell our projectile how quickly to move.
    // For now if we set it to 0, our weapon will hitscan instead.
    private float bulletSpeed;

    [Header("Sprite Settings")]
    [SerializeField]
    private Sprite bulletSprite;
    [SerializeField]
    private GameObject bulletPrefab;

    [Header("Sound Settings")]
    public AudioClip ac_targetAcquired;
    public AudioClip ac_shoot;

    private Transform target;
    private bool hasTargetLocked = false;
    private bool canShoot = true;

    void Start()
    {
        // Initialize our enemy detection collider's size
        GetComponent<CircleCollider2D>().radius = attackRange;
    }

    void FixedUpdate()
    {
        if( ! hasTargetLocked && lockOnTimer > 0 )
        {
            // If timer is set, tick it
            lockOnTimer -= Time.deltaTime;
            // Then check if we are finished
            if( lockOnTimer <= 0 )
            {
                hasTargetLocked = true;
            }
        }

        // If we have a target,
        if( hasTargetLocked )
        {
            // ...and we are able to shoot it,
            if( canShoot )
            {
                // ...shoot it and reset our attack timer.
                Shoot();
                attackTimer = timeBetweenAttacks;
                canShoot = false;
            }
            // ...and we are unable to shoot yet,
            else
            {
                // ...decrement our attack timer.
                attackTimer -= Time.deltaTime;
                // If our timer hits 0, we can shoot again!
                if( attackTimer <= 0 )
                {
                    canShoot = true;
                }
            }
        }

        // If target goes out of range
        if( hasTargetLocked && Vector3.Distance( transform.position, target.position ) > attackRange )
        {
            hasTargetLocked = false;
            Debug.Log(gameObject.name + " is safe... for now.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Don't worry about bullets!
        if( collision.tag == "Bullet" )
        {
            return;
        }

        Debug.Log(collision.gameObject.name + " has entered the danger zone.");

        // If we've found an enemy, target acquired
        if ( collision.tag == "Enemy" )
        {
            lockOnTimer = timeToLockTarget;
            target = collision.transform;
        }
    }

    private void Shoot()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        
        // With 0 bulletSpeed set, we will hitscan
        if( bulletSpeed == 0 )
        {

        }
        // Otherwise we fire projectiles
        else
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rot);
            bullet.transform.SetParent(transform);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(dir * bulletSpeed, ForceMode2D.Impulse);
        }      
    }
}
