using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class PlayerMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<PlayerTag>().ForEach(
            (ref PhysicsVelocity velocity, ref Translation position, in MoveData moveData) =>
            {
                float sqrdMagnitude = velocity.Linear.x * velocity.Linear.x + velocity.Linear.y * velocity.Linear.y;

                float3 normalizedDirection = math.normalizesafe(moveData.direction);

                position.Value.z = 0f;
                float3 velocityToAdd = normalizedDirection * moveData.speed * deltaTime;
                velocityToAdd.z = 0;
                velocity.Linear += velocityToAdd;
                // Vector3.ClampMagnitude()

                // if (math.sqrt(sqrdMagnitude) < 40f)
                // {
                // }

            }).Run();
    }
}