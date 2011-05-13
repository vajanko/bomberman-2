using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomber
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region Map

        private Map map;
        private PlayersPanel bottomPanel;
        private int rounds = 0;
        private int players = 0;
        private string mapFile = "";

        protected void startTournament(int rounds, int players, string mapFile)
        {
            // this panel is for all rounds
            bottomPanel = new PlayersPanel(this, spriteBatch, players, rounds);
            bottomPanel.DrawOrder = 2;
            Components.Add(bottomPanel);

            // in each round new map is created
            //for (int i = 0; i < rounds; i++)
            //{
            //    startRound(players, mapFile);
            //}
            this.rounds = rounds;
            this.players = players;
            this.mapFile = mapFile;
            startRound(players, mapFile);   // start first round
        }

        protected void startRound(int players, string mapFile)
        {
            this.rounds--;
            map = new Map(this, spriteBatch, players, mapFile);
            map.DrawOrder = 1;

            Rectangle rec = GraphicsDevice.DisplayMode.TitleSafeArea;
            rec.Height -= (int)MapObject.Size * 3;
            map.Area = rec;

            // set panel area each time round is started - size of map could change
            bottomPanel.Area = new Rectangle(rec.X, (int)(map.Height * MapObject.Size),
                (int)(map.Width * MapObject.Size), (int)MapObject.Size);

            // play some initial animation - for preparing
            Components.Add(map);
        }

        public void EndRound(int playerIndex)
        {
            bottomPanel.Winner(playerIndex);    // increase number of wins to some player
            Components.Remove(map);
            if (this.rounds > 0)
                startRound(this.players, this.mapFile);
        }
        public void PlayerKilled(int playerIndex)
        {

        }

        public void EndTournament() { }

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.graphics.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // TODO: Add game components here
            spriteBatch = new SpriteBatch(GraphicsDevice);

            startTournament(3, 2, "map1.dat");
            //this.Components.Add(map);   // map becomes updeatable and drawable
            //this.Components.Add(bottomPanel);            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            // deleted

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            base.Draw(gameTime);
        }
    }
}
