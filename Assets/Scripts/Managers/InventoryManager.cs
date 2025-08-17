using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Inventory inventory;
    public Canvas canvas;
    public GameObject slotPrefab;
    public GameObject itemPrefab;
    public Transform gridTransform;
    public Transform itemsTransform;
    public RectTransform inventoryPanel;
    public ParticleSystem mergeParticle;
    private Slot[,] slots;
    public float slotSize = 50f;

    void Start()
    {
        inventory = new Inventory(3, 3);
        InitializeGrid();
        AddTestItem();
    }

    public void InitializeGrid()
    {
        foreach (Transform child in gridTransform)
            Destroy(child.gameObject);

        inventoryPanel.sizeDelta = new Vector2(inventory.width * slotSize, inventory.height * slotSize);

        slots = new Slot[inventory.width, inventory.height];
        for (int x = 0; x < inventory.width; x++)
        {
            for (int y = 0; y < inventory.height; y++)
            {
                GameObject slotObj = Instantiate(slotPrefab, gridTransform);
                slotObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * slotSize, -y * slotSize);
                Slot slot = slotObj.GetComponent<Slot>();
                slot.gridPos = new Vector2Int(x, y);
                slots[x, y] = slot;
            }
        }
    }

    public void RebuildGrid()
    {
        InitializeGrid();
    }


    public bool CanPlaceItem(ItemData item, Vector2Int pos)
    {
        return inventory.CanPlaceItem(item, pos);
    }

    public void PlaceItem(ItemInstance item)
    {
        if (item.view == null)
        {
            GameObject itemObj = Instantiate(itemPrefab, itemsTransform);
            ItemView itemView = itemObj.GetComponent<ItemView>();
            itemView.Setup(item);
        }
        else
        {
            item.view.UpdatePosition();
        }
        inventory.PlaceItem(item);
    }

    public void RemoveItem(ItemInstance item)
    {
        inventory.RemoveItem(item);
    }

    public Vector2Int GetPositionForItem(ItemInstance item)
    {
        return inventory.GetPositionForItem(item);
    }

    public Slot GetNearestSlot(Vector2 screenPos)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gridTransform as RectTransform, screenPos, canvas.worldCamera, out localPos);
        int x = Mathf.FloorToInt(localPos.x / slotSize);
        int y = Mathf.FloorToInt(-localPos.y / slotSize);
        if (x >= 0 && x < inventory.width && y >= 0 && y < inventory.height)
        {
            return slots[x, y];
        }
        return null;
    }

    public Slot GetSlot(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < inventory.width && pos.y >= 0 && pos.y < inventory.height)
            return slots[pos.x, pos.y];
        return null;
    }

    public ItemInstance MergeItems(ItemInstance item1, ItemInstance item2)
    {
        ItemInstance[] itemsToDestroy;
        ItemInstance mergedItem = inventory.MergeItems(item1, item2, out itemsToDestroy);

        if (mergedItem != null)
        {
            if (itemsToDestroy != null)
            {
                foreach (var item in itemsToDestroy)
                {
                    if (item.view != null)
                        Destroy(item.view.gameObject);
                }
            }
            AddItem(mergedItem.data, mergedItem.position);
            mergeParticle.Play();
            return mergedItem;
        }
        else
        {
            Debug.LogWarning("Merge doesn't possible");
            return null;
        }
    }


    public void AddItem(ItemData itemData, Vector2Int position)
    {
        ItemInstance item = new ItemInstance { data = itemData, dataName = itemData.name, position = position };
        if (CanPlaceItem(item.data, position))
        {
            PlaceItem(item);
        }
        else
        {
            Debug.LogWarning("Cannot plase item at " + position);
        }
    }

    public void ExpandInventory(int width,  int height)
    {
        var itemsToRedraw = new List<ItemInstance>(inventory.items);

        foreach (var item in itemsToRedraw)
        {
            if (item.view != null)
            {
                Destroy(item.view.gameObject);
                item.view = null;
            }
        }

        inventory.Expand(width, height);

        RebuildGrid();

        foreach (var item in itemsToRedraw)
        {
            PlaceItem(item);
        }
    }

    public void SaveInventory()
    {
        string json = JsonUtility.ToJson(inventory);
        PlayerPrefs.SetString("Inventory", json);
    }

    public void LoadInventory()
    {
        if (PlayerPrefs.HasKey("Inventory"))
        {
            foreach (var item in inventory.items)
            {
                if (item.view != null)
                {
                    Destroy(item.view.gameObject);
                    item.view = null;
                }
            }

            string json = PlayerPrefs.GetString("Inventory");
            inventory = JsonUtility.FromJson<Inventory>(json);
            inventory.grid = new bool[inventory.width, inventory.height];
            RebuildGrid();

            foreach (var item in inventory.items)
            {

                if (item.view == null)
                {
                    item.data = Resources.Load<ItemData>(item.dataName);
                    GameObject itemObj = Instantiate(itemPrefab, itemsTransform);
                    ItemView itemView = itemObj.GetComponent<ItemView>();
                    itemView.Setup(item);
                    inventory.PlaceItem(item);
                }
            }
        }
        else
        {
            inventory = new Inventory(3, 3);
        }
        InitializeGrid();
    }
    void AddTestItem()
    {
        ItemData rifleData = Resources.Load<ItemData>("Rifle1");
        ItemData coffeData = Resources.Load<ItemData>("Coffe2");
        AddItem(rifleData, new Vector2Int(0, 0));
        AddItem(coffeData, new Vector2Int(1, 0));
        AddItem(coffeData, new Vector2Int(2, 0));
    }


}