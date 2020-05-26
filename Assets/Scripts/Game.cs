using Assets.Scripts.Rendering;
using Assets.Scripts.Transform;
using DefaultEcs;
using DefaultNamespace;
using UnityEngine;

public class Game : MonoBehaviour
{
    public float Scale = 100;
    public Mesh ParticleMesh;
    public Material ParticleMaterial;
    public Texture2D CreationTexture;
    
    World world;

    ParticleCreationSystem creationSystem;
    AnchorMoveSystem anchorSystem;
    ParticleMoveSystem moveSystem;
    ApplyVelocitySystem velocitySystem;
    TransformWriteSystem transformSystem;
    RenderingSystem renderSystem;
    
    void Start()
    {
        world = new World();
        Setup();

        var global = world.CreateEntity();
        global.Set(new Global());
        
        var creation = world.CreateEntity();
        creation.Set(CreationTexture);
    }

    void Setup()
    {
        creationSystem = new ParticleCreationSystem(world, this, ParticleMesh, ParticleMaterial);
        anchorSystem = new AnchorMoveSystem(world);
        moveSystem = new ParticleMoveSystem(world);
        velocitySystem = new ApplyVelocitySystem(world);
        transformSystem = new TransformWriteSystem(world);
        renderSystem = new RenderingSystem(world);
    }

    void Update()
    {
        creationSystem.Update();
        anchorSystem.Update();
        moveSystem.Update();
        velocitySystem.Update();
        transformSystem.Update();
        renderSystem.Update();
    }
}
