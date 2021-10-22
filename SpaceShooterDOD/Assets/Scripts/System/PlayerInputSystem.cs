using System;
using Unity.Entities;
using UnityEngine;

public class PlayerInputSystem : SystemBase
{
    public bool PressingShoot;
    public float shootCD = 1f;
    private double timeStamp;

    protected override void OnUpdate()
    {
        Entities.WithoutBurst().WithAll<PlayerTag>().ForEach((ref MoveData moveData, ref InputData inputData) =>
        {
            bool isRightKeyPressed = Input.GetKey(inputData.rightKey);
            bool isLeftKeyPressed = Input.GetKey(inputData.leftKey);
            bool isUpKeyPressed = Input.GetKey(inputData.upKey);
            bool isDownKeyPressed = Input.GetKey(inputData.downKey);
            PressingShoot = Input.GetKey(inputData.shoot);

            moveData.direction.x = Convert.ToInt16(isRightKeyPressed);
            moveData.direction.x -= Convert.ToInt16(isLeftKeyPressed);
            moveData.direction.y = Convert.ToInt16(isUpKeyPressed);
            moveData.direction.y -= Convert.ToInt16(isDownKeyPressed);
            
            inputData.isShooting = PressingShoot;

            if (World.Time.ElapsedTime > shootCD + timeStamp)
            {
                if (PressingShoot)
                {
                    timeStamp = World.Time.ElapsedTime;
                }
            }

            if (isDownKeyPressed || isLeftKeyPressed || isRightKeyPressed || isUpKeyPressed)
                moveData.lastDirection = moveData.direction;
        }).Run();
    }
}