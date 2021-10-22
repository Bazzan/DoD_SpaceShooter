using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class MoveForwardSystem : SystemBase
{
    
    protected override void OnUpdate()
    {
        float deltaTime = UnityEngine.Time.deltaTime;


        Entities.WithAny<AsteroidTag, ChaserTag, EnemyTag>().ForEach(
            (ref PhysicsVelocity velocity, in Translation position, in MoveData moveData, in Rotation rotation) =>
            {
                // float sqrdMagnitude = velocity.Linear.x * velocity.Linear.x + velocity.Linear.y * velocity.Linear.y;


                float3 forwardDirection = math.forward(rotation.Value);
                float3 nextDeltaPosition = math.normalizesafe(forwardDirection)  * moveData.speed * deltaTime;
                nextDeltaPosition.z = 0f;
                velocity.Linear += nextDeltaPosition;

            }).Run();
    }
}