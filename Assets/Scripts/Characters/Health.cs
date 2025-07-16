using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int currentHealth = 10;
    [SerializeField] private int maxHealth = 10;

    public event Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Debug.Log($"o camarada [{this.gameObject.name}] morreu");
            OnDeath?.Invoke();
        }
    }
}
