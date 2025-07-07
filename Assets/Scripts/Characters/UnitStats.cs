using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class UnitStats : MonoBehaviour
{
    public int maxHP = 10;
    public int currentHp = 10;
    public int speed = 10;
    public int attack = 1;
    public int defense = 1;

    private int meter = 0;
    private int meterMax = 100;

    public event Action<int, int> OnMeterChanged;

    public void AddMeter(int delta)
    {
        meter += delta;
        //Debug.Log($"the Unit [{name}] has {meter}/{meterMax}");
        OnMeterChanged?.Invoke(meter, meterMax);
    }

//read onlyy
    public int Meter => meter;
    public int MeterMax => meterMax;

}
