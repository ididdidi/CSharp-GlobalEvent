namespace ru.mofrison.GlobalSignals
{
    public class Log
    {
        /// <summary>
        /// Types of messages sent to log
        /// </summary>
        public enum Type { Entry, Warning, Error }

        /// <summary>
        /// Base class for log messages
        /// </summary>
        public class Entry : Signal<Entry>
        {
            private Type type;
            private string text;
            public Type Type { get => type; }
            public string Text { get => text; }

            /// <summary>
            /// Base class constructor
            /// </summary>
            /// <param name="type">Message type</param>
            /// <param name="text">Message text</param>
            protected Entry(Type type, string text)
            {
                this.type = type;
                this.text = text;
            }

            /// <summary>
            /// The method required to send the signal
            /// </summary>
            /// <param name="text">Message text</param>
            public static void Send(string text)
            {
                try
                {
                    hendlers.Invoke(new Entry(Type.Entry, text));
                }
                catch (System.NullReferenceException e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        /// <summary>
        /// A more complex example of signal implementation. Adds only unique methods as delegates.
        /// </summary>
        public sealed class Warning : Entry
        {
            /// <summary>
            /// Class constructor
            /// </summary>
            /// <param name="text">Message text</param>
            public Warning(string text) : base(Type.Warning, text) { }
            
            /// <summary>
            /// The method required to send the signal
            /// </summary>
            /// <param name="text">Message text</param>
            public new static void Send(string text)
            {
                try
                {
                    hendlers.Invoke(new Warning(text));
                }
                catch (System.NullReferenceException e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        /// <summary>
        /// A more complex example of signal implementation. Adds only unique methods as delegates.
        /// </summary>
        public class Error : Entry
        {
            /// <summary>
            /// Class constructor
            /// </summary>
            /// <param name="text">Message text</param>
            public Error(string text) : base(Type.Error, text) { }

            /// <summary>
            /// The method required to send the signal
            /// </summary>
            /// <param name="text">Message text</param>
            public new static void Send(string text)
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

}