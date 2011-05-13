using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bomber
{
    /// <summary>
    /// This is an acestor of all drawable map components
    /// </summary>
    abstract class MapObject : IGameComponent, IDrawable, IUpdateable, ILoadable
    {
        #region Static Fields

        static public float Size = 50f;

        #endregion

        #region Protected Fields

        protected string textureFile = null;
        protected Map map;    

        #endregion

        #region Properities

        //shortcuts
        public SpriteBatch SpriteBatch { get { return map.SpriteBatch; } }
        public ContentManager Content { get { return map.Game.Content; } }

        /// <summary>
        /// Get or set the x-coordinate of the box position in the map
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
        /// Get or set the y-coordinate of the box position in the map
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
        /// Real position in the world (on the screen)
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            protected set 
            {
                Left = value.X;
                Top = value.Y;
            }
        }
        private Vector2 _position = Vector2.Zero;
        public float Left
        {
            get { return _position.X; }
            set 
            {   // also set the map position
                MapX = (int)Math.Round(value / Size, 0);
                _position.X = value;
            }
        }
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
        /// (Get/Set) The real width of the object
        /// </summary>
        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }
        private float _width = Size;

        /// <summary>
        /// (Get/Set) The real height of the object
        /// </summary>
        public float Height
        {
            get { return _height; }
            set { _height = value; }
        }
        private float _height = Size;

        /// <summary>
        /// (Get) The clipping area of the object. It is the rectangle where this object will be drawn.
        /// </summary>
        public Rectangle Area { get { return new Rectangle((int)_position.X, (int)(_position.Y), (int)_width, (int)_height); } }
        //private Rectangle area;

        private Texture2D _texture = null;
        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        /// <summary>
        /// (Get) Is it possible to pass through this map object
        /// </summary>
        public bool IsPassable
        {
            get;
            protected set;
        }

        #endregion

        #region IGameComponent Members

        public virtual void Initialize() { }

        #endregion

        #region IDrawable Members

        /// <summary>
        /// Draw the game component to the sprite batch.
        /// Overide if any animation is needed
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
            // notice that we do not call SpirteBatch.Begin() and .End(), because this component is
            // drawn inside the map, where .Begin() and .End methods are called
            if (Texture != null)
                SpriteBatch.Draw(Texture, Area, Color.White);
        }

        #endregion

        #region IUpdateable Members

        public virtual void Update(GameTime gameTime) { }

        #endregion

        #region ILoadable Members

        public virtual void LoadContent()
        {
            if (textureFile != null)
                Texture = Content.Load<Texture2D>(textureFile);
        }
        public virtual void UnloadContent()
        {
            if (Texture != null)
                Texture.Dispose();
            Texture = null;
        }

        #endregion

        #region Constructors

        public MapObject(Map map, string textureFile)
        {
            this.map = map;
            this.textureFile = textureFile;
        }
        public MapObject(Map map, int x, int y)
        {
            this.map = map;
            this.Position = new Vector2(x * Size, y * Size);
        }
        public MapObject(Map map, int x, int y, string textureFile)
            : this(map, x, y)
        {
            this.textureFile = textureFile;
        }

        #endregion
    }
}
