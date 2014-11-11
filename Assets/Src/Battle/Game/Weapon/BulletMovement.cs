using UnityEngine;
using System.Collections;

public class BulletMovement : MonoBehaviour
{
    private Vector2 speed = new Vector2(10, 10);
    private Vector2 direction = new Vector2(0, 1);

    private Vector2 movement;

    private void Update()
    {
        movement = new Vector2(speed.x*direction.x, speed.y*direction.y);
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = movement;
    }
}