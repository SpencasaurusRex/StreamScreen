using System.Collections.Generic;
using CSCore;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;
using CSCore.Utils;
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
    public float yScale = 100;
    public float yOffset = -2;
    public Color VisualizerColor;

    public int TotalSamples;

    float scale = 1;

    FftProvider fft;
    
    const FftSize fftSize = FftSize.Fft4096;
    
    void Start()
    {
        line = GetComponent<LineRenderer>();
        leftChannel = new float[TotalSamples];
        
        capture = new WasapiLoopbackCapture();
        capture.Initialize();
        var soundInSource = new SoundInSource(capture);
        var source = soundInSource.ToSampleSource();
        
        fft = new FftProvider(source.WaveFormat.Channels, fftSize);
            
        stream = new SingleBlockNotificationStream(source);
        stream.SingleBlockRead += SingleBlockRead;

        waveSource = stream.ToWaveSource(16);
        buffer = new byte[waveSource.WaveFormat.BytesPerSecond / 2];

        soundInSource.DataAvailable += DataAvailable;

        capture.DataAvailable += (sender, args) => DataAvailable(sender, args); 
        capture.Start();
    }

    public void SingleBlockRead(System.Object sender, SingleBlockReadEventArgs args)
    {
        leftChannel[leftChannelIndex++] = args.Left;
        if (leftChannelIndex >= leftChannel.Length)
            leftChannelIndex %= leftChannel.Length;
    }

    public void DataAvailable(System.Object sender, DataAvailableEventArgs args)
    {
        Debug.Log("DataAvailable");
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
        
        
    }

    Complex[] fftData = new Complex[(int) fftSize];
    
    void Update()
    {
        if (!fft.IsNewDataAvailable) return;
        fft.GetFftData(fftData);

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
