using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Installable : MonoBehaviour
{
    private InventoryItem item;
    private Rigidbody2D rb;
    public bool isInstalled;

    private void Awake()
    {
        item = GetComponent<InventoryItem>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnInstall()
    {
        if (item != null)
        {
            item.enabled = false;
        }

        gameObject.SetActive(true);
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = true;
        
        if( GetComponent<TurretAI>() != null )
        {
            GetComponent<TurretAI>().enabled = true;
            GetComponent<CircleCollider2D>().enabled = true;
        }

        isInstalled = true;
    }
    public void OnUninstall()
    {
        if( item != null )
        {
            item.enabled = true;
            rb.isKinematic = false;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if( GetComponent<TurretAI>() != null )
        {
            GetComponent<TurretAI>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
        }

        isInstalled = false;
    }
}
