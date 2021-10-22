using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TargetToDirection))]
public class AssignPlayerToTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        AssignPlayer();
    }

    private void AssignPlayer()
    {
        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerTag>());
        Entity playerEntity = playerQuery.GetSingletonEntity();
        // EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        ComponentDataFromEntity<MoveData> moveDataGroup = GetComponentDataFromEntity<MoveData>();
        
        
        Entities.
            WithAny<ChaserTag,EnemyTag>().
            ForEach((ref TargetData targetData, ref MoveData moveData, in Entity entity) =>
        {
            if (playerEntity != Entity.Null)
            {
                targetData.TargetEntity = playerEntity;

                MoveData moveData2 = moveData;
                moveData2.speed = Random.Range(30, 80);
                moveData2.turnSpeed = Random.Range(0.001f, 0.01f);

                moveDataGroup[entity] = moveData2;

            }
        }).Run();
        // playerQuery.Dispose();
    }
}