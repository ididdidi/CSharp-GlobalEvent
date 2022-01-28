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
        private static HashSet<int> hashs = new HashSet<int>();

        /// <summary>
        /// Generates a 32 bit hash code needed to compare handlers
        /// </summary>
        /// <param name="hendler">Reference to a method or delegates</param>
        public static int GetHashCode(Hendler hendler)
        {
            return string.Format("{0}{1}{2}", hendler.Target?.GetHashCode(),
                hendler.Method.DeclaringType, hendler.Method.GetBaseDefinition()).GetHashCode();
        }

        /// <summary>
        /// Adds delegates
        /// </summary>
        /// <param name="hendler">Reference to a method or delegates</param>
        public static void Subscribe(Hendler hendler)
        {
            for (int i = 0; i < hendler.GetInvocationList().Length; i++)
            {
                AddHendler((Hendler)(hendler.GetInvocationList()[i]));
            }
        }

        /// <summary>
        /// Removes delegates
        /// </summary>
        /// <param name="hendler">Reference to a method or delegates</param>
        public static void Unsubscribe(Hendler hendler)
        {
            for (int i = 0; i < hendler.GetInvocationList().Length; i++)
            {
                RemoveHendler((Hendler)(hendler.GetInvocationList()[i]));
            }
        }

        /// <summary>
        /// Adds a method if it hasn't already been added
        /// </summary>
        /// <param name="hendler">Reference to a method</param>
        /// <returns>Unique hendlers</returns>
        protected static void AddHendler(Hendler hendler)
        {
            var hash = GetHashCode(hendler);
            if (!hashs.Contains(hash))
            {
                hendlers += hendler;
                hashs.Add(hash);
            }
            else throw new Exception(string.Format("The {0} has already been added in {1} hendlers", hendler.Method, typeof(T)));
        }

        /// <summary>
        /// Remove a method if it has been added
        /// </summary>
        /// <param name="hendler">Reference to a method</param>
        /// <returns>Unique hendlers</returns>
        protected static void RemoveHendler(Hendler hendler)
        {
            var hash = GetHashCode(hendler);
            if (hashs.Contains(hash))
            {
                hendlers -= hendler;
                hashs.Remove(hash);
            }
            else throw new Exception(string.Format("The {0} has not been added in {1} hendlers", hendler.Method, typeof(T)));
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