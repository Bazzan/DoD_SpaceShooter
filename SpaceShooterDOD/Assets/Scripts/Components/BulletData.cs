using System;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BulletData : IComponentData
{
    public float Speed;
    public bool IsActive;
    // public float LifeTime;

}
