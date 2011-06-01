using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomber
{
    abstract class MenuItem : IGame
    {
        #region Private Fields

        protected bool active = false;   // indicates whether the user has activated this item by arrow,

        protected Color color;           // color actually used for drawing text
        protected float shift;           // when item's active, it is moveing horizontaly. this is actual shift of position

        protected KeyboardState previous;          // state of the keybord at the previous update call
        protected KeyboardState current;           // state of the keyborad at this moment

        /// <summary>
        /// Menu where this item is placed
        /// </summary>
        protected Menu parent;

        protected SpriteBatch spriteBatch { get { return parent.SpriteBatch; } }
        protected ContentManager content { get { return parent.Content; } }

        #endregion

        #region Properities

        public string Text;              // content of this item
        public Color Color;              // color of the text
        public Color ActiveColor;        // color of text when item is active
        public Vector2 Position;         // position of the text in the spriteBatch
        public SpriteFont Font;          // font used to draw the text

        public bool Active  // active item is pusling
        {
            get { return active; }
            set
            {   // also reset the color of the text
                if (value) color = ActiveColor;
                else
                {
                    color = Color;
                    shift = 1;// important -> roll back all transformation, otherwise the last state will be used
                }
                active = value;
            }
        }

        #endregion

        public virtual void LoadContent()
        {
            Font = content.Load<SpriteFont>("Fonts/initial");
        }
        public virtual void Initialize()
        {
            shift = 1;          // not shifted
            Color = Color.Yellow;
            ActiveColor = Color.Green;
            Active = false;
        }
        public virtual void Draw(GameTime gameTime)
        {
            // this method is always called from menu, so spriteBatch.Begin() is not necessary

            // draw the text of menu item
            spriteBatch.DrawString(Font, Text, Position + Vector2.UnitX * shift, color);
        }
        public virtual void Update(GameTime gameTime)
        {
            if (active)     // if item is active -> move with the text
                shift = (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / 100) * 10;
            // interpolate between values -10 and +10

            // read keyborad state
            previous = current;
            current = Keyboard.GetState();

            // handle input
            updateInput();
        }
        public virtual void UnloadContent()
        {
            Font = null;
        }

        protected abstract void updateInput();


        #region Constructors

        public MenuItem(Menu parent)
        {
            this.parent = parent;

            Text = "";          // no text
            Color = Color.LightGreen;
            ActiveColor = Color.Green;
            Position = Vector2.Zero;
        }

        #endregion
    }
}
