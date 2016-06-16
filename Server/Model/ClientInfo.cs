using System.Net.Sockets;

namespace Server.Model
{
    public class ClientInfo
    {
        public TcpClient socket { get; set; }

        public string ip { get; set; }

        public ClientInfo()
        {
            socket = null;
            ip = "";
        }

    }
}
