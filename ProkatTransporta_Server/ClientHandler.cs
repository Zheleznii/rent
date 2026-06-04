using ProkatTransporta_Server.Commands;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ProkatTransporta_Server
{
    public static class ClientHandler
    {
        private static readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>
        {
            // ORM команды
            ["ADD_CLIENT"] = new LoggerDecoratorCommand(new AddClientCommand()),
            ["UPDATE_CLIENT"] = new LoggerDecoratorCommand(new UpdateClientCommand()),
            ["DELETE_CLIENT"] = new LoggerDecoratorCommand(new DeleteClientCommand()),
            ["SEARCH_CLIENT"] = new LoggerDecoratorCommand(new SearchClientCommand()),

            // SQL команды
            ["ADD_VEHICLE_SQL"] = new LoggerDecoratorCommand(new AddVehicleSqlCommand()),
            ["UPDATE_VEHICLE_SQL"] = new LoggerDecoratorCommand(new UpdateVehicleSqlCommand()),
            ["DELETE_VEHICLE_SQL"] = new LoggerDecoratorCommand(new DeleteVehicleSqlCommand()),
            ["SEARCH_VEHICLE_SQL"] = new LoggerDecoratorCommand(new SearchVehicleSqlCommand()),
        };

        public static void ProcessClient(TcpClient client)
        {
            try
            {
                using (var stream = client.GetStream())
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) return;

                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string[] parts = data.Split('|');
                    string commandKey = parts[0].ToUpper();

                    string response;
                    using (var db = new AppDbContext())
                    {
                        if (_commands.TryGetValue(commandKey, out ICommand command))
                            response = command.Execute(db, parts);
                        else
                            response = "ERROR|Unknown command";
                    }

                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseBytes, 0, responseBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ClientHandler error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}