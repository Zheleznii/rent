using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ProkatTransporta_Server.Tests
{
    [TestClass]
    public class ServerTests
    {
        private const string ServerHost = "127.0.0.1";
        private const int ServerPort = 5001;
        private static int? _stationId = null;

        private string SendCommand(string command)
        {
            using (var client = new TcpClient(ServerHost, ServerPort))
            using (var stream = client.GetStream())
            {
                byte[] data = Encoding.UTF8.GetBytes(command);
                stream.Write(data, 0, data.Length);
                byte[] buffer = new byte[8192];
                int bytes = stream.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytes);
            }
        }

        // ==================== ТЕСТЫ ДЛЯ CLIENT (ORM) ====================

        [TestMethod]
        public void AddClient_ValidData_ReturnsSuccess()
        {
            string uniquePassport = Guid.NewGuid().ToString("N");
            string cmd = $"ADD_CLIENT|Тестовый Клиент|{uniquePassport}|+79991234567|true";
            string response = SendCommand(cmd);
            Assert.IsTrue(response.StartsWith("OK|Client added with Id="));
        }

        [TestMethod]
        public void UpdateClient_ExistingClient_ReturnsSuccess()
        {
            string uniquePassport = Guid.NewGuid().ToString("N");
            string addCmd = $"ADD_CLIENT|До Обновления|{uniquePassport}|+71111111111|false";
            string addResponse = SendCommand(addCmd);
            Assert.IsTrue(addResponse.StartsWith("OK|Client added with Id="));
            int clientId = int.Parse(addResponse.Split('=')[1]);

            string updateCmd = $"UPDATE_CLIENT|{clientId}|После Обновления|{uniquePassport}|+72222222222|true";
            string updateResponse = SendCommand(updateCmd);
            Assert.AreEqual("OK|Client updated", updateResponse);
        }

        [AssemblyInitialize]
        public static void InitializeDatabase(TestContext context)
        {
            using (var db = new AppDbContext())
            {
                if (!db.Stations.Any())
                {
                    db.Stations.Add(new Station
                    {
                        Name = "Тестовая станция",
                        Address = "Адрес",
                        City = "Город"
                    });
                    db.SaveChanges();
                }
                _stationId = db.Stations.First().Id;
            }
        }

        private int GetFirstStationId()
        {
            if (_stationId.HasValue) return _stationId.Value;
            using (var db = new AppDbContext())
            {
                var station = db.Stations.FirstOrDefault();
                if (station == null) throw new Exception("Нет станций в БД");
                return station.Id;
            }
        }

        [TestMethod]
        public void AddVehicleSql_ValidData_ReturnsSuccess()
        {
            int stationId = GetFirstStationId();
            string uniqueModel = $"ТестАвто_{Guid.NewGuid():N}";
            string cmd = $"ADD_VEHICLE_SQL|автомобиль|{uniqueModel}|true|25.50|{stationId}";
            string response = SendCommand(cmd);
            Assert.IsTrue(response.StartsWith("OK|Vehicle added with Id="));
        }

        [TestMethod]
        public void DeleteVehicleSql_ExistingVehicle_ReturnsSuccess()
        {
            int stationId = GetFirstStationId();

            string uniqueModel = $"АвтоУдаление_{Guid.NewGuid():N}";
            string addCmd = $"ADD_VEHICLE_SQL|велосипед|{uniqueModel}|false|10.00|{stationId}";
            string addResponse = SendCommand(addCmd);
            Assert.IsTrue(addResponse.StartsWith("OK|Vehicle added with Id="));
            int vehicleId = int.Parse(addResponse.Split('=')[1]);


            string deleteCmd = $"DELETE_VEHICLE_SQL|{vehicleId}";
            string deleteResponse = SendCommand(deleteCmd);
            string response = SendCommand(deleteCmd);
            Console.WriteLine($"Response: {response}");
            Assert.AreEqual("OK|Vehicle deleted", deleteResponse);
        }

    }
}