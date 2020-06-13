using System;
using UnityEngine;

public class GizmosLine : MonoBehaviour
{
    public Range TimeDomain;
    public Range AmplitudeDomain;

    public Range SpaceDomainX;
    public Range SpaceDomainY;
    
    public float SampleRate;
    public bool Render;
    
    void OnDrawGizmos()
    {
        if (!Render) return;
        if (SampleRate == 0) return;
        
        var signalProviders = GetComponents<SignalProvider>();

        float increment = 1f / SampleRate;

        float left = TimeDomain.MapTo(SpaceDomainX, TimeDomain.Min);
        float mid = AmplitudeDomain.MapTo(SpaceDomainY, AmplitudeDomain.MidPoint);
        Vector2 lastPos = new Vector2(left, mid);
        
        Vector2 pos = Vector2.zero;
        for (float t = TimeDomain.Min; t < TimeDomain.Max; t += increment)
        {
            float amplitude = 0;
            foreach (var signalProvider in signalProviders)
            {
                amplitude += signalProvider.Evaluate(t);
            }
            
            pos.x = TimeDomain.MapTo(SpaceDomainX, t);
            pos.y = AmplitudeDomain.MapTo(SpaceDomainY, amplitude);

            Gizmos.DrawLine(lastPos, pos);
            lastPos = pos;
        }
    }

    void OnValidate()
    {
        SpaceDomainX.Validate();
        SpaceDomainY.Validate();
        AmplitudeDomain.Validate();
        TimeDomain.Validate();
        if (SampleRate < 0) SampleRate = 0;
        
    }
}