using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class WarpEntitesSystem : SystemBase
{
    private float3 OutSideScreenPositions;
    protected override void OnUpdate()
    {
        float3 CornerValue = (float3)Camera.main.ScreenToWorldPoint(new float3(1f, 1f, Camera.main.transform.position.z));


        Entities.ForEach((ref Translation translation) =>
        {
            if (translation.Value.x > CornerValue.x)
            {
                translation.Value.x = -CornerValue.x;
            }
            else if (translation.Value.x < -CornerValue.x)
            {
                translation.Value.x = CornerValue.x;
            }

            if (translation.Value.y > CornerValue.y)
            {
                translation.Value.y = -CornerValue.y;
            }
            else if (translation.Value.y < -CornerValue.y)
            {
                translation.Value.y = CornerValue.y;
            }
        }).ScheduleParallel();
    }
}