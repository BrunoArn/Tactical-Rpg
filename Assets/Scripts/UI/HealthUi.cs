using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class HealthUi : MonoBehaviour
{
    [SerializeField] private Transform fillHealthBar;

    [SerializeField] Health health;

    void Awake()
    {
        if (fillHealthBar == null)
            Debug.Log($"MeterUI on {name} needs a fill Transform assigned");

        //assign to the take damage event
        if (health)
        {
            health.OnTakeDamage += SetHealthValue;
        }

    }

    void OnDestroy()
    {
        health.OnTakeDamage -= SetHealthValue;
    }

    public void SetHealthValue(int current, int max)
    {
        float t = (float)current / (float)max;
        t = Mathf.Clamp01(t);
        fillHealthBar.localScale = new Vector3(1f, t, 1f);
    }

    public void ShowHealthBar()
    {

    }

    public void HideHealthBar()
    {
        
    }
}
