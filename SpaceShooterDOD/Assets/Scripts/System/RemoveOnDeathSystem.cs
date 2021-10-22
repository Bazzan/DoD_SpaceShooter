using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(TransformSystemGroup))]
public class RemoveOnDeathSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem endCommandBufferSystem;
    private EntityManager manager;

    protected override void OnCreate()
    {
        base.OnCreate();
        endCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer entityCommandBuffer = endCommandBufferSystem.CreateCommandBuffer();

        float deltaTime = Time.DeltaTime;


        // checking Health;
        Entities.WithAny<PlayerTag, ChaserTag,EnemyTag>().ForEach((Entity entity, in HealthData healthData) =>
        {
            if (healthData.isDead)
                entityCommandBuffer.DestroyEntity(entity);
        }).Schedule();


        // checking life time Add to pool again
        Entities.ForEach((ref LifeTimeData lifeTimeData, in Entity entity) =>
        {
            lifeTimeData.LifeTime -= deltaTime;

            if (lifeTimeData.LifeTime <= 0f)
                entityCommandBuffer.DestroyEntity(entity);
        }).Schedule();

        endCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
    }
}


// Entities.WithAny<BulletTag>().WithStructuralChanges().WithoutBurst().ForEach(
//     (LifeTimeData lifeTimeData, in Entity bullet) =>
//     {
//         lifeTimeData.LifeTime -= deltaTime;
//         Debug.Log("bullet life tiume");
//         if (lifeTimeData.LifeTime < 0)
//         {
//             ShootSystem.bulletQueue.Enqueue(bullet);
//             manager.SetEnabled(bullet, false);
//         }
//     }).Run();

// Entities.WithAll<EnemyTag>().WithoutBurst().WithStructuralChanges().ForEach((Entity enemy, in HealthData healthData) =>
// {
//     if (!healthData.isDead) return;
//
//     manager.SetEnabled(enemy, false);
//     SpawnEnemies.EnemyQueue.Enqueue(enemy);
// }).Run();