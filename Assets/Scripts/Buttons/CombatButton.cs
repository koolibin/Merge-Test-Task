using UnityEngine;
using UnityEngine.UI;

public class CombatButton : MonoBehaviour
{
    public MainManager mainManager;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCombatClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnCombatClick);
    }

    private void OnCombatClick()
    {
        mainManager.SwitchToCombat();
    }
}
