using System;
using System.Collections;
using UnityEngine;

public class EnemyActionController : MonoBehaviour, ICombatUnit
{
    private System.Action onTurnEnd;
    [SerializeField] int secondToSKip = 3;
    private GridUnit gridUnit;

    void Awake()
    {
        gridUnit = GetComponent<GridUnit>();
    }

    public void BeforeStart(System.Action onTurnEndCallBack)
    {
        onTurnEnd = onTurnEndCallBack;
        gridUnit.stats.meter += gridUnit.stats.speed;
        Debug.Log($"o {this.name} ta com {gridUnit.stats.meter} de meter");
        if (gridUnit.stats.meter >= gridUnit.stats.meterMax)
        {
            Debug.Log($"o {this.name} agiu");
            gridUnit.stats.meter -= gridUnit.stats.meterMax;
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
