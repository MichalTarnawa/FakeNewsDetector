using FakeNewsDetector.Helpers;
using FakeNewsDetector.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FakeNewsDetector.ViewModel
{
    public class MainWindow_VM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private readonly GradioApiService _apiService = new GradioApiService();

        public ObservableCollection<ChatMessage> ChatHistory { get; set; }

        private string _message;
        public string Message
        {
            get => _message;
            set { _message = value; OnChanged(); }
        }
        private string _imagesource0;
        public string ImageSource0
        {
            get => _imagesource0;
            set { _imagesource0 = value; OnChanged(); }
        } 
        private string _imageloaded0 = "Hidden";
        public string ImageLoaded0
        {
            get => _imageloaded0;
            set { _imageloaded0 = value; OnChanged(); }
        }
        
        private string _apiUrl = "";
        public string ApiUrl
        {
            get => _apiUrl;
            set { _apiUrl = value; OnChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnChanged(); CommandManager.InvalidateRequerySuggested(); }
        }
        private BitmapImage image;
        public ICommand SendCommand { get; }
        public ICommand FilePick { get; }
        public ICommand RemoveImage0 { get; }
        public MainWindow_VM()
        {
            ChatHistory = new ObservableCollection<ChatMessage>
            {
                
                new ChatMessage("# Witaj w Systemie OSINT Wieloagentowym! \n\n" +
                                "Ten system jest obsługiwany przez Koordynatora AI, który zarządza grupą agentów specjalizujących się w wykrywaniu manipulacji.\n\n" +
                                "### Co możesz zrobić:\n" +
                                "* Wklej podejrzany artykuł.\n" +
                                "* Podaj link do zdjęcia, by zweryfikować czy to nie fake news.\n" +
                                "* Zapytaj o rzetelność danego portalu.\n\n" +
                                "--- \n*Wyślij wiadomość, aby rozpocząć analizę.*", false)
            };

            SendCommand = new RelayCommand(SendMessage, CanSend);
            FilePick = new RelayCommand(Image_pick);
            RemoveImage0 = new RelayCommand(Remove_Image0);
            Debug.WriteLine(IsBusy);
        }

        private async void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(Message) || IsBusy) return;

            string userText = Message;
            Message = string.Empty;
            string[] images = new string[1];
            IsBusy = true;

            ChatHistory.Add(new ChatMessage(userText, true));

            
            if (!string.IsNullOrEmpty(ImageSource0))
            {
                byte[] bytes = File.ReadAllBytes(ImageSource0);
                images[0] = Convert.ToBase64String(bytes);
            }
            var loadingMessage = new ChatMessage("⏳ *Agenci analizują zapytanie...*", false);
            ChatHistory.Add(loadingMessage);

            
            string response = await Task.Run(() => _apiService.SendMessageAsync(userText, images, ApiUrl));

            
            ChatHistory.Remove(loadingMessage);
            ChatHistory.Add(new ChatMessage(response, false));

            IsBusy = false;
        }
        private void Image_pick() { 
          var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                    Title = "Wybierz obraz do analizy"
                };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                if (ImageLoaded0 == "Hidden")
                {
                    ImageLoaded0 = "Visible";
                }
                ImageSource0 = filePath;
            }
        }
        private void Remove_Image0()
        {
            ImageSource0 = string.Empty;
            ImageLoaded0 = "Hidden";
        }
        private bool CanSend()
        {
            return !string.IsNullOrWhiteSpace(Message) && !IsBusy;
        }
    }
}