using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class TargetToDirection : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.
            WithAny<ChaserTag,EnemyTag>().
            WithNone<PlayerTag>().
            ForEach(
            (ref MoveData moveData, in Translation position, in TargetData targetData) =>
            {
                ComponentDataFromEntity<Translation> translationGroup = GetComponentDataFromEntity<Translation>(
                    true);
                if (!translationGroup.HasComponent(targetData.TargetEntity)) return;
                
                Translation targetPosition = translationGroup[targetData.TargetEntity];
                float3 directionToTarget = targetPosition.Value - position.Value;
                moveData.direction = directionToTarget;
            }).Run();
    }
}
