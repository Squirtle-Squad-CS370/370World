using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private int numSlots = 7;

    public InventorySlot[] slots;

    public AudioClip pickUpSuccessSFX;
    public AudioClip pickUpFailSFX;

    void Start()
    {
        // Start out with empty slots
        for( int i = 0; i < numSlots; i++ )
        {
            slots[i].isEmpty = true;
            slots[i].itemImage.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Inventory - collision detected
        if( collision.gameObject.tag == "Item" )
        {
            // Inventory - collided with item
            GameObject itemPickedUp = collision.gameObject;
            InventoryItem item = itemPickedUp.GetComponent<InventoryItem>();
            AddItem(item);
        }
    }

    private void AddItem(InventoryItem item)
    {
        bool itemAdded = false;

        // Iterate through all our slots
        for( int i = 0; i < numSlots; i++ )
        {
            // Find an empty one
            if( slots[i].isEmpty )
            {
                // Add item to slot
                slots[i].itemImage.sprite = item.image;
                slots[i].isEmpty = false;
                slots[i].itemImage.enabled = true;
                itemAdded = true;
                AudioController.Instance.PlaySound(pickUpSuccessSFX);
                item.OnPickup();
                Debug.Log("Item <" + item.itemName + "> added to inventory slot " + i + ".");
                break;
            }
        }

        if( itemAdded == false )
        {
            AudioController.Instance.PlaySound(pickUpFailSFX);
            Debug.Log("Inventory full! Unable to pick up " + item.itemName + ".");
        }
    }
}
