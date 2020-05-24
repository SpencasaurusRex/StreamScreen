using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Transform
{
    public class TransformWriteSystem
    {
        World world;
        EntitySet translateSet;
        EntitySet translateRotateSet;
        EntitySet translateScaleSet;
        EntitySet translateRotateScaleSet;
        EntitySet missingLocalToWorldSet;
        
        public TransformWriteSystem(World world)
        {
            this.world = world;
            translateSet = world.GetEntities().With<Translation>().Without<Rotation>().Without<Scale>().With<LocalToWorld>().AsSet();
            translateRotateSet = world.GetEntities().With<Translation>().With<Rotation>().Without<Scale>().With<LocalToWorld>().AsSet();
            translateScaleSet = world.GetEntities().With<Translation>().Without<Rotation>().With<Scale>().With<LocalToWorld>().AsSet();
            translateRotateScaleSet = world.GetEntities().With<Translation>().With<Rotation>().With<Scale>().With<LocalToWorld>().AsSet();

            missingLocalToWorldSet = world.GetEntities().With<Translation>().Without<LocalToWorld>().AsSet();
        }

        public void Update()
        {
            foreach (var entity in missingLocalToWorldSet.GetEntities())
            {
                entity.Set(new LocalToWorld());
            }

            foreach (var entity in translateSet.GetEntities())
            {
                var translation = entity.Get<Translation>();
                var localToWorld = entity.Get<LocalToWorld>();

                localToWorld.Matrix = Matrix4x4.Translate(translation.Value);
            }

            foreach (var entity in translateRotateSet.GetEntities())
            {
                var translation = entity.Get<Translation>();
                var rotation = entity.Get<Rotation>();
                var localToWorld = entity.Get<LocalToWorld>();

                localToWorld.Matrix = Matrix4x4.TRS(translation.Value, rotation.Value, Vector3.one);
            }

            foreach (var entity in translateScaleSet.GetEntities())
            {
                var translation = entity.Get<Translation>();
                var scale = entity.Get<Scale>();
                var localToWorld = entity.Get<LocalToWorld>();

                localToWorld.Matrix = Matrix4x4.TRS(translation.Value, Quaternion.identity, scale.Value);
            }

            foreach (var entity in translateRotateScaleSet.GetEntities())
            {
                var translation = entity.Get<Translation>();
                var rotation = entity.Get<Rotation>();
                var scale = entity.Get<Scale>();
                var localToWorld = entity.Get<LocalToWorld>();

                localToWorld.Matrix = Matrix4x4.TRS(translation.Value, rotation.Value, scale.Value);
            }
        }
    }
}
