using UnityEngine;

public class Destructible : MonoBehaviour
{
    Collider coll;

    [Header("Destructible Settings")]
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    // What this object will drop when destroyed:
    private GameObject dropPrefab;
    [SerializeField]
    //How often it will drop. By default it will drop 100% of the time.
    [Tooltip("Between 0 and 1")]
    private float dropRate = 1.0f;
    [SerializeField]
    private AudioClip ac_hit;
    [SerializeField]
    private AudioClip ac_destroyed;

    private int health;

    void Start()
    {
        coll = GetComponent<Collider>();
        health = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        // Subtract the damage amount from our health
        health -= amount;

        // Play hit SFX
        AudioController.Instance.PlaySound(ac_hit, transform.position);
        
        // If dead now
        if( health <= 0 )
        {
            Die();
        }
    }

    private void Die()
    {
        // Play death/destruction SFX
        AudioController.Instance.PlaySound(ac_destroyed, transform.position);

        // Spawn any drops
        if (dropPrefab != null && Random.value <= dropRate)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }

        // Die
        Destroy(gameObject);
    }
}
