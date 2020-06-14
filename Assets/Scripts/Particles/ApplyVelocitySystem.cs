using Assets.Scripts.Transform;
using DefaultEcs;
using UnityEngine;

namespace DefaultNamespace
{
    public class ApplyVelocitySystem
    {
        EntitySet moverSet;
        
        public ApplyVelocitySystem(World world)
        {
            moverSet = world.GetEntities().With<Velocity>().With<Translation>().AsSet();
        }

        public void Update()
        {
            foreach (var mover in moverSet.GetEntities())
            {
                var velocity = mover.Get<Velocity>();
                var translation = mover.Get<Translation>();

                translation.Value.x += velocity.Value.x * Time.deltaTime;
                translation.Value.y += velocity.Value.y * Time.deltaTime;
            }
        }
    }
}