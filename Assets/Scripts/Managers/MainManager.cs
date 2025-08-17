using UnityEngine;

public class MainManager : MonoBehaviour
{
    public enum GameStage { Merge, Combat};
    public GameStage currentStage = GameStage.Merge;

    public GameObject combatPanel;
    public GameObject mergePanel;
    public CombatManager combatManager;
    public InventoryManager inventoryManager;
    void Start()
    {
        mergePanel.SetActive(currentStage == GameStage.Merge);
        combatPanel.SetActive(currentStage == GameStage.Combat);
    }


    public void SwitchToCombat()
    {
        inventoryManager.SaveInventory();
        currentStage = GameStage.Combat;
        mergePanel.SetActive(false);
        combatPanel.SetActive(true);
        combatManager.ResetStartCombat();
    }

    public void SwitchToMerge()
    {
        currentStage = GameStage.Merge;
        combatPanel.SetActive(false);
        mergePanel.SetActive(true);
    }
}
