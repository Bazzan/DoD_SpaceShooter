using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

// [UpdateInGroup(typeof(SimulationSystemGroup))]
// [UpdateBefore(typeof(TargetToDirection))]
// [UpdateBefore(typeof(SimulationSystemGroup))]
public class ShootSystem : JobComponentSystem
{
    // private PlayerInputSystem playerInputSystem;
    public Entity player;
    public ComponentDataFromEntity<BulletData> bulletDataGroup;

    private EntityManager manager;
    public static NativeQueue<Entity> bulletQueue = new NativeQueue<Entity>(Allocator.Persistent);

    // private NativeQueue<BulletData> bulletData;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!Input.GetKey(KeyCode.Space)) return inputDeps;
        Debug.Log("shoooting");
        float deltaTime = Time.DeltaTime;


        Entities.WithoutBurst().WithAny<PlayerTag>().WithStructuralChanges().ForEach(
            (ref Translation position, ref GunData gunData, in Rotation rotation, in MoveData moveData,
                in PhysicsVelocity playerVelocity) =>
            {
                gunData.currentFireWaitTime += deltaTime;
                if (gunData.currentFireWaitTime >= gunData.fireRate)
                {
                    gunData.currentFireWaitTime = 0;
                    // Entity bullet = bulletQueue.Dequeue();
                    // manager.SetEnabled(bullet, true);

                    Entity bullet = manager.Instantiate(gunData.bulletPrefab);

                    manager.SetComponentData(bullet, new Translation
                    {
                        Value = position.Value + math.mul(rotation.Value, new float3(0, 0, 1))
                    });
                    manager.SetComponentData(bullet, new Rotation {Value = rotation.Value});
                    manager.SetComponentData(bullet, new LifeTimeData {LifeTime = 5f});
                    manager.SetComponentData(bullet, new PhysicsVelocity
                    {
                        Angular = float3.zero,
                        Linear = moveData.lastDirection * gunData.bulletSpeed + playerVelocity.Linear
                    });
                }
            }).Run();



        return inputDeps;
    }


    void assignPlayer()
    {
        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerTag>());
        player = playerQuery.GetSingletonEntity();
        EntityDataManager.instance.player = player;
    }

    void SpawnBulletsToQueue()
    {
        ComponentDataFromEntity<GunData> gunDataGroup = GetComponentDataFromEntity<GunData>();
        GunData gunData = gunDataGroup[player];
        for (int i = 0; i < 20; i++)
        {
            Entity bullet = manager.Instantiate(gunData.bulletPrefab);
            manager.SetEnabled(bullet, false);
            bulletQueue.Enqueue(bullet);
        }
    }
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        assignPlayer();

        // SpawnBulletsToQueue();

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        bulletQueue.Dispose();
    }
}
