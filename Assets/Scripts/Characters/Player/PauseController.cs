using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{

    [SerializeField] GameStateVariable gameState;
    private GameState previousGameState;

    private CombatControls controls;
    //referencia pro ultimo state do jogo

    void Awake()
    {
        controls = new CombatControls();
    }

    void OnEnable()
    {
        if (gameState != null)
        {
            gameState.OnValueChanged.AddListener(OnGameStateChanged);
        }

        controls.Ui.Pause.performed += OnPausePerformed;
        controls.Ui.Enable();
    }

    void OnDisable()
    {
        if (gameState != null)
        {
            gameState.OnValueChanged.RemoveListener(OnGameStateChanged);
        }

        controls.Ui.Pause.performed -= OnPausePerformed;
        controls.Ui.Disable();
    }

    void OnDestroy()
    {
        controls?.Dispose();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (gameState == null) return;

        if(gameState.CurrentState != GameState.Pause)
        {
            previousGameState = gameState.CurrentState;
            gameState.SetValue(GameState.Pause);
        }
        else
        {
            gameState.SetValue(previousGameState);
        }
        
    }
    //Lister if someone else changes to pause state
    private void OnGameStateChanged(GameState oldState, GameState newState)
    {
        if (newState == GameState.Pause)
        {
            previousGameState = oldState;
        }
    }
}
