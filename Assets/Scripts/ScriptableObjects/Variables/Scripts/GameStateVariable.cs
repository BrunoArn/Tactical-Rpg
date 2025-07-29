using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameStateVariable", menuName = "Variables/GameStateVariable")]
public class GameStateVariable : ScriptableObject
{
    public GameState CurrentState;
    public UnityEvent<GameState, GameState> OnValueChanged;

    public void SetValue(GameState newState)
    {
        if (newState == CurrentState) return;

        var oldState = CurrentState;
        CurrentState = newState;
        OnValueChanged.Invoke(oldState, newState);
    }
    
}
