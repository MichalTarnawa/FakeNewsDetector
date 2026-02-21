using FakeNewsDetector.Helpers;
using FakeNewsDetector.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public ICommand SendCommand { get; }

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
        }

        private async void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(Message) || IsBusy) return;

            string userText = Message;
            Message = string.Empty;
            IsBusy = true;

            ChatHistory.Add(new ChatMessage(userText, true));

            
            var loadingMessage = new ChatMessage("⏳ *Agenci analizują zapytanie...*", false);
            ChatHistory.Add(loadingMessage);

            
            string response = await Task.Run(() => _apiService.SendMessageAsync(userText, ApiUrl));

            
            ChatHistory.Remove(loadingMessage);
            ChatHistory.Add(new ChatMessage(response, false));

            IsBusy = false;
        }

        private bool CanSend()
        {
            return !string.IsNullOrWhiteSpace(Message) && !IsBusy;
        }
    }
}