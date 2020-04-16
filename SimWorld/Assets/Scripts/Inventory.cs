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
        // If we collide with an item, pick it up
        if( collision.gameObject.tag == "Item" )
        {
            GameObject itemPickedUp = collision.gameObject;
            InventoryItem item = itemPickedUp.GetComponent<InventoryItem>();
            AddItem(item);
        }
    }

    // Add an item to the inventory
    private void AddItem(InventoryItem item)
    {
        int itemSlot = -1; // previously was a bool, but this is more useful

        // First, check if stackable
        if (item.isStackable)
        {
            // If stackable and already exists in inventory
            for (int i = 0; i < numSlots; i++)
            {
                if( ! slots[i].isEmpty && slots[i].item.name == item.name)
                {
                    // Add to quantity
                    slots[i].item.quantity += item.quantity;
                    //itemAdded = true;
                    itemSlot = i;

                    // Item has been added to stack,
                    // so we break out of loop
                    break;
                }
            }
        }

        // Otherwise, unable to add to existing stack
        //if( itemAdded == false )
        if( itemSlot == -1 )
        {
            // Iterate through all our slots
            for (int i = 0; i < numSlots; i++)
            {
                // Find an empty one
                if (slots[i].isEmpty)
                {
                    // Add item to slot
                    slots[i].item = item;
                    slots[i].itemImage.sprite = item.image;
                    slots[i].isEmpty = false;
                    slots[i].itemImage.enabled = true;
                    //itemAdded = true;
                    itemSlot = i;

                    // Item has been placed in empty slot,
                    // so we break out of loop
                    break;
                }
            }
        }

        // if( itemAdded == true )
        if( itemSlot != -1 )
        {
            // Play "item picked up" SFX
            AudioController.Instance.PlaySound(pickUpSuccessSFX);

            // Update slot's quantity text
            slots[itemSlot].UpdateQuantity();

            // Let item know it was picked up
            item.OnPickup();

            Debug.Log("Item <" + item.name + "> added to inventory slot " + itemSlot + ".");
        }
        else
        {
            // Play "inventory full" SFX
            AudioController.Instance.PlaySound(pickUpFailSFX);

            Debug.Log("Inventory full! Unable to pick up " + item.name + ".");
        }
    }
    // Remove an item from the inventory
    private void RemoveItem(int ID)
    {
        if( slots[ID].isEmpty )
        {
            Debug.Log("Inventory - attempted to remove item from empty slot.");
            return;
        }

        slots[ID].item = null;
        slots[ID].itemImage.sprite = null;
        slots[ID].isEmpty = true;
        slots[ID].itemImage.enabled = false;
    }
    // Remove item and drop it on the ground
    private void DropItem(int ID)
    {
        if( slots[ID].isEmpty )
        {
            Debug.Log("Inventory - attempted to drop item from empty slot.");
            return;
        }

        float rndX = UnityEngine.Random.Range(-1f, 1f);
        float rndY = UnityEngine.Random.Range(-1f, 1f);
        Vector3 rndDir = new Vector3(rndX, rndY, 0f);

        slots[ID].item.transform.position = transform.position;
        slots[ID].item.go.SetActive(true);
        slots[ID].item.rb.AddForce(rndDir * 5f);

        RemoveItem(ID);
    }
}
