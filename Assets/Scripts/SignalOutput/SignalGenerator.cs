using Sirenix.OdinInspector;
using UnityEngine;

public class SignalGenerator : SerializedMonoBehaviour, ISignalOutput
{
    public SignalType Type;

    [Min(0f)]
    public float Amplitude;
    [Min(0f)]
    public float Frequency;
    
    #region SampleRate
    public int SampleRate
    {
        get => sampleRate;
        set => sampleRate = value;
    }
    public int sampleRate;
    int lastSampleRate;
    
    public bool SampleRateChanged => lastSampleRate != sampleRate;
    #endregion
    
    public FixedQueue<float> Buffer { get; set; }

    const float TwoPi = Mathf.PI * 2;
    float currentTime;
    
    const int BufferLength = 4096;

    void Update()
    {
        if (Buffer == null) return;
        
        int totalSamples = (int)(Time.deltaTime * SampleRate);
        float inverseSamples = 1f / totalSamples;

        int samples = Mathf.Min(totalSamples, BufferLength);
        switch (Type)
        {
            case SignalType.Sine:
                for (int i = 0; i < samples; i++)
                {
                    var t = currentTime + Time.deltaTime * i * inverseSamples;
                    Buffer.Write(Amplitude * Mathf.Sin(TwoPi * t * Frequency));
                }
                break;
            case SignalType.Cosine:
                for (int i = 0; i < samples; i++)
                {
                    var t = currentTime + Time.deltaTime * i * inverseSamples;
                    Buffer.Write(Amplitude * Mathf.Cos(TwoPi * t * Frequency));
                }
                break;
            case SignalType.SawTooth:
                for (int i = 0; i < samples; i++)
                {
                    var t = currentTime + Time.deltaTime * i * inverseSamples;
                    Buffer.Write(2 * Amplitude * (Frequency * t - Mathf.Floor(Frequency * t)) - Amplitude);
                }
                break;
            case SignalType.Triangle:
                for (int i = 0; i < samples; i++)
                {
                    var t = currentTime + Time.deltaTime * i * inverseSamples;
                    Buffer.Write(2 * Mathf.Abs(2 * Amplitude * (Frequency * t - Mathf.Floor(Frequency * t)) -
                                               Amplitude) - Amplitude);
                }
                break;
            case SignalType.Square:
                for (int i = 0; i < samples; i++)
                {
                    var t = currentTime + Time.deltaTime * i * inverseSamples;
                    Buffer.Write(Amplitude * Mathf.Sign(Mathf.Sin(TwoPi * t * Frequency)));
                }
                break;
            default:
                for (int i = 0; i < samples; i++)
                {
                    var t = currentTime + Time.deltaTime * i * inverseSamples;
                    Buffer.Write(0);
                }
                break;
        }
        
        currentTime += Time.deltaTime;
        lastSampleRate = sampleRate;
    }
}

public enum SignalType
{
    Sine,
    Cosine,
    SawTooth,
    Triangle,
    Square
}
