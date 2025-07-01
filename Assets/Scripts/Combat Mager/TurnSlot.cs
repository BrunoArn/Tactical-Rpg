using UnityEngine;

public class TurnSlot
{
    public GridUnit unit;
    public int order;

    public TurnSlot(GridUnit unit, int order)
    {
        this.unit = unit;
        this.order = order;
    }
}
