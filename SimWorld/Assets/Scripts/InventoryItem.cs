using System.Collections;
using System.Collections.Generic;
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
    private Collider2D coll;

    private void Awake()
    {
        go = gameObject;
        rb = gameObject.GetComponent<Rigidbody2D>();
        image = gameObject.GetComponent<SpriteRenderer>().sprite;
        coll = gameObject.GetComponent<Collider2D>();
    }

    public void OnPickup()
    {
        gameObject.SetActive(false);
        coll.enabled = false; // makes for easier dropping
    }
    public void OnDrop()
    {
        // Coroutine waits a second then reactivates collider
        // (so it is not immediately picked back up)
        StartCoroutine(EnableCollider());
    }
    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(1);
        coll.enabled = true;
    }
}
