using FakeNewsDetector.Helpers;
using FakeNewsDetector.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
        private string _imagesource1;
        public string ImageSource1
        {
            get => _imagesource1;
            set { _imagesource1 = value; OnChanged(); }
        }
        private string _imagesource2;
        public string ImageSource2
        {
            get => _imagesource2;
            set { _imagesource2 = value; OnChanged(); }
        }
        private string _imageloaded0 = "Hidden";
        public string ImageLoaded0
        {
            get => _imageloaded0;
            set { _imageloaded0 = value; OnChanged(); }
        }
        private string _imageloaded1 = "Hidden" ;
        public string ImageLoaded1
        {
            get => _imageloaded1;
            set { _imageloaded1 = value; OnChanged(); }
        }
        private string _imageloaded2 = "Hidden";
        public string ImageLoaded2
        {
            get => _imageloaded2;
            set { _imageloaded2 = value; OnChanged(); }
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
        public ICommand RemoveImage1 { get; }
        public ICommand RemoveImage2 { get; }
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
            RemoveImage1 = new RelayCommand(Remove_Image1);
            RemoveImage2 = new RelayCommand(Remove_Image2);
            Debug.WriteLine(IsBusy);
        }

        private async void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(Message) || IsBusy) return;

            string userText = Message;
            Message = string.Empty;
            IsBusy = true;
            if (!string.IsNullOrEmpty(ImageSource0))
            {
                userText += $" [Obraz: {ImageSource0}]";
            }
            if (!string.IsNullOrEmpty(ImageSource1))
            {
                userText += $" [Obraz: {ImageSource1}]";
            }
            if (!string.IsNullOrEmpty(ImageSource2))
            {
                userText += $" [Obraz: {ImageSource2}]";
            }

            ChatHistory.Add(new ChatMessage(userText, true));

            
            var loadingMessage = new ChatMessage("⏳ *Agenci analizują zapytanie...*", false);
            ChatHistory.Add(loadingMessage);

            
            string response = await Task.Run(() => _apiService.SendMessageAsync(userText, ApiUrl));

            
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
                image = new BitmapImage(new Uri(filePath));
                if (ImageLoaded0 == "Hidden")
                {
                    ImageSource0 = filePath;
                    ImageLoaded0 = "Visible";
                }
                else if (ImageSource0 != filePath && ImageLoaded1 == "Hidden")
                {
                    ImageSource1 = filePath;
                    ImageLoaded1 = "Visible";
                }
                else
                {
                    ImageSource2 = filePath;
                    ImageLoaded2 = "Visible";
                }
                Debug.WriteLine("dziala:" + _imagesource0);
            }
        }
        private void Remove_Image0()
        {
            ImageSource0 = string.Empty;
            ImageLoaded0 = "Hidden";
        }
        private void Remove_Image1()
        {
            ImageSource1 = string.Empty;
            ImageLoaded1 = "Hidden";
        }
        private void Remove_Image2()
        {
            ImageSource2 = string.Empty;
            ImageLoaded2 = "Hidden";
        }
        private bool CanSend()
        {
            return !string.IsNullOrWhiteSpace(Message) && !IsBusy;
        }
    }
}