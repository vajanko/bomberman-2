using System;

namespace Bomber
{
    class BonusSpeed : Bonus
    {
        protected override void upgrade(Player player)
        {
            player.AddSpeed(0.02f);
        }

        #region Constructors

        public BonusSpeed(Map map, int x, int y, string textureFile)
            : base(map, x, y, textureFile) { }

        #endregion
    }
}
