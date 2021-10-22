using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct EnemySpawnData : IComponentData
{
    public Entity Enemy;
    public float SpawnRate;
    public float SpawnWaitTime;
    public int numberToSpawn;
}
