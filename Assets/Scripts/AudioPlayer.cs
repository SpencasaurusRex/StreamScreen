using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public int SampleRate = 41000;
    public float TimeLength = 2;

    AudioClip clip;    
    int position;

    SignalOutput[] signals;
    
    void Start()
    {
        signals = GetComponents<SignalOutput>();
        int samples = (int)(SampleRate * TimeLength);
        AudioClip myClip = AudioClip.Create("MySinusoid", samples, 1, SampleRate, true, OnAudioRead, OnAudioSetPosition);
        AudioSource aud = GetComponent<AudioSource>();
        aud.clip = myClip;
        aud.Play();
    }

    void Update()
    {
        signals = GetComponents<SignalOutput>();
    }

    void OnAudioRead(float[] data)
    {
        for (int i = 0; i < data.Length; i++, position++)
        {
            data[i] = 0;
            float t = position / (float)SampleRate;
            
            foreach (var signal in signals)
            {
                // data[i] += signal.Evaluate(t);    
            }
            
            // data[i] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * 220 * position / SampleRate));
        }
    }

    void OnAudioSetPosition(int newPosition) {
        position = newPosition;
    }
}