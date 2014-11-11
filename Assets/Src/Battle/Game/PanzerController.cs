﻿
using System.Diagnostics;
using Assets.Src.Battle.Game;
using UnityEngine;


public class PanzerController : MonoBehaviour
{
    public GameObject ParentObject;
    public GameObject BulletPrefab;

    public float Speed;

    private Direction PreviousDirection;
    private bool ShouldMove;


    private float ShotCooldown = 100;//millis
    private Stopwatch stopwatch;
    // Use this for initialization
    private void Start()
    {
        PreviousDirection = Direction.Up;
        ShouldMove = false;
        stopwatch = Stopwatch.StartNew();
    }

    // Update is called once per frame
    private void Update()
    {
        Direction nextDirection = DefineDirection(PreviousDirection);
        RotatePanzer(nextDirection);
        MovePanzer(nextDirection);
        PreviousDirection = nextDirection;
        Debugger.Log("PanzerController.Update() elapsed :: " + stopwatch.Elapsed.Milliseconds);

        if (Input.GetKey(KeyCode.Space) && stopwatch.Elapsed.Milliseconds>=ShotCooldown)
            Shot();
        }

    private void Shot()
    {
//        var bullet = (GameObject)Instantiate(
//            BulletPrefab, 
//            new Vector3(this.transform.position.x, this.transform.position.y, 0), 
//            Quaternion.identity);

        var bullet = (GameObject)Instantiate(BulletPrefab);
        bullet.transform.parent = ParentObject.transform;
        bullet.transform.position = this.transform.position;

        stopwatch.Reset();
        stopwatch.Start();
           
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
            Vector3 directionVector = DirectionUtils.GetVector(direction);
            Vector3 nextPosition = this.transform.position + directionVector*Speed;
            this.transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime);
//            this.transform.position = nextPosition;
            ShouldMove = false;
        }
    }


    private void RotatePanzer(Direction direction)
    {
        transform.eulerAngles = new Vector3(0, 0, DirectionUtils.GetAngle(direction));
    }
}