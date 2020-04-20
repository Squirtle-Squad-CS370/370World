using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField]
    private Canvas canvas;

    public bool isEmpty;
    public int index;
    private Image itemImage;
    private RectTransform imageTransform;
    private TextMeshProUGUI quantityTxt;
    private RectTransform txtTransform;

    //public GameObject item_go;
    public InventoryItem item;

    private void Awake()
    {
        itemImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        imageTransform = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
        quantityTxt = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        txtTransform = gameObject.transform.GetChild(1).GetComponent<RectTransform>();
    }

    public void SetItem(InventoryItem item)
    {
        this.item = item;
        itemImage.sprite = item.image;
        itemImage.enabled = true;
        isEmpty = false;
        UpdateQuantity();
    }
    public void Clear()
    {
        item = null;
        itemImage.sprite = null;
        itemImage.enabled = false;
        isEmpty = true;
        UpdateQuantity();
    }

    public void UpdateQuantity()
    {
        if( item == null || item.quantity == 1 )
        {
            quantityTxt.enabled = false;
        }
        else
        {
            quantityTxt.text = "x" + item.quantity;
            quantityTxt.enabled = true;
        }
    }

    // UI functionality
    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Slight transparency
        Color tmpColor = itemImage.color;
        tmpColor.a = .6f;
        itemImage.color = tmpColor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the item image with the mouse cursor
        imageTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        txtTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // If released over some object
        if( eventData.pointerCurrentRaycast.gameObject != null )
        {
            // If released over another slot
            if( eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>() )
            {
                Inventory.Instance.SwapSlots(this, eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>() );
            }

            Debug.Log("Dropped item on top of " + eventData.pointerCurrentRaycast.gameObject.name);
        }
        else
        {
            Inventory.Instance.DropItem(index);
        }

        // return item image to its normal position
        imageTransform.anchoredPosition = Vector2.zero;
        txtTransform.anchoredPosition = new Vector2(-5f, 5f);

        // Return to full opacity
        Color tmpColor = itemImage.color;
        tmpColor.a = 1f;
        itemImage.color = tmpColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        
    }
}
