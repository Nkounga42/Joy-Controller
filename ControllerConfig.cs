using System;
using System.IO;
using System.Text.Json;

namespace JoyController
{
    public class ControllerConfig
    {
        public int Sensitivity { get; set; } = 10;
        public short Deadzone { get; set; } = 8000;
        public double AccelerationCurve { get; set; } = 1.5;
        public double SpeedMultiplier { get; set; } = 1.5;
        public short ScrollDeadzone { get; set; } = 6000;
        public int ScrollSensitivity { get; set; } = 30;

        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "JoyController",
            "config.json"
        );

        public static ControllerConfig Load()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    return JsonSerializer.Deserialize<ControllerConfig>(json) ?? new ControllerConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement de la configuration: {ex.Message}");
            }

            return new ControllerConfig();
        }

        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(ConfigFilePath)!;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sauvegarde de la configuration: {ex.Message}");
            }
        }
    }
}
