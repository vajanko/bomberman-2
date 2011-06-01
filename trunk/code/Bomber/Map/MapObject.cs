using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bomber
{
    /// <summary>
    /// This is an acestor of all drawable map components.
    /// All MapObjects are placed in the map.
    /// </summary>
    abstract class MapObject : IGame, IDestroyable
    {
        #region Static Fields

        /// <summary>
        /// Size of the square for one component in the map
        /// </summary>
        static public float Size = 50f;

        #endregion

        #region Protected Fields

        /// <summary>
        /// Path to the file in the ContentPipeline where texture for this component is stored
        /// Set to null when no texture is needed
        /// </summary>
        protected string textureFile = null;
        /// <summary>
        /// Texture of this map component
        /// </summary>
        protected Texture2D texture = null;
        /// <summary>
        /// Map (owner) wher this component is placed
        /// </summary>
        protected Map map;    

        #endregion

        #region Properties

        /// <summary>
        /// Shortcut to the map SpriteBatch for drawing
        /// </summary>
        public SpriteBatch SpriteBatch { get { return map.SpriteBatch; } }
        /// <summary>
        /// Shortcut to the map ContentManager for loading external content
        /// </summary>
        public ContentManager Content { get { return map.Game.Content; } }

        /// <summary>
        /// (Get/Set) X-coordinate of the square position in the map
        /// </summary>
        public int MapX
        {
            get { return _mapX; }
            set
            {   // check if position is inside map
                if (value >= 0 && value < map.Width)
                    _mapX = value;
            }
        }
        private int _mapX = 0;
        /// <summary>
        /// (Get/Set) Y-coordinate of the square position in the map
        /// </summary>
        public int MapY
        {
            get { return _mapY; }
            set
            {   // check if position is inside map
                if (value >= 0 && value < map.Height)
                    _mapY = value;
            }
        }
        private int _mapY = 0;

        /// <summary>
        /// (Get) Real position on the screen
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            protected set 
            {   // value is not set directly to _position because inside Left and Top
                // property also position in the squares is set
                Left = value.X;
                Top = value.Y;
            }
        }
        private Vector2 _position = Vector2.Zero;
        /// <summary>
        /// (Get/Set) X-coordinate of the real position on the screen
        /// </summary>
        public float Left
        {
            get { return _position.X; }
            set 
            {   // also set the map position
                MapX = (int)Math.Round(value / Size, 0);
                _position.X = value;
            }
        }
        /// <summary>
        /// (Get/Set) Y-coordinate of the real position on the screen
        /// </summary>
        public float Top
        {
            get { return _position.Y; }
            set 
            {   // also set the map position
                MapY = (int)Math.Round(value / Size, 0);
                _position.Y = value; 
            }
        }


        /// <summary>
        /// (Get) The real width of the object
        /// </summary>
        public float Width
        {
            get;
            protected set;
        }

        /// <summary>
        /// (Get) The real height of the object
        /// </summary>
        public float Height
        {
            get;
            protected set;
        }

        /// <summary>
        /// (Get) The clipping area of the object. It is the rectangle where this object will be drawn.
        /// </summary>
        public Rectangle Area { get { return new Rectangle((int)_position.X, (int)(_position.Y), (int)Width, (int)Height); } }

        /// <summary>
        /// (Get) Is it possible to pass through this map object
        /// </summary>
        public bool IsPassable
        {
            get;
            protected set;
        }

        #endregion

        #region IGame Members

        public virtual void LoadContent()
        {
            if (textureFile != null)
                texture = Content.Load<Texture2D>(textureFile);
        }
        public virtual void Initialize()
        {
            DestroyInitialize();
            IsPassable = false;
        }
        /// <summary>
        /// Draw the game component to the sprite batch.
        /// If an override needed, use normalDraw or destroingDraw methods
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
            if (Destroing)
                destroingDraw(gameTime);
            else
                normalDraw(gameTime);
        }
        public virtual void Update(GameTime gameTime)
        {
            if (Destroing)  // component is being destroing - an animation
            {
                if (gameTime.TotalGameTime.TotalMilliseconds - DestroyTime > DestroingTime)
                    Remove();   // destring time elapsed - remove from map
                else
                    destroingUpdate(gameTime);  // play an animation
            }
            else   // nothig special - component is updated like it is
                normalUpdate(gameTime);
        }
        public virtual void UnloadContent()
        {
            // prevent from disposing already disposed object
            if (texture != null && !texture.IsDisposed)
                texture.Dispose();
            texture = null;
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
            DestroingTime = 1000; // ms
        }

        public virtual bool Destroy(GameTime gameTime)
        {
            // if already destroyed
            if (Destroing || Destroyed) return false;

            Destroing = true;
            // this is time when this component was destroyed
            DestroyTime = gameTime.TotalGameTime.TotalMilliseconds;
            return true;
        }

        public virtual void Remove()
        {
            Destroyed = true;
            map.RemoveComponent(MapX, MapY);
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

        #region Draw and Update

        protected virtual void normalUpdate(GameTime gameTime) { }
        protected virtual void destroingUpdate(GameTime gameTime) { }

        protected virtual void normalDraw(GameTime gameTime)
        {
            // notice that we do not call SpirteBatch.Begin() and .End(), because this component is
            // drawn inside the map, where .Begin() and .End methods are called
            if (texture != null)
                SpriteBatch.Draw(texture, Area, Color.White);
        }
        protected virtual void destroingDraw(GameTime gameTime) { }

        #endregion

        #region Constructors

        /// <summary>
        /// This constructor always must be called. Default values of the properties are set here.
        /// </summary>
        private MapObject(Map map)
        {
            Width = Size;
            Height = Size;
            IsPassable = false;

            this.map = map;
        }
        public MapObject(Map map, string textureFile)
            : this(map)
        {
            this.textureFile = textureFile;
        }
        public MapObject(Map map, int x, int y)
            : this(map)
        {
            this.Position = new Vector2(x * Size, y * Size);
        }
        public MapObject(Map map, int x, int y, string textureFile)
            : this(map, textureFile)
        {
            this.map = map;
            this.Position = new Vector2(x * Size, y * Size);
            this.textureFile = textureFile;
        }

        #endregion
    }
}
