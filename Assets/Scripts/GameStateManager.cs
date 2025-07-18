using UnityEngine;

public enum GameState {Exploration, Combat, Pause }

public class GameStateManager : MonoBehaviour
{
    [SerializeField] GameStateVariable gameState;

    void OnEnable()
    {
        gameState.OnValueChanged.AddListener(OnGameStateChanged);
    }

    void OnDisable()
    {
        gameState.OnValueChanged.RemoveListener(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameState oldState, GameState newState)
    {
        Debug.Log("entrei na troca e vamos resolver");
        //to do change the state
        switch (oldState)
        {
            case GameState.Combat:
                //tira tudo
                break;
            case GameState.Exploration:
                ////tira tudo
                break;
            case GameState.Pause:
                ////tira tudo
                break;
        }
        switch (newState)
        {
            case GameState.Combat:
                //bota tudo
                break;
            case GameState.Exploration:
                //bota tudo
                break;
            case GameState.Pause:
                //bota tudo
                break;
        }
    }

    public void OnCombatRequest()
    {
        gameState.SetValue(GameState.Combat);
    }

    public void OnExplorationRequest()
    {
        gameState.SetValue(GameState.Exploration);
    }

    public void OnPauseRequest()
    {
        gameState.SetValue(GameState.Pause);
    }
}
