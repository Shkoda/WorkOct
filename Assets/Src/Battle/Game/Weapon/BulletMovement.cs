using Assets.Src.Battle.Game;
using UnityEngine;
using System.Collections;

public class BulletMovement : MonoBehaviour
{
    private Vector2 Speed = new Vector2(10, 10);
    private Vector2 DirectionVector = new Vector2(0, 1);

    private Vector2 movement;

    private void Update()
    {
        movement = new Vector2(Speed.x*DirectionVector.x, Speed.y*DirectionVector.y);
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = movement;
    }

    public void SetDirection(Direction direction)
    {
        transform.eulerAngles = new Vector3(0, 0, DirectionUtils.GetAngle(direction));
        DirectionVector = DirectionUtils.GetVector(direction);
    }
}