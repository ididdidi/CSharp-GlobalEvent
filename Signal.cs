using System.Collections.Generic;

namespace ru.mofrison.global_signals
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
        /// <param name="hendlers">Reference to a method or delegates</param>
        /// <returns>Unique hendlers</returns>
        protected static Hendler GetUniqueHendlers(Hendler hendlers)
        {
            HashSet<int> hashs = new HashSet<int>();

            foreach (Hendler hendler in hendlers.GetInvocationList())
            {
                var hash = (hendler.Target?.GetHashCode() + "" +
                            hendler.Method.DeclaringType +
                            hendler.Method.GetBaseDefinition()).GetHashCode();

                if (hashs.Contains(hash))
                {
                    hendlers -= hendler;
                }
                else { hashs.Add(hash); }
            }

            return hendlers;
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