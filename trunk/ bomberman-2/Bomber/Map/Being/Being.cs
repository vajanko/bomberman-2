using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomber
{
    abstract class Being : MapObject, IMovable, IDestroyable
    {
        protected static Random generator = new Random();

        #region Properities

        public bool DefHorizontalPosition { get { return Position.X % Size == 0; } }
        public bool DefVerticalPosition { get { return Position.Y % Size == 0; } }
        /// <summary>
        /// Get the default real position of the actual map position
        /// </summary>
        public Vector2 DefaultPosition { get { return new Vector2(Position.X * Size, Position.Y * Size); } }

        #endregion

        #region IMovable Members

        /// <summary>
        /// Move Being in a particular direction
        /// </summary>
        /// <param name="dir">Direction of the movement</param>
        /// <param name="frames">Elapsed number of frames since last movement (miliseconds)</param>
        public virtual void Move(Direction dir, int frames)
        {
            Speed += frames * Acceleration;

            float steps = Speed * frames;
            int x = MapX, y = MapY;
            // get position of some map component in the direction we are moveing in
            dir.Shift(ref x, ref y);
            
            Vector2 current = this.Position;    // this is my position
            Vector2 def = new Vector2(MapX * Size, MapY * Size);
            Vector2 future;     // this is possible future position
            if (dir.IsHorizontal() && current.Y != def.Y)
            { 
                future = def;
                if (current.Y > def.Y) dir = Direction.Up;
                else dir = Direction.Down;
            }
            else if (dir.IsVertical() && current.X != def.X)
            {
                future = def;
                if (current.X > def.X) dir = Direction.Left;
                else dir = Direction.Right;
            }
            else
            {
                if (map.IsPassable(x, y))   // if this map position is not free then set the future real position
                    // to default position and it will at least finish the movement to default pos.
                    // or it would not move if it is in default position
                    future = new Vector2(x * Size, y * Size);   // this is position where I want to get
                else future = def;
            }

            switch (dir)
            {
                case Direction.Up:
                    if (current.Y - steps < future.Y)
                        current.Y = future.Y;
                    else current.Y -= steps;
                    break;
                case Direction.Down:
                    if (current.Y + steps > future.Y)
                        current.Y = future.Y;
                    else current.Y += steps;
                    break;
                case Direction.Left:
                    if (current.X - steps < future.X)
                        current.X = future.X;
                    else current.X -= steps;
                    break;
                case Direction.Right:
                    if (current.X + steps > future.X)
                        current.X = future.X;
                    else current.X += steps;
                    break;
            }
            Position = current;
        }

        public void Stop()
        {
            Speed = 0;
        }

        /// <summary>
        /// Move the being in a particular direction. 
        /// At this moment do not care about the walls, just move the being.
        /// If the being is not rotated in the given direction, will be firstly.
        /// </summary>
        private void doStep(Direction dir, float frames)
        {
            // rotate if it is necessary
            rotateToDir(dir, ref frames);
            // and use the rest of frames to move
            switch (dir)
            {
                case Direction.Up: Top -= frames; break;
                case Direction.Down: Top += frames; break;
                case Direction.Left: Left -= frames; break;
                case Direction.Right: Left += frames; break;
            }
        }
        /// <summary>
        /// Rotate to a particular direction but not more than "frames" steps.
        /// If less than "frames" steps are needed, leave them for movement.
        /// </summary>
        private void rotateToDir(Direction dir, ref float frames)
        {
            //...
        }

        private const float maxSpeed = 0.25f;  // max speed that any moveable object may has
        private const float minSpeed = 0f;  // min speed that any moveable object may has
        private const float defaultSpeed = 0.1f;

        private const float maxAcc = 0.1f;
        private const float minAcc = 0.0001f;
        private const float defaultAcc = 0.003f;

        /// <summary>
        /// Acceleration of the moveable object
        /// </summary>
        public float Acceleration
        {
            get { return _acceleration; }
            set
            {
                if (value >= minAcc && value <= maxAcc)
                    _acceleration = value;
            }
        }
        private float _acceleration = defaultAcc;

        /// <summary>
        /// Speed of the moveable object. Value si set even if object is not moveing
        /// </summary>
        public float Speed
        {
            get { return _speed; }
            set
            {   // can not be greather than MaxSpeed
                if (value < minSpeed) value = minSpeed;
                else if (value > MaxSpeed) value = MaxSpeed;
                _speed = value;
            }
        }
        private float _speed = minSpeed;

        /// <summary>
        /// Maximum speed that this moveable object can reach generally. 
        /// </summary>
        public float MaxSpeed
        {
            get { return _maxSpeed; }
            set
            {
                if (value <= maxSpeed)
                    _maxSpeed = value;
            }
        }
        private float _maxSpeed = maxSpeed;

        #endregion

        #region Constructors

        public Being(Map map, int x, int y, string textureFile) : base(map, x, y, textureFile) { }

        #endregion

        #region IDestroyable Members

        public void Destroy(GameTime gameTime)
        {
            if (Destroing || Destroyed) return;

            Destroing = true;

            map.RemoveBeing(this);
        }

        public bool Destroing
        {
            get;
            protected set;
        }
        public bool Destroyed
        {
            get;
            protected set;
        }

        #endregion
    }
}
