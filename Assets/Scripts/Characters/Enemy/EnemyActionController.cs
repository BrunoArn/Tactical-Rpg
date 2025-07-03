using System;
using System.Collections;
using UnityEngine;

public class EnemyActionController : MonoBehaviour, ICombatUnit
{
    private System.Action onTurnEnd;
    [SerializeField] int secondToSKip = 3;

    public void StartTurn(Action onTurnEndCallBack)
    {
        onTurnEnd = onTurnEndCallBack;
        StartCoroutine(SkipTurnRoutine());
    }
    
    public void EndTurn()
    {
        Debug.Log("passei o turno");
        onTurnEnd?.Invoke(); // manda pro manager que ta tudo bem
    }

    IEnumerator SkipTurnRoutine()
    {
        yield return new WaitForSeconds(secondToSKip);
        EndTurn();
        
    }

}
