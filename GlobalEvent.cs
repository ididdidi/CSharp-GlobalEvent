using System.Collections.Generic;

namespace ru.mofrison.GlobalEvent
{
    /// <summary>
    /// A base class that is used to implement the mechanism for subscribing to global events
    /// Supports adding multiple subscribers as delegates at once
    /// </summary>
    /// <typeparam name="T">Extended from <see cref="GlobalEvent<T>"/></typeparam>
    public abstract class GlobalEvent<T> where T : GlobalEvent<T>
    {
        /// <summary>
        /// Container for global event handlers
        /// </summary>
        /// <param name="globalEvent">Signal instance</param>
        public delegate void Hendler(T globalEvent);
        private static Hendler hendlers;
        private static HashSet<int> hashs = new HashSet<int>();

        /// <summary>
        /// Checks if this handler is subscribed to this event
        /// </summary>
        /// <param name="hendler">Event handler</param>
        /// <returns>Returns <see cref="true"/> if the handler was already subscribed to this event</returns>
        private static bool Contains(Hendler hendler)
        {
            return hashs.Contains(hendler.GetHashCode());
        }

        /// <summary>
        /// Adds handlers
        /// </summary>
        /// <param name="hendler">Method reference or delegates with one parameter of type <see cref="T"/></param>
        public static void Subscribe(Hendler hendler)
        {
            for (int i = 0; i < hendler.GetInvocationList().Length; i++)
            {
                AddHendler((Hendler)(hendler.GetInvocationList()[i]));
            }
        }

        /// <summary>
        /// Removes handlers
        /// </summary>
        /// <param name="hendler">Method reference or delegates with one parameter of type <see cref="T"/></param>
        public static void Unsubscribe(Hendler hendler)
        {
            for (int i = 0; i < hendler.GetInvocationList().Length; i++)
            {
                RemoveHendler((Hendler)(hendler.GetInvocationList()[i]));
            }
        }

        /// <summary>
        /// The method required to send the global event
        /// </summary>
        /// <param name="globalEvent">Typed event instance</param>
        protected static void Handle(T globalEvent)
        {
            try
            {
                hendlers.Invoke(globalEvent);
            }
            catch (System.NullReferenceException e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Adds a method if it hasn't already been added
        /// </summary>
        /// <param name="hendler">Reference to a method</param>
        private static void AddHendler(Hendler hendler)
        {
            var hash = hendler.GetHashCode();
            if (!hashs.Contains(hash))
            {
                hendlers += hendler;
                hashs.Add(hash);
            }
            else throw new Exception($"The {hendler.Method} has already been added in {typeof(T)} hendlers");
        }

        /// <summary>
        /// Remove a method if it has been added
        /// </summary>
        /// <param name="hendler">Reference to a method</param>
        private static void RemoveHendler(Hendler hendler)
        {
            var hash = hendler.GetHashCode();
            if (hashs.Contains(hash))
            {
                hendlers -= hendler;
                hashs.Remove(hash);
            }
            else throw new Exception($"The {hendler.Method} has not been added in {typeof(T)} hendlers");
        }

        /// <summary>
        /// Exception generated if no one is subscribed to it at the time of sending the global event.
        /// </summary>
        public class Exception : System.Exception
        {
            public Exception(string message) : base(message)
            { }
        }
    }
}