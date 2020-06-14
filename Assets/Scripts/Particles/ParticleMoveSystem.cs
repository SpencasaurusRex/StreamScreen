using Assets.Scripts.Transform;
using CSCore;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;
using DefaultEcs;
using UnityEngine;
using Object = System.Object;

namespace DefaultNamespace
{
    public class ParticleMoveSystem
    {
        EntitySet particleSet;
        float t;
        WasapiCapture capture;
        IWaveSource waveSource;
        byte[] buffer;
        FftProvider fft;
        
        const FftSize fftSize = FftSize.Fft4096;
        float[] fftData = new float[(int)fftSize];
        
        public ParticleMoveSystem(World world)
        {
            particleSet = world.GetEntities().With<Translation>().With<Velocity>().AsSet();
            capture = new WasapiLoopbackCapture();
            capture.Initialize();
            var soundInSource = new SoundInSource(capture);
            var source = soundInSource.ToSampleSource();

            fft = new FftProvider(source.WaveFormat.Channels, fftSize);
            
            var notificationSource = new SingleBlockNotificationStream(source);
            notificationSource.SingleBlockRead += SingleBlockRead;

            waveSource = notificationSource.ToWaveSource(16);
            buffer = new byte[waveSource.WaveFormat.BytesPerSecond / 2];

            soundInSource.DataAvailable += DataAvailable;

            // capture.DataAvailable += (sender, args) => DataAvailable(sender, args); 
            capture.Start();
        }

        public void SingleBlockRead(Object sender, SingleBlockReadEventArgs args)
        {
            fft.Add(args.Left, args.Right);
        }

        public void DataAvailable(Object sender, DataAvailableEventArgs args)
        {
            int read;
            while ((read = waveSource.Read(buffer, 0, buffer.Length)) > 0) ;
            Debug.Log("Read " + read);
        }

        public void Update()
        {

            if (fft.IsNewDataAvailable)
            {
                fft.GetFftData(fftData);
            }

            bool outwardNormalized = Random.Range(0, 100) == 99; 
            
            foreach (var particle in particleSet.GetEntities())
            {
                var velocity = particle.Get<Velocity>();
                var translation = particle.Get<Translation>().Value;
                var original = particle.Get<Anchor>().Value;
                
                // Drag
                velocity.Value *= (1 - Time.deltaTime * 10);

                const float yScaling = 1;
                
                // Outward force - stronger when further
                // if (Random.Range(0, 100) > 90)
                // {
                //     var outwardForce = translation * (1 * Time.deltaTime);
                //     outwardForce.y *= yScaling;
                //     velocity.Value += outwardForce;                    
                // }
                
                //Outward force - stronger whe closer
                // if (Random.Range(0, 100) > 90)
                // {
                //     var outwardForce = Mathf.Min(1 / translation.magnitude, 1) * Time.deltaTime;
                //     velocity.Value += translation.normalized * (outwardForce * 10);    
                // }
                
                // Outward force - normalized
                // if (outwardNormalized)
                // {
                //     var outwardForce = translation.normalized * (100 * Time.deltaTime);
                //     velocity.Value += outwardForce;
                // }

                
                
                // Force towards original position
                velocity.Value += (original - translation) * (Time.deltaTime * 40);
                
                // Random variation
                float x = Random.Range(-1f, 1f);
                float y = Random.Range(-1f, 1f) * yScaling;
                var randomForce = new Vector3(x, y) * (Time.deltaTime * 0.2f);
                velocity.Value += randomForce;
            }

            t += Time.deltaTime;
        }

        public void OnDestroy()
        {
            capture?.Dispose();
        }

    }
}