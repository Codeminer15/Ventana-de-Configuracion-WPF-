using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Config.Models;
using WPF_Config.Data;
using Microsoft.VisualBasic;
using System.Windows.Media.Animation;


namespace WPF_Config;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool ventanaCargada = false;
    public MainWindow()
    {
        InitializeComponent();
        // Constructor principal de la ventana. Inicializa los componentes y desactiva el botón Guardar hasta que todos los campos estén validados.
        Loaded += (s, e) => ventanaCargada = true;
        btnGuardar.IsEnabled = false;

    }

    //Funcióm que permite mover libremente la ventana, despues de utilizar la barra de titulo de la ventana principal
    private void Windows_MousDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
    private async void Guardar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            LoadingOverlay.Visibility = Visibility.Visible;
            var storyboard = (Storyboard)LoadingSpinner.Resources["RotateStoryboard"];
            storyboard.Begin();

            await Task.Delay(500); // Simulación para que se vea el loader. Elimina si no es necesario.

            using var db = new DBConfig();
            db.Database.EnsureCreated(); // Crea la base de datos si no existe

            // Buscar si ya existe un registro
            var configExistente = db.UrlConfig.FirstOrDefault();

            if (configExistente != null)
            {
                // Actualizar campos del registro existente
                configExistente.UserAccessToken = txtAccessToken.Text;
                configExistente.PhoneNumberId = txtPhoneId.Text;
                configExistente.ApiUrl = txtUrl.Text;
                configExistente.Version = ((ComboBoxItem)cmbVersion.SelectedItem).Content.ToString();
                configExistente.LastUpdate = DateTime.Now;
            }
            else
            {
                // Crear un nuevo registro si no existe
                var nuevaConfig = new UrlConfig
                {
                    UserAccessToken = txtAccessToken.Text,
                    PhoneNumberId = txtPhoneId.Text,
                    ApiUrl = txtUrl.Text,
                    Version = ((ComboBoxItem)cmbVersion.SelectedItem).Content.ToString(),
                    LastUpdate = DateTime.Now
                };
                db.UrlConfig.Add(nuevaConfig);
            }

            db.SaveChanges();

            // Ocultar el spinner antes de mostrar el MessageBox
            storyboard.Stop();
            LoadingOverlay.Visibility = Visibility.Collapsed;

            MessageBox.Show("Configuración guardada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            var storyboard = (Storyboard)LoadingSpinner.Resources["RotateStoryboard"];
            storyboard.Stop();
            LoadingOverlay.Visibility = Visibility.Collapsed;

            MessageBox.Show("Error al guardar:" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        this.Close(); 
    }

    // Valida si el campo TextBox tiene contenido.
    // Si está vacío, cambia el borde y fondo a rojo para indicar error.
    // Si tiene contenido, restaura los estilos por defecto.
    private bool ValidarCampo(TextBox campo)
    {
        // Buscar el Border contenedor de forma segura
        DependencyObject parent = campo;
        while (parent != null && !(parent is Border))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        var border = parent as Border;

        if (string.IsNullOrWhiteSpace(campo.Text))
        {
            if (border != null)
                border.BorderBrush = Brushes.Red;

            return false;
        }
        else
        {
            if (border != null)
                border.BorderBrush = Brushes.Gray;

            return true;
        }
    }

    // Valida todos los campos del formulario (token, número, URL y versión seleccionada).
    // Cambia estilos visuales según validez y activa o desactiva el botón Guardar en consecuencia.
    private void ValidarCampos(object sender, RoutedEventArgs e)
    {
        if (!ventanaCargada) return;

        bool tokenOk = ValidarCampo(txtAccessToken);
        bool phoneIdOk = ValidarCampo(txtPhoneId);
        bool urlOk = ValidarCampo(txtUrl); // ya no lo ignoramos
        bool versionOk = cmbVersion.SelectedItem is ComboBoxItem;

        cmbVersion.BorderBrush = versionOk ? Brushes.Gray : Brushes.Red;
        cmbVersion.Background = versionOk ? Brushes.Transparent : new SolidColorBrush(Color.FromRgb(255, 240, 240));

        btnGuardar.IsEnabled = tokenOk && phoneIdOk && urlOk && versionOk;
    }

    // Evento que restringe la entrada de texto en el campo PhoneId para que solo acepte caracteres numéricos.
    // Se ejecuta en tiempo real mientras el usuario escribe.
    private void TxtPhoneId_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !EsNumero(e.Text);
    }

    // Verifica si una cadena contiene únicamente dígitos numéricos.
    private bool EsNumero(string texto)
    {
        return texto.All(char.IsDigit);
    }

    private void btnCerrar_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}