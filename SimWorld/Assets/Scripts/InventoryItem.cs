using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public string itemName;
    public Sprite image;
    
    public void OnPickup()
    {
        this.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
