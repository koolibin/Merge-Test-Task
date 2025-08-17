using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CombatItemView : MonoBehaviour
{
    public ItemInstance itemInstance;
    private Image icon;
    private Image cooldownFill; 
    private Button button; 
    private CombatManager combatManager;
    public bool isOnCooldown { get; private set; } = false;

    void Awake()
    {
        icon = GetComponent<Image>();
        cooldownFill = transform.Find("CooldownFill").GetComponent<Image>(); 
        button = GetComponent<Button>();
        combatManager = FindFirstObjectByType<CombatManager>();
    }

    public void Setup(ItemInstance instance)
    {
        itemInstance = instance;
        ResetCooldown();
        icon.sprite = instance.data.icon;
        icon.preserveAspect = true;
        cooldownFill.fillAmount = 0;
        if (instance.data.activation == ItemData.ActivationType.Manual)
        {
            button.onClick.AddListener(() => combatManager.ManualActivate(itemInstance));
        }
        else
        {
            button.interactable = false;
        }
    }

    public void StartCooldown(float duration)
    {
        if (isOnCooldown) return;
        StartCoroutine(CooldownCoroutine(duration));
    }

    IEnumerator CooldownCoroutine(float duration)
    {
        isOnCooldown = true;
        button.interactable = false;
        float elapsed = 0;
        cooldownFill.fillAmount = 1;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cooldownFill.fillAmount = 1 - (elapsed / duration);
            yield return null;
        }
        ResetCooldown();
        
    }

    public void ResetCooldown()
    {
        isOnCooldown = false;
        cooldownFill.fillAmount = 0;
        button.interactable = true;
    }
}
