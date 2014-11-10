using UnityEngine;
using System.Collections;

public class PanzerController : MonoBehaviour
{
    public float MaxSpeed = 10;
    public float TurnSpeed = 5;

//    private Direction PreviousDirection;

    // Use this for initialization
    private void Start()
    {
//        PreviousDirection = Direction.Up;
    }

    // Update is called once per frame
    private void Update()
    {
        RotateSprite();
    }

//    private enum Direction
//    {
//        Up = 0,
//        Down = 180,
//        Left = 270,
//        Right = 90
//    }
//
//    private Direction DefineDirection(Direction PreviousDirection)
//    {
//      
//
//        float horizontal = Input.GetAxis("Horizontal");
//        if (horizontal < 0) return Direction.Left;
//        if (horizontal > 0) return Direction.Right;
//        float vertical = Input.GetAxis("Vertical");
//        if (vertical > 0) return Direction.Up;
//        if (vertical < 0) return Direction.Down;
//        return PreviousDirection;
//    }

    private void RotateSprite()
    {
//        if (Input.GetKey(KeyCode.UpArrow))
//            transform.Translate(Vector3.forward * TurnSpeed * Time.deltaTime);
//
//        if (Input.GetKey(KeyCode.DownArrow))
//            transform.Translate(-Vector3.forward * TurnSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftArrow))
            transform.Rotate(Vector3.up, -TurnSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.RightArrow))
            transform.Rotate(Vector3.up, TurnSpeed * Time.deltaTime);

    }
}