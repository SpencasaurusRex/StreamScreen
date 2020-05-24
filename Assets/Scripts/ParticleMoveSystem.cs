using Assets.Scripts.Transform;
using DefaultEcs;
using UnityEngine;

namespace DefaultNamespace
{
    public class ParticleMoveSystem
    {
        EntitySet particleSet;

        float t;
        
        public ParticleMoveSystem(World world)
        {
            particleSet = world.GetEntities().With<Translation>().With<Velocity>().AsSet();
        }

        public void Update()
        {
            foreach (var particle in particleSet.GetEntities())
            {
                var velocity = particle.Get<Velocity>();
                var translation = particle.Get<Translation>();
                var original = particle.Get<Vector3>();
                
                // Drag
                velocity.Value *= 1 - Time.deltaTime;

                const float yScaling = 2;
                
                // Outward force - stronger when further
                if (Random.Range(0, 100) > 90)
                {
                    var outwardForce = translation.Value * 4 * Time.deltaTime;
                    outwardForce.y *= yScaling;
                    velocity.Value += outwardForce;                    
                }
                
                // Outward force - stronger whe closer
                if (Random.Range(0, 100) > 90)
                {
                    var outwardForce = Mathf.Min(1 / translation.Value.magnitude, 1) * Time.deltaTime;
                    velocity.Value += translation.Value.normalized * outwardForce * 10;    
                }

                // Force towards original position
                velocity.Value += (original - translation.Value) * (Time.deltaTime * 20);
                
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