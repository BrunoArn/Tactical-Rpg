using UnityEngine;

public interface ICombatUnit
{
    void StartTurn(System.Action onTurnEndCallBack);
    void EndTurn();
}
