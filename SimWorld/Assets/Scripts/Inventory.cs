using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton

    public static Inventory Instance { get; protected set; }

    void Awake()
    {
        // Make sure this is the only instance of AudioController
        if (Instance != null)
        {
            Debug.LogError("Inventory - Another Inventory already exists.");
            return;
        }

        Instance = this;
    }

    #endregion

    private int numSlots = 7;

    public InventorySlot[] slots;

    public AudioClip pickUpSuccessSFX;
    public AudioClip pickUpFailSFX;

    void Start()
    {
        // Start out with empty slots
        for( int i = 0; i < numSlots; i++ )
        {
            slots[i].Clear();
            slots[i].index = i;
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
                    slots[i].SetItem(item);
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
    private void RemoveItem(int idx)
    {
        if( slots[idx].isEmpty )
        {
            Debug.Log("Inventory - attempted to remove item from empty slot.");
            return;
        }

        slots[idx].Clear();
    }
    // Remove item and drop it on the ground
    // If no direction is specified, will throw it randomly
    public void DropItem(int idx)
    {
        Vector3 dir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        DropItem(idx, dir);
    }
    public void DropItem(int idx, Vector3 dir)
    {
        if( slots[idx].isEmpty )
        {
            Debug.Log("Inventory - attempted to drop item from empty slot.");
            return;
        }

        slots[idx].item.transform.position = transform.position;
        slots[idx].item.go.SetActive(true);
        slots[idx].item.rb.AddForce(dir * 100f, ForceMode2D.Impulse);
        slots[idx].item.OnDrop();

        RemoveItem(idx);
    }
    
    public void SwapSlots(InventorySlot slot1, InventorySlot slot2)
    {
        // If both slots are occupied
        if( !slot1.isEmpty && !slot2.isEmpty )
        {
            InventoryItem tmpItem = slot1.item;
            slot1.SetItem(slot2.item);
            slot2.SetItem(tmpItem);
        }
        // Only slot 1 has an item
        else if( !slot1.isEmpty && slot2.isEmpty )
        {
            slot2.SetItem(slot1.item);
            slot1.Clear();
        }
        // Only slot 2 has an item
        else if( slot1.isEmpty && !slot2.isEmpty )
        {
            slot1.SetItem(slot2.item);
            slot2.Clear();
        }
        // Otherwise neither slot is filled, do nothing
    }
}
