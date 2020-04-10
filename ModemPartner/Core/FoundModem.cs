namespace ModemPartner.Core
{
    public class FoundModem
    {
        public string Port { get; }
        public string Modem { get; }

        public FoundModem() { }

        public FoundModem(string port, string modem)
        {
            Port = port;
            Modem = modem;
        }

        public FoundModem(string port)
        {
            Port = port;
        }
    }
}
