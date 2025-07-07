using UnityEngine;

public interface ICombatUnit
{

    void BeforeStart(System.Action onTurnEndCallBack);
    void StartTurn();
    void BeforeEndTurn();
    void EndTurn();
}
