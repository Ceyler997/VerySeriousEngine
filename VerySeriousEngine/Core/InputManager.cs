using SharpDX;
using System;
using System.Collections.Generic;

namespace VerySeriousEngine.Core
{
    public interface IGameInput
    {
        float Value { get; }
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

    // TODO: Keyboard axis works bad, need to fix it

    public class InputManager
    {
        private readonly Dictionary<string, HashSet<IActionListener>> actionListeners;
        private readonly Dictionary<string, HashSet<IAxisListener>> axisListeners;

        private readonly Dictionary<IGameInput, string> axesMapping;
        private readonly Dictionary<IGameInput, string> actionsMapping;
        
        private readonly Dictionary<IGameInput, float> lastHandledActionValue;

        public InputManager()
        {
            actionListeners = new Dictionary<string, HashSet<IActionListener>>();
            axisListeners = new Dictionary<string, HashSet<IAxisListener>>();

            axesMapping = new Dictionary<IGameInput, string>();
            actionsMapping = new Dictionary<IGameInput, string>();
            
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
            lastHandledActionValue[action] = 0.0f;
        }

        public void RemoveAction(IGameInput action)
        {
            if (action == null || actionsMapping.ContainsKey(action) == false)
                return;

            actionsMapping.Remove(action);
            lastHandledActionValue.Remove(action);
        }

        public void SubscribeOnAction(string actionName, IActionListener listener)
        {
            if (actionName == null)
                throw new ArgumentNullException(nameof(actionName));
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));
            if (actionsMapping.ContainsValue(actionName) == false)
                throw new ArgumentException("Action " + actionName + " not exists");

            if (actionListeners.ContainsKey(actionName))
                actionListeners[actionName].Add(listener);
            else
                actionListeners[actionName] = new HashSet<IActionListener> { listener };
        }

        public void UnsubscribeFromAction(string actionName, IActionListener listener)
        {
            if (listener == null || actionListeners.ContainsKey(actionName) == false)
                return;

            actionListeners[actionName].Remove(listener);
            if (actionListeners[actionName].Count == 0)
                actionListeners.Remove(actionName);
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
        }

        public void RemoveAxis(IGameInput axis)
        {
            if (axis == null || axesMapping.ContainsKey(axis) == false)
                return;

            axesMapping.Remove(axis);
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

        internal void Update()
        {
            UpdateAxis();
            UpdateActions();
        }

        private void UpdateActions()
        {
            // Action pressed/released will be handles once per every input
            foreach (var action in actionsMapping)
            {
                float currentValue = action.Key.Value;
                float lastHandled = lastHandledActionValue[action.Key];
                if (currentValue == lastHandled)
                    continue;

                if (actionListeners.ContainsKey(action.Value) == false)
                    continue; // no listeners

                if (lastHandled == 0.0f) // wasn't pressed, pressed now
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
        }

        private void UpdateAxis()
        {
            // Axes will be handles once per every axis, with sum of all inputs
            var inputCollector = new Dictionary<string, float>();
            foreach (var axis in axesMapping)
            {
                if (inputCollector.ContainsKey(axis.Value))
                    inputCollector[axis.Value] += axis.Key.Value;
                else
                    inputCollector[axis.Value] = axis.Key.Value;
            }

            foreach (var axis in axisListeners)
            {
                foreach (var listener in axis.Value)
                    listener.OnAxisUpdate(axis.Key, inputCollector[axis.Key]);
            }
        }
    }
}
