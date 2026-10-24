using CajaArequipa.Modelos;
using CajaArequipa.Servicios;

namespace CajaArequipa.Interfaz;

/// Interfaz del avance parcial: lista simple, lista doble, cola FIFO y pila LIFO.
public sealed class FrmPrincipal : Form
{
    private readonly GestorTransacciones gestor = new();

    private readonly TextBox txtCodigo = new() { PlaceholderText = "Ej. OP001", Width = 120 };
    private readonly ComboBox cmbTipo = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 145 };
    private readonly NumericUpDown numMonto = new() { DecimalPlaces = 2, Maximum = 1000000, Minimum = 0, Value = 50, Width = 110 };
    private readonly TextBox txtOrigen = new() { Text = "CTA001", Width = 100 };
    private readonly TextBox txtDestino = new() { Text = "CTA002", Width = 100 };
    private readonly TextBox txtBuscar = new() { PlaceholderText = "Ej. OP001", Width = 120 };
    private readonly TextBox txtReversa = new() { PlaceholderText = "Código procesado", Width = 140 };
    private readonly DataGridView grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
    private readonly TextBox txtSalidaHist = CrearSalida();
    private readonly TextBox txtSalidaRev = CrearSalida();
    private readonly Label lblCola = new() { AutoSize = true };
    private readonly Label lblSaldos = new() { AutoSize = true };

    public FrmPrincipal()
    {
        Text = "Billetera móvil — Examen parcial de Estructuras de Datos";
        Width = 1120;
        Height = 720;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 580);
        ConstruirInterfaz();
        ActualizarVista();
    }

    private void ConstruirInterfaz()
    {
        cmbTipo.Items.AddRange(Enum.GetNames<TipoTransaccion>());
        cmbTipo.SelectedItem = nameof(TipoTransaccion.Abono);
        if (cmbTipo.SelectedIndex < 0) cmbTipo.SelectedIndex = 0;

        var tabs = new TabControl { Dock = DockStyle.Fill };
        var tabOperaciones = new TabPage("Lista simple y cola FIFO");
        var tabHistorial = new TabPage("Lista doble e historial");
        var tabReversiones = new TabPage("Pila LIFO y reversas");
        tabs.TabPages.AddRange(new[] { tabOperaciones, tabHistorial, tabReversiones });
        Controls.Add(tabs);

        var panelRegistro = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10), WrapContents = true };
        panelRegistro.Controls.AddRange(new Control[]
        {
            Etiqueta("Código"), txtCodigo, Etiqueta("Tipo"), cmbTipo,
            Etiqueta("Monto S/"), numMonto, Etiqueta("Cuenta origen"), txtOrigen,
            Etiqueta("Cuenta destino"), txtDestino,
            Boton("Registrar en lista y cola", Registrar)
        });
        var panelAcciones = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10), WrapContents = true };
        panelAcciones.Controls.AddRange(new Control[]
        {
            Boton("Procesar siguiente (FIFO)", ProcesarSiguiente),
            Boton("Ver cola pendiente", VerCola),
            Boton("Actualizar vista", (_, _) => ActualizarVista()),
            lblCola
        });
        var panelInfo = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, Padding = new Padding(10) };
        panelInfo.Controls.Add(lblSaldos);
        var panelOperaciones = new Panel { Dock = DockStyle.Fill };
        panelOperaciones.Controls.Add(grid);
        panelOperaciones.Controls.Add(panelInfo);
        panelOperaciones.Controls.Add(panelAcciones);
        panelOperaciones.Controls.Add(panelRegistro);
        tabOperaciones.Controls.Add(panelOperaciones);

        var histTop = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10), WrapContents = true };
        histTop.Controls.AddRange(new Control[]
        {
            Etiqueta("Buscar código en lista simple"), txtBuscar,
            Boton("Buscar", Buscar),
            Boton("Historial adelante", (_, _) => MostrarHistorial(false)),
            Boton("Historial inverso", (_, _) => MostrarHistorial(true)),
            Boton("Mostrar lista simple", (_, _) => MostrarListaSimple())
        });
        var histPanel = new Panel { Dock = DockStyle.Fill };
        histPanel.Controls.Add(txtSalidaHist);
        histPanel.Controls.Add(histTop);
        tabHistorial.Controls.Add(histPanel);

        var revTop = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(12), WrapContents = true };
        revTop.Controls.AddRange(new Control[]
        {
            Etiqueta("Código a revertir"), txtReversa,
            Boton("Solicitar reversa (apilar)", SolicitarReversa),
            Boton("Procesar última reversa (LIFO)", ProcesarReversa),
            Boton("Ver pila", VerPila)
        });
        var revPanel = new Panel { Dock = DockStyle.Fill };
        revPanel.Controls.Add(txtSalidaRev);
        revPanel.Controls.Add(revTop);
        tabReversiones.Controls.Add(revPanel);
    }

    private static TextBox CrearSalida() => new()
    {
        Multiline = true,
        ReadOnly = true,
        ScrollBars = ScrollBars.Vertical,
        Dock = DockStyle.Fill
    };

    private static Label Etiqueta(string texto) => new() { Text = texto, AutoSize = true, Margin = new Padding(5, 8, 2, 2) };
    private static Button Boton(string texto, EventHandler accion)
    {
        var b = new Button { Text = texto, AutoSize = true, Margin = new Padding(4) };
        b.Click += accion;
        return b;
    }

    private void Registrar(object? sender, EventArgs e)
    {
        try
        {
            var tipo = Enum.Parse<TipoTransaccion>(cmbTipo.SelectedItem!.ToString()!);
            var t = gestor.Registrar(txtCodigo.Text, tipo, numMonto.Value, txtOrigen.Text, txtDestino.Text);
            AgregarSalida($"Registrada: {t.Codigo}. Se agregó a la lista simple, al historial doble y a la cola FIFO.");
            txtCodigo.Clear();
            ActualizarVista();
        }
        catch (Exception ex) { Aviso(ex.Message); }
    }

    private void ProcesarSiguiente(object? sender, EventArgs e)
    {
        var t = gestor.ProcesarSiguiente();
        if (t is null) { Aviso("No hay transacciones pendientes en la cola."); return; }
        AgregarSalida($"FIFO procesó {t.Codigo}: {t.Estado}. {t.MensajeResultado}");
        ActualizarVista();
    }

    private void VerCola(object? sender, EventArgs e)
    {
        var pendientes = gestor.Pendientes.ObtenerTodos();
        txtSalidaHist.Text = pendientes.Count == 0
            ? "La cola FIFO está vacía."
            : "Cola FIFO (frente → final):" + Environment.NewLine + string.Join(Environment.NewLine, pendientes.Select(t => t.Codigo));
    }

    private void Buscar(object? sender, EventArgs e)
    {
        var t = gestor.Buscar(txtBuscar.Text.Trim());
        txtSalidaHist.Text = t is null ? "No se encontró el código en la lista enlazada simple." : $"Encontrada en la lista simple:{Environment.NewLine}{t}{Environment.NewLine}{t.MensajeResultado}";
    }

    private void MostrarListaSimple()
    {
        var todos = gestor.Transacciones.ObtenerTodos();
        txtSalidaHist.Text = todos.Count == 0 ? "La lista simple está vacía." :
            "Lista enlazada simple (primero → siguiente):" + Environment.NewLine + string.Join(Environment.NewLine, todos.Select(t => t.ToString()));
    }

    private void MostrarHistorial(bool inverso)
    {
        var todos = gestor.Historial.ObtenerTodos(inverso);
        txtSalidaHist.Text = todos.Count == 0 ? "El historial está vacío." :
            (inverso ? "Lista doble (nuevo → antiguo):" : "Lista doble (antiguo → nuevo):") + Environment.NewLine +
            string.Join(Environment.NewLine, todos.Select(t => $"{t} | {t.MensajeResultado}"));
    }

    private void SolicitarReversa(object? sender, EventArgs e)
    {
        try
        {
            var codigo = txtReversa.Text.Trim();
            gestor.SolicitarReversa(codigo);
            AgregarSalida($"Solicitud de reversa apilada para {codigo}.");
            ActualizarVista();
        }
        catch (Exception ex) { Aviso(ex.Message); }
    }

    private void ProcesarReversa(object? sender, EventArgs e)
    {
        try
        {
            var t = gestor.ProcesarReversaMasReciente();
            if (t is null) { Aviso("La pila de reversas está vacía."); return; }
            AgregarSalida($"LIFO procesó reversa de {t.Codigo}: {t.Estado}. {t.MensajeResultado}");
            ActualizarVista();
        }
        catch (Exception ex) { Aviso(ex.Message); }
    }

    private void VerPila(object? sender, EventArgs e)
    {
        var elementos = gestor.SolicitudesReversa.ObtenerTodos();
        txtSalidaRev.Text = elementos.Count == 0 ? "La pila de reversas está vacía." :
            "Pila LIFO (cima → fondo):" + Environment.NewLine + string.Join(Environment.NewLine, elementos.Select(t => t.Codigo));
    }

    private void ActualizarVista()
    {
        grid.DataSource = null;
        grid.DataSource = gestor.Transacciones.ObtenerTodos()
            .Select(t => new
            {
                t.Codigo,
                Tipo = t.Tipo.ToString(),
                t.Monto,
                t.CuentaOrigen,
                t.CuentaDestino,
                Estado = t.Estado.ToString(),
                Registro = t.FechaRegistro.ToString("dd/MM/yyyy HH:mm:ss"),
                Procesamiento = t.FechaProcesamiento?.ToString("dd/MM/yyyy HH:mm:ss") ?? "—",
                t.CodigoReversa,
                t.MensajeResultado
            }).ToList();

        lblCola.Text = $"Pendientes en cola FIFO: {gestor.Pendientes.Cantidad}";
        lblSaldos.Text = "Saldos simulados: " + string.Join("   |   ", gestor.Saldos.OrderBy(x => x.Key).Select(x => $"{x.Key}: S/ {x.Value:F2}"));
    }

    private void AgregarSalida(string texto)
    {
        string mensaje = $"{DateTime.Now:HH:mm:ss} — {texto}{Environment.NewLine}";
        txtSalidaHist.Text = mensaje + txtSalidaHist.Text;
        txtSalidaRev.Text = mensaje + txtSalidaRev.Text;
    }

    private static void Aviso(string mensaje) =>
        MessageBox.Show(mensaje, "Billetera móvil — Examen parcial", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
