using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemInstance
{
    public string dataName;
    public ItemData data;
    public Vector2Int position;
    [NonSerialized] public ItemView view;
}

public class Inventory
{
    public int width = 3, height = 3;
    public bool[,] grid;
    public List<ItemInstance> items = new List<ItemInstance>();

    public Inventory(int w, int h)
    {
        width = w;
        height = h;
        grid = new bool[w, h];
    }

    public bool CanPlaceItem(ItemData item, Vector2Int pos)
    {
        foreach (var cell in item.shape)
        {
            Vector2Int checkPos = pos + cell;
            if (checkPos.x < 0 || checkPos.x >= width || checkPos.y < 0 || checkPos.y >= height || grid[checkPos.x, checkPos.y])
                return false;

        }
        return true;
    }

    public void PlaceItem(ItemInstance item)
    {
        if (!items.Contains(item))
            items.Add(item);

        foreach (var cell in item.data.shape)
        {
            Vector2Int pos = item.position + cell;
            if (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height)
            {
                grid[pos.x, pos.y] = true;
            }
            else Debug.LogError("Position not in grid " + pos);
        }

    }

    public void RemoveItem(ItemInstance item)
    {
        foreach (var cell in item.data.shape)
        {
            Vector2Int pos = item.position + cell;
            grid[pos.x, pos.y] = false;
        }
        items.Remove(item);

    }

    public Vector2Int GetPositionForItem(ItemInstance item)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (CanPlaceItem(item.data, pos))
                    return pos;
            }
        }
        return item.position;
    }

    public void Expand(int addWidth, int addHeight)
    {
        int newWidth = Mathf.Min(width + addWidth, 5);
        int newHeigth = Mathf.Min(height + addHeight, 5);
        bool[,] newGrid = new bool[newWidth, newHeigth];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                newGrid[x, y] = grid[x, y];
        grid = newGrid;
        width = newWidth;
        height = newHeigth;
    }

    public ItemInstance MergeItems(ItemInstance item1, ItemInstance item2, out ItemInstance[] itemsToDestroy)
    {
        itemsToDestroy = new ItemInstance[] { item1, item2 };

        if (item1.data.type != item2.data.type || item1.data.grade != item2.data.grade || item1.data.nextGrade == null)
        {
            itemsToDestroy = null;
            return null;
        }

        ItemInstance mergedItem = new ItemInstance
        {
            data = item1.data.nextGrade,
            dataName = item1.data.nextGrade.name,
            position = item1.position
        };

        RemoveItem(item1);
        RemoveItem(item2);

        if (CanPlaceItem(mergedItem.data, mergedItem.position))
        {
            Debug.Log("Item " + mergedItem.data + " placed at position " + mergedItem.position);
            return mergedItem;
        }
        else
        {
            Vector2Int newPos = GetPositionForItem(mergedItem);
            if (newPos != mergedItem.position)
            {
                mergedItem.position = newPos;
                if (CanPlaceItem(mergedItem.data, newPos))
                {
                    return mergedItem;
                }
            }
        }

        itemsToDestroy = null;
        return null;

    }
}