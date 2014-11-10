using UnityEngine;
using System.Collections;

public class PanzerController : MonoBehaviour
{
    public float Speed;

    private Direction PreviousDirection;
    private bool ShouldMove;

    // Use this for initialization
    private void Start()
    {
        PreviousDirection = Direction.Up;
        ShouldMove = false;
    }

    // Update is called once per frame
    private void Update()
    {
        Direction nextDirection = DefineDirection(PreviousDirection);
        RotatePanzer(nextDirection);
        MovePanzer(nextDirection);
        PreviousDirection = nextDirection;
    }

    private enum Direction
    {
        Up = 0,
        Down = 180,
        Left = 90,
        Right = -90
    }

    private static Vector3 DirectionVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return new Vector3(0, 1, 0);
            case Direction.Down:
                return new Vector3(0, -1, 0);
            case Direction.Right:
                return new Vector3(1, 0, 0);
            case Direction.Left:
                return new Vector3(-1, 0, 0);
        }
        return new Vector3(0, 0, 0);
    }

    private Direction DefineDirection(Direction PreviousDirection)
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        ShouldMove = horizontal != 0 || vertical != 0;

        if (horizontal < 0) return Direction.Left;
        if (horizontal > 0) return Direction.Right;
        
        if (vertical > 0) return Direction.Up;
        if (vertical < 0) return Direction.Down;
        return PreviousDirection;
    }

    private void MovePanzer(Direction direction)
    {
        if (ShouldMove)
        {
            Vector3 vector = DirectionVector(direction);
            Vector3 nextPosition = this.transform.position + vector * Speed;
            this.transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime);
//            this.transform.position = nextPosition;
            ShouldMove = false;
        }   
     }

   

    private void RotatePanzer(Direction direction)
    {
        transform.eulerAngles = new Vector3(0, 0, (float) direction);
    }
}