using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class GizmosLine : MonoBehaviour, SignalInput 
{
    public Range AmplitudeDomain;

    public Range SpaceDomainX;
    public Range SpaceDomainY;

    public SignalOutput[] Outputs { get; set; }

    [ShowInInspector]
    public float TimeLength
    {
        get => timeLength;
        set
        {
            timeLength = value;
            // OnTimeLengthChanged?.Invoke();
        }
    }
    
    float timeLength;
    
    // public event TimeLengthChanged OnTimeLengthChanged;
    // public bool Subscribed => OnTimeLengthChanged != null;
    
    public bool Hide;
    public bool Interpolate;
    
    void OnDrawGizmos()
    {
        if (Outputs == null || Outputs.Length == 0) return;
        
        float left = SpaceDomainX.Min;
        float mid = AmplitudeDomain.MapTo(SpaceDomainY, AmplitudeDomain.MidPoint);
        Vector2 lastPos = new Vector2(left, mid);
        
        Vector2 pos = Vector2.zero;
        var totalSamples = Outputs.Max(o => o.SampleRate);
        var xScale = SpaceDomainX.Size / totalSamples;

        int x = 0;
        var output = Outputs[0];
        var buffer = output.Buffer;
        for (int i = buffer.Index; i < buffer.Elements.Length; i++, x++)
        {
            pos.x = x * xScale + left;
            pos.y = AmplitudeDomain.MapTo(SpaceDomainY, buffer.Elements[i]);

            if (!Hide) Gizmos.DrawLine(lastPos, pos);
            lastPos = pos;
        }

        for (int i = 0; i < buffer.Index; i++, x++)
        {
            pos.x = x * xScale + left;
            pos.y = AmplitudeDomain.MapTo(SpaceDomainY, buffer.Elements[i]);

            if (!Hide) Gizmos.DrawLine(lastPos, pos);
            lastPos = pos;
        }
    }

    void OnValidate()
    {
        SpaceDomainX.Validate();
        SpaceDomainY.Validate();
        AmplitudeDomain.Validate();
        if (TimeLength < 0) TimeLength = 0;
        
        // TODO Tell manager
    }
}