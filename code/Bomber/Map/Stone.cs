using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomber
{
    /// <summary>
    /// Stone is used in the map to create places where noone can enter and which could
    /// not be destroyed. This is also useful for palyers to hide from the explosions
    /// </summary>
    class Stone : DestroyableObject
    {
        public override void Initialize()
        {
            base.Initialize();
            IsPassable = false; // no moveing object can pass throught stone
        }

        public override bool Destroy(GameTime gameTime)
        {
            return false; // stone could not be destroyed
        }

        #region Constructors

        public Stone(Map map, int x, int y, string textureFile) : base(map, x, y, textureFile) { }

        #endregion
    }
}
