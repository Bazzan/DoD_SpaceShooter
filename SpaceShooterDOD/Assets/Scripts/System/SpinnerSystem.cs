using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpinnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.
            WithAll<SpinnerTag>().
            ForEach((ref Rotation rotation, in MoveData moveData) =>
        {
            quaternion normalizedRotation = math.normalizesafe(rotation.Value);
            quaternion angelToRotate = quaternion.AxisAngle(math.forward(), moveData.turnSpeed * deltaTime);

            rotation.Value = math.mul(normalizedRotation, angelToRotate);
        }).ScheduleParallel();
    }
}