using System;
using Microsoft.Xna.Framework;

namespace Bomber
{
    /// <summary>
    /// Defines method for component that could be destoried
    /// </summary>
    interface IDestroyable
    {
        /// <summary>
        /// Destory the component
        /// </summary>
        void Destroy(GameTime gameTime);
        /// <summary>
        /// Get whether the component is being destroing
        /// </summary>
        bool Destroing
        {
            get;
        }
        /// <summary>
        /// Get whether the component is destroied
        /// </summary>
        bool Destroyed
        {
            get;
        }
    }
}
