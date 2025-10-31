using System;
using System.Drawing;
using System.Windows.Forms;
using SharpDX.XInput;

namespace JoyController
{
    public class MainForm : Form
    {
        private Controller controller;
        private ControllerConfig config;
        private System.Windows.Forms.Timer statusTimer;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        
        // UI Controls
        private Panel statusPanel;
        private Label statusLabel;
        private Label connectionStatusLabel;
        private PictureBox statusIcon;
        
        private GroupBox stickVisualizerBox;
        private Panel leftStickVisualizer;
        private Panel leftStickIndicator;
        private Panel rightStickVisualizer;
        private Panel rightStickIndicator;
        
        private GroupBox buttonsBox;
        private GroupBox triggersBox;
        private GroupBox dpadBox;
        private GroupBox systemButtonsBox;
        
        private Label buttonALabel;
        private Label buttonBLabel;
        private Label buttonXLabel;
        private Label buttonYLabel;
        private Label buttonLBLabel;
        private Label buttonRBLabel;
        private Label buttonLTLabel;
        private Label buttonRTLabel;
        private Label buttonDpadUpLabel;
        private Label buttonDpadDownLabel;
        private Label buttonDpadLeftLabel;
        private Label buttonDpadRightLabel;
        private Label buttonStartLabel;
        private Label buttonBackLabel;
        private Label buttonLeftThumbLabel;
        private Label buttonRightThumbLabel;
        
        private Button configButton;
        private Button startStopButton;
        private CheckBox startWithWindowsCheckBox;
        private CheckBox minimizeToTrayCheckBox;
        
        private bool isRunning = false;

        public MainForm()
        {
            controller = new Controller(UserIndex.One);
            config = ControllerConfig.Load();
            
            InitializeComponent();
            InitializeTrayIcon();
            
            statusTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60 FPS
            statusTimer.Tick += StatusTimer_Tick;
            statusTimer.Start();
        }

        private void InitializeComponent()
        {
            this.Text = "🎮 Joy Controller - Contrôle Souris par Manette";
            this.Size = new Size(900, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 250);
            
            // === Panel de statut en haut ===
            statusPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(900, 80),
                BackColor = Color.FromArgb(0, 120, 215),
                Dock = DockStyle.Top
            };
            this.Controls.Add(statusPanel);
            
            statusIcon = new PictureBox
            {
                Location = new Point(20, 15),
                Size = new Size(50, 50),
                BackColor = Color.Transparent
            };
            statusPanel.Controls.Add(statusIcon);
            
            statusLabel = new Label
            {
                Text = "Joy Controller",
                Location = new Point(80, 15),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            statusPanel.Controls.Add(statusLabel);
            
            connectionStatusLabel = new Label
            {
                Text = "🔌 Vérification de la connexion...",
                Location = new Point(80, 45),
                Size = new Size(600, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(230, 230, 230),
                BackColor = Color.Transparent
            };
            statusPanel.Controls.Add(connectionStatusLabel);
            
            // === Visualiseur de joysticks ===
            stickVisualizerBox = new GroupBox
            {
                Text = "📊 Visualisation des Joysticks",
                Location = new Point(20, 100),
                Size = new Size(860, 200),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50)
            };
            this.Controls.Add(stickVisualizerBox);
            
            // Joystick gauche
            Label leftStickLabel = new Label
            {
                Text = "Joystick Gauche\n(Déplacement curseur)",
                Location = new Point(80, 30),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter
            };
            stickVisualizerBox.Controls.Add(leftStickLabel);
            
            leftStickVisualizer = new Panel
            {
                Location = new Point(85, 75),
                Size = new Size(140, 140),
                BackColor = Color.FromArgb(220, 220, 225),
                BorderStyle = BorderStyle.FixedSingle
            };
            stickVisualizerBox.Controls.Add(leftStickVisualizer);
            
            leftStickIndicator = new Panel
            {
                Location = new Point(60, 60),
                Size = new Size(20, 20),
                BackColor = Color.FromArgb(0, 120, 215),
                BorderStyle = BorderStyle.FixedSingle
            };
            leftStickVisualizer.Controls.Add(leftStickIndicator);
            
            // Joystick droit
            Label rightStickLabel = new Label
            {
                Text = "Joystick Droit\n(Scroll)",
                Location = new Point(430, 30),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter
            };
            stickVisualizerBox.Controls.Add(rightStickLabel);
            
            rightStickVisualizer = new Panel
            {
                Location = new Point(435, 75),
                Size = new Size(140, 140),
                BackColor = Color.FromArgb(220, 220, 225),
                BorderStyle = BorderStyle.FixedSingle
            };
            stickVisualizerBox.Controls.Add(rightStickVisualizer);
            
            rightStickIndicator = new Panel
            {
                Location = new Point(60, 60),
                Size = new Size(20, 20),
                BackColor = Color.FromArgb(16, 124, 16),
                BorderStyle = BorderStyle.FixedSingle
            };
            rightStickVisualizer.Controls.Add(rightStickIndicator);
            
            // === Indicateurs de boutons ABXY ===
            buttonsBox = new GroupBox
            {
                Text = "🎯 Boutons ABXY",
                Location = new Point(20, 320),
                Size = new Size(420, 100),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50)
            };
            this.Controls.Add(buttonsBox);
            
            buttonALabel = CreateButtonLabel("A\nClic gauche", 20, 30, 85, 50, Color.FromArgb(100, 200, 100));
            buttonBLabel = CreateButtonLabel("B\nClic droit", 115, 30, 85, 50, Color.FromArgb(220, 100, 100));
            buttonXLabel = CreateButtonLabel("X\nClic milieu", 210, 30, 85, 50, Color.FromArgb(100, 150, 220));
            buttonYLabel = CreateButtonLabel("Y\nNon assigné", 305, 30, 100, 50, Color.FromArgb(220, 200, 100));
            
            buttonsBox.Controls.Add(buttonALabel);
            buttonsBox.Controls.Add(buttonBLabel);
            buttonsBox.Controls.Add(buttonXLabel);
            buttonsBox.Controls.Add(buttonYLabel);
            
            // === Bumpers & Triggers ===
            triggersBox = new GroupBox
            {
                Text = "⚡ Gâchettes & Bumpers",
                Location = new Point(460, 320),
                Size = new Size(420, 100),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50)
            };
            this.Controls.Add(triggersBox);
            
            buttonLBLabel = CreateButtonLabel("LB", 20, 30, 85, 50, Color.FromArgb(150, 150, 200));
            buttonRBLabel = CreateButtonLabel("RB", 115, 30, 85, 50, Color.FromArgb(150, 150, 200));
            buttonLTLabel = CreateButtonLabel("LT", 210, 30, 85, 50, Color.FromArgb(180, 120, 180));
            buttonLTLabel.Name = "buttonLTLabel";
            buttonRTLabel = CreateButtonLabel("RT", 305, 30, 85, 50, Color.FromArgb(180, 120, 180));
            buttonRTLabel.Name = "buttonRTLabel";
            
            triggersBox.Controls.Add(buttonLBLabel);
            triggersBox.Controls.Add(buttonRBLabel);
            triggersBox.Controls.Add(buttonLTLabel);
            triggersBox.Controls.Add(buttonRTLabel);
            
            // === D-Pad ===
            dpadBox = new GroupBox
            {
                Text = "🎮 D-Pad",
                Location = new Point(20, 440),
                Size = new Size(420, 100),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50)
            };
            this.Controls.Add(dpadBox);
            
            buttonDpadUpLabel = CreateButtonLabel("↑", 20, 30, 85, 50, Color.FromArgb(120, 180, 120));
            buttonDpadDownLabel = CreateButtonLabel("↓", 115, 30, 85, 50, Color.FromArgb(120, 180, 120));
            buttonDpadLeftLabel = CreateButtonLabel("←", 210, 30, 85, 50, Color.FromArgb(120, 180, 120));
            buttonDpadRightLabel = CreateButtonLabel("→", 305, 30, 85, 50, Color.FromArgb(120, 180, 120));
            
            dpadBox.Controls.Add(buttonDpadUpLabel);
            dpadBox.Controls.Add(buttonDpadDownLabel);
            dpadBox.Controls.Add(buttonDpadLeftLabel);
            dpadBox.Controls.Add(buttonDpadRightLabel);
            
            // === Boutons système & Sticks ===
            systemButtonsBox = new GroupBox
            {
                Text = "🔘 Système & Sticks",
                Location = new Point(460, 440),
                Size = new Size(420, 100),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50)
            };
            this.Controls.Add(systemButtonsBox);
            
            buttonStartLabel = CreateButtonLabel("Start", 20, 30, 85, 50, Color.FromArgb(100, 100, 100));
            buttonBackLabel = CreateButtonLabel("Back", 115, 30, 85, 50, Color.FromArgb(100, 100, 100));
            buttonLeftThumbLabel = CreateButtonLabel("L3", 210, 30, 85, 50, Color.FromArgb(200, 150, 100));
            buttonRightThumbLabel = CreateButtonLabel("R3", 305, 30, 85, 50, Color.FromArgb(200, 150, 100));
            
            systemButtonsBox.Controls.Add(buttonStartLabel);
            systemButtonsBox.Controls.Add(buttonBackLabel);
            systemButtonsBox.Controls.Add(buttonLeftThumbLabel);
            systemButtonsBox.Controls.Add(buttonRightThumbLabel);
            
            // === Boutons de contrôle ===
            startStopButton = new Button
            {
                Text = "▶️ Démarrer le Contrôle",
                Location = new Point(20, 560),
                Size = new Size(200, 45),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(16, 124, 16),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            startStopButton.FlatAppearance.BorderSize = 0;
            startStopButton.Click += StartStopButton_Click;
            this.Controls.Add(startStopButton);
            
            configButton = new Button
            {
                Text = "⚙️ Configuration",
                Location = new Point(240, 560),
                Size = new Size(200, 45),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            configButton.FlatAppearance.BorderSize = 0;
            configButton.Click += ConfigButton_Click;
            this.Controls.Add(configButton);
            
            Button exitButton = new Button
            {
                Text = "❌ Quitter",
                Location = new Point(460, 560),
                Size = new Size(420, 45),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(232, 17, 35),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Click += (s, e) => Application.Exit();
            this.Controls.Add(exitButton);
            
            // === Options ===
            minimizeToTrayCheckBox = new CheckBox
            {
                Text = "Réduire dans la barre des tâches",
                Location = new Point(20, 625),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 9),
                Checked = true
            };
            this.Controls.Add(minimizeToTrayCheckBox);
            
            // Informations en bas
            Label infoLabel = new Label
            {
                Text = "💡 Astuce: Les boutons s'allument quand ils sont pressés sur la manette",
                Location = new Point(20, 660),
                Size = new Size(860, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(infoLabel);
        }
        
        private Label CreateButtonLabel(string text, int x, int y, int width, int height, Color color)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                BackColor = color,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };
        }
        
        private void InitializeTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Afficher", null, (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; });
            trayMenu.Items.Add("Configuration", null, ConfigButton_Click);
            trayMenu.Items.Add("-");
            trayMenu.Items.Add("Quitter", null, (s, e) => Application.Exit());
            
            trayIcon = new NotifyIcon
            {
                Text = "Joy Controller",
                ContextMenuStrip = trayMenu,
                Visible = true
            };
            
            trayIcon.DoubleClick += (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; };
            
            // Icône simple (à remplacer par une vraie icône si nécessaire)
            using (Bitmap bmp = new Bitmap(32, 32))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.FillEllipse(new SolidBrush(Color.FromArgb(0, 120, 215)), 2, 2, 28, 28);
                trayIcon.Icon = Icon.FromHandle(bmp.GetHicon());
            }
        }
        
        private void StatusTimer_Tick(object? sender, EventArgs e)
        {
            if (controller == null || !controller.IsConnected)
            {
                connectionStatusLabel.Text = "❌ Manette déconnectée";
                connectionStatusLabel.ForeColor = Color.FromArgb(255, 200, 200);
                statusPanel.BackColor = Color.FromArgb(180, 50, 50);
                
                if (isRunning)
                {
                    isRunning = false;
                    startStopButton.Text = "▶️ Démarrer le Contrôle";
                    startStopButton.BackColor = Color.FromArgb(16, 124, 16);
                }
                return;
            }
            
            connectionStatusLabel.Text = isRunning ? "✅ Contrôle actif - Appuyez sur 'Start' pour arrêter" : "🔌 Manette connectée";
            connectionStatusLabel.ForeColor = Color.FromArgb(230, 230, 230);
            statusPanel.BackColor = isRunning ? Color.FromArgb(16, 124, 16) : Color.FromArgb(0, 120, 215);
            
            var state = controller.GetState();
            var gamepad = state.Gamepad;
            
            // Mettre à jour les visualiseurs de joysticks
            UpdateStickVisualizer(leftStickIndicator, gamepad.LeftThumbX, gamepad.LeftThumbY, config.Deadzone);
            UpdateStickVisualizer(rightStickIndicator, gamepad.RightThumbX, gamepad.RightThumbY, config.ScrollDeadzone);
            
            // Mettre à jour les indicateurs de boutons
            UpdateButtonIndicator(buttonALabel, (gamepad.Buttons & GamepadButtonFlags.A) != 0);
            UpdateButtonIndicator(buttonBLabel, (gamepad.Buttons & GamepadButtonFlags.B) != 0);
            UpdateButtonIndicator(buttonXLabel, (gamepad.Buttons & GamepadButtonFlags.X) != 0);
            UpdateButtonIndicator(buttonYLabel, (gamepad.Buttons & GamepadButtonFlags.Y) != 0);
            
            UpdateButtonIndicator(buttonLBLabel, (gamepad.Buttons & GamepadButtonFlags.LeftShoulder) != 0);
            UpdateButtonIndicator(buttonRBLabel, (gamepad.Buttons & GamepadButtonFlags.RightShoulder) != 0);
            UpdateTriggerIndicator(buttonLTLabel, gamepad.LeftTrigger);
            UpdateTriggerIndicator(buttonRTLabel, gamepad.RightTrigger);
            
            UpdateButtonIndicator(buttonDpadUpLabel, (gamepad.Buttons & GamepadButtonFlags.DPadUp) != 0);
            UpdateButtonIndicator(buttonDpadDownLabel, (gamepad.Buttons & GamepadButtonFlags.DPadDown) != 0);
            UpdateButtonIndicator(buttonDpadLeftLabel, (gamepad.Buttons & GamepadButtonFlags.DPadLeft) != 0);
            UpdateButtonIndicator(buttonDpadRightLabel, (gamepad.Buttons & GamepadButtonFlags.DPadRight) != 0);
            
            UpdateButtonIndicator(buttonStartLabel, (gamepad.Buttons & GamepadButtonFlags.Start) != 0);
            UpdateButtonIndicator(buttonBackLabel, (gamepad.Buttons & GamepadButtonFlags.Back) != 0);
            UpdateButtonIndicator(buttonLeftThumbLabel, (gamepad.Buttons & GamepadButtonFlags.LeftThumb) != 0);
            UpdateButtonIndicator(buttonRightThumbLabel, (gamepad.Buttons & GamepadButtonFlags.RightThumb) != 0);
        }
        
        private void UpdateStickVisualizer(Panel indicator, short x, short y, short deadzone)
        {
            // Convertir en int pour éviter l'overflow avec Math.Abs
            int xInt = x;
            int yInt = y;
            
            // Normaliser les valeurs
            double normalizedX = Math.Abs(xInt) < deadzone ? 0 : xInt / 32767.0;
            double normalizedY = Math.Abs(yInt) < deadzone ? 0 : yInt / 32767.0;
            
            // Calculer la position dans le visualiseur (140x140 pixels)
            int centerX = 60; // Centre du visualiseur
            int centerY = 60;
            int maxOffset = 50; // Distance maximale du centre
            
            int posX = centerX + (int)(normalizedX * maxOffset);
            int posY = centerY - (int)(normalizedY * maxOffset); // Inverser Y
            
            // Limiter les positions pour rester dans le visualiseur
            posX = Math.Clamp(posX, 0, 120);
            posY = Math.Clamp(posY, 0, 120);
            
            indicator.Location = new Point(posX, posY);
        }
        
        private void UpdateButtonIndicator(Label label, bool pressed)
        {
            if (pressed)
            {
                label.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                label.BorderStyle = BorderStyle.Fixed3D;
                // Augmenter légèrement la luminosité
                var baseColor = label.BackColor;
                label.BackColor = ControlPaint.Light(baseColor, 0.3f);
            }
            else
            {
                label.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                label.BorderStyle = BorderStyle.FixedSingle;
                // Restaurer la couleur d'origine (stocker dans Tag)
                if (label.Tag != null && label.Tag is Color)
                {
                    label.BackColor = (Color)label.Tag;
                }
            }
            
            // Sauvegarder la couleur de base dans Tag lors du premier appel
            if (label.Tag == null)
            {
                label.Tag = label.BackColor;
            }
        }
        
        private void UpdateTriggerIndicator(Label label, byte value)
        {
            // Les triggers vont de 0 à 255
            bool pressed = value > 30; // Seuil de déclenchement
            
            // Déterminer le nom du trigger (LT ou RT) - on utilise la référence directe
            string triggerName = (label == buttonLTLabel) ? "LT" : "RT";
            
            // Afficher la valeur du trigger
            if (value > 30)
            {
                label.Text = $"{triggerName}\n{value}";
            }
            else
            {
                label.Text = triggerName;
            }
            
            UpdateButtonIndicator(label, pressed);
        }
        
        private void StartStopButton_Click(object? sender, EventArgs e)
        {
            if (!controller.IsConnected)
            {
                MessageBox.Show(
                    "❌ Aucune manette détectée. Veuillez connecter une manette Xbox.",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            
            isRunning = !isRunning;
            
            if (isRunning)
            {
                startStopButton.Text = "⏸️ Arrêter le Contrôle";
                startStopButton.BackColor = Color.FromArgb(232, 17, 35);
                
                // Démarrer le thread de contrôle
                var controlThread = new System.Threading.Thread(() => ControlLoop());
                controlThread.IsBackground = true;
                controlThread.Start();
            }
            else
            {
                startStopButton.Text = "▶️ Démarrer le Contrôle";
                startStopButton.BackColor = Color.FromArgb(16, 124, 16);
            }
        }
        
        private void ConfigButton_Click(object? sender, EventArgs e)
        {
            using (var configForm = new ConfigForm(config))
            {
                if (configForm.ShowDialog() == DialogResult.OK)
                {
                    config = ControllerConfig.Load();
                }
            }
        }
        
        private void ControlLoop()
        {
            var mouseController = new MouseController(controller, config);
            while (isRunning && controller.IsConnected)
            {
                mouseController.Update();
                System.Threading.Thread.Sleep(10); // 100 FPS
            }
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && minimizeToTrayCheckBox.Checked)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                statusTimer?.Stop();
                statusTimer?.Dispose();
                trayIcon?.Dispose();
                isRunning = false;
            }
            
            base.OnFormClosing(e);
        }
    }
}
