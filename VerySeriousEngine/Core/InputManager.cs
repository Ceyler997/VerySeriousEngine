using SharpDX.Windows;
using System.Collections.Generic;
using System.Windows.Forms;

namespace VerySeriousEngine.Core
{
    public delegate void InputAction(Keys key);

    public interface IActionListener
    {
        void OnPressed(Keys key);
        void OnReleased(Keys key);
    }

    public class InputManager
    {
        private readonly Dictionary<Keys, HashSet<IActionListener>> actionSubscribers;
        private readonly HashSet<Keys> pressedKeys;
        private readonly HashSet<Keys> unhandledActionsPressed;
        private readonly HashSet<Keys> unhandledActionsReleased;

        public InputManager(RenderForm form)
        {
            actionSubscribers = new Dictionary<Keys, HashSet<IActionListener>>();
            pressedKeys = new HashSet<Keys>();
            unhandledActionsPressed = new HashSet<Keys>();
            unhandledActionsReleased = new HashSet<Keys>();
            
            form.KeyDown += Form_KeyDown;
            form.KeyUp += Form_KeyUp;
        }

        private void Form_KeyDown(object sender, KeyEventArgs keyEvent)
        {
            var key = keyEvent.KeyCode;
            if (pressedKeys.Contains(key))
                return;

            pressedKeys.Add(key);
            if (actionSubscribers.ContainsKey(key))
                unhandledActionsPressed.Add(key);
        }

        private void Form_KeyUp(object sender, KeyEventArgs keyEvent)
        {
            pressedKeys.Remove(keyEvent.KeyCode);

            if (actionSubscribers.ContainsKey(keyEvent.KeyCode))
                unhandledActionsReleased.Add(keyEvent.KeyCode);
        }

        public void Update()
        {
            foreach (var key in unhandledActionsPressed)
                foreach (var subscriber in actionSubscribers[key])
                    subscriber.OnPressed(key);
            unhandledActionsPressed.Clear();

            foreach (var key in unhandledActionsReleased)
                foreach (var subscriber in actionSubscribers[key])
                    subscriber.OnReleased(key);
            unhandledActionsReleased.Clear();
        }

        public void SubscribeOnAction(Keys key, IActionListener listener)
        {
            if (listener == null)
                return;

            if(actionSubscribers.ContainsKey(key))
                actionSubscribers[key].Add(listener);
            else
                actionSubscribers[key] = new HashSet<IActionListener>() { listener };
        }

        public void UnsubscribeFromAction(Keys key, IActionListener listener)
        {
            if (listener == null || actionSubscribers.ContainsKey(key) == false)
                return;

            actionSubscribers[key].Remove(listener);
            if (actionSubscribers[key].Count == 0)
                actionSubscribers.Remove(key);
        }
    }
}
