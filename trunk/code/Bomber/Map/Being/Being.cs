using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomber
{
    abstract class Being : DestroyableObject, IMovable
    {
        #region Properities

        public bool DefHorizontalPosition { get { return Position.X % Size == 0; } }
        public bool DefVerticalPosition { get { return Position.Y % Size == 0; } }
        /// <summary>
        /// Get the default real position of the actual map position
        /// </summary>
        public Vector2 DefaultPosition { get { return new Vector2(Position.X * Size, Position.Y * Size); } }

        protected Direction lastDirection = Direction.None;

        #endregion

        #region IMovable Members

        /// <summary>
        /// Move Being in a particular direction
        /// </summary>
        /// <param name="dir">Direction of the movement</param>
        /// <param name="frames">Elapsed number of frames since last movement (miliseconds)</param>
        public virtual void Move(Direction dir, int frames)
        {
            float steps = Speed * frames;
            int x = MapX, y = MapY;
            // get position of some map component in the direction we are moveing in
            dir.Shift(ref x, ref y);

            Vector2 current = this.Position;    // this is my position
            Vector2 def = new Vector2(MapX * Size, MapY * Size);
            Vector2 future;     // this is possible future position
            
            Speed += frames * Acceleration;


            // we want to move horizontal, but vertical movement is not finished yet
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
            doStep(dir, steps, ref current, future);
            Position = current;
            lastDirection = dir;
        }

        public void Stop()
        {
            Speed = 0;
            lastDirection = Direction.None;
        }

        /// <summary>
        /// Move the being in a particular direction. 
        /// At this moment do not care about the walls, just move the being.
        /// If the being is not rotated in the given direction, will be firstly.
        /// </summary>
        protected void doStep(Direction dir, float steps, ref Vector2 current, Vector2 future)
        {
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
        }
        /// <summary>
        /// Rotate to a particular direction but not more than "frames" steps.
        /// If less than "frames" steps are needed, leave them for movement.
        /// </summary>
        private void rotateToDir(Direction dir, ref float frames)
        {
            //...
        }

        // max speed that any moveable object may has in this game
        private const float maxSpeed = 0.4f;
        // min speed that any moveable object may has in this game
        private const float minSpeed = 0f;
        // this is the initial value of speed of 
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
        /// Actual speed of the moveable object. Value is set to zero when not moveing
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
        /// Maximum speed that this moveable object can reach when moveing. This value is not changeing, during
        /// the game, only by some external effect.
        /// When Speed reaches this value it is not accelerating any more
        /// </summary>
        public float MaxSpeed
        {
            get { return _maxSpeed; }
            set
            {
                if (value > maxSpeed) value = maxSpeed;
                _maxSpeed = value;
            }
        }
        private float _maxSpeed = defaultSpeed;

        #endregion

        #region IDestroyable Members

        public override void DestroyInitialize()
        {
            base.DestroyInitialize();
            DestroingTime = 0;  // no animation
        }
        public override bool Destroy(GameTime gameTime)
        {
            if (base.Destroy(gameTime)) // if not already destoryed remove from map
            {
                map.RemoveBeing(this);
                return true;
            }
            else return false;  // this means that it is already destroing
        }

        #endregion

        #region Constructors

        public Being(Map map, int x, int y, string textureFile) : base(map, x, y, textureFile) { }

        #endregion

    }
}
