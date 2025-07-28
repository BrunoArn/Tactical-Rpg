using UnityEngine;
using UnityEngine.Events;

public class GameStateListener : MonoBehaviour
{
    [System.Serializable]
    public struct StateResponse
    {
        public GameState state;
        public UnityEvent onEnter;
        public UnityEvent onExit;
    }

    [Tooltip("Reference to the shared GameStateVariable SO")]
    [SerializeField] private GameStateVariable gameStateVariable;

    [Tooltip("Configure responses for each state change")]
    [SerializeField] private StateResponse[] responses;

    private void OnEnable()
    {
        // Subscribe to the SO event
        if (gameStateVariable != null)
            gameStateVariable.OnValueChanged.AddListener(HandleStateChanged);
    }

    private void OnDisable()
    {
        // Unsubscribe
        if (gameStateVariable != null)
            gameStateVariable.OnValueChanged.RemoveListener(HandleStateChanged);
    }
    
     private void HandleStateChanged(GameState oldState, GameState newState)
    {
        // Chama tudo que precisar sair
        foreach (var r in responses)
        {
            if (r.state == oldState && r.onExit != null)
                r.onExit.Invoke();
        }

        // Chama tudo que precisa ficar ONlines
        foreach (var r in responses)
        {
            if (r.state == newState && r.onEnter != null)
                r.onEnter.Invoke();
        }
    }
}
