using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

// [UpdateAfter(typeof(TransformSystemGroup))]
public class FaceDirectionSystem : SystemBase
{
    protected override void OnUpdate()
    {

        Entities.WithAny<PlayerTag,EnemyTag,ChaserTag>()
            .ForEach((ref Rotation rotation, in PhysicsVelocity velocity, in Translation translation,
                in MoveData moveData) =>
            {
                FaceDirection(ref rotation, in velocity, in moveData);
            }).Schedule();
    }


    private static void FaceDirection(ref Rotation rotation, in PhysicsVelocity velocity, in MoveData moveData)
    {
        quaternion targetRotation;
        if (moveData.direction.Equals(float3.zero))
            targetRotation = Quaternion.LookRotation(
                moveData.lastDirection, math.forward());
        else
            targetRotation = quaternion.LookRotationSafe(moveData.direction, math.forward());

        rotation.Value = math.slerp(rotation.Value, targetRotation, moveData.turnSpeed);
    }
}