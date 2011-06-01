using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomber
{
    class Button : MenuItem
    {
        static private readonly Keys hitKey = Keys.Enter;      // key on which this button reac
        private Action call;    // this method will be called when button is pressed

        protected void action()
        {
            if (call != null)
                call();
        }
        protected override void updateInput()
        {
            // if key was pressed and released - action
            if (previous.IsKeyDown(hitKey) && current.IsKeyUp(hitKey))
                action();
        }

        #region Constructors

        public Button(Menu parent, Action action, string text)
            : base(parent)
        {
            call = action;
            this.Text = text;
        }

        #endregion

    }
}
