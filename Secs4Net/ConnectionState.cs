namespace Secs4Net {

    /// <summary>
    /// Connection state
    /// </summary>
    public enum ConnectionState {

        Disconnected,
        /// <summary>
        /// connecting
        /// </summary>
        Connecting,

        /// <summary>
        /// connected
        /// </summary>
        Connected,

        /// <summary>
        /// selected
        /// </summary>
        Selected,

        /// <summary>
        /// retry
        /// </summary>
        Retry
    }
}
