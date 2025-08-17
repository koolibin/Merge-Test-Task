using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpandButton : MonoBehaviour
{
    public TMP_InputField widthInput;
    public TMP_InputField heightInput;

    public InventoryManager inventoryManager;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnExpandClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnExpandClick);
    }

    public void OnExpandClick()
    {
        int addWidth = 0;
        int addHeight = 0;

        if (widthInput != null && !string.IsNullOrEmpty(widthInput.text))
        {
            if (int.TryParse(widthInput.text, out int parsedWidth))
            {
                if (parsedWidth >= 0)
                    addWidth = parsedWidth;
                else
                    addWidth = 0;
            }
            else
            {
                addWidth = 0;
            }
        }
        else
        {
            addWidth = 0;
        }

        if (heightInput != null && !string.IsNullOrEmpty(heightInput.text))
        {
            if (int.TryParse(heightInput.text, out int parsedHeight))
            {
                if (parsedHeight >= 0)
                    addHeight = parsedHeight;
                else
                    addHeight = 0;
            }
            else
            {
                addHeight = 0;
            }
        }
        else
        {
            addHeight = 0;
        }

        inventoryManager.ExpandInventory(addWidth, addHeight);
        //    if (widthExpand <= 0 || heigthExpand <= 0)
        //    {
        //        Debug.LogWarning("Width and Heigth should be positive");
        //        return;
        //    }

        //    if (widthExpand + inventoryManager.inventory.width > 5 || heigthExpand + inventoryManager.inventory.height > 5)
        //    {
        //        Debug.LogWarning("Inventory limited at 5x5 cells");
        //        return;
        //    }

        //    var itemsToRedraw = new List<ItemInstance>(inventoryManager.inventory.items);

        //    foreach (var item in itemsToRedraw)
        //    {
        //        if (item.view != null)
        //        {
        //            Destroy(item.view.gameObject);
        //            item.view = null;
        //        }
        //    }

        //    inventoryManager.inventory.Expand(widthExpand, heigthExpand);

        //    inventoryManager.RebuildGrid();

        //    foreach (var item in itemsToRedraw)
        //    {
        //        inventoryManager.PlaceItem(item);
        //    }

        //    inventoryManager.SaveInventory();
    }

}
