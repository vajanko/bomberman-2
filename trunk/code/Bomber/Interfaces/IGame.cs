using Microsoft.Xna.Framework;

namespace Bomber
{
    /// <summary>
    /// Lifecycle of each game component:
    /// - Create an instance
    /// - LoadContent
    /// - Initialize
    /// - Enter the loop:
    ///     - Update
    ///     - Draw
    /// - UnloadContent
    /// </summary>
    interface IGame
    {
        /// <summary>
        /// Load external content (usually textures)
        /// </summary>
        void LoadContent();

        /// <summary>
        /// Initialize instance variables to default values. This is especially useful when
        /// reusing the same components
        /// </summary>
        void Initialize();

        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);
    
        /// <summary>
        /// Release all external content (usually textures)
        /// </summary>
        void UnloadContent();
    }
}
