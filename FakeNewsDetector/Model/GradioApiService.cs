using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace FakeNewsDetector.Model
{
    public class GradioApiService : IDisposable
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _bridgeUrl = "http://127.0.0.1:5005/analyze";
        private Process _pythonProcess;

        public GradioApiService()
        {
            _httpClient.Timeout = TimeSpan.FromMinutes(5);

            //uruchomienie skryptu przy starcie
            StartPythonBridge();

            //zabicie procesu
            Application.Current.Exit += (s, e) => Dispose();
        }

        private void StartPythonBridge()
        {
            try
            {
                
                string modelFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Model");
                string scriptPath = Path.Combine(modelFolderPath, "bridge.py");

                if (!File.Exists(scriptPath))
                {
                    MessageBox.Show($"Nie znaleziono pliku skryptu:\n{scriptPath}\n\nUpewnij się, że we właściwościach pliku bridge.py ustawiłeś 'Kopiuj, jeśli nowszy'.", "Błąd plików", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var psi = new ProcessStartInfo
                {
                    FileName = "python",//loklizacja Pyhtona
                    Arguments = $"\"{scriptPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true, //TRUE ukrywa czarne okienko konsoli
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = modelFolderPath //Ważne: uruchamiamy z poziomu folderu Model
                };

                _pythonProcess = Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się automatycznie uruchomić Pythona.\nBłąd: {ex.Message}", "Błąd uruchamiania", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task<string> SendMessageAsync(string message, string apiUrl)
        {
            if (string.IsNullOrWhiteSpace(apiUrl))
                return "Błąd: Nie podano adresu URL do API Gradio.";

            try
            {
                var payload = new
                {
                    url = apiUrl.Trim().TrimEnd('/'),
                    question = message
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(_bridgeUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseString);

                    if (doc.RootElement.TryGetProperty("answer", out var answerProp))
                        return answerProp.GetString() ?? "Otrzymano pustą odpowiedź.";

                    if (doc.RootElement.TryGetProperty("error", out var errorProp))
                        return $"Błąd w moście Python: {errorProp.GetString()}";
                }

                return $"Błąd serwera lokalnego ({response.StatusCode}). Czy skrypt na pewno się uruchomił?";
            }
            catch (Exception ex)
            {
                return $"Błąd komunikacji: {ex.Message}\nUpewnij się, że lokalny most działa.";
            }
        }

        public void Dispose()
        {
            try
            {
                if (_pythonProcess != null && !_pythonProcess.HasExited)
                {
                    _pythonProcess.Kill();
                    _pythonProcess.Dispose();
                }
            }
            catch
            {
            }
        }
    }
}