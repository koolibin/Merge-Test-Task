using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    public InventoryManager inventoryManager;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnLoadClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnLoadClick);
    }

    private void OnLoadClick()
    {
        inventoryManager.LoadInventory();
    }
}
