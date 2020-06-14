using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class SignalManager : MonoBehaviour
{
    public SignalOutput[] Outputs;
    public SignalInput[] Inputs;

    public int SampleRate;
    public int TimeLength;
    
    FixedQueue<float>[] Buffers;
    float maxTime;
    
    void Start()
    {
        Setup();
    }

    [Button]
    void CheckComponents()
    {
        var comps = GetComponents<Component>();
        
        var outputs = new List<SignalOutput>();
        var inputs = new List<SignalInput>();
        
        foreach (var comp in comps)
        {
            if (comp is SignalOutput)
            {
                var output = comp as SignalOutput;
                outputs.Add(output);
                output.SampleRate = SampleRate;
                // if (!output.Subscribed)
                //     output.OnSampleRateChanged += Setup;
            }
            else if (comp is SignalInput)
            {
                var input = comp as SignalInput;
                inputs.Add(input);
                input.TimeLength = TimeLength;
                // if (!input.Subscribed)
                // {
                //     input.OnTimeLengthChanged += Setup;
                // }
            }
        }
        
        Outputs = outputs.ToArray();
        Inputs = inputs.ToArray();
        
        Debug.Log($"{Outputs.Length} Outputs | {Inputs.Length} Inputs");
        Setup();
    }

    void Setup()
    {
        if (Inputs == null || Outputs == null) return;
        maxTime = Inputs.Max(input => input.TimeLength);
        HashSet<int> sampleRates = new HashSet<int>();
        foreach (var o in Outputs)
        {
            sampleRates.Add(o.SampleRate);
        }

        Buffers = new FixedQueue<float>[sampleRates.Count];
        
        int i = 0;
        foreach (var sampleRate in sampleRates)
        {
            int samples = (int)(sampleRate * maxTime);
            var buffer = new FixedQueue<float>(samples); 
            Buffers[i++] = buffer;

            foreach (var output in Outputs)
            {
                if (output.SampleRate == sampleRate)
                {
                    Debug.Log("Setting the buffer");
                    output.Buffer = buffer;
                }
            }
        }

        foreach (var input in Inputs)
        {
            input.Outputs = Outputs;
        }
    }

    void OnValidate()
    {
        if (Inputs == null || Outputs == null) return;
        if (Math.Abs(Inputs.Max(input => input.TimeLength) - maxTime) > float.Epsilon)
            Setup();
    }
}

