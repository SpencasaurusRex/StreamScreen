using System;
using UnityEngine;

[Serializable]
public struct Range
{
    public float Min;
    public float Max;

    public float Size => Max - Min;
    public float MidPoint => (Min + Max) * 0.5f;
    
    public void Validate()
    {
        float min = Mathf.Min(Min, Max);
        Max = Mathf.Max(Min, Max);
        Min = min;
    }

    public float MapTo(Range other, float value)
    {
        float t = (value - Min) / Size;
        return value * other.Size + other.Min;
    }
}