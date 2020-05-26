using System.Collections.Generic;
using System.Linq;
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
        EntitySet globalSet;
        Mesh particleMesh;
        Material particleMaterial;

        public ParticleCreationSystem(World world, Game game, Mesh particleMesh, Material particleMaterial)
        {
            textureSet = world.GetEntities().With<Texture2D>().AsSet();
            globalSet = world.GetEntities().With<Global>().AsSet();
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
                int totalPoints = 0;

                var texture = textureEntity.Get<Texture2D>();
                Vector3 offset = new Vector3(texture.width * scaleInverse * 0.5f, texture.height * scaleInverse * 0.5f);

                List<Anchor>[] configurations = new List<Anchor>[2]; 
                configurations[0] = new List<Anchor>(); // Top
                configurations[1] = new List<Anchor>(); // Bottom

                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        var color = texture.GetPixel(x, y);
                        if (color.r + color.g + color.b > 0.19f)
                        {
                            // for (int i = 0; i < 2; i++)
                            {
                                var position = new Vector3(x, y, 0) * scaleInverse - offset;
                                var entity = world.CreateEntity();

                                float rotation = Mathf.Atan2(position.y, position.x);
                                while (rotation < 0) rotation += Mathf.PI * 2;
                                float distance = position.magnitude;
                            
                                int config = position.y > 0 ? 0 : 1;
                                configurations[config].Add(new Anchor {Rotation = rotation, Distance = distance });

                                entity.Set(particleMesh);
                                entity.Set(new Translation { Value = Vector3.zero});
                                entity.Set(new Velocity { Value = Vector3.zero });
                                entity.Set(particleMaterial);
                                entity.Set(new Scale { Value = Vector3.one * (scaleInverse * 1.1f) });
                                entity.Set(new Anchor { Rotation = rotation, Distance = distance });
                                entity.Set(config);
                                totalPoints++;   
                            }
                        }
                    }
                }
                
                // Sort configurations
                for (int i = 0; i < configurations.Length; i++)
                {
                    configurations[i] = configurations[i].OrderByDescending(x => x.Rotation).ToList();
                }

                var global = globalSet.GetEntities()[0];
                global.Set(configurations);

                Debug.Log("Total Points: " + totalPoints);
                textureEntity.Dispose();
            }
        }
    }
}