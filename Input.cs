using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Beri00 
{
    public class Input
    {
        public string device = "keyboard";
        public enum actions 
        {
            left,
            right,
            up,
            down,
            button0,
            button1
        };
        public Input()
        {

        }

        public void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            if (padState.IsConnected) device = "gamepad"; else device = "keyboard";

            if (device == "keyboard")
            {

            }
            else if (device == "gamepad")
            {

            }
        }
    }
}