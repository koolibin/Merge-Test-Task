using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public Slider healthBar; 
    public Transform itemsBar;
    public GameObject combatItemPrefab;
    public GameObject damageTextPrefab;
    //public TextMeshProUGUI damageText;
    public TextMeshProUGUI healthText;
    public ParticleSystem damageParticle;
    public ParticleSystem bombParticle;
    public ParticleSystem healParticle;
    private InventoryManager inventoryManager;
    private float health = 100f;
    private List<CombatItemView> combatItemViews = new List<CombatItemView>();
    private List<Coroutine> autoAttackCoroutines = new List<Coroutine>();

    void Awake()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        UpdateHealth(health);
    }


    private void OnDisable()
    {
        StopAllCoroutines();
        autoAttackCoroutines.Clear();

    }

    public void ResetStartCombat()
    {
        foreach (Transform child in itemsBar) Destroy(child.gameObject);
        combatItemViews.Clear();

        if (inventoryManager.inventory != null)
        {
            foreach (var item in inventoryManager.inventory.items)
            {
                GameObject itemObj = Instantiate(combatItemPrefab, itemsBar);
                CombatItemView view = itemObj.GetComponent<CombatItemView>();
                view.Setup(item);
                combatItemViews.Add(view);
                if (item.data.activation == ItemData.ActivationType.Auto)
                {
                    StartCoroutine(AutoAttack(view));
                }
            }
            
            
        }
    }

    IEnumerator AutoAttack(CombatItemView view)
    {
        var item = view.itemInstance;
        while (true)
        {
            DealDamage(item.data.attack);
            view.StartCooldown(item.data.cooldown);
            yield return new WaitForSeconds(item.data.cooldown);
        }
    }

    public void ManualActivate(ItemInstance item)
    {
        CombatItemView view = combatItemViews.Find(v => v.itemInstance == item);
        if (view == null || view.isOnCooldown) return;

        if (item.data.activation == ItemData.ActivationType.Manual)
        {
            
            if (item.data.target == ItemData.Target.Self)
            {
                health = Mathf.Min(health + item.data.heal, 150f);
                UpdateHealth(health);
                healParticle.Play();
            }
            else if (item.data.target == ItemData.Target.Nearest)
            {
                DealDamage(item.data.attack);
                bombParticle.Play();
            }
            
            view.StartCooldown(item.data.cooldown);
        }
    }

    void DealDamage(int damage)
    {
        Debug.Log("Damage: " + damage);
        damageParticle.Play();
        // Enemy HP not active
        GameObject damageTextObj = Instantiate(damageTextPrefab, healthBar.GetComponent<RectTransform>());
        TextMeshProUGUI damageText = damageTextObj.GetComponent<TextMeshProUGUI>();
        if (damageText != null)
        {
            damageText.text = $"-{damage}";
            RectTransform rt = damageText.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + Random.Range(-20, 20), rt.anchoredPosition.y);
            StartCoroutine(AnimateDamageText(damageTextObj, rt));
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component missing", damageTextObj);
            Destroy(damageTextObj);
        }
    }

    private IEnumerator AnimateDamageText(GameObject textObj, RectTransform rt)
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector2 startPos = rt.anchoredPosition;
        float moveDistance = 50f;
        CanvasGroup canvasGroup = textObj.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = textObj.AddComponent<CanvasGroup>();
        Destroy(textObj, duration * 2f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rt.anchoredPosition = startPos + new Vector2(0, moveDistance * t);
            canvasGroup.alpha = 1f - t;
            yield return null;
        }
        
    }

    private void UpdateHealth(float h)
    {
        healthBar.value = h / 150f;
        healthText.text = $"{h}";
    }
}
