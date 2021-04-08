namespace ru.mofrison.global_signals
{
    /// <summary>
    /// A more complex example of signal implementation. Adds only unique methods as delegates.
    /// </summary>
    public sealed class Warning : Signal<Warning>
    {
        private string text;
        public string Text { get => text; }

        public Warning(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Overrides a similar method of the base class.
        /// Checks whether the delegate being added is present in the list of already added delegates.
        /// </summary>
        /// <param name="hendlers">Reference to a method or delegates</param>
        public new static void Subscribe(Hendler hendlers)
        {
            Signal<Warning>.hendlers = GetUniqueHendlers(Signal<Warning>.hendlers + hendlers);
        }

        /// <summary>
        /// The method required to send the signal
        /// </summary>
        /// <param name="text">Sent data</param>
        public static void Send(string text)
        {
            try
            {
                hendlers.Invoke(new Error(text));
            }
            catch (System.NullReferenceException e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
