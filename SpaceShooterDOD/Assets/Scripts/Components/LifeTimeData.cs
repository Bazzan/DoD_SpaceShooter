using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponentAttribute]
public struct LifeTimeData : IComponentData
{
    public float LifeTime;

}
