using Assets.Scripts.Transform;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Rendering
{
    public class RenderingSystem
    {
        World world;
        EntitySet meshSet;

        public RenderingSystem(World world)
        {
            this.world = world;
            meshSet = world.GetEntities().With<Mesh>().With<Material>().With<LocalToWorld>().AsSet();
        }

        public void Update()
        {
            foreach (var entity in meshSet.GetEntities())
            {
                var mesh = entity.Get<Mesh>();
                var material = entity.Get<Material>();
                var transform = entity.Get<LocalToWorld>().Matrix;

                Render(mesh, material, transform);
            }
        }

        void Render(Mesh mesh, Material material, Matrix4x4 transform)
        {
            Graphics.DrawMesh(mesh, transform, material, 0, null);
            // TODO
            //Graphics.DrawMeshInstanced(mesh, );
        }
    }
}
