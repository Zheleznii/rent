using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProkatTransporta
{
    public static class ServerConnection
    {
        private const string ServerIp = "127.0.0.1";
        private const int ServerPort = 5001;
        public static string SendRequestToServer(string message)
        {
            try
            {
                using (TcpClient client = new TcpClient(ServerIp, ServerPort))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] requestData = Encoding.UTF8.GetBytes(message);
                    stream.Write(requestData, 0, requestData.Length);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;

                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, bytesRead);
                        }

                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }
    }
}
