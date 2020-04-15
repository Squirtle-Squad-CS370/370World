using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public bool isEmpty;
    public Image itemImage;
    public GameObject item_go;

    private void Awake()
    {
        itemImage = gameObject.transform.GetChild(0).GetComponent<Image>();
    }
}
