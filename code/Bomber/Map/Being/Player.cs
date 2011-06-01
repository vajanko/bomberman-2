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

        public Keys CUp = Keys.Up;
        public Keys CDown = Keys.Down;
        public Keys CLeft = Keys.Left;
        public Keys CRight = Keys.Right;
        public Keys CAction1 = Keys.Enter;
        public Keys CAction2 = Keys.Space;

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

        /// <summary>
        /// (Get) Unic identifier for each player
        /// </summary>
        public int Id
        {
            get;
            protected set;
        }

        /// <summary>
        /// (Get) If true player won't die
        /// </summary>
        public bool IsInmortal
        {
            get;
            protected set;
        }

        #endregion

        #region Bombs


        protected const int bombsCount = 1; // initial number of player's bombs
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
        private double lastPlanted = 0;
        private const double plantingInterval = 200;
        /// <summary>
        /// Plants a bomb at players actual position, byt only if player has any bomb in it's collection
        /// </summary>
        protected void plantBomb(GameTime gameTime)
        {
            // allow to plant bombs only in some interval
            if (gameTime.TotalGameTime.TotalMilliseconds - lastPlanted < plantingInterval) return;

            if (!map.IsFree(MapX, MapY)) return;    // shall not plant bomb when position is not free
            if (bombCollection.Count > 0)   // if player has bombs
            {
                Bomb b = bombCollection.Dequeue();      // take some bomb from my preprared
                b.Plant(MapX, MapY, gameTime);          // plant it at my position
                // remember the time when bomb was planted
                lastPlanted = gameTime.TotalGameTime.TotalMilliseconds;
            }
        }

        #endregion

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
        public void AddSpeed(float speed)
        {
            MaxSpeed += speed;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            loadBombs();
        }
        public override void Initialize()
        {
            base.Initialize();
            Acceleration = 0.005f;
            MaxSpeed = 0.14f;
            fireLength = 2;
        }

        #region Draw and Update

        protected override void normalUpdate(GameTime gameTime)
        {
            base.normalUpdate(gameTime);

            previous = current;
            current = Keyboard.GetState();

            int frames = gameTime.ElapsedGameTime.Milliseconds;

            if (current.IsKeyDown(CUp))
                Move(Direction.Up, frames);
            else if (current.IsKeyDown(CDown))
                Move(Direction.Down, frames);
            else if (current.IsKeyDown(CLeft))
                Move(Direction.Left, frames);
            else if (current.IsKeyDown(CRight))
                Move(Direction.Right, frames);
            else Stop();

            if (current.IsKeyDown(CAction1))
                plantBomb(gameTime);
        }

        #endregion

        public override void DestroyInitialize()
        {
            base.DestroyInitialize();
            // for debug or temporary for some special functionality set IsInmortal to true and player will not die
            IsInmortal = false;
        }
        public override bool Destroy(GameTime gameTime)
        {
            // not possible to destoroy player if inmortal
            if (IsInmortal) return false;

            if (base.Destroy(gameTime))
            {   // tell to map that this player was killed - show on the screen
                map.PlayerKilled(this, gameTime);
                return true;
            }
            else return false;
        }

        #region Constructors

        public Player(Map map, int x, int y, string textureFile, int id) : base(map, x, y, textureFile) 
        {
            bombCollection = new Queue<Bomb>(bombsCount);
            Id = id;
        }

        #endregion
    }
}
