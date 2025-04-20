using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terraria.Util
{
    public enum ScrollWheelDirection
    {
        Up,
        Down
    }

    public enum MouseButtons
    {
        Left,
        Right
    }

    public static class InputManager
    {
        public delegate void KeyEventHandler(object sender, KeboardEventArgs e);
        public delegate void MouseEventHandler(object sender, MouseEventArgs e);
        public delegate void MouseScrollEventHandler(object sender, ScrollWheelEventArgs e);
        public static event KeyEventHandler KeyPressEvent;
        public static event KeyEventHandler KeyUpEvent;
        public static event MouseScrollEventHandler MouseScrollEvent;
        public static event MouseEventHandler MouseButtonEvent;

        private static KeyboardState _prevKb;
        private static HashSet<Keys> _prevPressedKeys;

        private static MouseState _prevMouse;
        private static int _prevScrollWheelValue;

        static InputManager()
        {
            _prevKb = Keyboard.GetState();
            _prevPressedKeys = new(_prevKb.GetPressedKeys());
            _prevScrollWheelValue = Mouse.GetState().ScrollWheelValue;
            _prevMouse = Mouse.GetState();
        }

        public static void Update()
        {
            RaiseKeyPress();
            RaiseScrollWheelEvent();
            RaiseMouseButtonEvent();
        }

        private static void RaiseMouseButtonEvent()
        {
            MouseState mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed && !(_prevMouse.LeftButton == ButtonState.Pressed))
            {
                MouseButtonEvent?.Invoke(typeof(InputManager), new MouseEventArgs { MouseButton = MouseButtons.Left});
            }
            if (mouse.RightButton == ButtonState.Pressed && !(_prevMouse.RightButton == ButtonState.Pressed))
            {
                MouseButtonEvent?.Invoke(typeof(InputManager), new MouseEventArgs{ MouseButton = MouseButtons.Right });
            }

            _prevMouse = mouse;
        }

        private static void RaiseKeyPress()
        {
            KeyboardState kb = Keyboard.GetState();
            Keys[] pressedKeys = kb.GetPressedKeys();

            foreach (var key in pressedKeys)
            {
                if (!_prevPressedKeys.Contains(key))
                {
                    KeyPressEvent?.Invoke(typeof(InputManager), new KeboardEventArgs { Key = key });
                }
            }

            _prevPressedKeys.Clear();
            foreach (var key in pressedKeys)
            {
                _prevPressedKeys.Add(key);
            }

            _prevKb = kb;
        }

        private static void RaiseScrollWheelEvent()
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.ScrollWheelValue > _prevScrollWheelValue)
            {
                MouseScrollEvent?.Invoke(typeof(InputManager), new ScrollWheelEventArgs { Dir = ScrollWheelDirection.Up});
            }
            if (mouse.ScrollWheelValue < _prevScrollWheelValue)
            {
                MouseScrollEvent?.Invoke(typeof(InputManager), new ScrollWheelEventArgs { Dir = ScrollWheelDirection.Down});
            }

            _prevScrollWheelValue = mouse.ScrollWheelValue;
        }

        public static bool IsAnyKeyPressed()
        {
            var keys = Keyboard.GetState().GetPressedKeys();
            if (keys.Length > 0)
            {
                return true;
            }
            else
                return false;
        }
    }

    public class KeboardEventArgs
    {
        public Keys Key;
    }
    public class MouseEventArgs
    {
        public MouseButtons MouseButton;
    }
    public class ScrollWheelEventArgs
    {
        public ScrollWheelDirection Dir;
    }
}
