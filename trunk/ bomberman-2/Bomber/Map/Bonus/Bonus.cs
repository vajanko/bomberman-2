using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bomber
{
    abstract class Bonus : MapObject, IDestroyable
    {
        public override void Initialize()
        {
            base.Initialize();
            Destroing = false;
            Destroyed = false;
            IsPassable = true;
        }
        public override void Update(GameTime gameTime)
        {
            if (Destroing)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds - destroyedTime > destroingTime)
                {
                    Destroyed = true;
                    map.RemoveComponent(MapX, MapY);
                }
            }
            else
            {   // check for players position
                base.Update(gameTime);
                // if there is some player on my position upgrade him
                Player p = map.GetPlayer(MapX, MapY);
                if (p != null)
                {
                    upgrade(p);
                    remove();       // remove from map
                }

            }
        }

        /// <summary>
        /// When player enter bonus position, he is upgreaded by it. This could be: get a new bomb,
        /// improve the fire, speed, ...
        /// </summary>
        /// <param name="player">Instance of player to updrade</param>
        protected abstract void upgrade(Player player);

        protected void remove()
        {
            map.RemoveComponent(MapX, MapY);
        }

        #region Constructors

        public Bonus(Map map, int x, int y, string textureFile)
            : base(map, x, y, textureFile) { }

        #endregion

        #region IDestroyable Members

        protected const double defaultTime = 1000;
        protected double destroingTime = defaultTime;
        protected double destroyedTime;

        protected ParticleSystem fire { get { return map.Explostion; } }

        public void Destroy(GameTime gameTime)
        {
            if (Destroing || Destroyed) return;

            Destroing = true;
            destroyedTime = gameTime.TotalGameTime.TotalMilliseconds;

            fire.AddParticles(Position + new Vector2(Size / 2), 10, destroingTime * 2, 0.2f, gameTime);
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
