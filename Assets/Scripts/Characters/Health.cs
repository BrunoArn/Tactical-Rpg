using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int currentHealth = 10;
    [SerializeField] private int maxHealth = 10;

    //events to people handle
    public event Action<int, int> OnTakeDamage;
    public event Action OnDeath;

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        OnTakeDamage?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Debug.Log($"o camarada [{this.gameObject.name}] morreu");
            OnDeath?.Invoke();
        }
    }
}
