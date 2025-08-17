using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPrefab;
    private GameObject currentPopup;


    public void ShowItemInfo(ItemData data)
    {
        if (currentPopup != null)
        {
            Destroy(currentPopup);
        }

        currentPopup = Instantiate(popupPrefab, transform);
        currentPopup.name = $"Popup_{data.name}";

        TextMeshProUGUI descriptionText = currentPopup.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        Button closeButton = currentPopup.transform.Find("CloseButton").GetComponent<Button>();

        descriptionText.text = data.description;

        closeButton.onClick.AddListener(ClosePopup);

        RectTransform popupRT = currentPopup.GetComponent<RectTransform>();
        popupRT.anchoredPosition3D = new Vector3(0, 0, 1);
    }

    public void ClosePopup()
    {
        if (currentPopup != null)
        {
            Destroy(currentPopup);
            currentPopup = null;
        }
    }

}