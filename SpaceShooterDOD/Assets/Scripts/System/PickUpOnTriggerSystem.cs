using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;


// [UpdateAfter(typeof(EndFramePhysicsSystem))]
public class PickUpOnTriggerSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    private EndSimulationEntityCommandBufferSystem endCommandBuffer;

    // [BurstCompile]
    struct PickupOnTriggerSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> players;
        [ReadOnly] public ComponentDataFromEntity<ChaserTag> chasers;
        [ReadOnly] public ComponentDataFromEntity<EnemyTag> enemies;
        public ComponentDataFromEntity<HealthData> healthDatas;

        public EntityCommandBuffer entityCommandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;


            if (chasers.HasComponent(entityA) && enemies.HasComponent(entityB))
            {
                HealthData modifieHealthData = healthDatas[entityB];
                modifieHealthData.isDead = true;
                healthDatas[entityB] = modifieHealthData;
            }
            else if (enemies.HasComponent(entityA) && chasers.HasComponent(entityB))
            {
                entityCommandBuffer.DestroyEntity(entityB);
                entityCommandBuffer.DestroyEntity(entityA);
            }
            
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        endCommandBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        PickupOnTriggerSystemJob job = new PickupOnTriggerSystemJob();

        job.players = GetComponentDataFromEntity<PlayerTag>(true);
        job.chasers = GetComponentDataFromEntity<ChaserTag>(true);
        job.enemies = GetComponentDataFromEntity<EnemyTag>(true);
        job.healthDatas = GetComponentDataFromEntity<HealthData>(false);
        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        // jobHandle.Complete();
        endCommandBuffer.AddJobHandleForProducer(jobHandle);
        
        return jobHandle;
    }
}