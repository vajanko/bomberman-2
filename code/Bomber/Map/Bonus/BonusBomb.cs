using System;

namespace Bomber
{
    class BonusBomb : Bonus
    {
        protected override void upgrade(Player player)
        {
            Bomb b = new Bomb(map, player, "Images/bomb1");
            b.Initialize();
            b.LoadContent();
            player.AddBomb(b);
        }

        #region Constructors

        public BonusBomb(Map map, int x, int y, string textureFile)
            : base(map, x, y, textureFile) { }

        #endregion
    }
}
