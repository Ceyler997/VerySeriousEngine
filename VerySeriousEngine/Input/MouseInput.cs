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
        private readonly float modifier;
        private readonly MouseButtons button;

        private Point prevLocation;
        public float Value { get; set; }

        public MouseInput(MouseAxis axis, float inputModifier = 1.0f)
        {
            this.axis = axis;
            modifier = inputModifier;

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

        public MouseInput(MouseButtons button, float inputModifier = 1.0f)
        {
            this.button = button;
            modifier = inputModifier;

            Game.GameInstance.Form.MouseDown += Form_MouseDown;
            Game.GameInstance.Form.MouseUp += Form_MouseUp;
        }

        private void Form_MouseWheel(object sender, MouseEventArgs mouseEvent)
        {
            Value = mouseEvent.Delta * modifier;
        }

        private void Form_MouseDown(object sender, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.Button == button)
                Value = modifier;
        }

        private void Form_MouseUp(object sender, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.Button == button)
                Value = 0.0f;
        }

        private void Form_MouseMove(object sender, MouseEventArgs mouseEvent)
        {
            switch (axis)
            {
                case MouseAxis.MouseRight:
                    Value = (mouseEvent.X - prevLocation.X) * modifier;
                    break;
                case MouseAxis.MouseDown:
                    Value = (mouseEvent.Y - prevLocation.Y) * modifier;
                    break;
                default:
                    Logger.LogWarning("Wrong mouse axis");
                    break;
            }

            prevLocation = mouseEvent.Location;
        }
    }
}
