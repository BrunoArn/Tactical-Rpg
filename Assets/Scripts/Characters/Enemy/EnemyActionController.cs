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
        //manda pro endturn a funcao do comatmanager
        onTurnEnd = onTurnEndCallBack;
        //adiciona o meter e avisa que aumentou
        gridUnit.stats.AddMeter(gridUnit.stats.speed);
        if (gridUnit.stats.Meter >= gridUnit.stats.MeterMax)
        {
            StartTurn();
        }
        else
        {
            EndTurn();
        }
    }

    public void StartTurn()
    {
        StartCoroutine(SkipTurnRoutine());
    }

    public void BeforeEndTurn()
    {
        gridUnit.stats.AddMeter(-gridUnit.stats.MeterMax);
        EndTurn();
        
    }
    
    public void EndTurn()
    {
        onTurnEnd?.Invoke(); // manda pro manager que ta tudo bem
    }

    IEnumerator SkipTurnRoutine()
    {
        Debug.Log($"o {this.name} passou o turno");
        yield return new WaitForSeconds(secondToSKip);
        BeforeEndTurn();
        
    }

}
