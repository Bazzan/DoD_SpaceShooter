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
public class DeathOnTrigger : JobComponentSystem
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
        [ReadOnly] public ComponentDataFromEntity<BulletTag> Bullets;

        public ComponentDataFromEntity<LifeTimeData> LifeTimeGroup;

        public ComponentDataFromEntity<HealthData> healthDatas;
        private bool playerHasCollided;


        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;


            if (Bullets.HasComponent(entityA) && enemies.HasComponent(entityB))
            {
                HealthData modifieHealthData = healthDatas[entityB];
                modifieHealthData.isDead = true;
                healthDatas[entityB] = modifieHealthData;
                LifeTimeData modifiedLifeTimeData = LifeTimeGroup[entityA];
                modifiedLifeTimeData.LifeTime = 0;
                LifeTimeGroup[entityA] = modifiedLifeTimeData;
            }
            else if (enemies.HasComponent(entityA) && Bullets.HasComponent(entityB))
            {
                HealthData modifieHealthData = healthDatas[entityA];
                modifieHealthData.isDead = true;
                healthDatas[entityA] = modifieHealthData;
                LifeTimeData modifiedLifeTimeData = LifeTimeGroup[entityB];
                modifiedLifeTimeData.LifeTime = 0;
                LifeTimeGroup[entityB] = modifiedLifeTimeData;
            }
            else if (chasers.HasComponent(entityA) && enemies.HasComponent(entityB))
            {
                HealthData modifieHealthData = healthDatas[entityB];
                modifieHealthData.isDead = true;
                healthDatas[entityB] = modifieHealthData;
                healthDatas[entityA] = modifieHealthData;
            }
            else if (enemies.HasComponent(entityA) && chasers.HasComponent(entityB))
            {
                HealthData modifieHealthData = healthDatas[entityB];
                modifieHealthData.isDead = true;
                healthDatas[entityB] = modifieHealthData;
                healthDatas[entityA] = modifieHealthData;
            }
            else if (players.HasComponent(entityA) && enemies.HasComponent(entityB) && !playerHasCollided)
            {
                HealthData playerHealthData = healthDatas[entityA];
                HealthData enemyModifiedHealthData1 = healthDatas[entityB];
                enemyModifiedHealthData1.isDead = true;
                healthDatas[entityB] = enemyModifiedHealthData1;

                HealthData playerModifiedHealthData2 = playerHealthData;
                if (healthDatas[entityA].currentHealth < 0)
                {
                    playerModifiedHealthData2.isDead = true;
                    healthDatas[entityA] = playerModifiedHealthData2;
                }
                else
                {
                    playerModifiedHealthData2.currentHealth = playerHealthData.currentHealth - 10;
                    healthDatas[entityA] = playerModifiedHealthData2;
                }

                // Debug.Log(playerModifiedHealthData2.currentHealth + " new currrent " +
                          // playerModifiedHealthData2.isDead);

                Debug.Log("Damage player");

                playerHasCollided = true;
            }
            else if (enemies.HasComponent(entityA) && players.HasComponent(entityB) && !playerHasCollided)
            {
                HealthData playerHealthData = healthDatas[entityB];
                HealthData enemyModifiedHealthData1 = healthDatas[entityA];
                enemyModifiedHealthData1.isDead = true;
                healthDatas[entityA] = enemyModifiedHealthData1;
                HealthData playerModifiedHealthData2 = playerHealthData;

                if (playerHealthData.currentHealth < 0)
                {
                    Debug.Log(healthDatas[entityB].currentHealth);
                    playerModifiedHealthData2.isDead = true;
                    healthDatas[entityB] = playerModifiedHealthData2;
                }
                else
                {
                    Debug.Log(healthDatas[entityB].currentHealth);

                    playerModifiedHealthData2.currentHealth = playerHealthData.currentHealth - 10;
                    healthDatas[entityB] = playerModifiedHealthData2;
                    // Debug.Log(playerModifiedHealthData2.currentHealth + " new currrent");
                }

                // Debug.Log("Damage player");
                playerHasCollided = true;
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
        job.Bullets = GetComponentDataFromEntity<BulletTag>(true);
        job.healthDatas = GetComponentDataFromEntity<HealthData>(false);
        job.LifeTimeGroup = GetComponentDataFromEntity<LifeTimeData>(false);
        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        // jobHandle.Complete();
        endCommandBuffer.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}