using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class EntityDataManager : MonoBehaviour
{
    public static EntityDataManager instance;
    private float3 upperCamera;

    public Entity player;
    private EntityManager manager;

    private void Awake()
    {
        if (instance != null) return;
        instance = this;
    }

    private void Start()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery playerQuery = manager.CreateEntityQuery(
            ComponentType.ReadOnly<PlayerTag>());
        NativeArray<Entity> playerEntities = playerQuery.ToEntityArray(Allocator.TempJob);
        player = playerEntities[0];
        playerEntities.Dispose();
        upperCamera = Camera.main.ScreenToWorldPoint(new float3(1f, 1f, 0));
    }
}