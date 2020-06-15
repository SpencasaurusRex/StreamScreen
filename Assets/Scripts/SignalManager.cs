using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class SignalManager : MonoBehaviour
{
    public ISignalOutput[] Outputs;
    public ISignalInput[] Inputs;

    FixedQueue<float>[] Buffers;
    float maxTime;
    
    void Start()
    {
        CheckComponents();
    }

    void Update()
    {
        if (Outputs == null || Inputs == null) return;
        
        bool recreateBuffers = false;
        foreach (var output in Outputs)
        {
            if (output.SampleRateChanged)
            {
                recreateBuffers = true;
            }
        }

        foreach (var input in Inputs)
        {
            if (input.TimeLengthChanged)
            {
                recreateBuffers = true;
            }
        }
        
        if (recreateBuffers)
            Setup();
    }

    [Button]
    void CheckComponents()
    {
        var comps = GetComponents<Component>();
        
        var outputs = new List<ISignalOutput>();
        var inputs = new List<ISignalInput>();
        
        foreach (var comp in comps)
        {
            if (comp is ISignalOutput)
            {
                var output = comp as ISignalOutput;
                outputs.Add(output);
            }
            else if (comp is ISignalInput)
            {
                var input = comp as ISignalInput;
                inputs.Add(input);
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
        
        if (maxTime <= float.Epsilon) return;
        
        HashSet<int> sampleRates = new HashSet<int>();
        foreach (var o in Outputs)
        {
            if (o.SampleRate == 0) continue;
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

