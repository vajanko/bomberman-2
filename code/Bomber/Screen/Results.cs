using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomber
{
    class Results : Screen
    {
        // how long the result list will be displayed
        private const int displayTime = 3000;  // ms
        // how long already displayed
        private int time = 0;
        private int[] wins;
        private SpriteFont font;

        protected Texture2D[] textures;

        protected override void LoadContent()
        {
            // load background
            base.LoadContent();

            // load font for writing results
            font = Game.Content.Load<SpriteFont>("Fonts/initial");

            // load images of players
            for (int i = 0; i < wins.Length; i++)
                textures[i] = Game.Content.Load<Texture2D>("Images/player" + i.ToString());
        }
        public override void Draw(GameTime gameTime)
        {
            //base.Draw(gameTime);
            spriteBatch.Begin();
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "Results", new Vector2(300, 100), Color.White);
            for (int i = 0; i < wins.Length; i++)
            {
                spriteBatch.Draw(textures[i], new Rectangle(200, 200 + i * 100, (int)MapObject.Size, (int)MapObject.Size),
                    Color.White);
                spriteBatch.DrawString(font, wins[i].ToString(), new Vector2(300, 200 + i * 100), Color.White);
            }
            spriteBatch.End();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (time > displayTime || (previous.IsKeyDown(close) && current.IsKeyUp(close))) 
                parent.FinishMe(this);
            else time += gameTime.ElapsedGameTime.Milliseconds;
        }
        protected override void UnloadContent()
        {
            // unload background
            base.UnloadContent();
            // unload palyers texture
            for (int i = 0; i < textures.Length; i++)
                textures[i].Dispose();
        }


        #region Constructors

        public Results(Game game, SpriteBatch spriteBatch, Screen parent, int[] wins)
            : base(game, spriteBatch, parent)
        {
            backgroundFile = "Images/mainMenu";
            this.wins = wins;
            textures = new Texture2D[wins.Length];
        }

        #endregion
    }
}
