using System.Windows.Forms;
using VerySeriousEngine.Core;

namespace VerySeriousEngine.Input
{
    public class KeyboardInput : IGameInput
    {
        private readonly Keys key;

        public InputStateUpdate StateUpdate { get; set; }

        public KeyboardInput(Keys key)
        {
            this.key = key;
            Game.GameInstance.Form.KeyDown += Form_KeyDown;
            Game.GameInstance.Form.KeyUp += Form_KeyUp;
        }

        private void Form_KeyDown(object sender, KeyEventArgs keyUpEvent)
        {
            if (keyUpEvent.KeyCode == key && StateUpdate != null)
                StateUpdate(this, 1.0f);
        }

        private void Form_KeyUp(object sender, KeyEventArgs keyUpEvent)
        {
            if (keyUpEvent.KeyCode == key && StateUpdate != null)
                StateUpdate(this, 0.0f);
        }
    }
}
