using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class ItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public ItemInstance itemInstance;
    private InventoryManager inventoryManager;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPos;
    private Vector2Int originalGridPosition;
    private Vector2 offset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    public void Setup(ItemInstance instance)
    {
        itemInstance = instance;
        itemInstance.view = this;
        Image image = GetComponent<Image>();
        image.sprite = instance.data.icon;


        Vector2Int shapeSize = instance.data.GetShapeSize();
        rectTransform.sizeDelta = new Vector2(shapeSize.x * inventoryManager.slotSize, shapeSize.y * inventoryManager.slotSize);

        image.preserveAspect = false;
        image.raycastTarget = true;
        image.alphaHitTestMinimumThreshold = 0.01f;

        UpdatePosition();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = rectTransform.anchoredPosition;
        originalGridPosition = itemInstance.position;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        inventoryManager.RemoveItem(itemInstance);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPointerPosition;
        RectTransform panelRect = inventoryManager.inventoryPanel.GetComponent<RectTransform>();
        offset = new Vector2(panelRect.rect.width * 0.5f - (inventoryManager.slotSize / inventoryManager.canvas.scaleFactor), -panelRect.rect.height * 0.5f + (inventoryManager.slotSize / inventoryManager.canvas.scaleFactor));
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect,
            eventData.position,
            inventoryManager.canvas.worldCamera,
            out localPointerPosition))
        {
            rectTransform.anchoredPosition = localPointerPosition + offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        Slot nearestSlot = inventoryManager.GetNearestSlot(eventData.position);

        if (nearestSlot != null)
        {
            ItemInstance targetItem = inventoryManager.inventory.items.Find(item => item.position == nearestSlot.gridPos && item != itemInstance);
            if (targetItem != null && itemInstance.data.nextGrade != null)
            {
                ItemInstance mergedItem = inventoryManager.MergeItems(targetItem, itemInstance);
                if (mergedItem != null)
                {
                    Destroy(gameObject);
                    return;
                }
            }
            if (inventoryManager.CanPlaceItem(itemInstance.data, nearestSlot.gridPos))
            {
                itemInstance.position = nearestSlot.gridPos;
                inventoryManager.PlaceItem(itemInstance);
                UpdatePosition();
                return;
            }
        }

        itemInstance.position = inventoryManager.GetPositionForItem(itemInstance);
        if (!inventoryManager.CanPlaceItem(itemInstance.data, itemInstance.position))
        {
            itemInstance.position = originalGridPosition;
        }
        inventoryManager.PlaceItem(itemInstance);
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        Slot slot = inventoryManager.GetSlot(itemInstance.position);
        rectTransform.anchoredPosition = slot.GetComponent<RectTransform>().anchoredPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PopupManager popupManager = FindFirstObjectByType<PopupManager>();
        if (popupManager != null)
        {
            popupManager.ShowItemInfo(itemInstance.data);
        }
        else
        {
            Debug.LogError("PopupManager not found");
        }
    }
}