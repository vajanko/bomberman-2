using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomber
{
    /// <summary>
    /// Defines methods for component that are movable
    /// </summary>
    interface IMovable
    {
        void Move(Direction dir, int frames);
        void Stop();

        float Acceleration
        {
            get;
            set;
        }

        float Speed
        {
            get;
            set;
        }

        float MaxSpeed
        {
            get;
            set;
        }
    }

    enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        None = 4
    }

    static class DirectionExtension
    {
        static private readonly int[][] shifts = { new int[] { 0, -1 }, new int[]{0,1}, 
                                                   new int[]{-1,0}, new int[]{1,0}};

        public static void Shift(this Direction dir, ref int x, ref int y)
        {
            x += shifts[(int)dir][0];
            y += shifts[(int)dir][1];
        }
        static public bool IsHorizontal(this Direction dir)
        {
            return (dir == Direction.Left || dir == Direction.Right);
        }
        static public bool IsVertical(this Direction dir)
        {
            return (dir == Direction.Up || dir == Direction.Down);
        }
    }
}
