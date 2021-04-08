namespace ru.mofrison.global_signals
{
    class Message : Signal<Message>
    {
        private string text;
        public string Text { get => text; }

        private Message(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// The method required to send the signal
        /// </summary>
        /// <param name="text">Sent data</param>
        public static void Send(string text)
        {
            hendlers?.Invoke(new Message(text));
        }

        public void Send(Signal<Message> signal)
        {
            throw new System.NotImplementedException();
        }
    }
}
