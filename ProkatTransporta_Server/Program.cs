using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProkatTransporta_Server
{
    class Program
    {

        static void Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5001);
                server.Start();
                Console.WriteLine("Сервер запущен на 127.0.0.1:5001");

                while (true)
                {
                    Console.Write("Ждём подключения... ");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Подключено!");
                    ClientHandler.ProcessClient(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Критическая ошибка сервера: {e}");
            }
            finally
            {
                server?.Stop();
            }

            Console.WriteLine("Нажмите Enter для выхода...");
            Console.Read();
        }
    }
}
