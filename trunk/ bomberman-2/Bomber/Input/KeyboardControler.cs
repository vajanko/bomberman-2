using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bomber
{
    class KeyboardControler : GameComponent
    {
        private KeyboardState last;
        private KeyboardState now;
        private Player player;

        public Keys Up;
        public Keys Down;
        public Keys Left;
        public Keys Right;
        public Keys Action;

        public override void Initialize()
        {
            now = Keyboard.GetState();
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            last = now;
            now = Keyboard.GetState();
            if (last.IsKeyDown(Up) && now.IsKeyDown(Up))
                player.Move(Direction.Up, gameTime.ElapsedGameTime.Milliseconds);
            if (last.IsKeyDown(Down) && now.IsKeyDown(Down))
                player.Move(Direction.Down, gameTime.ElapsedGameTime.Milliseconds);
            if (last.IsKeyDown(Left) && now.IsKeyDown(Left))
                player.Move(Direction.Left, gameTime.ElapsedGameTime.Milliseconds);
            if (last.IsKeyDown(Right) && now.IsKeyDown(Right))
                player.Move(Direction.Right, gameTime.ElapsedGameTime.Milliseconds);
        }

        public KeyboardControler(Game game, Player player)
            : base(game)
        {
            this.player = player;
        }
    }
}
