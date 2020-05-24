using Assets.Scripts.Transform;
using DefaultEcs;
using UnityEngine;

namespace DefaultNamespace
{
    public class ParticleCreationSystem
    {
        Game game;
        World world;
        EntitySet textureSet;
        Mesh particleMesh;
        Material particleMaterial;
        
        public ParticleCreationSystem(World world, Game game, Mesh particleMesh, Material particleMaterial)
        {
            textureSet = world.GetEntities().With<Texture2D>().AsSet();
            this.world = world;
            this.game = game;
            this.particleMesh = particleMesh;
            this.particleMaterial = particleMaterial;
        }

        public void Update()
        {
            float scaleInverse = 1f / game.Scale;
            foreach (var textureEntity in textureSet.GetEntities())
            {
                var texture = textureEntity.Get<Texture2D>();
                Vector3 offset = new Vector3(texture.width * scaleInverse * 0.5f, texture.height * scaleInverse * 0.5f);
                
                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        var color = texture.GetPixel(x, y);
                        if (color.r + color.g + color.b > 0.19f)
                        {
                            var position = new Vector3(x, y, 0) * scaleInverse - offset;
                            var entity = world.CreateEntity();
                            entity.Set(particleMesh);
                            entity.Set(new Translation { Value = position});
                            entity.Set(new Velocity { Value = Vector3.zero });
                            entity.Set(particleMaterial);
                            entity.Set(position);
                            entity.Set(new Scale { Value = Vector3.one * scaleInverse * 1.5f} );
                        }
                    }
                }
                
                textureEntity.Dispose();
            }
        }
    }
}