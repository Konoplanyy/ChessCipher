using System;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

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

    private async void FromFile_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var file = await AskLoadFileAsync();
            if (file == null) return;

            await using var stream = await file.OpenReadAsync();
            using var reader = new StreamReader(stream);
            var text = await reader.ReadToEndAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var match = JsonSerializer.Deserialize<ChessCipherLibrary.Models.Match>(text, options);
            if (match is null)
            {
                // MessageBox.Show("Не вдалося розпарсити матч (match == null).");
                return;
            }

            var result = ChessCipherLibrary.ChessCipher.FromMatch(match);

            InputTextBox.Text = result;
            System.Diagnostics.Debug.WriteLine(result);
        }
        catch (JsonException ex)
        {
            // MessageBox.Show($"JSON помилка: {ex.Message}");
        }
        catch (Exception ex)
        {
            // MessageBox.Show($"Помилка: {ex.Message}");
        }
    }
    
    private async Task<IStorageFile?> AskLoadFileAsync()
    {
        var file = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select a file",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("JSON") {Patterns = new[] {"*.json"} }
            }
        });
            
        return file.Count > 0 ? file[0] : null;
    }
}