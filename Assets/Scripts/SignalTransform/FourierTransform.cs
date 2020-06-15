using System.Diagnostics;
using UnityEngine;

public class FourierTransform : MonoBehaviour, ISignalInput
{
    public Range FrequencyDomain;
    public int FrequencyResolution = 400;

    public ISignalOutput[] Outputs { get; set; }

    float[] frequencies;

    ISignalOutput output;
    const float TwoPi = Mathf.PI * 2;
    
    void Execute()
    {
        var stop = Stopwatch.StartNew();
        if (Outputs == null || Outputs.Length == 0) return;
        var data = Outputs[0].Buffer; 
            
        float frequencyRange = FrequencyDomain.Size;
        float[] e = data.Elements;
        
        int sampleRate = Outputs[0].SampleRate;
        int samples = (int)(sampleRate * TimeLength) ;
        int startIndex = data.Index + e.Length - samples;
        
        Vector2 total = Vector2.zero;
        for (int i = 0; i < FrequencyResolution; i++)
        {
            float frequency = FrequencyDomain.Min + frequencyRange * i / FrequencyResolution;
            // float total = 0;
            int j = 0;
            int s = startIndex;
            total.x = 0;
            total.y = 0;
            for (; s < e.Length; s++, j++)
            {
                float sample = e[s]; 
                float cos = sample * Mathf.Cos(TwoPi * frequency * j / sampleRate);
                float sin = sample * Mathf.Sin(TwoPi * frequency * j / sampleRate);
                total.x += cos;
                total.y += sin;
                //total += sin;
            }

            s = 0;
            if (startIndex > e.Length)
            {
                s = startIndex % e.Length; 
            }

            for (;s < data.Index; s++, j++)
            {
                float sample = e[s]; 
                float cos = sample * Mathf.Cos(TwoPi * frequency * j / sampleRate);
                float sin = sample * Mathf.Sin(TwoPi * frequency * j / sampleRate);
                total.x += cos;
                total.y += sin;
                //total += sin;
            }
            total /= samples;
            frequencies[i] = total.magnitude - 0.1f;
        }
        stop.Stop();
        UnityEngine.Debug.Log(stop.ElapsedMilliseconds);
    }

    void Start()
    {
        frequencies = new float[FrequencyResolution];
    }

    void Update()
    {
        Execute();
        lastTimeLength = timeLength;
    }
    
    public float XScale = 1;
    public float YScale;
    public Color Color;
    public Color FrequencyColor;
    
    void OnDrawGizmos()
    {
        // if (data1 == null) return;
        //
        // Gizmos.color = Color;
        // Vector2 lastPos = new Vector2(0, 0);
        // Vector2 pos = Vector2.zero;
        //
        // float lastY = 0;
        // for (int i = 0; i < data1.Length; i++)
        // {
        //     pos.x = i * XScale;
        //     pos.y = data1[i] * YScale;
        //
        //     // if (Mathf.Abs(pos.y - lastY) > 0.15f && i > 0)
        //     // {
        //     //     Debug.Log(i);
        //     // }
        //
        //     lastY = data1[i] * YScale;
        //     
        //     Gizmos.DrawLine(lastPos, pos);
        //     lastPos = pos;
        // }

        // lastPos = new Vector2(0, 0);
        // pos = Vector2.zero;
        // for (int i = 0; i < data2.Length; i++)
        // {
        //     pos.x = i * XScale;
        //     pos.y = data2[i] * YScale;
        //     
        //     Gizmos.DrawLine(lastPos, pos);
        //     lastPos = pos;
        // }

        if (frequencies == null) return;

        Gizmos.color = FrequencyColor;        
        var lastPos = Vector2.zero;
        var pos = Vector2.zero;
        for (int i = 0; i < frequencies.Length; i++)
        {
            pos.x = i * XScale;
            pos.y = frequencies[i] * YScale;
            Gizmos.DrawLine(lastPos, pos);
            lastPos = pos;
        }
    }
    
    void OnValidate()
    {
        // if (data1 != null && Samples != data1.Length)
        // {
        //     data1 = new float[Samples];
        //     data2 = new float[Samples];
        //     newData = data1;
        //     oldData = data2;
        // }

        if (frequencies != null && FrequencyResolution != frequencies.Length)
        {
            frequencies = new float[FrequencyResolution];
        }
    }

    #region TimeLength
    public float TimeLength
    {
        get => timeLength;
        set => timeLength = value;
    }
    
    public float timeLength;
    float lastTimeLength;
    public bool TimeLengthChanged => timeLength != lastTimeLength; 
    #endregion
}