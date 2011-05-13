using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomber
{
    class Animation
    {
        private const long frameDelay = 0xf4240L;
        private long currentFrame = 0;
        private long currentTick;
        private Texture2D texture;

        private bool plaing = false;

        public bool Loop = false;

        /// <summary>
        /// Get the width of the animation frames. All are the same width
        /// </summary>
        public int Width { get { return _width; } }
        private int _width = 0;
        /// <summary>
        /// Get the height of the animation frames. All are the same height
        /// </summary>
        public int Height { get { return _height; } }
        private int _height = 0;
        /// <summary>
        /// Get the number of frames in this animation
        /// </summary>
        public int Frames { get { return _frames; } }
        private int _frames = 0;

        /// <summary>
        /// Get or set the area where the animation will be drawn on the screen
        /// </summary>
        public Rectangle Area;

        public Rectangle CurrentTextureArea
        {
            get
            {
                int x = (int)(currentFrame % 3) * Width;
                int y = (int)(currentFrame / 3) * Height;
                return new Rectangle(x, y, Width, Height);
            }
        }

        public event Action OnFinished;

        /// <summary>
        /// Initiate the animation
        /// </summary>
        public void Play()
        {
            plaing = true;
            currentFrame = 0;
        }
        public void Pause()
        {
            plaing = false;
        }
        public void Resume()
        {
            plaing = true;
        }
        public void Update(GameTime gameTime)
        {
            if (!plaing) return;    // animation not started - do not allow to update frames

            // count ticks elapsed since last updata and also the rest of ticks from last update
            currentTick += gameTime.ElapsedGameTime.Ticks;
            // for each frameDelay elapsed add one frame
            currentFrame += currentTick / frameDelay;
            // count the rest of ticks for next update
            currentTick %= frameDelay;
            // move current frame to the beginnig if behind the end (if loop)
            if (currentFrame >= Frames && !Loop) // end of animation
            {
                plaing = false;                  // stop it
                currentFrame = Frames - 1;       // set the last frame of the animation
                if (OnFinished != null)          // start on finish action
                    OnFinished();
            }
            else
                currentFrame %= Frames;
        }
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(texture, Area, CurrentTextureArea, Color.White);
        }

        #region Constructors

        /// <summary>
        /// Create an animation
        /// </summary>
        /// <param name="texture">Texture containing all animation frames</param>
        /// <param name="width">Width of one frame</param>
        /// <param name="height">Height of one frame</param>
        /// <param name="frames">Number of frames in the texture</param>
        public Animation(Texture2D texture, int width, int height, int frames)
        {
            this.texture = texture;
            this._width = width;
            this._height = height;
            this._frames = frames;
        }

        #endregion
   
   }
}
