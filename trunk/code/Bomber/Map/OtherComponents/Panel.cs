using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bomber
{
    class PlayersPanel : DrawableGameComponent
    {
        protected SpriteBatch spriteBatch;
        //protected Map map;
        protected Texture2D background;
        public Rectangle Area = Rectangle.Empty;

        protected SpriteFont font;
        protected int players;
        protected Texture2D[] textures;

        protected int[] wins;
        protected int rounds;
        public int[] Wins { get { return wins; } }

        protected ContentManager content { get { return Game.Content; } }

        public override void Initialize()
        {
            //players = map.Players.Count;
            wins = new int[players];
            for (int i = 0; i < players; i++)
                wins[i] = 0;
            textures = new Texture2D[players];
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, Area, Color.White);
            Rectangle rec = new Rectangle(Area.X + (int)(MapObject.Size * 2), Area.Y, (int)MapObject.Size, (int)MapObject.Size);
            spriteBatch.DrawString(font, "Wins:", new Vector2(0, rec.Y + 10), Color.Black);
            for (int i = 0; i < players; i++)
            {
                spriteBatch.Draw(textures[i], rec, Color.White);
                spriteBatch.DrawString(font, wins[i].ToString(), new Vector2(rec.X + MapObject.Size, rec.Y+10), Color.Black);
                rec.X += (int)(MapObject.Size * 2);
            }
            rec.X += (int)(MapObject.Size);
            rec.Y += 10;
            spriteBatch.DrawString(font, "Rounds: " + rounds.ToString(), new Vector2(rec.X, rec.Y), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        protected override void LoadContent()
        {            
            background = content.Load<Texture2D>("Images/playersPanel");
            font = content.Load<SpriteFont>("Fonts/initial");
            for (int i = 0; i < players; i++)
                textures[i] = content.Load<Texture2D>("Images/player" + i.ToString());
            base.LoadContent();
        }
        protected override void UnloadContent()
        {
            for (int i = 0; i < players; i++)
                textures[i].Dispose();
            base.UnloadContent();
        }

        public void Winner(int playerIndex)
        {   // someone has won
            if (playerIndex >= 0)   // if is it less than zero - none win
                wins[playerIndex]++;
            rounds--;   // this round has finished
        }

        /// <summary>
        /// Do not call this constructor
        /// </summary>
        private PlayersPanel(Game game) : base(game) { }
        public PlayersPanel(Game game, SpriteBatch spriteBatch, int players, int rounds)
            : this(game)
        {
            this.spriteBatch = spriteBatch;
            //this.map = map;
            this.players = players;
            this.rounds = rounds;
        }
    }
}
