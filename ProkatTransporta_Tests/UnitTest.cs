using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProkatTransporta_Server;
using ProkatTransporta_Server.Commands;
using System;
using System.Linq;

namespace ProkatTransporta_Tests
{
    [TestClass]
    public class ClientCommandUnitTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [TestMethod]
        public void AddClientCommand_ValidData_AddsClient()
        {
            using (var context = CreateContext())
            {
                var command = new AddClientCommand();
                string[] parts = { "ADD_CLIENT", "Модульный Тест", "MOD123", "+79991112233", "true" };

                var result = command.Execute(context, parts);

                Assert.IsTrue(result.StartsWith("OK|Client added with Id="));
                var client = context.Clients.FirstOrDefault(c => c.Passport_Or_Driver_License == "MOD123");
                Assert.IsNotNull(client);
                Assert.AreEqual("Модульный Тест", client.Full_Name);
                Assert.AreEqual("+79991112233", client.Phone);
                Assert.IsTrue(client.Is_Profile_Confirmed);
            }
        }

        [TestMethod]
        public void UpdateClientCommand_ExistingClient_UpdatesClient()
        {
            using (var context = CreateContext())
            {
                var addCommand = new AddClientCommand();
                addCommand.Execute(context, new[] { "ADD_CLIENT", "Старое Имя", "OLD123", "111", "false" });
                var client = context.Clients.First(c => c.Passport_Or_Driver_License == "OLD123");

                var updateCommand = new UpdateClientCommand();
                string[] parts = { "UPDATE_CLIENT", client.Id.ToString(), "Новое Имя", "OLD123", "222", "true" };

                var result = updateCommand.Execute(context, parts);

                Assert.AreEqual("OK|Client updated", result);
                Assert.AreEqual("Новое Имя", client.Full_Name);
                Assert.AreEqual("222", client.Phone);
                Assert.IsTrue(client.Is_Profile_Confirmed);
            }
        }
    }
}