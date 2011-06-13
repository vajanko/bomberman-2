using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomber
{
    class MainMenu : Menu
    {
        #region Actions

        private int rounds;
        private int players;
        private bool playing = false;
        private PlayersPanel bottomPanel;
        private void start()
        {
            rounds = (items[2] as NumericButton).Value;
            players = (items[1] as NumericButton).Value;
            playing = true;

            // this panel is for all rounds
            bottomPanel = new PlayersPanel(Game, spriteBatch, players, rounds);

            // important is that this number is higher than on the map
            bottomPanel.DrawOrder = 2;
            Game.Components.Add(bottomPanel);

            // create map
            startRound();
        }
        private void startRound()
        {
            rounds--;
            Map map = new Map(Game, spriteBatch, this, players, "map1.dat");
            map.DrawOrder = 1;

            Rectangle rec = GraphicsDevice.DisplayMode.TitleSafeArea;
            rec.Height -= (int)MapObject.Size * 3;
            map.Area = rec;

            // set panel area each time round is started - size of map could change
            bottomPanel.Area = new Rectangle(rec.X, (int)(map.Height * MapObject.Size),
                (int)(map.Width * MapObject.Size), (int)MapObject.Size);            

            Game.Components.Add(map);
            NextScreen(map);
        }
        private void showResults()
        {
            Results res = new Results(Game, spriteBatch, this, bottomPanel.Wins);
            Game.Components.Add(res);
            NextScreen(res);
        }
        public override void FinishMe(Screen screen)
        {
            Game.Components.Remove(screen);

            if (playing)    // this call was made from map
            {
                // update the score
                bottomPanel.Winner((screen as Map).WinnerIndex);
                if (rounds > 0)
                    startRound();   // start next round
                else    // tournament has finished return to the main menu
                {   // stop game
                    playing = false;
                    showResults();
                    Game.Components.Remove(bottomPanel);
                }
            }
            else
            {   // this means: return to the main menu
                Enabled = true;
                Visible = true;
            }
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // when close key pressed - exit the game
            if (previous.IsKeyDown(close) && current.IsKeyUp(close))
                Game.Exit();
        }

        #region Constructors

        public MainMenu(Game game, SpriteBatch spriteBatch, Screen parent)
            : base(game, spriteBatch, parent)
        {
            backgroundFile = "Images/mainMenu";
            items = new List<MenuItem>(5);

            Button b = new Button(this, new Action(start), "Start");
            b.Position = new Vector2(300, 100);
            items.Add(b);

            NumericButton nb = new NumericButton(this, "Players");
            nb.Position = new Vector2(300, 200);
            nb.MinValue = 2;
            nb.MaxValue = 4;
            nb.Value = 2;
            items.Add(nb);

            nb = new NumericButton(this, "Rounds");
            nb.Position = new Vector2(300, 300);
            nb.MinValue = 1;
            nb.MaxValue = 10;
            nb.Value = 4;
            items.Add(nb);

            b = new Button(this, new Action(Game.Exit), "Exit");
            b.Position = new Vector2(300, 400);
            items.Add(b);
        }

        #endregion
    }
}
