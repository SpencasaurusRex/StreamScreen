using System;
using System.Collections.Generic;
using DefaultEcs;
using UnityEngine;

namespace DefaultNamespace
{
    public class AnchorMoveSystem
    {
        EntitySet anchorSet;
        EntitySet globalSet;
        float r;
        int topConfigCounter;
        int bottomConfigCounter;
        float topRotationOffset;
        float bottomRotationOffset;

        const float TWO_PI = Mathf.PI * 2;
        
        public AnchorMoveSystem(World world)
        {
            anchorSet = world.GetEntities().With<Anchor>().AsSet();
            globalSet = world.GetEntities().With<Global>().AsSet();
        }

        public void Update()
        {
            var delta = Time.deltaTime * 0.5f;
            r += delta;
            if (r >= Mathf.PI)
            {
                topConfigCounter = 0;
                bottomConfigCounter = 0;
                r %= Mathf.PI;    
            }

            var configurations = globalSet.GetEntities()[0].Get<List<Anchor>[]>();

            foreach (var entity in anchorSet.GetEntities())
            {
                var anchor = entity.Get<Anchor>();
                var config = entity.Get<int>();
                float newRotation = (anchor.Rotation + delta) % TWO_PI;

                if (config == 0 && newRotation >= Mathf.PI && anchor.Rotation < Math.PI)
                {
                    entity.Set(1);
                    // Get corresponding bottom config point
                    var corrPoint = configurations[1][bottomConfigCounter++];
                    if (bottomConfigCounter >= configurations[1].Count)
                    {
                        // If more points on other config, prevent out of index
                        bottomConfigCounter--;
                    }

                    anchor.Rotation = (corrPoint.Rotation + Mathf.PI + r) % TWO_PI;
                    anchor.Distance = corrPoint.Distance;
                    
                    var dir = UnityEngine.Random.onUnitSphere;
                    dir.z = 0;
                    entity.Get<Velocity>().Value += dir * 3;
                }
                else if (config == 1 && anchor.Rotation > newRotation)
                {
                    entity.Set(0);
                    // Get corresponding top config point
                    var corrPoint = configurations[0][topConfigCounter++];
                    if (topConfigCounter >= configurations[0].Count)
                    {
                        // If more points on other config, prevent out of index
                        topConfigCounter--;
                    }

                    anchor.Rotation = (corrPoint.Rotation + Mathf.PI + r) % TWO_PI;
                    anchor.Distance = corrPoint.Distance;

                    var dir = UnityEngine.Random.onUnitSphere;
                    dir.z = 0;
                    entity.Get<Velocity>().Value += dir * 3;
                }
                else
                {
                    anchor.Rotation = newRotation;
                }
                
                anchor.Value = new Vector3(Mathf.Cos(anchor.Rotation), Mathf.Sin(anchor.Rotation), 0) * anchor.Distance;
            }
        }
    }
}