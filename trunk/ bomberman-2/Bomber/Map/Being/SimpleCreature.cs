using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomber
{
    class SimpleCreature : Being
    {
        public override void Move(Direction dir, int frames)
        {
            if (dir == Direction.None)
                manageMoveing();

            Vector2 current = Position;
            Speed += frames * Acceleration;
            float steps = Speed * frames;
            moved += steps;

            if (moved >= Size)
            {
                Position = new Vector2(MapX * Size, MapY * Size);
                manageMoveing();
            }
            else
            {
                switch (dir)
                {
                    case Direction.Up: current.Y -= steps; break;
                    case Direction.Down: current.Y += steps; break;
                    case Direction.Left: current.X -= steps; break;
                    case Direction.Right: current.X += steps; break;
                }
                Position = current;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            MaxSpeed = 0.1f;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Move(currentDirection, gameTime.ElapsedGameTime.Milliseconds);
        }

        #region Moveing

        protected Direction currentDirection = Direction.None;
        protected float moved = 0;
        protected void manageMoveing()
        {
            int x = MapX;
            int y = MapY;
            // get some random direction
            Direction dir = (Direction)(generator.Next(0, 100) % 4);

            for (int i = 0; i < 4; i++)
            {
                x = MapX;
                y = MapY;
                // move current position in the random direction
                dir.Shift(ref x, ref y);
                if (map.IsPassable(x, y))   // if move is possible, move to this position
                {
                    currentDirection = dir;
                    moved = 0;
                    break;
                }
                dir++;
                if (dir >= Direction.None) dir = 0;
            }
            Stop();
        }

        #endregion

        #region Constructors

        public SimpleCreature(Map map, int x, int y, string textureFile)
            : base(map, x, y, textureFile) { }

        #endregion
    }
}
