using System.Windows.Forms;
using VerySeriousEngine.Core;

namespace VerySeriousEngine.Input
{
    public class KeyboardInput : IGameInput
    {
        private readonly Keys key;
        private readonly float modifier;

        public float Value { get; set; }

        //  Parameters:
        //      key:
        //          Key, that triggers the input
        //      pressValue:
        //          value, that will be passed to the delegate on key press
        public KeyboardInput(Keys key, float pressModifier = 1.0f)
        {
            this.key = key;
            modifier = pressModifier;
            Game.GameInstance.Form.KeyDown += Form_KeyDown;
            Game.GameInstance.Form.KeyUp += Form_KeyUp;
        }

        private void Form_KeyDown(object sender, KeyEventArgs keyUpEvent)
        {
            if (keyUpEvent.KeyCode == key)
                Value = modifier;
        }

        private void Form_KeyUp(object sender, KeyEventArgs keyUpEvent)
        {
            if (keyUpEvent.KeyCode == key)
                Value = 0.0f;
        }
    }
}
