using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnEnemies : SystemBase
{
    private Entity player;
    private EntityManager manager;


    public static NativeQueue<Entity> EnemyQueue = new NativeQueue<Entity>(Allocator.Persistent);

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithoutBurst().WithStructuralChanges().ForEach((ref EnemySpawnData enemySpawnData) =>
        {
            enemySpawnData.SpawnWaitTime += deltaTime;
            if (enemySpawnData.SpawnRate >= enemySpawnData.SpawnWaitTime) return;

            enemySpawnData.SpawnWaitTime = 0f;

            

            SpawnWave(enemySpawnData);

            enemySpawnData.numberToSpawn++;
        }).Run();
    }

    private void SpawnWave(EnemySpawnData enemySpawnData)
    {
        for (int i = 0; i < enemySpawnData.numberToSpawn; i++)
        {
            float3 spawnPos = SpawnPos();
            Entity Enemy = manager.Instantiate(enemySpawnData.Enemy);

            manager.SetComponentData(
                Enemy, new Translation
                {
                    Value = spawnPos
                });
            manager.SetComponentData(Enemy, new TargetData {TargetEntity = player});
            manager.SetComponentData(Enemy, new MoveData
            {
                direction = float3.zero,
                isActive = true,
                lastDirection = float3.zero,
                speed = Random.Range(30, 50),
                turnSpeed = Random.Range(0.001f, 0.01f)
            });
            manager.SetComponentData(Enemy, new HealthData
            {
                currentHealth = 100,
                isDead = false
            });
        }
    }

    private static float3 SpawnPos()
    {
        float3 CameraViewportToWorldPoint = Camera.main.ViewportToWorldPoint(
            new Vector3(1, 1, Camera.main.transform.position.z));
        float3 spawnPos = float3.zero;
        int randomSpawn = Random.Range(1, 5);
        if (randomSpawn == 1)
        {
            spawnPos.y = Random.Range(-CameraViewportToWorldPoint.y, CameraViewportToWorldPoint.y);
            spawnPos.x = CameraViewportToWorldPoint.x;
        }
        else if (randomSpawn == 2)
        {
            spawnPos.y = Random.Range(-CameraViewportToWorldPoint.y, CameraViewportToWorldPoint.y);
            spawnPos.x = -CameraViewportToWorldPoint.x;
        }
        else if (randomSpawn == 3)
        {
            spawnPos.x = Random.Range(-CameraViewportToWorldPoint.x, CameraViewportToWorldPoint.x);
            spawnPos.y = CameraViewportToWorldPoint.y;
        }
        else if (randomSpawn == 4)
        {
            spawnPos.x = Random.Range(-CameraViewportToWorldPoint.x, CameraViewportToWorldPoint.x);
            spawnPos.y = -CameraViewportToWorldPoint.y;
        }

        spawnPos.z = 0;
        return spawnPos;
    }


    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        player = EntityDataManager.instance.player;
        // SpawnEnemiesToQueue();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        EnemyQueue.Dispose();
    }
    void SpawnEnemiesToQueue()
    {
        ComponentDataFromEntity<EnemySpawnData> spawnData = GetComponentDataFromEntity<EnemySpawnData>();
        Entity spawnDataEntity = GetSingletonEntity<EnemySpawnData>();
        EnemySpawnData enemySpawnData = spawnData[spawnDataEntity];

        for (int i = 0; i < 200; i++)
        {
            Entity enemy = manager.Instantiate(enemySpawnData.Enemy);
            manager.SetEnabled(enemy, false);
            EnemyQueue.Enqueue(enemy);
        }
    }


}