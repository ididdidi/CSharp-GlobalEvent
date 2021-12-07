using System.Collections.Generic;

namespace ru.mofrison.GlobalSignals
{
    /// <summary>
    /// A base class that is used to implement the mechanism for subscribing to signals.
    /// Supports adding multiple subscribers as delegates at once. 
    /// A delegate can be added multiple times. This will result in multiple calls to the same delegate.
    /// </summary>
    /// <typeparam name="T">Extended from Signal<T></typeparam>
    public abstract class Signal<T> where T : Signal<T>
    {
        /// <summary>
        /// Container for signal handlers
        /// </summary>
        /// <param name="signal">Signal instance</param>
        public delegate void Hendler(T signal);
        protected static Hendler hendlers;

        /// <summary>
        /// Adds delegates
        /// </summary>
        /// <param name="hendlers">Reference to a method or delegates</param>
        public static void Subscribe(Hendler hendlers)
        {
            Signal<T>.hendlers += hendlers;
        }

        /// <summary>
        /// Removes delegates
        /// </summary>
        /// <param name="hendlers">Reference to a method or delegates</param>
        public static void Unsubscribe(Hendler hendlers)
        {
            Signal<T>.hendlers -= hendlers;
        }

        /// <summary>
        /// Auxiliary method for removing duplicate delegates.
        /// </summary>
        /// <param name="hendler">Reference to a method or delegates</param>
        /// <returns>Unique hendlers</returns>
        protected static Hendler GetUniqueHendlers(Hendler hendler)
        {
            HashSet<int> hashs = new HashSet<int>();

            Hendler[] hendlers = hendler.GetInvocationList() as Hendler[];
            if(hendlers != null)
            {
                for (int i = 0; i < hendlers.Length; i++)
                {
                    var atrs = string.Format("{0}{1}{2}", hendlers[i].Target?.GetHashCode(), hendlers[i].Method.DeclaringType, hendlers[i].Method.GetBaseDefinition());
                    var hash = atrs.GetHashCode();

                    if (hashs.Contains(hash))
                    {
                        hendler -= hendlers[i];
                    }
                    else { hashs.Add(hash); }
                }
            }

            return hendler;
        }

        /// <summary>
        /// Exception generated if no one is subscribed to it at the time of sending the signal.
        /// </summary>
        public class Exception : System.Exception
        {
            public Exception(string message) : base(message)
            { }
        }
    }
}