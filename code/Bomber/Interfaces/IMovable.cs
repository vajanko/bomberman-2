using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomber
{
    /// <summary>
    /// Defines methods for component that could be moved
    /// </summary>
    interface IMovable
    {
        void Move(Direction dir, int frames);
        void Stop();

        /// <summary>
        /// Acceleration of the moveable component
        /// </summary>
        float Acceleration
        {
            get;
            set;
        }

        /// <summary>
        /// (Get/Set) Current speed of a moveable component. Set to zero when not moveing
        /// </summary>
        float Speed
        {
            get;
            set;
        }

        /// <summary>
        /// (Get/Set) Maximal speed that could be reached by the acceleration
        /// </summary>
        float MaxSpeed
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Four possible move direction
    /// </summary>
    enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        None = 4
    }

    /// <summary>
    /// Extension method for Direction enum
    /// </summary>
    static class DirectionExtension
    {
        static private readonly int[][] shifts = { new int[] { 0, -1 }, new int[]{0,1}, 
                                                   new int[]{-1,0}, new int[]{1,0}};
        /// <summary>
        /// Shift position (x,y) in a particular direction
        /// </summary>
        public static void Shift(this Direction dir, ref int x, ref int y)
        {
            // notice there is no switch
            x += shifts[(int)dir][0];
            y += shifts[(int)dir][1];
        }
        /// <summary>
        /// Direction is Left of Right
        /// </summary>
        static public bool IsHorizontal(this Direction dir)
        {
            return (dir == Direction.Left || dir == Direction.Right);
        }
        /// <summary>
        /// Direction is Up or Down
        /// </summary>
        static public bool IsVertical(this Direction dir)
        {
            return (dir == Direction.Up || dir == Direction.Down);
        }
    }
}
