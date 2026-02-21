using System;

namespace FakeNewsDetector.Model
{
    public class ChatMessage
    {
        public string Text { get; set; }
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public ChatMessage() { }

        public ChatMessage(string text, bool isUser)
        {
            Text = text;
            IsUser = isUser;
        }
    }
}