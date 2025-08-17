using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChoiceItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemData itemData;
    private InventoryManager inventoryManager;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPos;


    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        originalPos = rectTransform.position;
    }

    public void Setup(ItemData data)
    {
        itemData = data;
        Image image = GetComponent<Image>();
        image.sprite = data.icon;
        image.preserveAspect = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / inventoryManager.canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        Slot nearestSlot = inventoryManager.GetNearestSlot(eventData.position);
        if (nearestSlot != null)
        {
            ItemInstance newItem = new ItemInstance { data = itemData, dataName = itemData.name, position = nearestSlot.gridPos };
            ItemInstance targetItem = inventoryManager.inventory.items.Find(item => item.position == nearestSlot.gridPos);
            if (targetItem != null)
            {
                inventoryManager.MergeItems(targetItem, newItem);
                Destroy(gameObject);
                return;
            }

            if (inventoryManager.inventory.CanPlaceItem(newItem.data, newItem.position))
            {
                inventoryManager.AddItem(itemData, newItem.position);
                Destroy(gameObject);
                return;
            }
        }
        rectTransform.anchoredPosition = originalPos;
    }
}