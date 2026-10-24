using CajaArequipa.Interfaz;

namespace CajaArequipa;

/// Clase principal donde inicia la ejecución de la aplicación de escritorio (Windows Forms).
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        // Inicializa la configuración visual de la aplicación y ejecuta el formulario principal.
        ApplicationConfiguration.Initialize();
        Application.Run(new FrmPrincipal());
    }
}
