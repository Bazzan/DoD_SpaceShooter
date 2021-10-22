using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct GunData : IComponentData
{
    public Entity bulletPrefab;

    public float currentFireWaitTime;
    public float fireRate;
    public float bulletSpeed;
}
