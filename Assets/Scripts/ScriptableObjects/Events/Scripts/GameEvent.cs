using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Events/GameEvent", fileName="NewGameEvent")]
public class GameEvent : ScriptableObject
{
    private readonly List<GameEventListener> eventListeners = new List<GameEventListener>();

    public void Raise()
    {
        for (int i = eventListeners.Count  -1; i >= 0; i--)
        {
            eventListeners[i].OnEventRaised();
        }
    }

    public void RegisterListerner(GameEventListener Listener)
    {
        if (!eventListeners.Contains(Listener))
        {
            eventListeners.Add(Listener);
        }
    }

    public void UnregisterListerner(GameEventListener Listener)
    {
        if (eventListeners.Contains(Listener))
        {
            eventListeners.Remove(Listener);
        }
    }
}
