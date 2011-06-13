using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bomber
{
    class DestroyableObject : MapObject, IDestroyable
    {
        #region IGame Members

        public override void Initialize()
        {
            base.Initialize();
            // initialize the properities defined by IDestroyable
            DestroyInitialize();
        }

        /// <summary>
        /// Draw the game component to the sprite batch.
        /// If an override needed, use normalDraw or destroingDraw methods
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            if (Destroing)
                destroingDraw(gameTime);
            else
                normalDraw(gameTime);
        }
        public override void Update(GameTime gameTime)
        {
            if (Destroing)  // component is being destroing - an animation
            {
                if (gameTime.TotalGameTime.TotalMilliseconds - DestroyTime > DestroingTime)
                    Remove();   // destring time elapsed - remove from map
                else
                    destroingUpdate(gameTime);  // play an animation
            }
            else   // nothig special - component is updated like it is
                normalUpdate(gameTime);
        }

        #endregion

        #region IDestroyable Members

        // shortcuts to the map
        protected ParticleSystem fire { get { return map.Explostion; } }
        protected ParticleSystem smoke { get { return map.Smoke; } }

        public double DestroyTime
        {
            get;
            protected set;
        }
        public double DestroingTime
        {
            get;
            protected set;
        }

        public virtual void DestroyInitialize()
        {
            Destroing = false;
            Destroyed = false;
            DestroyTime = 0;
            DestroingTime = 1000; // ms
        }

        public virtual bool Destroy(GameTime gameTime)
        {
            // if already destroyed
            if (Destroing || Destroyed) return false;

            Destroing = true;
            // this is time when this component was destroyed
            DestroyTime = gameTime.TotalGameTime.TotalMilliseconds;
            return true;
        }

        public virtual void Remove()
        {
            Destroyed = true;
            map.RemoveComponent(MapX, MapY);
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

        #region Draw and Update

        protected virtual void normalUpdate(GameTime gameTime) { }
        protected virtual void destroingUpdate(GameTime gameTime) { }

        protected virtual void normalDraw(GameTime gameTime)
        {
            // notice that we do not call SpirteBatch.Begin() and .End(), because this component is
            // drawn inside the map, where .Begin() and .End methods are called
            if (texture != null)
                SpriteBatch.Draw(texture, Area, Color.White);
        }
        protected virtual void destroingDraw(GameTime gameTime) { }

        #endregion

        #region Constructors

        public DestroyableObject(Map map, int x, int y, string textureFile)
            : base(map, x, y, textureFile) { }

        public DestroyableObject(Map map, string textureFile) : base(map, textureFile) { }

        public DestroyableObject(Map map, int x, int y) : base(map, x, y) { }

        #endregion
    }
}
