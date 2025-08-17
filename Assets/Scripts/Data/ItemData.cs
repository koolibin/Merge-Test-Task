using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Rifle, Pistol, Bomb, Coffe, Painkiller }
    public enum ActivationType { Auto, Manual }
    public enum Target { Nearest, Self }

    public ItemType type;
    public int grade = 1;
    public int attack;
    public int heal;
    public float cooldown;
    public Target target;
    public ActivationType activation = ActivationType.Auto;
    public List<Vector2Int> shape;
    public Sprite icon;
    public string description;
    public ItemData nextGrade;

    public Vector2Int GetShapeSize()
    {
        if (shape == null || shape.Count == 0)
        {
            return new Vector2Int(1, 1);
        }

        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var cell in shape)
        {
            minX = Mathf.Min(minX, cell.x);
            maxX = Mathf.Max(maxX, cell.x);
            minY = Mathf.Min(minY, cell.y);
            maxY = Mathf.Max(maxY, cell.y);
        }

        return new Vector2Int(maxX - minX + 1, maxY - minY + 1);
    }
}