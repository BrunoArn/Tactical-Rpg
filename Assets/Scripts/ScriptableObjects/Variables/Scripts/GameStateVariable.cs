using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameStateVariable", menuName = "Variables/GameStateVariable")]
public class GameStateVariable : ScriptableObject
{
    public GameState CurrentState;
    public GameState PreviousState;
    public UnityEvent<GameState, GameState> OnValueChanged;

    public void SetValue(GameState newState)
    {
        if (newState == CurrentState) return;

        PreviousState = CurrentState;
        CurrentState = newState;
        OnValueChanged.Invoke(PreviousState, newState);
    }
    
}
