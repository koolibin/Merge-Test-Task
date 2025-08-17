using UnityEngine;
using UnityEngine.UI;

public class MergeButton : MonoBehaviour
{
    public MainManager mainManager;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnMergeClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnMergeClick);
    }

    private void OnMergeClick()
    {
        mainManager.SwitchToMerge();
    }
}