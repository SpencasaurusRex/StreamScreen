using System;
using System.Collections.Generic;
using CSCore;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;
using CSCore.Streams.Effects;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Visualizer : MonoBehaviour
{
    public Camera camera;
    public LineRenderer line;
    
    WasapiCapture capture;
    float[] leftChannel;
    int leftChannelIndex;
    
    Queue<float> rightChannel = new Queue<float>();
    SingleBlockNotificationStream stream;
    IWaveSource waveSource;
    byte[] buffer;
    
    public float SamplesPerUnit = 10000;
    public float xScale = 100;
    public float yScale = 100;
    public float yOffset = -2;
    public Color VisualizerColor;
    public int Samples = 20;
    public int LowerSampleRange = 500;
    public int UpperSampleRange = 1000;
    
    public int TotalSamples;

    float scaleVelocity = 0;
    float scale = 1;
    public float SampleScale;

    public int PersistenSampleLength = 5;
    public float PersistenSampleXScale = 0.1f;
    public float PersistenSampleYScale = 10f;
    public int PersSampleLowerIndex = 8;
    public int PersSampleUpperIndex = 136;

    public float pitchShiftFactor = 5;
    
    public int IndexSpread = 5;
    public float Factor = 0.5f;
    public float Threshold = 0.1f;

    PitchShifter pitchShifter;
    FftTransform fft1;
    FftProvider fft2;

    public Color Color1;
    public Color Color2;
    public Color Color3;
    
    public int fftSize = (int)FftSize.Fft16384 / 2;
    float[] smoothedSamples;
    
    void Start()
    {
        fftData = new float[fftSize];

        persistentSamples = new FixedQueue<float>[PersSampleUpperIndex - PersSampleLowerIndex];
        smoothedSamples = new float[persistentSamples.Length];
        for (int i = 0; i < persistentSamples.Length; i++)
        {
            persistentSamples[i] = new FixedQueue<float>(PersistenSampleLength);
        }

        line = GetComponent<LineRenderer>();
        leftChannel = new float[TotalSamples];
        
        capture = new WasapiLoopbackCapture();
        capture.Initialize();
        var soundInSource = new SoundInSource(capture);
        var source = soundInSource.ToSampleSource().AppendSource(x => new PitchShifter(x), out pitchShifter);
        
        fft1 = new FftTransform(source.WaveFormat.Channels, fftSize);
        fft2 = new FftProvider(source.WaveFormat.Channels, FftSize.Fft2048);

        stream = new SingleBlockNotificationStream(pitchShifter);
        stream.SingleBlockRead += SingleBlockRead;

        waveSource = stream.ToWaveSource(16);
        buffer = new byte[waveSource.WaveFormat.BytesPerSecond / 2];

        soundInSource.DataAvailable += DataAvailable;

        capture.DataAvailable += (sender, args) => DataAvailable(sender, args); 
        capture.Start();
    }

    public void SingleBlockRead(System.Object sender, SingleBlockReadEventArgs args)
    {
        fft1.Add(args.Left, args.Right);
        fft2.Add(args.Left, args.Right);
        
        leftChannel[leftChannelIndex++] = args.Left;
        if (leftChannelIndex >= leftChannel.Length)
            leftChannelIndex %= leftChannel.Length;
    }

    public void DataAvailable(System.Object sender, DataAvailableEventArgs args)
    {
        int read;
        while ((read = waveSource.Read(buffer, 0, buffer.Length)) > 0) ;
    } 

    void OnDrawGizmos()
    {
        // if (leftChannel == null) return;
        // if (!camera) return;
        // Gizmos.color = VisualizerColor;
        //
        // Vector3 lastLeftPos = Vector3.zero;
        // // leftChannel.CopyTo(samples, 0);
        //
        // int relativeIndex = 0;
        //
        // Vector3 camOffset = (Vector2) camera.transform.position + Vector2.up * yOffset;
        // float totalLength = camera.aspect * camera.orthographicSize * 2;
        // float xScale = totalLength / leftChannel.Length;
        //
        // lastLeftPos = new Vector3(-totalLength / 2, 0) + camOffset ;
        // // Left side to current
        // for (int i = leftChannelIndex + 1; i < leftChannel.Length; i++, relativeIndex++)
        // {
        //     float x = relativeIndex * xScale - totalLength / 2;
        //     float y = leftChannel[i] * yScale + yOffset;
        //     Vector3 pos = new Vector3(x, y, 0) + camOffset;
        //     
        //     Gizmos.DrawLine(lastLeftPos, pos);
        //     lastLeftPos = pos;
        // }
        // // From current to right
        // lastLeftPos = new Vector3(relativeIndex * xScale - totalLength / 2, 0) + camOffset;
        // for (int i = 0; i < leftChannelIndex; i++, relativeIndex++)
        // {
        //     float x = relativeIndex * xScale - totalLength / 2;
        //     float y = leftChannel[i] * yScale + yOffset;
        //     Vector3 pos = new Vector3(x, y, 0) + camOffset;
        //     
        //     Gizmos.DrawLine(lastLeftPos, pos);
        //     lastLeftPos = pos;
        // }

        Gizmos.color = Color1;
        if (fftData == null) return;
        Vector3 lastPos = Vector3.zero;
        for (int i = 0; i < fftData.Length; i++)
        {

            // var x = fftData[i].Real * xScale;
            // var y = fftData[i].Imaginary * yScale;
            float t = (float) i / fftData.Length;

            var x = i * xScale;
            var scalar = 1; //(t * 2f + 0.5f);
            var y = fftData[i] * yScale * scalar;

            Vector3 pos = new Vector3(x, y, 0);

            Gizmos.DrawLine(lastPos, pos);
            lastPos = pos;
        }

        lastPos = Vector3.zero;
        Gizmos.color = Color2;
        for (int i = 0; i < persistentSamples?.Length; i++)
        {
            smoothedSamples[i] = 0;
            // float average = 0;
            
            var q = persistentSamples[i];
            var l = q.Elements.Length;
            for (int j = 0; j < l; j++)
            {
                // average += q.Elements[j];
                smoothedSamples[i] += q.Elements[j];
            }
            // average /= l;
            smoothedSamples[i] /= l;
            
            var pos = new Vector3(i * PersistenSampleXScale, smoothedSamples[i] * PersistenSampleYScale, 0);
            Gizmos.DrawLine(lastPos, pos);
            lastPos = pos;
        }

        lastPos = Vector3.zero;
        Gizmos.color = Color3;
        for (int i = 0; i < persistentSamples?.Length; i++)
        {
            // Neighbor smoothing
            float smoothedTotal = 0;
            for (int j = Math.Max(0, i - IndexSpread); j <= Math.Min(persistentSamples.Length - 1, i + IndexSpread); j++)
            {
                // Thresholding
                float f = 1;
                if (smoothedSamples[j] < Threshold) f = 0.5f;
                int indexDistance = Math.Abs(j - i);
                float distanceFactor = Mathf.Pow(Factor, indexDistance) * f;
                
                smoothedTotal += smoothedSamples[j] * distanceFactor;
            }

            // Y squashing
            var y = Mathf.Log(smoothedTotal + 1);
            var pos = new Vector3(i * PersistenSampleXScale, y * PersistenSampleYScale, 0);
            Gizmos.DrawLine(lastPos, pos);
            lastPos = pos;
        }
    }

    float[] fftData;
    float[] fftData2 = new float[(int) FftSize.Fft2048];
    FixedQueue<float>[] persistentSamples;
    
    void Update()
    {
        pitchShifter.PitchShiftFactor = pitchShiftFactor;
        
        if (fft1.IsNewDataAvailable)
        {
            fft1.GetFftData(fftData);
            for (int i = PersSampleLowerIndex; i < PersSampleUpperIndex; i++)
            {
                var data = fftData[i];
                persistentSamples[i - PersSampleLowerIndex].Write(data);
            }
        }

        // if (fft2.IsNewDataAvailable)
        //     fft2.GetFftData(fftData2);


        float total = 0;
        int range = UpperSampleRange - LowerSampleRange;
        int increase = Math.Max(1, range / Samples);
        float numOfSamples = range / increase;

        // int highestIndex = 0;
        // float highestValue = 0;
        
        for (int i = LowerSampleRange; i < UpperSampleRange; i += increase)
        {
            // float scalar = 1;
            // total += fftData[i] * SampleScale / numOfSamples * scalar;

            // if (fftData[i] > highestValue)
            // {
            //     highestValue = fftData[i];
            //     highestIndex = i;
            // }
        }
        // Debug.Log(highestIndex);

        scaleVelocity = total * 1.5f;
        scaleVelocity += (-scale * 8 + 1);
        scale += scaleVelocity * Time.deltaTime;
        
        transform.localScale = Vector3.one * scale;
        
        // if (leftChannel == null) return;
        // if (!camera) return;
        //
        // Vector3 lastLeftPos = Vector3.zero;
        //
        // int relativeIndex = 0;
        //
        // Vector3 camOffset = (Vector2) camera.transform.position + Vector2.up * yOffset;
        // float totalLength = camera.aspect * camera.orthographicSize * 2;
        // float xScale = totalLength / leftChannel.Length;
        //
        // lastLeftPos = new Vector3(-totalLength / 2, 0) + camOffset ;
        // // Left side to current
        // for (int i = leftChannelIndex + 1; i < leftChannel.Length; i++, relativeIndex++)
        // {
        //     float x = relativeIndex * xScale - totalLength / 2;
        //     float y = leftChannel[i] * yScale + yOffset;
        //     Vector3 pos = new Vector3(x, y, 0) + camOffset;
        //     
        //     
        //     lastLeftPos = pos;
        // }
        // // From current to right
        // lastLeftPos = new Vector3(relativeIndex * xScale - totalLength / 2, 0) + camOffset;
        // for (int i = 0; i < leftChannelIndex; i++, relativeIndex++)
        // {
        //     float x = relativeIndex * xScale - totalLength / 2;
        //     float y = leftChannel[i] * yScale + yOffset;
        //     Vector3 pos = new Vector3(x, y, 0) + camOffset;
        //     
        //     
        //     lastLeftPos = pos;
        // }
    }

    void OnApplicationQuit()
    {
        capture?.Dispose();
    }
}
