using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentHealth;
    [SerializeField] private TextMeshProUGUI maxHealth;
    [SerializeField] private Health health;

    private void Awake()
    {
        if (!health)
        {
            Debug.LogError("ta sem referencia de health", this);
            enabled = true;
            return;
        }
        health.OnTakeDamage += UpdateUI;
        UpdateUI(health.CurrentHealth, health.MaxHealth);

    }

    private void OnDestroy()
    {
        if (!health)
        {
            //health.OnTakeDamage
        }
    }

    private void UpdateUI(int current, int max)
    {
        if (health.CurrentHealth >= 10) currentHealth.text = health.CurrentHealth.ToString();
        else currentHealth.text = $"0{health.CurrentHealth.ToString()}";
        maxHealth.text = health.MaxHealth.ToString();
    }
}
