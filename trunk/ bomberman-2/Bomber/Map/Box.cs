using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomber
{
    class Box : MapObject, IDestroyable
    {
        public BonusType BonusType = BonusType.None;
        private Bonus bonus = null;

        #region Game

        public override void Initialize()
        {
            base.Initialize();
            Destroing = false;
            Destroyed = false;
            switch (BonusType)
            {
                case BonusType.Bomb: bonus = new BonusBomb(map, MapX, MapY, "Images/bonusBomb1"); break;
                case BonusType.Fire: bonus = new BonusFire(map, MapX, MapY, "Images/bonusFire1"); break;
            }
            if (bonus != null)
                bonus.Initialize();
            IsPassable = false;
        }
        public override void LoadContent()
        {
            base.LoadContent();
            if (bonus != null)
                bonus.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            if (Destroing)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds - destroyedTime > destroingTime)
                    remove();
            }
            else
                base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            if (!Destroing)
                base.Draw(gameTime);
        }
        // notice: We do not unload the bonus object. This will be done by map.

        #endregion

        #region IDestroyable Members

        protected const double defaultTime = 500;
        protected double destroingTime = defaultTime;
        protected double destroyedTime;

        protected ParticleSystem smoke { get { return map.Smoke; } }

        private void remove()
        {
            Destroyed = true;
            map.RemoveComponent(MapX, MapY);
            if (bonus != null)
                map.PlaceComponent(MapX, MapY, bonus);
        }

        public void Destroy(GameTime gameTime)
        {
            if (Destroing || Destroyed) return;

            Destroing = true;
            destroyedTime = gameTime.TotalGameTime.TotalMilliseconds;

            // add particle to the particle system and do not care about them any more
            smoke.AddParticles(Position + new Vector2(Size / 2), 30, destroingTime * 5, 1.5f, gameTime);
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

        #region Constructors

        public Box(Map map, int x, int y) : base(map, x, y) { }
        public Box(Map map, int x, int y, string textureFile) : base(map, x, y, textureFile) { }

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
