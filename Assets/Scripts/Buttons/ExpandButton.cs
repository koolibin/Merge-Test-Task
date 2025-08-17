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
        int addWidth;
        int addHeight;

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
    }

}
