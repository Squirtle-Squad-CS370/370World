using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void Start()
    {
        // Destroy the bullet after 5 seconds regardless
        // of whether or not it hits anything.
        Destroy(gameObject, 5f);
    }
    /*
    private void Update()
    {
        // We could have each bullet check on update
        // to see if it is out of world map bounds
        // and if so, destroy it. I think setting a delay
        // in Start() is probably easier though.
        if( transform.position.x < 0 ||
            transform.position.x > WorldController.Instance.World.Width  ||
            transform.position.y < 0 ||
            transform.position.y > WorldController.Instance.World.Height )
        {
            Destroy(gameObject);
        }
    }
    */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If we hit something, destroy the bullet.
        Destroy(gameObject);
    }
}
