using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomber
{
    class Menu : Screen
    {
        // keys for moveing between the menu items
        protected Keys up = Keys.Up;
        protected Keys down = Keys.Down;

        // collection of menu items
        protected List<MenuItem> items;
        private int activeItem = 0;     // index of active menu item

        /// <summary>
        /// (Get/Set) Index of currently ative menu item
        /// </summary>
        public int ActiveItem
        {
            get { return activeItem; }
            set
            {
                // deactivate previous item
                items[activeItem].Active = false;

                if (value < 0) activeItem = items.Count - 1;
                else if (value >= items.Count) activeItem = 0;
                else activeItem = value;

                // activate current active item
                items[activeItem].Active = true;
            }
        }

        #region DrawableGameComponent members

        protected override void LoadContent()
        {
            // load background
            base.LoadContent();
            // load menu items
            foreach (MenuItem i in items)   // same interface as map object
                i.LoadContent();
        }
        public override void Initialize()
        {
            base.Initialize();
            // initialize all menu items
            foreach (MenuItem i in items)
                i.Initialize();
            // activate the first menu item
            ActiveItem = 0;
        }
        public override void Update(GameTime gameTime)
        {
            // update keyborad state
            base.Update(gameTime);

            // when close key pressed - close this menu and return to the previous
            // if there is no previous nothing will happen
            if (previous.IsKeyDown(close) && current.IsKeyUp(close))
                PreviousScreen();

            // move the active item
            if (previous.IsKeyDown(up) && current.IsKeyUp(up))
                ActiveItem--;
            if (previous.IsKeyDown(down) && current.IsKeyUp(down))
                ActiveItem++;            

            // update only the active item
            items[activeItem].Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            // draw background
            if (background != null)
                spriteBatch.Draw(background, Vector2.Zero, Color.White);
            // draw menu items
            foreach (MenuItem i in items)
                i.Draw(gameTime);

            spriteBatch.End();
        }
        protected override void UnloadContent()
        {
            // unload background
            base.UnloadContent();
            // unload menu items
            foreach (MenuItem i in items)
                i.UnloadContent();
        }

        #endregion

        #region Constructors

        public Menu(Game game, SpriteBatch spriteBatch, Screen parent)
            : base(game, spriteBatch, parent) { }

        #endregion
    }
}
