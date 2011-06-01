using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomber
{
    class NumericButton : MenuItem
    {
        static readonly Keys up = Keys.Right;
        static readonly Keys down = Keys.Left;

        private int value = 1;
        public int MinValue = 0;
        public int MaxValue = 100;

        public int Value
        {
            get { return value; }
            set
            {
                if (value < MinValue) value = MinValue;
                else if (value > MaxValue) value = MaxValue;
                this.value = value;
            }
        }

        protected override void updateInput()
        {
            if (previous.IsKeyDown(up) && current.IsKeyUp(up))
                Value++;
            if (previous.IsKeyDown(down) && current.IsKeyUp(down))
                Value--;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.DrawString(Font, Text + ": " + Value.ToString(), Position + Vector2.UnitX * shift, color);
        }

        #region Constructors


        public NumericButton(Menu parent, string text)
            : base(parent)
        {
            this.Text = text;
        }

        #endregion

    }
}
