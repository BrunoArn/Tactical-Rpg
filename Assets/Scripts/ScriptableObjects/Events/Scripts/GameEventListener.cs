using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public GameEvent Event;

    [Tooltip("Responde to invoke when event is raised")]
    public UnityEvent Response;

    public void OnEnable()
    {
        Event.RegisterListerner(this);
    }

    public void OnDisable()
    {
        Event.UnregisterListerner(this);
    }

    public void OnEventRaised()
    {
        Response.Invoke();
    }
}
