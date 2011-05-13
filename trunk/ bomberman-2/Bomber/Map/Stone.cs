using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomber
{
    class Stone : MapObject
    {

        public override void Initialize()
        {
            base.Initialize();
            IsPassable = false;
        }

        #region Constructors

        public Stone(Map map, int x, int y) : base(map, x, y) { }
        public Stone(Map map, int x, int y, string textureFile) : base(map, x, y, textureFile) { }

        #endregion
    }
}
