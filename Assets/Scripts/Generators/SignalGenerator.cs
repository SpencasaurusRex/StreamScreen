using UnityEngine;

public class SignalGenerator : SignalProvider
{
    public SignalType Type;

    public float Amplitude;
    public float Frequency;

    const float TwoPi = Mathf.PI * 2;
    
    public override float Evaluate(float t)
    {
        switch (Type)
        {
            case SignalType.Sine:
                return Amplitude * Mathf.Sin(TwoPi * t * Frequency);
            case SignalType.SawTooth:
                return 2 * Amplitude * (Frequency * t - Mathf.Floor(Frequency * t)) - Amplitude;
            case SignalType.Triangle:
                return 2 * Mathf.Abs(2 * Amplitude * (Frequency * t - Mathf.Floor(Frequency * t)) - Amplitude) - Amplitude;
            case SignalType.Square:
                return Amplitude * Mathf.Sign(Mathf.Sin(TwoPi * t * Frequency));
            default:
                return 0;
        }
    }

    void OnValidate()
    {
        if (Amplitude < 0) Amplitude = 0;
    }
}

public enum SignalType
{
    Sine,
    SawTooth,
    Triangle,
    Square
}
