using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf.Meta;
using UnityEngine;

namespace Assets.Src.Battle.Game
{
     enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

     class DirectionUtils
    {


         public static Vector3 GetVector(Direction direction)
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
            throw new ArgumentException(direction + " is not valid argument");
         }
        public static int GetAngle(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return 0;
                case Direction.Down:
                    return 180;
                case Direction.Right:
                    return -90;
                case Direction.Left:
                    return 90;
            }
            throw new ArgumentException(direction + " is not valid argument");
        }
    }
}