using System;
using System.Collections;
using UnityEngine;

public class EnemyActionController : MonoBehaviour, ICombatUnit
{
    private System.Action onTurnEnd;
    [SerializeField] int secondToSKip = 3;
    private GridUnit gridUnit;

    [SerializeField] int meterMax = 100;
    private int meter = 0;

    void Awake()
    {
        gridUnit = GetComponent<GridUnit>();
    }

    public void BeforeStart(System.Action onTurnEndCallBack)
    {
        onTurnEnd = onTurnEndCallBack;
        meter += gridUnit.stats.speed;
        Debug.Log($"o {this.name} ta com {meter} de meter");
        if (meter >= meterMax)
        {
            Debug.Log($"o {this.name} agiu");
            meter -= meterMax;
            StartTurn();
        }
        else
        {
            Debug.Log($"o {this.name} nao deu");
            EndTurn();
        }
    }

    public void StartTurn()
    {
        StartCoroutine(SkipTurnRoutine());
    }
    
    public void EndTurn()
    {
        onTurnEnd?.Invoke(); // manda pro manager que ta tudo bem
    }

    IEnumerator SkipTurnRoutine()
    {
        Debug.Log($"o {this.name} passou o turno");
        yield return new WaitForSeconds(secondToSKip);
        EndTurn();
        
    }

}
