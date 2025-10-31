using System;
using System.Windows.Forms;
using JoyController;

class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        
        // Lancer l'application avec la nouvelle interface graphique
        Application.Run(new MainForm());
    }
}
