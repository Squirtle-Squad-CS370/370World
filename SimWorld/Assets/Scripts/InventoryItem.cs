using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class InventoryItem : MonoBehaviour
{
    new public string name;
    public Sprite image;
    public bool isStackable;
    public int quantity = 1;

    [HideInInspector]
    public GameObject go;
    [HideInInspector]
    public Rigidbody2D rb;

    private void Start()
    {
        go = gameObject;
        rb = gameObject.GetComponent<Rigidbody2D>();
        image = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    public void OnPickup()
    {
        gameObject.SetActive(false);
    }
}
