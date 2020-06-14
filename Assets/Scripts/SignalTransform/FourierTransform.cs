// using UnityEngine;
//
// public class FourierTransform : MonoBehaviour
// {
//     public int Samples = 2048;
//     public float SampleRate = 41000;
//     public Range FrequencyDomain;
//     public int FrequencyResolution = 400;
//
//     float t;
//     // float[] data1;
//     // float[] data2;
//     //
//     // float[] oldData;
//     // float[] newData;
//     FixedQueue<float> data;
//
//     float[] frequencies;
//
//     SignalOutput output;
//     const float TwoPi = Mathf.PI * 2;
//     
//     void Execute()
//     {
//         float frequencyRange = FrequencyDomain.Size;
//         var e = data.Elements;
//         for (int i = 0; i < FrequencyResolution; i++)
//         {
//             float frequency = FrequencyDomain.Min + frequencyRange * i / FrequencyResolution;
//             // float total = 0;
//             Vector2 total = Vector2.zero;
//             int j = 0;
//             for (int s = data.Index; s < e.Length; s++, j++)
//             {
//                 float sample = e[s]; 
//                 float cos = sample * Mathf.Cos(TwoPi * frequency * j / SampleRate);
//                 float sin = sample * Mathf.Sin(TwoPi * frequency * j / SampleRate);
//                 total.x += cos;
//                 total.y += sin;
//                 //total += sin;
//             }
//             for (int s = 0; s < data.Index; s++, j++)
//             {
//                 float sample = e[s]; 
//                 float cos = sample * Mathf.Cos(TwoPi * frequency * j / SampleRate);
//                 float sin = sample * Mathf.Sin(TwoPi * frequency * j / SampleRate);
//                 total.x += cos;
//                 total.y += sin;
//                 //total += sin;
//             }
//             total /= Samples;
//             frequencies[i] = total.magnitude - 0.1f;
//         }
//     }
//
//     void Start()
//     {
//         // data1 = new float[Samples];
//         // data2 = new float[Samples];
//         // oldData = data2;
//         // newData = data1;
//         data = new FixedQueue<float>(Samples);
//
//         frequencies = new float[FrequencyResolution];
//     }
//
//     void GetData()
//     {
//         int oldIndex = Mathf.CeilToInt(Time.deltaTime * SampleRate);
//         int newIndex = 0;
//
//         // Copy relevant data from last frame
//         // for (; oldIndex < Samples; oldIndex++, newIndex++)
//         // {
//         //     newData[newIndex] = oldData[oldIndex];
//         // }
//         
//         t += Time.deltaTime;
//         float endTime = t;
//         float startTime = t - Samples / SampleRate;
//
//         // Evaluate signals for the rest of the data
//         // for (; newIndex < Samples; newIndex++)
//         // {
//         //     float total = 0;
//         //     
//         //     
//         //     newData[newIndex] = total;
//         //     t = Mathf.Lerp(startTime, endTime, (float) newIndex / (Samples - 1));
//         // }
//
//         // Swap the pointers back
//         // if (newData == data1)
//         // {
//         //     newData = data2;
//         //     oldData = data1;
//         // }
//         // else
//         // {
//         //     newData = data1;
//         //     oldData = data2;
//         // }
//     }
//
//     void Update()
//     {
//         if (!output && TryGetComponent<SignalOutput>(out var sp))
//         {
//             output = sp;
//             output.OnDataAvailable += OnDataAvailable;
//         }
//         
//         GetData();
//         Execute();
//     }
//
//     void OnDataAvailable(float[] values, int length)
//     {
//         for (int i = 0; i < length; i++)
//         {
//             data.Write(values[i]);
//         }
//     }
//
//     public float XScale = 1;
//     public float YScale;
//     public Color Color;
//     public Color FrequencyColor;
//     
//     void OnDrawGizmos()
//     {
//         // if (data1 == null) return;
//         //
//         // Gizmos.color = Color;
//         // Vector2 lastPos = new Vector2(0, 0);
//         // Vector2 pos = Vector2.zero;
//         //
//         // float lastY = 0;
//         // for (int i = 0; i < data1.Length; i++)
//         // {
//         //     pos.x = i * XScale;
//         //     pos.y = data1[i] * YScale;
//         //
//         //     // if (Mathf.Abs(pos.y - lastY) > 0.15f && i > 0)
//         //     // {
//         //     //     Debug.Log(i);
//         //     // }
//         //
//         //     lastY = data1[i] * YScale;
//         //     
//         //     Gizmos.DrawLine(lastPos, pos);
//         //     lastPos = pos;
//         // }
//
//         // lastPos = new Vector2(0, 0);
//         // pos = Vector2.zero;
//         // for (int i = 0; i < data2.Length; i++)
//         // {
//         //     pos.x = i * XScale;
//         //     pos.y = data2[i] * YScale;
//         //     
//         //     Gizmos.DrawLine(lastPos, pos);
//         //     lastPos = pos;
//         // }
//
//         if (frequencies == null) return;
//
//         Gizmos.color = FrequencyColor;        
//         var lastPos = Vector2.zero;
//         var pos = Vector2.zero;
//         for (int i = 0; i < frequencies.Length; i++)
//         {
//             pos.x = i * XScale;
//             pos.y = frequencies[i] * YScale;
//             Gizmos.DrawLine(lastPos, pos);
//             lastPos = pos;
//         }
//     }
//     
//     void OnValidate()
//     {
//         // if (data1 != null && Samples != data1.Length)
//         // {
//         //     data1 = new float[Samples];
//         //     data2 = new float[Samples];
//         //     newData = data1;
//         //     oldData = data2;
//         // }
//
//         if (frequencies != null && FrequencyResolution != frequencies.Length)
//         {
//             frequencies = new float[FrequencyResolution];
//         }
//     }
// }