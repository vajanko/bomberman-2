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
    class Map : DrawableGameComponent, IDestroyable
    {
        #region Private Fields

        private string backgroundFile = "Images/mapBack1";
        private Vector2 position = Vector2.Zero;

        // file where the map is stored
        private string mapFile = "";

        private Random generator;

        #endregion
        
        #region Properities

        private SpriteBatch _spriteBatch = null;
        public SpriteBatch SpriteBatch { get { return _spriteBatch; } }

        /// <summary>
        /// The area where the map is dispalyed on the screen
        /// </summary>
        public Rectangle Area = Rectangle.Empty;

        /// <summary>
        /// Number of map components horizontally
        /// </summary>
        public int Width { get { return components.Width; } }
        /// <summary>
        /// Number of map components vertically
        /// </summary>
        public int Height { get { return components.Height; } }

        /// <summary>
        /// Get or set background image texture
        /// </summary>
        public Texture2D Backgorund
        {
            get { return _background; }
            set { _background = value; }
        }
        private Texture2D _background;

        /// <summary>
        /// Number of playres that are not killed
        /// </summary>
        public int ActivePlayers
        {
            get;
            protected set;
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
            if (components[x, y] != null)   // this component will be unloaded in the next loop
                toUnload.AddFirst(components[x, y]);
            components[x, y] = null;        // stop update and draw this component
        }

        public void PlaceComponent(int x, int y, MapObject comp)
        {
            components[x, y] = comp;
        }

        public void DestroyBeing(int x, int y, GameTime gameTime)
        {            
            for (int i = creatures.Count - 1; i >= 0; i--)
                if (creatures[i].MapX == x && creatures[i].MapY == y)
                    creatures[i].Destroy(gameTime);
            for (int i = 0; i < players.Length; i++)
                if (!players[i].Destroyed && players[i].MapX == x && players[i].MapY == y)
                {
                    players[i].Destroy(gameTime);
                    ActivePlayers--;
                    (Game as Game1).PlayerKilled(i);
                    if (ActivePlayers <= 1)     // only one player is living - game finished
                        Destroy(gameTime);
                }
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
        /// At this time all components must be already added to the collection
        /// </summary>
        public override void Initialize()
        {
            // initialize all map components
            foreach (MapObject comp in components)
                comp.Initialize();
            // initialize players controll
            for (int i = 0; i < players.Length; i++)
            {
                Player p = players[i];
                p.Up = PlayersControl[i][0];
                p.Down = PlayersControl[i][1];
                p.Left = PlayersControl[i][2];
                p.Right = PlayersControl[i][3];
                p.Action1 = PlayersControl[i][4];
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

            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            if (Destroing && gameTime.TotalGameTime.TotalMilliseconds - finihed > time)
                remove();

            // update all map components
            foreach (MapObject comp in components)
                comp.Update(gameTime);
            // update particle systems
            explosion.Update(gameTime);
            smoke.Update(gameTime);

            // update moveing objects
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
            SpriteBatch.Draw(_background, Area, Color.White);

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
        /// <summary>
        /// Load content for this map, create new map components and load their content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            _background = this.Game.Content.Load<Texture2D>(backgroundFile);

            // at this time map contains all components, but not loaded
            foreach (MapObject comp in components)  // load map components
                comp.LoadContent();
            foreach (Player p in players)           // load players
                p.LoadContent();
            foreach (Being b in creatures)
                b.LoadContent();

            // load particle systems
            explosion.LoadContent();
            smoke.LoadContent();
        }
        protected override void UnloadContent()
        {
            // unload map
            _background.Dispose();

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

            base.UnloadContent();
        }

        #endregion

        #region Map Construction

        private void buildStones()
        {
            Stone s;
            for (int j = 0; j < Width; j++)
            {
                s = new Stone(this, j, 0);
                s.LoadContent();
                components[j, 0] = s;

                s = new Stone(this, j, Height - 1);
                s.LoadContent();
                components[j, Height - 1] = s;
            }
            for (int i = 1; i < Height - 1; i++)
            {
                s = new Stone(this, 0, i);
                s.LoadContent();
                components[0, i] = s;

                s = new Stone(this, Width - 1, i);
                s.LoadContent();
                components[Width - 1, i] = s;
            }
            for (int i = 2; i < Height - 1; i += 2)
                for (int j = 2; j < Width - 1; j += 2)
                {
                    s = new Stone(this, j, i);
                    s.LoadContent();
                    components[j, i] = s;
                }
        }
        private bool createMapFromFile(string filename)
        {
            // try to open the file where map is stored
            StreamReader reader;
            try
            {
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
            {
                int index = c - '0';
                if (index <= ActivePlayers)   // only add player if it is allowed
                    players[index - 1] = new Player(this, x, y, "Images/player" + index.ToString());
            }
            else
            {
                MapObject comp = null;

                switch (c)
                {
                    case 's': comp = new Stone(this, x, y, "Images/stone1"); break;
                    case 'b':
                        comp = new Box(this, x, y, "Images/box2");
                        int type = generator.Next(0, 10);
                        if (type < 2)
                            (comp as Box).BonusType = (BonusType)type;
                        break;
                }
                if (comp != null)   // just create an instace of this component, but do not load
                    components[x, y] = comp;
            }
        }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Do not call this constuctor!
        /// </summary>
        /// <param name="game"></param>
        private Map(Game game) : base(game) 
        {
            explosion = new ParticleSystem(this, "Images/explosion");
            smoke = new ParticleSystem(this, "Images/smoke");
            toUnload = new LinkedList<MapObject>();
            generator = new Random();
        }

        public Map(Game game, SpriteBatch spriteBatch, int width, int height)
            : this(game)
        {
            _spriteBatch = spriteBatch;

            components = new MapObjectCollection(width, height);
        }

        public Map(Game game, SpriteBatch spriteBatch, int playersCount, string mapFile)
            : this(game)
        {
            _spriteBatch = spriteBatch;
            this.ActivePlayers = playersCount;

            players = new Player[playersCount];

            createMapFromFile(mapFile);

            creatures = new List<Being>();
            SimpleCreature cr;
            for (int i = 0; i < 10; i++)
            {
                cr = new SimpleCreature(this, generator.Next(1, Width - 1), generator.Next(1, Height - 1), "Images/simple1");
                creatures.Add(cr);
            }
        }

        #endregion

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

        #region IDestroyable Members

        protected const double defaultTime = 3000; // miliseconds
        protected double time = defaultTime;       // after this time elapses, the game on map finises
        protected double finihed;                  // time when game finished

        public void Destroy(GameTime gameTime)
        {
            if (Destroing) return;

            Destroing = true;
            finihed = gameTime.TotalGameTime.TotalMilliseconds;
        }
        protected void remove()
        {
            Destroyed = true;
            for (int j = 0; j < players.Length; j++)
                if (!players[j].Destroing)  // find player who is living
                {
                    (Game as Game1).EndRound(j);
                    return;
                }
            // all players are death
            (Game as Game1).EndRound(-1);
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
    }
}
