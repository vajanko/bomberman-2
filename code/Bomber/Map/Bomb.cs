using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bomber
{
    class Bomb : MapObject
    {
        Player owner = null;        // the player who has planted this bomb        

        #region Properities

        /// <summary>
        /// Length of the bomb's fire. Size of the area destroyed by this bomb. It is actually the property
        /// of the player who has planted this bomb
        /// </summary>
        public int FireLenght
        {
            get { return owner.FireLength; }
        }

        #endregion

        #region IGame Members

        public override void Initialize()
        {   // bomb may be reused after it has been exloded, just call this method
            base.Initialize();
            IsPassable = false;
        }

        #endregion

        #region Draw and Update

        protected override void normalDraw(GameTime gameTime)
        {
            // base.normalDraw(gameTime);

            // sin + 1 is a number in rang (0,2)
            double elapsed = gameTime.TotalGameTime.TotalMilliseconds - planted;
            int size = (int)((Math.Sin((double)elapsed / 100) + 1) * 5);

            // draw the bomb stratched from each side -> result is that the bomb is pusling
            SpriteBatch.Draw(texture, new Rectangle((int)Position.X + size, (int)Position.Y + size,
                (int)Width - 2 * size, (int)Height - 2 * size), Color.White);
        }

        protected override void normalUpdate(GameTime gameTime)
        {
            // base.normalUpdate(gameTime);

            // bomb is updated only if it is active - then measure the time since planted
            if (gameTime.TotalGameTime.TotalMilliseconds - planted > time)
                Destroy(gameTime);// if enought time has been elapsed - exlode (animation, destoroy others)
            // at this time bomb becomes destroing (not destroyed)
        }

        #endregion

        /// <summary>
        /// Plant and initialize the bomb at a particular position. It is added to the map components
        /// and becomes updeatable.
        /// </summary>
        /// <param name="x">x-coordinate of the map position</param>
        /// <param name="y">y-coordiante of the map position</param>
        public void Plant(int x, int y, GameTime gameTime)
        {
            this.Position = new Vector2(x * Size, y * Size);    // this will setup also the map position
            this.Initialize();  // initialize timers

            if (map.IsFree(x, y))       // by adding to the map it becomes updateable
                map.Components[x, y] = this;

            // this is the time when bomb was planted
            planted = gameTime.TotalGameTime.TotalMilliseconds;
        }

        #region Constructors

        public Bomb(Map map, Player owner, string textureFile)
            : base(map, textureFile)
        {
            this.owner = owner;
        }

        public Bomb(Map map, Player owner, int x, int y)
            : base(map, x, y)
        {
            this.owner = owner;
        }
        public Bomb(Map map, Player owner, int x, int y, string textureFile) : base(map, x, y, textureFile) 
        {
            this.owner = owner;
        }

        #endregion

        #region IDestroyable Members

        protected const double defaultTime = 3000; // miliseconds
        protected double time = defaultTime;       // after this time elapses, this bomb is going to exlode
        protected double planted;                  // time when the bomb was planted

        // shortcut to map particle system
        protected ParticleSystem explosion { get { return map.Explostion; } }

        public override void DestroyInitialize()
        {
            base.DestroyInitialize();
            DestroingTime = 100;    // ms
        }

        // when exloaded, save the positions of the fire here
        private int[] fireX = new int[4];
        private int[] fireY = new int[4];
        public override bool Destroy(GameTime gameTime)
        {   // just find the exloading area - do not draw

            // this prevents the destroing loop when one bomb exploads next another one
            if (!base.Destroy(gameTime)) return false;
            
            int x, y;
            float halfSize = Size / 2;
            int length;

            // at the centre of the explostion
            map.DestroyBeing(MapX, MapY, gameTime);            

            // find the edge positions of the fire
            for (Direction dir = Direction.Up; dir < Direction.None; dir++)
            {
                x = MapX; y = MapY;         // this is my position
                length = 0;
                do
                {
                    // destroing time is very short (100 ms), so the bomb is returned almost imediatelly to the
                    // player, but particles are living longer time - 500 ms
                    explosion.AddParticles(new Vector2(x * Size + halfSize, y * Size + halfSize), 
                        4, 500, 0.2f, gameTime);
                    dir.Shift(ref x, ref y);    // shift my position in a particular direction
                    // kill beings
                    length++;
                    if (length <= FireLenght)
                    {
                        map.DestroyBeing(x, y, gameTime);
                        fireX[(int)dir] = x;
                        fireY[(int)dir] = y;
                    }
                } while (map.IsFree(x, y) && length <= FireLenght);     // and check if it this position is free
            }

            // now destroy other objects
            for (int i = 0; i < 4; i++) // only components on the edge of fire are destoryed
                map.DestroyComponent(fireX[i], fireY[i], gameTime);

            return true;
        }

        public override void Remove()
        {
            base.Remove();
            owner.AddBomb(this);             // return this bomb to the player
        }

        #endregion
    }
}
