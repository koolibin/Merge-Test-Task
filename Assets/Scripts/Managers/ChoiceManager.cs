using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
    public GameObject choiceItemPrefab;
    public Transform choiceItemContainer;
    public ItemData[] availableItems;
    private List<GameObject> choiceItems = new List<GameObject>();

    void Start()
    {
        GenerateChoiceItems();
    }

    public void GenerateChoiceItems()
    {
        foreach (var item in choiceItems)
            Destroy(item);
        choiceItems.Clear();

        for (int i = 0; i < 3; i++)
        {
            ItemData randomItem = availableItems[Random.Range(0, availableItems.Length)];
            GameObject itemObj = Instantiate(choiceItemPrefab, choiceItemContainer);
            ChoiceItemView view = itemObj.GetComponent<ChoiceItemView>();
            view.Setup(randomItem);
            choiceItems.Add(itemObj);
        }
    }
}