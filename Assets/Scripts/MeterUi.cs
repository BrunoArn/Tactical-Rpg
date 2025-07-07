using UnityEngine;

public class MeterUi : MonoBehaviour
{
    [SerializeField] private Transform fillBar;

    private UnitStats stats;

    void Awake()
    {
        stats = GetComponent<UnitStats>();
        if (fillBar == null)
            Debug.Log($"MeterUI on {name} needs a fill Transform assigned");
        stats.OnMeterChanged += RefreshBar;

    }

    void OnDestroy()
    {
        stats.OnMeterChanged -= RefreshBar;
    }


    private void RefreshBar(int current, int max)
    {
        float t = (float)current / (float)max ;
        t = Mathf.Clamp01(t);
        fillBar.localScale = new Vector3(1f, t, 1f);
        Debug.Log("refresh Bar updated");
    }
}
