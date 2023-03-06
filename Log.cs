namespace ru.mofrison.GlobalEvent
{
    public class Log
    {
        /// <summary>
        /// Base class for log messages
        /// </summary>
        public class Message : GlobalEvent<Message>
        {
            public string Text { get; }

            /// <summary>
            /// Base class constructor
            /// </summary>
            /// <param name="text">Message text</param>
            protected Message(string text) => Text = text;

            /// <summary>
            /// The method required to send the global event
            /// </summary>
            /// <param name="text">Message text</param>
            public static void Send(string text) => Handle(new Message(text));
        }

        /// <summary>
        /// A more complex example of global event implementation. Adds only unique methods as delegates.
        /// </summary>
        public sealed class Warning : GlobalEvent<Warning>
        {
            public string Text { get; }

            /// <summary>
            /// Class constructor
            /// </summary>
            /// <param name="text">Message text</param>
            private Warning(string text) => Text = text;

            /// <summary>
            /// The method required to send the global event
            /// </summary>
            /// <param name="text">Warning message text</param>
            public static void Send(string text) => Handle(new Warning(text));
        }

        /// <summary>
        /// A more complex example of global event implementation. Adds only unique methods as delegates.
        /// </summary>
        public sealed class Error : GlobalEvent<Error>
        {
            public string Text { get; }

            /// <summary>
            /// Class constructor
            /// </summary>
            /// <param name="text">Message text</param>
            private Error(string text) => Text = text;

            /// <summary>
            /// The method required to send the global event
            /// </summary>
            /// <param name="text">Error message text</param>
            public static void Send(string text) => Handle(new Error(text));
        }
    }

}