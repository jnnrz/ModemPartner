namespace ModemPartner.Core.Utils
{
    /// <summary>
    /// Defines the <see cref="ComPortUtil" />.
    /// A collection of useful methods for better handling of COM ports.
    /// </summary>
    internal static class ComPortUtil
    {
        /// <summary>
        /// Extracts the COM port from the device's name.
        /// </summary>
        /// <param name="name">Device's name<see cref="string"/>.</param>
        /// <returns>The COM port<see cref="string"/>.</returns>
        public static string ExtractComPortFromName(string name)
        {
            int openBracket = name.IndexOf('(');
            int closeBracket = name.IndexOf(')');
            return name.Substring(openBracket + 1, closeBracket - openBracket - 1);
        }
    }
}
