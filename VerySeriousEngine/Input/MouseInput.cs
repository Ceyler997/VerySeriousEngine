using System;
using System.Drawing;
using System.Windows.Forms;
using VerySeriousEngine.Core;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Input
{
    public enum MouseAxis
    {
        MouseDown,
        MouseRight,
        MouseWheelUp,
    }

    public class MouseInput : IGameInput
    {
        private readonly MouseAxis axis;
        private Point prevLocation;

        private readonly MouseButtons button;

        public InputStateUpdate StateUpdate { get; set; }

        public MouseInput(MouseAxis axis)
        {
            this.axis = axis;

            switch (axis)
            {
                case MouseAxis.MouseRight:
                case MouseAxis.MouseDown:
                    Game.GameInstance.Form.MouseMove += Form_MouseMove;
                    break;
                case MouseAxis.MouseWheelUp:
                    Game.GameInstance.Form.MouseWheel += Form_MouseWheel;
                    break;
                default:
                    Logger.LogWarning("Unknown mouse axis");
                    break;
            }
        }

        private void Form_MouseWheel(object sender, MouseEventArgs mouseEvent)
        {
            StateUpdate?.Invoke(this, mouseEvent.Delta);
        }

        public MouseInput(MouseButtons button)
        {
            this.button = button;

            Game.GameInstance.Form.MouseDown += Form_MouseDown;
            Game.GameInstance.Form.MouseUp += Form_MouseUp;
        }

        private void Form_MouseDown(object sender, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.Button == button && StateUpdate != null)
                StateUpdate(this, 1.0f);
        }

        private void Form_MouseUp(object sender, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.Button == button && StateUpdate != null)
                StateUpdate(this, 0.0f);
        }

        private void Form_MouseMove(object sender, MouseEventArgs mouseEvent)
        {
            if(StateUpdate != null)
            {
                switch(axis)
                {
                    case MouseAxis.MouseRight:
                        StateUpdate(this, mouseEvent.X - prevLocation.X);
                        break;
                    case MouseAxis.MouseDown:
                        StateUpdate(this, mouseEvent.Y - prevLocation.Y);
                        break;
                    default:
                        Logger.LogWarning("Wrong mouse axis");
                        break;
                }
            }

            prevLocation = mouseEvent.Location;
        }
    }
}
