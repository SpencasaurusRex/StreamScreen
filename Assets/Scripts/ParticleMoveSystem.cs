using Assets.Scripts.Transform;
using CSCore.SoundIn;
using DefaultEcs;
using UnityEngine;

namespace DefaultNamespace
{
    public class ParticleMoveSystem
    {
        EntitySet particleSet;
        float t;
        // WasapiCapture capture;
        public ParticleMoveSystem(World world)
        {
            particleSet = world.GetEntities().With<Translation>().With<Velocity>().AsSet();
            // capture = new WasapiLoopbackCapture();
            // capture.Initialize();
            // capture.DataAvailable += (sender, args) => DataAvailable(sender, args); 
            // capture.Start();
        }
        
        // public void DataAvailable(System.Object sender, DataAvailableEventArgs args)
        // {
        //     Debug.Log(args.Data.Length + " " + args.Offset + " " + args.ByteCount);
        // }

        public void Update()
        {

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
    }
}