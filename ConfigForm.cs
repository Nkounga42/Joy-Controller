using System;
using System.Drawing;
using System.Windows.Forms;
using SharpDX.XInput;

namespace JoyController
{
    public class ConfigForm : Form
    {
        private ControllerConfig config;
        private Controller? controller;
        
        // Controls
        private TrackBar sensitivityTrackBar = null!;
        private TrackBar deadzoneTrackBar = null!;
        private TrackBar accelerationTrackBar = null!;
        private TrackBar speedMultiplierTrackBar = null!;
        private TrackBar scrollDeadzoneTrackBar = null!;
        private TrackBar scrollSensitivityTrackBar = null!;
        
        private Label sensitivityLabel = null!;
        private Label deadzoneLabel = null!;
        private Label accelerationLabel = null!;
        private Label speedMultiplierLabel = null!;
        private Label scrollDeadzoneLabel = null!;
        private Label scrollSensitivityLabel = null!;
        
        private Label sensitivityValue = null!;
        private Label deadzoneValue = null!;
        private Label accelerationValue = null!;
        private Label speedMultiplierValue = null!;
        private Label scrollDeadzoneValue = null!;
        private Label scrollSensitivityValue = null!;
        
        private Button saveButton = null!;
        private Button cancelButton = null!;
        private Button resetButton = null!;
        private Button calibrateButton = null!;
        
        private Label calibrationLabel = null!;
        private ProgressBar leftStickXBar = null!;
        private ProgressBar leftStickYBar = null!;
        private ProgressBar rightStickYBar = null!;
        private Label leftStickXLabel = null!;
        private Label leftStickYLabel = null!;
        private Label rightStickYLabel = null!;
        
        private System.Windows.Forms.Timer? calibrationTimer;
        private bool isCalibrating = false;

        public ConfigForm(ControllerConfig config)
        {
            this.config = config;
            InitializeComponent();
            LoadConfig();
            
            // Essayer de détecter la manette
            controller = new Controller(UserIndex.One);
        }

        private void InitializeComponent()
        {
            this.Text = "⚙️ Configuration Joy Controller";
            this.Size = new Size(600, 750);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 245);

            int yPos = 20;
            int labelWidth = 180;
            int trackBarWidth = 300;
            int valueWidth = 60;
            int spacing = 70;

            // === Section Sensibilité ===
            CreateLabelAndTrackBar(
                ref sensitivityLabel, ref sensitivityTrackBar, ref sensitivityValue,
                "Sensibilité du curseur:", 5, 50, 10,
                yPos, labelWidth, trackBarWidth, valueWidth
            );
            sensitivityTrackBar.ValueChanged += (s, e) => {
                sensitivityValue.Text = sensitivityTrackBar.Value.ToString();
            };
            yPos += spacing;

            // === Section Zone morte ===
            CreateLabelAndTrackBar(
                ref deadzoneLabel, ref deadzoneTrackBar, ref deadzoneValue,
                "Zone morte (deadzone):", 3000, 15000, 1000,
                yPos, labelWidth, trackBarWidth, valueWidth
            );
            deadzoneTrackBar.ValueChanged += (s, e) => {
                deadzoneValue.Text = deadzoneTrackBar.Value.ToString();
            };
            yPos += spacing;

            // === Section Accélération ===
            CreateLabelAndTrackBar(
                ref accelerationLabel, ref accelerationTrackBar, ref accelerationValue,
                "Courbe d'accélération:", 10, 30, 1,
                yPos, labelWidth, trackBarWidth, valueWidth
            );
            accelerationTrackBar.ValueChanged += (s, e) => {
                double value = accelerationTrackBar.Value / 10.0;
                accelerationValue.Text = value.ToString("F1");
            };
            yPos += spacing;

            // === Section Multiplicateur de vitesse ===
            CreateLabelAndTrackBar(
                ref speedMultiplierLabel, ref speedMultiplierTrackBar, ref speedMultiplierValue,
                "Multiplicateur vitesse:", 5, 30, 1,
                yPos, labelWidth, trackBarWidth, valueWidth
            );
            speedMultiplierTrackBar.ValueChanged += (s, e) => {
                double value = speedMultiplierTrackBar.Value / 10.0;
                speedMultiplierValue.Text = value.ToString("F1");
            };
            yPos += spacing;

            // === Section Scroll deadzone ===
            CreateLabelAndTrackBar(
                ref scrollDeadzoneLabel, ref scrollDeadzoneTrackBar, ref scrollDeadzoneValue,
                "Zone morte scroll:", 3000, 12000, 1000,
                yPos, labelWidth, trackBarWidth, valueWidth
            );
            scrollDeadzoneTrackBar.ValueChanged += (s, e) => {
                scrollDeadzoneValue.Text = scrollDeadzoneTrackBar.Value.ToString();
            };
            yPos += spacing;

            // === Section Sensibilité scroll ===
            CreateLabelAndTrackBar(
                ref scrollSensitivityLabel, ref scrollSensitivityTrackBar, ref scrollSensitivityValue,
                "Sensibilité scroll:", 10, 100, 10,
                yPos, labelWidth, trackBarWidth, valueWidth
            );
            scrollSensitivityTrackBar.ValueChanged += (s, e) => {
                scrollSensitivityValue.Text = scrollSensitivityTrackBar.Value.ToString();
            };
            yPos += spacing + 20;

            // === Section Calibrage ===
            calibrationLabel = new Label {
                Text = "🎯 Calibrage en temps réel",
                Location = new Point(20, yPos),
                Size = new Size(560, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215)
            };
            this.Controls.Add(calibrationLabel);
            yPos += 35;

            // Joystick gauche X
            leftStickXLabel = new Label {
                Text = "Joystick gauche X:",
                Location = new Point(20, yPos),
                Size = new Size(150, 20),
                Font = new Font("Segoe UI", 9)
            };
            leftStickXBar = new ProgressBar {
                Location = new Point(170, yPos),
                Size = new Size(400, 20),
                Minimum = -32768,
                Maximum = 32767,
                Value = 0,
                Style = ProgressBarStyle.Continuous
            };
            this.Controls.Add(leftStickXLabel);
            this.Controls.Add(leftStickXBar);
            yPos += 30;

            // Joystick gauche Y
            leftStickYLabel = new Label {
                Text = "Joystick gauche Y:",
                Location = new Point(20, yPos),
                Size = new Size(150, 20),
                Font = new Font("Segoe UI", 9)
            };
            leftStickYBar = new ProgressBar {
                Location = new Point(170, yPos),
                Size = new Size(400, 20),
                Minimum = -32768,
                Maximum = 32767,
                Value = 0,
                Style = ProgressBarStyle.Continuous
            };
            this.Controls.Add(leftStickYLabel);
            this.Controls.Add(leftStickYBar);
            yPos += 30;

            // Joystick droit Y
            rightStickYLabel = new Label {
                Text = "Joystick droit Y (scroll):",
                Location = new Point(20, yPos),
                Size = new Size(150, 20),
                Font = new Font("Segoe UI", 9)
            };
            rightStickYBar = new ProgressBar {
                Location = new Point(170, yPos),
                Size = new Size(400, 20),
                Minimum = -32768,
                Maximum = 32767,
                Value = 0,
                Style = ProgressBarStyle.Continuous
            };
            this.Controls.Add(rightStickYLabel);
            this.Controls.Add(rightStickYBar);
            yPos += 40;

            // === Boutons ===
            calibrateButton = new Button {
                Text = "▶️ Démarrer calibrage",
                Location = new Point(20, yPos),
                Size = new Size(140, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            calibrateButton.FlatAppearance.BorderSize = 0;
            calibrateButton.Click += CalibrateButton_Click;
            this.Controls.Add(calibrateButton);

            resetButton = new Button {
                Text = "🔄 Réinitialiser",
                Location = new Point(170, yPos),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(220, 220, 220),
                FlatStyle = FlatStyle.Flat
            };
            resetButton.FlatAppearance.BorderSize = 0;
            resetButton.Click += ResetButton_Click;
            this.Controls.Add(resetButton);

            saveButton = new Button {
                Text = "💾 Sauvegarder",
                Location = new Point(320, yPos),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(16, 124, 16),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            cancelButton = new Button {
                Text = "❌ Annuler",
                Location = new Point(450, yPos),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(220, 220, 220),
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(cancelButton);
        }

        private void CreateLabelAndTrackBar(
            ref Label label, ref TrackBar trackBar, ref Label valueLabel,
            string labelText, int min, int max, int tickFreq,
            int yPos, int labelWidth, int trackBarWidth, int valueWidth)
        {
            label = new Label {
                Text = labelText,
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            trackBar = new TrackBar {
                Location = new Point(200, yPos - 5),
                Size = new Size(trackBarWidth, 45),
                Minimum = min,
                Maximum = max,
                TickFrequency = tickFreq,
                TickStyle = TickStyle.BottomRight
            };

            valueLabel = new Label {
                Text = "0",
                Location = new Point(510, yPos),
                Size = new Size(valueWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.Controls.Add(label);
            this.Controls.Add(trackBar);
            this.Controls.Add(valueLabel);
        }

        private void LoadConfig()
        {
            sensitivityTrackBar.Value = config.Sensitivity;
            deadzoneTrackBar.Value = config.Deadzone;
            accelerationTrackBar.Value = (int)(config.AccelerationCurve * 10);
            speedMultiplierTrackBar.Value = (int)(config.SpeedMultiplier * 10);
            scrollDeadzoneTrackBar.Value = config.ScrollDeadzone;
            scrollSensitivityTrackBar.Value = config.ScrollSensitivity;

            sensitivityValue.Text = config.Sensitivity.ToString();
            deadzoneValue.Text = config.Deadzone.ToString();
            accelerationValue.Text = config.AccelerationCurve.ToString("F1");
            speedMultiplierValue.Text = config.SpeedMultiplier.ToString("F1");
            scrollDeadzoneValue.Text = config.ScrollDeadzone.ToString();
            scrollSensitivityValue.Text = config.ScrollSensitivity.ToString();
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            config.Sensitivity = sensitivityTrackBar.Value;
            config.Deadzone = (short)deadzoneTrackBar.Value;
            config.AccelerationCurve = accelerationTrackBar.Value / 10.0;
            config.SpeedMultiplier = speedMultiplierTrackBar.Value / 10.0;
            config.ScrollDeadzone = (short)scrollDeadzoneTrackBar.Value;
            config.ScrollSensitivity = scrollSensitivityTrackBar.Value;

            config.Save();
            MessageBox.Show("✅ Configuration sauvegardée avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
        }

        private void ResetButton_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Voulez-vous réinitialiser tous les paramètres aux valeurs par défaut ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                config = new ControllerConfig();
                LoadConfig();
            }
        }

        private void CalibrateButton_Click(object? sender, EventArgs e)
        {
            if (!isCalibrating)
            {
                StartCalibration();
            }
            else
            {
                StopCalibration();
            }
        }

        private void StartCalibration()
        {
            if (controller == null || !controller.IsConnected)
            {
                MessageBox.Show(
                    "❌ Aucune manette détectée. Connectez une manette Xbox et réessayez.",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            isCalibrating = true;
            calibrateButton.Text = "⏸️ Arrêter calibrage";
            calibrateButton.BackColor = Color.FromArgb(232, 17, 35);

            calibrationTimer = new System.Windows.Forms.Timer { Interval = 50 }; // 20 FPS
            calibrationTimer.Tick += CalibrationTimer_Tick;
            calibrationTimer.Start();
        }

        private void StopCalibration()
        {
            isCalibrating = false;
            calibrateButton.Text = "▶️ Démarrer calibrage";
            calibrateButton.BackColor = Color.FromArgb(0, 120, 215);

            if (calibrationTimer != null)
            {
                calibrationTimer.Stop();
                calibrationTimer.Dispose();
                calibrationTimer = null;
            }

            leftStickXBar.Value = 0;
            leftStickYBar.Value = 0;
            rightStickYBar.Value = 0;
        }

        private void CalibrationTimer_Tick(object? sender, EventArgs e)
        {
            if (controller == null || !controller.IsConnected)
            {
                StopCalibration();
                MessageBox.Show("⚠️ Manette déconnectée.", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var state = controller.GetState();
            var gamepad = state.Gamepad;

            leftStickXBar.Value = Math.Clamp((int)gamepad.LeftThumbX, -32768, 32767);
            leftStickYBar.Value = Math.Clamp((int)gamepad.LeftThumbY, -32768, 32767);
            rightStickYBar.Value = Math.Clamp((int)gamepad.RightThumbY, -32768, 32767);

            leftStickXLabel.Text = $"Joystick gauche X: {gamepad.LeftThumbX}";
            leftStickYLabel.Text = $"Joystick gauche Y: {gamepad.LeftThumbY}";
            rightStickYLabel.Text = $"Joystick droit Y: {gamepad.RightThumbY}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopCalibration();
            base.OnFormClosing(e);
        }
    }
}
