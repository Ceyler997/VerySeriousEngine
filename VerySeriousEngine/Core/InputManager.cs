using SharpDX;
using System;
using System.Collections.Generic;

namespace VerySeriousEngine.Core
{
    public delegate void InputStateUpdate(IGameInput input, float value);

    public interface IGameInput
    {
        InputStateUpdate StateUpdate { get; set; }
    }

    public interface IActionListener
    {
        void OnPressed(string action);
        void OnReleased(string action);
    }

    public interface IAxisListener
    {
        void OnAxisUpdate(string axis, float value);
    }

    public class InputManager
    {
        private readonly Dictionary<string, HashSet<IActionListener>> actionListeners;
        private readonly Dictionary<string, HashSet<IAxisListener>> axisListeners;

        private readonly Dictionary<IGameInput, string> axesMapping;
        private readonly Dictionary<IGameInput, string> actionsMapping;

        private readonly Dictionary<IGameInput, float> currentInputValue;
        private readonly Dictionary<IGameInput, float> lastHandledActionValue;

        public InputManager()
        {
            actionListeners = new Dictionary<string, HashSet<IActionListener>>();
            axisListeners = new Dictionary<string, HashSet<IAxisListener>>();

            axesMapping = new Dictionary<IGameInput, string>();
            actionsMapping = new Dictionary<IGameInput, string>();

            currentInputValue = new Dictionary<IGameInput, float>();
            lastHandledActionValue = new Dictionary<IGameInput, float>();
        }

        #region Actions

        public void AddAction(IGameInput action, string actionName)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (actionName == null)
                throw new ArgumentNullException(nameof(actionName));

            actionsMapping.Add(action, actionName);
            currentInputValue[action] = 0.0f;
            lastHandledActionValue[action] = 0.0f;
            action.StateUpdate = InputUpdate;
        }

        public void RemoveAction(IGameInput action)
        {
            if (action == null || actionsMapping.ContainsKey(action) == false)
                return;

            if (action.StateUpdate == InputUpdate)
                action.StateUpdate = null;

            actionsMapping.Remove(action);
            currentInputValue.Remove(action);
            lastHandledActionValue.Remove(action);
        }

        public void SubscribeOnAction(string action, IActionListener listener)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));
            if (actionsMapping.ContainsValue(action) == false)
                throw new ArgumentException("Action " + action + " not exists");

            if (actionListeners.ContainsKey(action))
                actionListeners[action].Add(listener);
            else
                actionListeners[action] = new HashSet<IActionListener> { listener };
        }

        public void UnsubscribeFromAction(string action, IActionListener listener)
        {
            if (listener == null || actionListeners.ContainsKey(action) == false)
                return;

            actionListeners[action].Remove(listener);
            if (actionListeners[action].Count == 0)
                actionListeners.Remove(action);
        }
        #endregion

        #region Axes

        public void AddAxis(IGameInput axis, string axisName)
        {
            if (axis == null)
                throw new ArgumentNullException(nameof(axis));
            if (axisName == null)
                throw new ArgumentNullException(nameof(axisName));

            axesMapping.Add(axis, axisName);
            currentInputValue[axis] = 0.0f;
            axis.StateUpdate = InputUpdate;
        }

        public void RemoveAxis(IGameInput axis)
        {
            if (axis == null || axesMapping.ContainsKey(axis) == false)
                return;

            if (axis.StateUpdate == InputUpdate)
                axis.StateUpdate = null;

            axesMapping.Remove(axis);
            currentInputValue.Remove(axis);
        }

        public void SubscribeOnAxis(string axis, IAxisListener listener)
        {
            if (listener == null) // or if axis not exists
                return;

            if (axisListeners.ContainsKey(axis))
                axisListeners[axis].Add(listener);
            else
                axisListeners[axis] = new HashSet<IAxisListener> { listener };
        }

        public void UnsubscribeFromAxis(string axis, IAxisListener listener)
        {
            if (listener == null || axisListeners.ContainsKey(axis) == false)
                return;

            axisListeners[axis].Remove(listener);
            if (axisListeners[axis].Count == 0)
                axisListeners.Remove(axis);
        }
        #endregion

        private void InputUpdate(IGameInput input, float value)
        {
            currentInputValue[input] = value;
        }

        internal void Update()
        {
            // Action pressed/released will be handles once per every input
            foreach(var action in actionsMapping)
            {
                float currentValue = currentInputValue[action.Key];
                float lastHandled = lastHandledActionValue[action.Key];
                if (currentValue == lastHandled)
                    continue;

                if(lastHandled == 0.0f) // wasn't pressed, pressed now
                {
                    foreach (var listener in actionListeners[action.Value])
                        listener.OnPressed(action.Value);
                }
                else // was pressed, released now
                {
                    foreach (var listener in actionListeners[action.Value])
                        listener.OnReleased(action.Value);
                }

                lastHandledActionValue[action.Key] = currentValue;
            }

            // Axes will be handles once per every axis, with sum of all inputs
            var inputCollector = new Dictionary<string, float>();
            foreach (var axis in axesMapping)
            {
                if (inputCollector.ContainsKey(axis.Value))
                    inputCollector[axis.Value] += currentInputValue[axis.Key];
                else
                    inputCollector[axis.Value] = currentInputValue[axis.Key];
                currentInputValue[axis.Key] = 0.0f;
            }

                foreach (var axis in axisListeners)
            {
                foreach (var listener in axis.Value)
                    listener.OnAxisUpdate(axis.Key, inputCollector[axis.Key]);
            }
        }
    }
}
