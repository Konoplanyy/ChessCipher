using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ChessCipher;

public partial class MainWindow: Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void EncodeButton_Click(object sender, RoutedEventArgs e)
    {
        string inputText = InputTextBox.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(inputText))
        {
            return;
        }

        SecondWindow.Text = inputText;
        var SW = new SecondWindow();
        SW.Show();
        
    }
}