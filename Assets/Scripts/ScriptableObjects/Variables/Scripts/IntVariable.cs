using UnityEngine;
using UnityEngine.UIElements.Experimental;

[CreateAssetMenu(fileName = "IntVariable", menuName = "Variables/IntVariable")]
public class IntVariable : ScriptableObject
{
    [SerializeField] private int value;

    public int Value => value;

    public void SetValue(int newValue)
    {
        this.value = newValue;
    }
}
