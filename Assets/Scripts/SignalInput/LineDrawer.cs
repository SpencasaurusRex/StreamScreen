using System.Collections.Generic;
using System.Linq;
using Shapes;
using UnityEngine;

public class LineDrawer : MonoBehaviour, ISignalInput 
{
    public Range AmplitudeDomain;

    public Range SpaceDomainX;
    public Range SpaceDomainY;

    public ISignalOutput[] Outputs { get; set; }

    public bool DrawGizmos;

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
    
    // public bool Interpolate;
    
    Polyline line;

    void Start()
    {
        line = GetComponent<Polyline>();
    }

    List<Vector2> points = new List<Vector2>();
    
    void Update()
    {
        if (Outputs == null || Outputs.Length == 0) return;
        var totalSamples = Outputs.Max(o => o.SampleRate) * TimeLength;

        if (points.Count > totalSamples)
        {
            points.Clear();
        }
        while (points.Count < totalSamples)
        {
            points.Add(Vector2.zero);
        }
        
        float left = SpaceDomainX.Min;
        var xScale = SpaceDomainX.Size / totalSamples;

        Vector2 pos;
        int x = 0;
        var output = Outputs[0];
        FixedQueue<float> buffer = output.Buffer;
        for (int i = buffer.Index; i < buffer.Elements.Length; i++, x++)
        {
            pos = points[x];
            pos.x = x * xScale + left;
            pos.y = AmplitudeDomain.MapTo(SpaceDomainY, buffer.Elements[i]);
            points[x] = pos;
        }
        
        for (int i = 0; i < buffer.Index; i++, x++)
        {
            pos = points[x];
            pos.x = x * xScale + left;
            pos.y = AmplitudeDomain.MapTo(SpaceDomainY, buffer.Elements[i]);
            points[x] = pos;
        }

        line.SetPoints(points);
        
        lastTimeLength = timeLength;
    }
    
    void OnDrawGizmos()
    {
        if (!DrawGizmos || Outputs?.Length == 0) return;

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
        
            Gizmos.DrawLine(lastPos, pos);
            lastPos = pos;
        }
        
        for (int i = 0; i < buffer.Index; i++, x++)
        {
            pos.x = x * xScale + left;
            pos.y = AmplitudeDomain.MapTo(SpaceDomainY, buffer.Elements[i]);
        
            Gizmos.DrawLine(lastPos, pos);
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