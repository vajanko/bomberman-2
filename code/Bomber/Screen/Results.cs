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

        #region Fields

        // how long the result list will be displayed
        private const int displayTime = 3000;  // ms
        // how long already displayed
        private int time = 0;
        private int[] wins;     // number of wins for each player
        private int winner;     // index of the winner
        private SpriteFont font;

        // texture of each player
        protected Texture2D[] textures;

        #endregion

        #region IGame Members

        public override void Initialize()
        {
            base.Initialize();

            int max = -1;
            // find player with maximum number of wins
            for (int i = 0; i < wins.Length; i++)
                if (wins[i] > max)
                    max = wins[i];
            int maxPlayers = 0; // number of players who have maximum number of wins
            // we expect one - only one winner of whole tournament
            for (int i = 0; i < wins.Length; i++)
                if (wins[i] == max)
                {
                    maxPlayers++;
                    winner = i;     // this will be only useful if maxPlayers == 1
                }
            if (maxPlayers > 1) // noone is the winner of whole tournament
                winner = -1;
        }
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
                Rectangle rec;
                if (i == winner)    // we are drawing the texture of the winner
                {
                    // sin + 1 is a number in range (0,2)
                    double elapsed = gameTime.TotalGameTime.TotalMilliseconds;
                    int size = (int)((Math.Sin((double)elapsed / 100) + 1) * 5);

                    rec = new Rectangle(200 + size, 200 + i * 100 + size, (int)MapObject.Size - 2 * size, (int)MapObject.Size - 2 * size);
                }
                else rec = new Rectangle(200, 200 + i * 100, (int)MapObject.Size, (int)MapObject.Size);

                spriteBatch.Draw(textures[i], rec, Color.White);  
                spriteBatch.DrawString(font, wins[i].ToString(), new Vector2(300, 200 + i * 100), Color.White);
            }
            spriteBatch.End();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (time > displayTime || (previous.IsKeyDown(close) && current.IsKeyUp(close)))
            {
                if (parent != null)
                    parent.FinishMe(this);
            }
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

        #endregion

        #region Constructors

        public Results(Game game, SpriteBatch spriteBatch, Screen parent, int[] wins)
            : base(game, spriteBatch, parent)
        {
            backgroundFile = "Images/mainMenu";
            this.wins = wins;
            // only load texture of participating players
            textures = new Texture2D[wins.Length];
        }

        #endregion
    }
}
