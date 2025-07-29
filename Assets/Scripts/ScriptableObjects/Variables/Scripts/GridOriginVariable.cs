using UnityEngine;

[CreateAssetMenu(fileName = "GridOriginVariable", menuName = "Variables/GridOriginVariable")]
public class GridOriginVariable : ScriptableObject
{
    [SerializeField] Vector2Int value;
    //read only 
    public Vector2Int Value => value;

    public void SetValue(Vector2Int newValue)
    {
        value = newValue;
    }
}
