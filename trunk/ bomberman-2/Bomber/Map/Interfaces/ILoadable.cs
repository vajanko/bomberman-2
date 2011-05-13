using System;
using Microsoft.Xna.Framework.Content;

namespace Bomber
{
    /// <summary>
    /// Interface for map components that should be loaded and unloaded
    /// </summary>
    interface ILoadable
    {
        void LoadContent();
        void UnloadContent();
    }
}
