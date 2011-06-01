using System;

namespace Bomber
{
    class BonusFire : Bonus
    {
        protected const int defaultLength = 1;
        protected int length = defaultLength;

        protected override void upgrade(Player player)
        {
            player.AddFire(length);
        }

        #region Constructors

        public BonusFire(Map map, int x, int y, string textureFile)
            : base(map, x, y, textureFile) { }

        #endregion
    }
}
