using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomber
{
    /// <summary>
    /// Barrel is used in the map to create place where no one can enter. But it can be destoryed
    /// by a bomb. Inside the barrel some bonus may hide.
    /// </summary>
    class Barrel : DestroyableObject
    {
        #region Fields

        private Bonus bonus = null;     // instance of the bonus hidden inside barrel

        #endregion

        #region IGame Memmbers

        public override void Initialize()
        {
            base.Initialize();
            if (bonus != null)
                bonus.Initialize();

            // players and creatures shall not pass the barrel
            IsPassable = false;
        }
        public override void LoadContent()
        {   // only difference to the default load content: also load bonus (if any)
            base.LoadContent();
            if (bonus != null)
                bonus.LoadContent();
        }       

        #endregion

        #region IDestroyable Members

        public override void DestroyInitialize()
        {
            base.DestroyInitialize();
            DestroingTime = 500;    // ms
        }
        public override bool Destroy(GameTime gameTime)
        {
            if (base.Destroy(gameTime))
            {   // add particle to the particle system and do not care about them any more
                smoke.AddParticles(Position + new Vector2(Size / 2), 30, DestroingTime * 5, 1.5f, gameTime);
                return true;
            }
            return false;   // this meads that barrel is already destroing or destroyed
        }
        public override void Remove()
        {
            // remove barrel from map
            base.Remove();
            // if there is some bonus palce it on the map instead of this barrel
            if (bonus != null)
                map.PlaceComponent(MapX, MapY, bonus);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create an instance of barrel without any bonus inside
        /// </summary>
        /// <param name="map">Map where this component will be placed</param>
        /// <param name="x">X-coordinate of the map position</param>
        /// <param name="y">Y-coordinate of the map position</param>
        /// <param name="textureFile">Path to the file where barrel texture is stored</param>
        public Barrel(Map map, int x, int y, string textureFile)
            : base(map, x, y, textureFile) { }

        /// <summary>
        /// Create an instance of barrel with a particular bonus hidden inside
        /// </summary>
        /// <param name="map">Map where this component will be placed</param>
        /// <param name="x">X-coordinate of the map position</param>
        /// <param name="y">Y-coordinate of the map position</param>
        /// <param name="textureFile">Path to the file where barrel texture is stored</param>
        /// <param name="type">Type of the bonus hidden inside the barrel</param>
        public Barrel(Map map, int x, int y, string textureFile, BonusType type)
            : this(map, x, y, textureFile)
        {
            //BonusType = type;
            switch (type)
            {
                case BonusType.Bomb: bonus = new BonusBomb(map, MapX, MapY, "Images/bonusBomb1"); break;
                case BonusType.Fire: bonus = new BonusFire(map, MapX, MapY, "Images/bonusFire1"); break;
                case BonusType.Speed: bonus = new BonusSpeed(map, MapX, MapY, "Images/bonusSpeed1"); break;
            }
        }

        #endregion        
    }

    enum BonusType
    {
        Bomb = 0,
        Fire = 1,
        Speed = 2,
        None
    }
}
