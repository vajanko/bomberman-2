using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomber
{
    class Player : Being
    {
        #region Control

        public Keys Up = Keys.Up;
        public Keys Down = Keys.Down;
        public Keys Left = Keys.Left;
        public Keys Right = Keys.Right;
        public Keys Action1 = Keys.Enter;
        public Keys Action2 = Keys.Space;

        KeyboardState previous;
        KeyboardState current;

        #endregion

        #region Properities

        protected int fireLength = 1;
        /// <summary>
        /// (Get) Lenght of the players bombs fire
        /// </summary>
        public int FireLength
        {
            get { return fireLength; }
        }

        #endregion

        #region Bombs


        protected const int bombsCount = 4; // initial number of player's bombs
        /// <summary>
        /// Collection of player's bombs. If it is empty, player shall not plant a bomb.
        /// </summary>
        protected Queue<Bomb> bombCollection;
        //protected LinkedList<Bomb> bombCollection = new LinkedList<Bomb>();

        /// <summary>
        /// Creates and loads initial count of bombs to player's bomb collection.
        /// When planting a bomb, one of this will be used. When bomb exploades, it is returned to the
        /// player and reused. If bomb collection is empty, player shall not plant a bomb.
        /// </summary>
        protected void loadBombs()
        {
            Bomb b;
            for (int i = 0; i < bombsCount; i++)
            {
                b = new Bomb(map, this, "Images/bomb1");
                b.LoadContent();
                bombCollection.Enqueue(b);
            }
        }
        /// <summary>
        /// Plants a bomb at players actual position, byt only if player has any bomb in it's collection
        /// </summary>
        protected void plantBomb(GameTime gameTime)
        {
            if (!map.IsFree(MapX, MapY)) return;    // shall not plant bomb when position is not free
            if (bombCollection.Count > 0)   // if player has bombs
            {
                Bomb b = bombCollection.Dequeue();      // take some bomb from my preprared
                b.Plant(MapX, MapY, gameTime);          // plant it at my position
            }
        }

        /// <summary>
        /// This method is called by a bomb when it has exploaded. It is returned to the player and reused.
        /// Could be called by bonus to add to player a new bomb
        /// </summary>
        public void AddBomb(Bomb bomb)
        {
            bombCollection.Enqueue(bomb);
        }
        public void AddFire(int lenght)
        {
            fireLength += lenght;
        }

        #endregion

        public override void LoadContent()
        {
            base.LoadContent();
            loadBombs();
        }
        public override void Update(GameTime gameTime)
        {
            if (Destroyed || Destroing) return;

            base.Update(gameTime);  // update the acceleration
            previous = current;
            current = Keyboard.GetState();

            int frames = gameTime.ElapsedGameTime.Milliseconds;

            if (current.IsKeyDown(Up))
                Move(Direction.Up, frames);
            else if (current.IsKeyDown(Down))
                Move(Direction.Down, frames);
            else if (current.IsKeyDown(Left))
                Move(Direction.Left, frames);
            else if (current.IsKeyDown(Right))
                Move(Direction.Right, frames);
            else Stop();

            if (current.IsKeyDown(Action1))
                plantBomb(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            if (Destroyed || Destroing) return;
            base.Draw(gameTime);
        }

        #region Constructors

        public Player(Map map, int x, int y, string textureFile) : base(map, x, y, textureFile) 
        {
            bombCollection = new Queue<Bomb>(bombsCount);
        }

        #endregion
    }
}
