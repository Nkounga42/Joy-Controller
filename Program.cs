using System;
using System.Runtime.InteropServices;
using SharpDX.XInput;

class Program
{
    #region Win32 API Imports
    
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

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

    static void Main()
    {
        Console.WriteLine("=== Joy Controller - Contrôle de souris par manette Xbox ===\n");
        
        var controller = new Controller(UserIndex.One);

        if (!controller.IsConnected)
        {
            Console.WriteLine("❌ Aucune manette détectée sur le port 1.");
            Console.WriteLine("Vérifiez que votre manette est connectée et réessayez.");
            Console.WriteLine("\nAppuyez sur une touche pour quitter...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("✅ Manette détectée !\n");
        Console.WriteLine("Contrôles :");
        Console.WriteLine("  • Joystick gauche  : Déplacer le curseur (vélocité variable)");
        Console.WriteLine("  • Joystick droit   : Scroll (vélocité variable)");
        Console.WriteLine("  • Bouton A         : Clic gauche");
        Console.WriteLine("  • Bouton B         : Clic droit");
        Console.WriteLine("  • Bouton X         : Clic milieu");
        Console.WriteLine("  • Boutons Y/LB/RB  : Affichage info");
        Console.WriteLine("  • D-Pad (↑↓←→)     : Affichage info");
        Console.WriteLine("  • Gâchettes LT/RT  : Affichage valeur");
        Console.WriteLine("  • Bouton Start     : Quitter");
        Console.WriteLine("\n📊 Mode DEBUG : Toutes les valeurs s'affichent en temps réel\n");
        Console.WriteLine("Appuyez sur Start pour arrêter le programme.\n");

        // === Paramètres de sensibilité ===
        const int sensitivity = 10; // Vitesse de déplacement du curseur (10-30 recommandé)
        const short deadzone = 8000; // Zone morte du joystick (5000-10000 recommandé)
        // Pour modifier l'accélération, voir lignes 101 (courbe) et 105 (multiplicateur)

        // États précédents des boutons pour détecter les pressions
        bool previousA = false;
        bool previousB = false;
        bool previousX = false;
        bool previousY = false;
        bool previousLB = false;
        bool previousRB = false;
        bool previousDpadUp = false;
        bool previousDpadDown = false;
        bool previousDpadLeft = false;
        bool previousDpadRight = false;
        byte previousLT = 0;
        byte previousRT = 0;

        while (true)
        {
            if (!controller.IsConnected)
            {
                Console.WriteLine("\n⚠️  Manette déconnectée. Arrêt du programme.");
                break;
            }

            var state = controller.GetState();
            var gamepad = state.Gamepad;

            // Bouton Start pour quitter
            if ((gamepad.Buttons & GamepadButtonFlags.Start) != 0)
            {
                Console.WriteLine("\n👋 Arrêt du programme.");
                break;
            }

            // === Déplacement du curseur avec le joystick gauche (vélocité variable) ===
            int lx = gamepad.LeftThumbX;
            int ly = gamepad.LeftThumbY;

            // Application de la zone morte
            if (Math.Abs(lx) < deadzone) lx = 0;
            if (Math.Abs(ly) < deadzone) ly = 0;

            if (lx != 0 || ly != 0)
            {
                GetCursorPos(out POINT pos);
                
                // Normaliser les valeurs entre -1 et 1
                double velocityX = lx / 32767.0;
                double velocityY = ly / 32767.0;
                
                // Appliquer une courbe d'accélération pour plus de contrôle
                // Les petits mouvements restent précis, les grands mouvements sont rapides
                // Ajustez l'exposant: 1.0 = linéaire, 1.5 = modéré (défaut), 2.0 = forte accélération
                double accelX = Math.Sign(velocityX) * Math.Pow(Math.Abs(velocityX), 1.5);
                double accelY = Math.Sign(velocityY) * Math.Pow(Math.Abs(velocityY), 1.5);
                
                // Calculer le déplacement avec la vélocité
                // Ajustez le multiplicateur (actuellement 2) pour changer la vitesse globale
                int deltaX = (int)(accelX * sensitivity * 1.5);
                int deltaY = -(int)(accelY * sensitivity * 1.5); // Inverser Y pour correspondre à l'écran
                
                Console.WriteLine($"📍 Joystick gauche - X: {gamepad.LeftThumbX,6} Y: {gamepad.LeftThumbY,6} | Vélocité X: {velocityX,5:F2} Y: {velocityY,5:F2} | Delta X: {deltaX,3} Y: {deltaY,3}");
                
                SetCursorPos(pos.X + deltaX, pos.Y + deltaY);
            }

            // === Clics de souris avec les boutons ===
            
            // Bouton A - Clic gauche
            bool currentA = (gamepad.Buttons & GamepadButtonFlags.A) != 0;
            if (currentA && !previousA)
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                Console.WriteLine($"🎮 [BOUTON A] - Clic gauche | Flag: {GamepadButtonFlags.A} | Valeur: {(int)GamepadButtonFlags.A}");
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
                Console.WriteLine($"🎮 [BOUTON B] - Clic droit | Flag: {GamepadButtonFlags.B} | Valeur: {(int)GamepadButtonFlags.B}");
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
                Console.WriteLine($"🎮 [BOUTON X] - Clic milieu | Flag: {GamepadButtonFlags.X} | Valeur: {(int)GamepadButtonFlags.X}");
            }
            else if (!currentX && previousX)
            {
                mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
            }
            previousX = currentX;

            // Bouton Y
            bool currentY = (gamepad.Buttons & GamepadButtonFlags.Y) != 0;
            if (currentY && !previousY)
            {
                Console.WriteLine($"🎮 [BOUTON Y] | Flag: {GamepadButtonFlags.Y} | Valeur: {(int)GamepadButtonFlags.Y}");
            }
            previousY = currentY;

            // Bouton LB (Left Bumper)
            bool currentLB = (gamepad.Buttons & GamepadButtonFlags.LeftShoulder) != 0;
            if (currentLB && !previousLB)
            {
                Console.WriteLine($"🎮 [BOUTON LB] (Left Bumper) | Flag: {GamepadButtonFlags.LeftShoulder} | Valeur: {(int)GamepadButtonFlags.LeftShoulder}");
            }
            previousLB = currentLB;

            // Bouton RB (Right Bumper)
            bool currentRB = (gamepad.Buttons & GamepadButtonFlags.RightShoulder) != 0;
            if (currentRB && !previousRB)
            {
                Console.WriteLine($"🎮 [BOUTON RB] (Right Bumper) | Flag: {GamepadButtonFlags.RightShoulder} | Valeur: {(int)GamepadButtonFlags.RightShoulder}");
            }
            previousRB = currentRB;

            // D-Pad Up
            bool currentDpadUp = (gamepad.Buttons & GamepadButtonFlags.DPadUp) != 0;
            if (currentDpadUp && !previousDpadUp)
            {
                Console.WriteLine($"🎮 [D-PAD HAUT] | Flag: {GamepadButtonFlags.DPadUp} | Valeur: {(int)GamepadButtonFlags.DPadUp}");
            }
            previousDpadUp = currentDpadUp;

            // D-Pad Down
            bool currentDpadDown = (gamepad.Buttons & GamepadButtonFlags.DPadDown) != 0;
            if (currentDpadDown && !previousDpadDown)
            {
                Console.WriteLine($"🎮 [D-PAD BAS] | Flag: {GamepadButtonFlags.DPadDown} | Valeur: {(int)GamepadButtonFlags.DPadDown}");
            }
            previousDpadDown = currentDpadDown;

            // D-Pad Left
            bool currentDpadLeft = (gamepad.Buttons & GamepadButtonFlags.DPadLeft) != 0;
            if (currentDpadLeft && !previousDpadLeft)
            {
                Console.WriteLine($"🎮 [D-PAD GAUCHE] | Flag: {GamepadButtonFlags.DPadLeft} | Valeur: {(int)GamepadButtonFlags.DPadLeft}");
            }
            previousDpadLeft = currentDpadLeft;

            // D-Pad Right
            bool currentDpadRight = (gamepad.Buttons & GamepadButtonFlags.DPadRight) != 0;
            if (currentDpadRight && !previousDpadRight)
            {
                Console.WriteLine($"🎮 [D-PAD DROITE] | Flag: {GamepadButtonFlags.DPadRight} | Valeur: {(int)GamepadButtonFlags.DPadRight}");
            }
            previousDpadRight = currentDpadRight;

            // Gâchette gauche LT (Left Trigger)
            byte currentLT = gamepad.LeftTrigger;
            if (currentLT > 30 && previousLT <= 30)
            {
                Console.WriteLine($"🎮 [GÂCHETTE LT] (Left Trigger) - Valeur: {currentLT}/255 ({(currentLT/255.0*100):F1}%)");
            }
            previousLT = currentLT;

            // Gâchette droite RT (Right Trigger)
            byte currentRT = gamepad.RightTrigger;
            if (currentRT > 30 && previousRT <= 30)
            {
                Console.WriteLine($"🎮 [GÂCHETTE RT] (Right Trigger) - Valeur: {currentRT}/255 ({(currentRT/255.0*100):F1}%)");
            }
            previousRT = currentRT;

            // === Scroll avec le joystick droit ===
            
            int ry = gamepad.RightThumbY;
            
            // Zone morte pour le scroll (ajustez entre 3000 et 10000)
            const short scrollDeadzone = 6000;
            if (Math.Abs(ry) < scrollDeadzone) ry = 0;
            
            if (ry != 0)
            {
                // Calcul de la vélocité du scroll proportionnelle à l'inclinaison du joystick
                // Plus le joystick est incliné, plus le scroll est rapide
                double velocity = ry / 32767.0; // Normaliser entre -1 et 1
                int scrollAmount = (int)(velocity * 30); // Multiplier par la sensibilité (ajustez entre 10 et 50)
                
                Console.WriteLine($"🔄 Scroll - Joystick droit Y: {gamepad.RightThumbY,6} | Vélocité: {velocity,5:F2} | Amount: {scrollAmount,3}");
                
                mouse_event(MOUSEEVENTF_WHEEL, 0, 0, scrollAmount, 0);
            }

            System.Threading.Thread.Sleep(10); // 100 FPS
        }
    }
}
