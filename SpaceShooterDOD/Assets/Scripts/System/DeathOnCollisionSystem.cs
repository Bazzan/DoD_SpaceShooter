using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

// [UpdateAfter(typeof(Unity.Entities.SimulationSystemGroup))]
// public class DeathOnCollisionSystem : JobComponentSystem
// {
//     private BuildPhysicsWorld buildPhysicsWorld;
//     private StepPhysicsWorld stepPhysicsWorld;
//
//     protected override void OnCreate()
//     {
//         base.OnCreate();
//         buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
//         stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
//     }
//
//     // [BurstCompile]
//     struct DeathOnCollisionSystemJob : ICollisionEventsJob
//     {
//         [ReadOnly] public ComponentDataFromEntity<DeathColliderTag> deathColliderGroup;
//         [ReadOnly] public ComponentDataFromEntity<ChaserTag> chaserColliderGroup;
//         [ReadOnly] public ComponentDataFromEntity<PlayerTag> playerGroup;
//         public ComponentDataFromEntity<HealthData> healthGroup;
//         private bool playerHasCollided;
//         
//         public void Execute(CollisionEvent collisionEvent)
//         {
//             Entity entityA = collisionEvent.EntityA;
//             Entity entityB = collisionEvent.EntityB;
//             
//             
//             // bool entityAIsDeathCollider = if (deathColliderGroup[entityA]);
//             // bool entityBIsDeathCollider = deathColliderGroup.HasComponent(entityB);
//             bool entityAIsDeathCollider = deathColliderGroup.HasComponent(entityA);
//             bool entityBIsDeathCollider = deathColliderGroup.HasComponent(entityB);
//             // if (!entityAIsDeathCollider && !entityBIsDeathCollider) return;
//             
//             bool entityAIsChaser = chaserColliderGroup.HasComponent(entityA);
//             bool entityBIsChaser = chaserColliderGroup.HasComponent(entityB);
//             
//             // if (entityAIsChaser && entityBIsChaser) return;
//             bool entityAIsPlayerCollider = playerGroup.HasComponent(entityA);
//             bool entityBIsPlayerCollider = playerGroup.HasComponent(entityB);
//             
//             if (entityAIsDeathCollider && entityBIsChaser)
//             {
//                 HealthData modifieHealthData = healthGroup[entityB];
//                 modifieHealthData.isDead = true;
//                 healthGroup[entityB] = modifieHealthData;
//             }
//             else if (entityBIsDeathCollider && entityAIsChaser)
//             {
//                 HealthData modifiHealthData = healthGroup[entityA];
//                 modifiHealthData.isDead = true;
//                 healthGroup[entityA] = modifiHealthData;
//             }
//             
//             // else if (entityBIsDeathCollider && entityAIsPlayerCollider && !playerHasCollided)
//             // {
//             //     HealthData modifiHealthData = healthGroup[entityA];
//             //     modifiHealthData.isDead = true;
//             //     healthGroup[entityA] = modifiHealthData;
//             //     playerHasCollided = true;
//             //     Debug.Log("player hit");
//             // }
//             // else if (entityBIsDeathCollider && entityBIsPlayerCollider)
//             // {
//             //     HealthData modifieHealthData = healthGroup[entityB];
//             //     modifieHealthData.isDead = true;
//             //     healthGroup[entityB] = modifieHealthData;
//             //     Debug.Log("player hit");
//             // }
//             else if (entityBIsChaser && entityAIsChaser)
//             {
//                 HealthData modifiHealthData = healthGroup[entityA];
//                 modifiHealthData.isDead = true;
//                 healthGroup[entityA] = modifiHealthData;
//             }
//         }
//     }
//     
//     
//     protected override JobHandle OnUpdate(JobHandle inputDeps)
//     {
//         DeathOnCollisionSystemJob job = new DeathOnCollisionSystemJob();
//     
//         job.deathColliderGroup = GetComponentDataFromEntity<DeathColliderTag>(true);
//         job.chaserColliderGroup = GetComponentDataFromEntity<ChaserTag>(true);
//         job.healthGroup = GetComponentDataFromEntity<HealthData>();
//         job.playerGroup = GetComponentDataFromEntity<PlayerTag>(true);
//
//
//         JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
//         // JobHandle jobHandle = job.Run(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
//         jobHandle.Complete();
//
//         return jobHandle;
//     }
// }