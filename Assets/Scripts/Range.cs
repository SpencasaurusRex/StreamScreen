using System;
using UnityEngine;

[Serializable]
public struct Range
{
    public float Min;
    public float Max;

    public float Size => Max - Min;
    public float MidPoint => (Min + Max) * 0.5f;

    public Range(float min, float max)
    {
        Min = min;
        Max = min;
    }

    public void Validate()
    {
        float min = Mathf.Min(Min, Max);
        Max = Mathf.Max(Min, Max);
        Min = min;
    }

    public float MapTo(Range other, float value)
    {
        float t = (value - Min) / Size;
        return t * other.Size + other.Min;
    }
}