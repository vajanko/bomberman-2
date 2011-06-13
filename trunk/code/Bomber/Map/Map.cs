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
    class Map : Screen, IDestroyable
    {
        #region Properties

        /// <summary>
        /// The area where the map is dispalyed on the screen
        /// </summary>
        public Rectangle Area = Rectangle.Empty;

        /// <summary>
        /// (Get) Number of map components horizontally
        /// </summary>
        public int Width { get { return components.Width; } }
        /// <summary>
        /// (Get) Number of map components vertically
        /// </summary>
        public int Height { get { return components.Height; } }

        /// <summary>
        /// (Get) Number of playres that are not killed
        /// </summary>
        public int ActivePlayers
        {
            get;
            protected set;
        }

        /// <summary>
        /// (Get) Index of the last palyer on the map, who has won the match
        /// </summary>
        public int WinnerIndex
        {
            get
            {
                if (ActivePlayers != 1) // noone is the winner
                    return -1;

                for (int i = 0; i < players.Length; i++)
                    if (!players[i].Destroing)
                        return i;

                return -1;
            }
        }

        #endregion

        #region Map Components

        private MapObjectCollection components;
        public MapObjectCollection Components { get { return components; } }

        // collection of object that should be unloaded
        private LinkedList<MapObject> toUnload;

        private ParticleSystem explosion;
        public ParticleSystem Explostion { get { return explosion; } }
        private ParticleSystem smoke;
        public ParticleSystem Smoke { get { return smoke; } }

        private Player[] players;
        public Player[] Players { get { return players; } }

        private List<Being> creatures;
        
        /// <summary>
        /// Is the position (x,y) in the map free? Notice that even if the position is free, there may be some moveable
        /// game object.
        /// </summary>
        public bool IsFree(int x, int y)
        {
            return components[x, y] == null;
        }

        /// <summary>
        /// Is the position (x,y) in the map passble?
        /// </summary>
        public bool IsPassable(int x, int y)
        {   // position is passable if there is nothing or the component is passable
            return (IsFree(x, y) || components[x, y].IsPassable);
        }

        /// <summary>
        /// Get player on a particular position. If ther is no player return null
        /// </summary>
        public Player GetPlayer(int x, int y)
        {
            foreach (Player p in players)
                if (p.MapX == x && p.MapY == y)
                    return p;
            return null;
        }

        public void DestroyComponent(int x, int y, GameTime gameTime)
        {
            IDestroyable toDestroy = (components[x,y] as IDestroyable);
            if (toDestroy != null)
                toDestroy.Destroy(gameTime);
        }

        public void RemoveComponent(int x, int y)
        {
            if (components[x, y] != null)   // this component will be unloaded
                toUnload.AddFirst(components[x, y]);
            components[x, y] = null;        // stop update and draw this component
        }

        public void PlaceComponent(int x, int y, MapObject comp)
        {
            components[x, y] = comp;
        }

        /// <summary>
        /// Destroy all beings on position [x,y]
        /// </summary>
        public void DestroyBeing(int x, int y, GameTime gameTime)
        {            
            // Check for creature position from the end of array - when item is removed next items 
            // are shifted
            for (int i = creatures.Count - 1; i >= 0; i--)
                if (creatures[i].MapX == x && creatures[i].MapY == y)
                    // find creature at position (x,y), but do not remove from list
                    // this is done by the creature itself
                    creatures[i].Destroy(gameTime);
            // Check for player position
            for (int i = 0; i < players.Length; i++)
                if (!players[i].Destroyed && players[i].MapX == x && players[i].MapY == y)
                 //{
                    players[i].Destroy(gameTime);
                 //   ActivePlayers--;
                 //   (Game as Game1).PlayerKilled(i);
                 //   if (ActivePlayers <= 1)     // only one player is living - game finished
                 //       Destroy(gameTime);
                 //}
        }
        public void PlayerKilled(Player player, GameTime gameTime)
        {
            ActivePlayers--;
            
            //(Game as Game1).PlayerKilled(i);
            if (ActivePlayers <= 1)     // only one player is living - game finished
                Destroy(gameTime);
        }
        public void RemoveBeing(Being being)
        {
            creatures.Remove(being);
            //players.Remove(being);
            toUnload.AddFirst(being);
        }


        #endregion

        #region DrawableGameComponent Members

        /// <summary>
        /// Load content for this map, create new map components and load their content
        /// </summary>
        protected override void LoadContent()
        {
            // load background
            base.LoadContent();

            // at this time map contains all components, but not loaded
            foreach (MapObject comp in components)  // load map components
                comp.LoadContent();
            foreach (Player p in players)           // load players
                p.LoadContent();
            foreach (Being b in creatures)          // load creatures
                b.LoadContent();

            // load particle systems
            explosion.LoadContent();
            smoke.LoadContent();
        }
        /// <summary>
        /// At this time all components must be already added to the collection
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // initialize all map components
            foreach (MapObject comp in components)
                comp.Initialize();
            // initialize players controll
            for (int i = 0; i < players.Length; i++)
            {
                Player p = players[i];
                p.CUp = PlayersControl[i][0];
                p.CDown = PlayersControl[i][1];
                p.CLeft = PlayersControl[i][2];
                p.CRight = PlayersControl[i][3];
                p.CAction1 = PlayersControl[i][4];
            }
            // initialize players
            foreach (Player p in players)
                p.Initialize();
            // initialize creature
            foreach (Being b in creatures)
                b.Initialize();

            // initialize particle system
            explosion.Initialize();
            smoke.Initialize();

            DestroyInitialize();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // when exit key is pressed and released - return to main menu
            if (previous.IsKeyDown(close) && current.IsKeyUp(close))
                Game.Exit();

            // when final animation is played and enought time has been elapsed, then remove
            // map from the game
            if (Destroing && gameTime.TotalGameTime.TotalMilliseconds - DestroyTime > DestroingTime)
                Remove();

            // update all map components
            foreach (MapObject comp in components)
                comp.Update(gameTime);

            // update particle systems
            explosion.Update(gameTime);
            smoke.Update(gameTime);

            // update moveing objects: players and creatures
            foreach (Player p in players)
                p.Update(gameTime);
            foreach (Being b in creatures)
                b.Update(gameTime);

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            
            // draw map background
            SpriteBatch.Draw(background, Vector2.Zero, Color.White);

            // draw all game components inside the map
            foreach (MapObject comp in components)
                comp.Draw(gameTime);
            // draw moveing objects
            foreach (Player p in players)
                p.Draw(gameTime);
            foreach (Being b in creatures)
                b.Draw(gameTime);

            SpriteBatch.End();

            // change blend mode to additive
            SpriteBatch.Begin(SpriteBlendMode.Additive, SpriteSortMode.Deferred, SaveStateMode.None);

            // draw particles
            explosion.Draw(gameTime);
            smoke.Draw(gameTime);

            SpriteBatch.End();
        }
        protected override void UnloadContent()
        {
            // unload all map components
            foreach (MapObject comp in components)
                comp.UnloadContent();
            components.Clear();
            components = null;

            foreach (Player p in players)
                p.UnloadContent();
            foreach (Being b in creatures)
                b.UnloadContent();
            foreach (MapObject o in toUnload)
                o.UnloadContent();

            // unload particle system
            explosion.UnloadContent();
            smoke.UnloadContent();

            // unload background
            base.UnloadContent();
        }

        #endregion

        #region Map Construction

        private bool createMapFromFile(string filename)
        {
            // try to open the file where map is stored
            StreamReader reader;
            try
            {
                filename = Path.Combine(Content.RootDirectory, "Maps\\" + filename);
                reader = new StreamReader(filename);
            }
            catch (Exception)
            {   // could not read the file
                return false;
            }

            // read the map size of 
            string[] size;
            size = reader.ReadLine().Split(' ');
            int width, height;
            if (!int.TryParse(size[0], out width))
                return false;
            if (!int.TryParse(size[1], out height))
                return false;

            // create component colleciton acording readed map size
            components = new MapObjectCollection(width, height);

            // read map components and add them to the map component collection
            for (int i = 0; i < height; i++)
            {
                string line = reader.ReadLine();
                for (int j = 0; j < width; j++)
                    createComponent(line[j], j, i);
            }

            // map has been loaded
            return true;
        }

        private void createComponent(char c, int x, int y)
        {   // digit means - it is a place for player
            if (char.IsDigit(c))
            {   // in the map file, there can be more digits (reserved for players) but we do not use all of them
                int index = c - '0';
                if (index < ActivePlayers)   // only add player if it is allowed
                    players[index] = new Player(this, x, y, "Images/player" + index.ToString(), index);
            }
            else
            {
                MapObject comp = null;

                switch (c)
                {
                    // create stone
                    case 's': comp = new Stone(this, x, y, "Images/stone2"); break;
                    // '.' means free space - some barrel may be created
                    case '.':
                        if (generator.Next(0, 10) < 3) return;  // do not create barrel on each position

                        int rnd = generator.Next(0, 7);
                        BonusType type = BonusType.None;
                        if (rnd < 4)    // only 4 of 7 barrel have some bonus
                            type = (BonusType)rnd;
                        comp = new Barrel(this, x, y, "Images/box2", type);
                        break;
                }
                if (comp != null)   // just create an instace of this component, but do not load
                    components[x, y] = comp;
            }
        }

        #endregion

        #region IDestroyable Members

        public double DestroyTime
        {
            get;
            protected set;
        }
        public double DestroingTime
        {
            get;
            protected set;
        }

        public virtual void DestroyInitialize()
        {
            Destroing = false;
            Destroyed = false;
            DestroyTime = 0;
            DestroingTime = 3000; // ms
        }

        public bool Destroy(GameTime gameTime)
        {
            // if already destroyed
            if (Destroing || Destroyed) return false;

            Destroing = true;
            // this is time when this component was destroyed
            DestroyTime = gameTime.TotalGameTime.TotalMilliseconds;
            return true;
        }

        public void Remove()
        {
            Destroyed = true;
            // tell to the MainMenu (in this case) that game has finished
            parent.FinishMe(this);
        }

        public bool Destroing
        {
            get;
            protected set;
        }
        public bool Destroyed
        {
            get;
            protected set;
        }

        #endregion

        #region Constructors
        
        private Map(Game game, SpriteBatch spriteBatch, Screen parent)
            : base(game, spriteBatch, parent)
        {
            backgroundFile = "Images/mapBack1";
            explosion = new ParticleSystem(this, "Images/explosion");
            smoke = new ParticleSystem(this, "Images/smoke");
            toUnload = new LinkedList<MapObject>();
            generator = new Random();
        }

        //public Map(Game game, SpriteBatch spriteBatch, int width, int height)
        //    : this(game)
        //{
        //    _spriteBatch = spriteBatch;

        //    components = new MapObjectCollection(width, height);
        //}

        public Map(Game game, SpriteBatch spriteBatch, Screen parent, int playersCount, string mapFile)
            : this(game, spriteBatch, parent)
        {            
            this.ActivePlayers = playersCount;

            players = new Player[playersCount];

            createMapFromFile(mapFile);

            creatures = new List<Being>();
            SimpleCreature cr;
            for (int i = 0; i < playersCount * 2; i++)
            {
                cr = new SimpleCreature(this, generator.Next(3, Width - 3), generator.Next(3, Height - 3), "Images/simple1");
                creatures.Add(cr);
            }
        }

        #endregion



        /// <summary>
        /// Number generator is used to randomly distribute barrels on the map,
        /// for creating bonuses or creatures on random positions, ...
        /// </summary>
        static public Random generator;

        static public Keys[][] PlayersControl;
        public const int MaxPlayers = 4;
        static Map()
        {
            PlayersControl = new Keys[MaxPlayers][];
            for (int i = 0; i < MaxPlayers; i++)
                PlayersControl[i] = new Keys[6];

            PlayersControl[0][0] = Keys.W;
            PlayersControl[0][1] = Keys.S;
            PlayersControl[0][2] = Keys.A;
            PlayersControl[0][3] = Keys.D;
            PlayersControl[0][4] = Keys.Tab;

            PlayersControl[1][0] = Keys.I;
            PlayersControl[1][1] = Keys.K;
            PlayersControl[1][2] = Keys.J;
            PlayersControl[1][3] = Keys.L;
            PlayersControl[1][4] = Keys.Space;

            PlayersControl[2][0] = Keys.Up;
            PlayersControl[2][1] = Keys.Down;
            PlayersControl[2][2] = Keys.Left;
            PlayersControl[2][3] = Keys.Right;
            PlayersControl[2][4] = Keys.RightControl;

            PlayersControl[3][0] = Keys.NumPad8;
            PlayersControl[3][1] = Keys.NumPad5;
            PlayersControl[3][2] = Keys.NumPad4;
            PlayersControl[3][3] = Keys.NumPad6;
            PlayersControl[3][4] = Keys.NumPad0;
        }
    }
}
