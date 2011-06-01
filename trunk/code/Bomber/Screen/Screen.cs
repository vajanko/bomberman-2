using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomber
{
    class Screen : DrawableGameComponent
    {
        /// <summary>
        /// Key which close this scree. Default is Escape
        /// </summary>
        protected Keys close = Keys.Escape;
        /// <summary>
        /// Path to the file where background image is stored
        /// </summary>
        protected string backgroundFile = null;
        /// <summary>
        /// Background texture
        /// </summary>
        protected Texture2D background = null;

        // the screen which created this one, when null there is no parent
        protected Screen parent;
        protected SpriteBatch spriteBatch;
        public ContentManager Content { get { return Game.Content; } }
        public SpriteBatch SpriteBatch { get { return spriteBatch; } }

        /// <summary>
        /// State of the keyboard at the time of last update call
        /// </summary>
        protected KeyboardState previous;
        /// <summary>
        /// State of the keyboard at this time
        /// </summary>
        protected KeyboardState current;

        #region DrawableGameComponent Members

        protected override void LoadContent()
        {
            // load background image
            if (backgroundFile != null)
                background = Content.Load<Texture2D>(backgroundFile);
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // update keyboard state - when override, this method must be called
            previous = current;
            current = Keyboard.GetState();
        }
        protected override void UnloadContent()
        {
            // prevent from disposing already disposed texture
            if (background != null && !background.IsDisposed)
                background.Dispose();
        }

        #endregion

        #region Screens

        public Screen PreviousScreen()
        {
            if (parent != null)
            {
                // disable this screen
                Enabled = false;
                Visible = false;
                // enable paretn screen
                parent.Enabled = true;
                parent.Visible = true;
            }

            return parent;
        }
        public void NextScreen(Screen next)
        {
            // enable next screen
            next.Enabled = true;
            next.Visible = true;
            // disable this screen
            Enabled = false;
            Visible = false;
        }
        public virtual void FinishMe(Screen screen) { }

        #endregion

        #region Constructors

        /// <summary>
        /// This constructor should not be called directly
        /// </summary>
        private Screen(Game game):base(game) { }

        public Screen(Game game, SpriteBatch spriteBatch, Screen parent)
            : this(game)
        {
            this.parent = parent;
            this.spriteBatch = spriteBatch;
        }

        #endregion
    }
}
