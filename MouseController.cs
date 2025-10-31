using System;
using System.Runtime.InteropServices;
using SharpDX.XInput;

namespace JoyController
{
    public class MouseController
    {
        #region Win32 API Imports
        
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT { public int X; public int Y; }

        // Constantes pour mouse_event
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        const uint MOUSEEVENTF_WHEEL = 0x0800;

        #endregion

        private Controller controller;
        private ControllerConfig config;
        
        // États précédents des boutons
        private bool previousA = false;
        private bool previousB = false;
        private bool previousX = false;

        public MouseController(Controller controller, ControllerConfig config)
        {
            this.controller = controller;
            this.config = config;
        }

        public void Update()
        {
            if (!controller.IsConnected)
                return;

            var state = controller.GetState();
            var gamepad = state.Gamepad;

            // === Déplacement du curseur avec le joystick gauche ===
            UpdateCursorMovement(gamepad);

            // === Clics de souris avec les boutons ===
            UpdateMouseButtons(gamepad);

            // === Scroll avec le joystick droit ===
            UpdateScroll(gamepad);
        }

        private void UpdateCursorMovement(Gamepad gamepad)
        {
            int lx = gamepad.LeftThumbX;
            int ly = gamepad.LeftThumbY;

            // Application de la zone morte
            if (Math.Abs(lx) < config.Deadzone) lx = 0;
            if (Math.Abs(ly) < config.Deadzone) ly = 0;

            if (lx != 0 || ly != 0)
            {
                GetCursorPos(out POINT pos);
                
                // Normaliser les valeurs entre -1 et 1
                double velocityX = lx / 32767.0;
                double velocityY = ly / 32767.0;
                
                // Appliquer une courbe d'accélération
                double accelX = Math.Sign(velocityX) * Math.Pow(Math.Abs(velocityX), config.AccelerationCurve);
                double accelY = Math.Sign(velocityY) * Math.Pow(Math.Abs(velocityY), config.AccelerationCurve);
                
                // Calculer le déplacement avec la vélocité
                int deltaX = (int)(accelX * config.Sensitivity * config.SpeedMultiplier);
                int deltaY = -(int)(accelY * config.Sensitivity * config.SpeedMultiplier); // Inverser Y
                
                SetCursorPos(pos.X + deltaX, pos.Y + deltaY);
            }
        }

        private void UpdateMouseButtons(Gamepad gamepad)
        {
            // Bouton A - Clic gauche
            bool currentA = (gamepad.Buttons & GamepadButtonFlags.A) != 0;
            if (currentA && !previousA)
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            }
            else if (!currentA && previousA)
            {
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
            previousA = currentA;

            // Bouton B - Clic droit
            bool currentB = (gamepad.Buttons & GamepadButtonFlags.B) != 0;
            if (currentB && !previousB)
            {
                mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            }
            else if (!currentB && previousB)
            {
                mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
            }
            previousB = currentB;

            // Bouton X - Clic milieu
            bool currentX = (gamepad.Buttons & GamepadButtonFlags.X) != 0;
            if (currentX && !previousX)
            {
                mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
            }
            else if (!currentX && previousX)
            {
                mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
            }
            previousX = currentX;
        }

        private void UpdateScroll(Gamepad gamepad)
        {
            int ry = gamepad.RightThumbY;
            
            // Zone morte pour le scroll
            if (Math.Abs(ry) < config.ScrollDeadzone) ry = 0;
            
            if (ry != 0)
            {
                double velocity = ry / 32767.0; // Normaliser entre -1 et 1
                int scrollAmount = (int)(velocity * config.ScrollSensitivity);
                
                mouse_event(MOUSEEVENTF_WHEEL, 0, 0, scrollAmount, 0);
            }
        }

        public void UpdateConfig(ControllerConfig newConfig)
        {
            this.config = newConfig;
        }
    }
}
