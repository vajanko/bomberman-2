using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomber
{
    /// <summary>
    /// Barrel is used in the map to create place where noone can enter. But it can be destoryed
    /// by a bomb. Inside the barrel some bonus may hide.
    /// </summary>
    class Barrel : MapObject
    {
        public BonusType BonusType = BonusType.None;
        private Bonus bonus = null;

        #region Game

        public override void Initialize()
        {
            base.Initialize();
            Destroing = false;
            Destroyed = false;
            if (bonus != null)
                bonus.Initialize();

            // players and creatures shall not pass the barrel
            IsPassable = false;
        }
        public override void LoadContent()
        {
            base.LoadContent();
            if (bonus != null)
                bonus.LoadContent();
        }       
        // notice: We do not unload the bonus object. This will be done by map.
        // unload method for this component is in the base class

        #endregion

        #region Draw and Update

        // nothig to override

        #endregion

        #region IDestroyable Members

        protected ParticleSystem smoke { get { return map.Smoke; } }

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
            return false;
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

        //public Box(Map map, int x, int y) : base(map, x, y) { }
        public Barrel(Map map, int x, int y, string textureFile)
            : base(map, x, y, textureFile)
        {
        }
        public Barrel(Map map, int x, int y, string textureFile, BonusType type)
            : this(map, x, y, textureFile)
        {
            BonusType = type;
            switch (BonusType)
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
