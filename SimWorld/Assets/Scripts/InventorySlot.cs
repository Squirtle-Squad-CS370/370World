using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public bool isEmpty;
    public Image itemImage;
    public TextMeshProUGUI quantityTxt;

    //public GameObject item_go;
    public InventoryItem item;

    private void Awake()
    {
        itemImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        quantityTxt = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
    
    public void UpdateQuantity()
    {
        quantityTxt.text = "x" + item.quantity;

        if( item.quantity == 1 )
        {
            quantityTxt.enabled = false;
        }
        else
        {
            quantityTxt.enabled = true;
        }
    }
}
