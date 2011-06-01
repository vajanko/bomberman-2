using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bomber
{
    abstract class Bonus : MapObject
    {
        public override void Initialize()
        {
            base.Initialize();
            Destroing = false;
            Destroyed = false;
            IsPassable = true;
        }

        /// <summary>
        /// When player enter bonus position, he is upgreaded by it. This could be: get a new bomb,
        /// improve the fire, speed, ...
        /// </summary>
        /// <param name="player">Instance of player to updrade</param>
        protected abstract void upgrade(Player player);

        #region Draw and Update

        protected override void normalUpdate(GameTime gameTime)
        {
            // base.normalUpdate(gameTime);

            // if there is some player on my position upgrade him
            Player p = map.GetPlayer(MapX, MapY);
            if (p != null)
            {
                upgrade(p);
                Remove();       // remove from map
            }
        }

        #endregion

        #region Constructors

        public Bonus(Map map, int x, int y, string textureFile)
            : base(map, x, y, textureFile) { }

        #endregion

        #region IDestroyable Members

        protected ParticleSystem fire { get { return map.Explostion; } }

        public override bool Destroy(GameTime gameTime)
        {
            if (base.Destroy(gameTime))
            {
                fire.AddParticles(Position + new Vector2(Size / 2), 10, DestroingTime * 2, 0.2f, gameTime);
                return true;
            }
            else return false;
        }

        #endregion
    }
}
