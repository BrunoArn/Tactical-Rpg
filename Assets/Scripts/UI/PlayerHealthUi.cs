using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUi : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
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
        healthSlider.value = (float)current / max;
    }
}
