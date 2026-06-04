using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace ProkatTransporta_Server.Commands
{
    public interface ICommand
    {
        string Execute(AppDbContext db, string[] parameters);
    }

    public class LoggerDecoratorCommand : ICommand
    {
        private readonly ICommand _inner;
        public LoggerDecoratorCommand(ICommand inner) => _inner = inner;

        public string Execute(AppDbContext db, string[] parameters)
        {
            try
            {
                Console.WriteLine($"Executing {_inner.GetType().Name}");
                return _inner.Execute(db, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"ERROR|{ex.Message}";
            }
        }
    }

    // ====================== CLIENT (ORM) ======================
    public class AddClientCommand : ICommand
    {
        public string Execute(AppDbContext db, string[] parts)
        {
            if (parts.Length < 5) return "ERROR|Не переданы все параметрв";

            var client = new Client
            {
                Full_Name = parts[1],
                Passport_Or_Driver_License = parts[2],
                Phone = parts[3],
                Registration_Date = DateTime.Now,
                Is_Profile_Confirmed = bool.Parse(parts[4])
            };
            db.Clients.Add(client);
            db.SaveChanges();
            return $"OK|Client added with Id={client.Id}";
        }
    }

    public class UpdateClientCommand : ICommand
    {
        public string Execute(AppDbContext db, string[] parts)
        {
            // UPDATE_CLIENT|Id|FullName|Passport|Phone|IsProfileConfirmed
            if (parts.Length < 6) return "ERROR|Not enough parameters";
            int id = int.Parse(parts[1]);
            var client = db.Clients.Find(id);
            if (client == null) return "ERROR|Client not found";

            client.Full_Name = parts[2];
            client.Passport_Or_Driver_License = parts[3];
            client.Phone = parts[4];
            client.Is_Profile_Confirmed = bool.Parse(parts[5]);
            db.SaveChanges();
            return "OK|Client updated";
        }
    }

    public class DeleteClientCommand : ICommand
    {
        public string Execute(AppDbContext db, string[] parts)
        {
            if (parts.Length < 2) return "ERROR|Client Id required";
            int id = int.Parse(parts[1]);
            var client = db.Clients.Find(id);
            if (client == null) return "ERROR|Client not found";
            db.Clients.Remove(client);
            db.SaveChanges();
            return "OK|Client deleted";
        }
    }

    public class SearchClientCommand : ICommand
    {
        public string Execute(AppDbContext db, string[] parts)
        {
            // SEARCH_CLIENT|nameFilter|phoneFilter
            string nameFilter = parts.Length > 1 ? parts[1] : "";
            string phoneFilter = parts.Length > 2 ? parts[2] : "";

            var query = db.Clients.AsQueryable();
            if (!string.IsNullOrEmpty(nameFilter))
                query = query.Where(c => c.Full_Name.Contains(nameFilter));
            if (!string.IsNullOrEmpty(phoneFilter))
                query = query.Where(c => c.Phone.Contains(phoneFilter));

            var results = query.ToList();
            if (!results.Any()) return "ERROR|No clients found";
            return "OK|" + string.Join(";", results.Select(c => $"{c.Id}|{c.Full_Name}|{c.Phone}|{c.Passport_Or_Driver_License}"));
        }
    }

    // ====================== VEHICLE (SQL) ======================
    public class AddVehicleSqlCommand : ICommand
    {
        public string Execute(AppDbContext db, string[] parts)
        {
            try
            {
                if (parts.Length < 6) return "ERROR|Not enough parameters";
                string type = parts[1];
                string model = parts[2];
                bool fuelNeeded = bool.Parse(parts[3]);
                decimal hourlyRate = decimal.Parse(parts[4], CultureInfo.InvariantCulture);
                int stationId = int.Parse(parts[5]);

                string sql = @"INSERT INTO vehicles (type, model, fuel_needed, hourly_rate, current_station_id, is_available)
                           VALUES (@type, @model, @fuel, @rate, @stationId, 1);
                           SELECT SCOPE_IDENTITY();";
                using (var connection = new SqlConnection(db.GetConnectionString()))
                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@model", model);
                    cmd.Parameters.AddWithValue("@fuel", fuelNeeded);
                    cmd.Parameters.AddWithValue("@rate", hourlyRate);
                    cmd.Parameters.AddWithValue("@stationId", stationId);
                    connection.Open();
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());
                    return $"OK|Vehicle added with Id={newId}";
                }
            }
            catch (Exception ex)
            {
                return $"ERROR|{ex.Message}";
            }
        }
    }

    public class UpdateVehicleSqlCommand : ICommand
    {
        public string Execute(AppDbContext db, string[] parts)
        {
            if (parts.Length < 8) return "ERROR|Not enough parameters";
            string sql = @"UPDATE vehicles SET type=@type, model=@model, fuel_needed=@fuel,
                           hourly_rate=@rate, current_station_id=@stationId, is_available=@avail
                           WHERE vehicle_id=@id";
            using (var connection = new SqlConnection(db.GetConnectionString()))
            using (var cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@id", int.Parse(parts[1]));
                cmd.Parameters.AddWithValue("@type", parts[2]);
                cmd.Parameters.AddWithValue("@model", parts[3]);
                cmd.Parameters.AddWithValue("@fuel", bool.Parse(parts[4]));
                cmd.Parameters.AddWithValue("@rate", decimal.Parse(parts[5]));
                cmd.Parameters.AddWithValue("@stationId", int.Parse(parts[6]));
                cmd.Parameters.AddWithValue("@avail", bool.Parse(parts[7]));

                bool wasOpen = (connection.State == System.Data.ConnectionState.Open);
                if (!wasOpen) connection.Open();
                try
                {
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0 ? "OK|Vehicle updated" : "ERROR|Vehicle not found";
                }
                finally
                {
                    if (!wasOpen) connection.Close();
                }
            }
        }
    }

    public class DeleteVehicleSqlCommand : ICommand
    {
        public string Execute(AppDbContext db, string[] parts)
        {
            if (parts.Length < 2) return "ERROR|Vehicle Id required";
            string sql = "DELETE FROM vehicles WHERE vehicle_id=@id";
            using (var connection = new SqlConnection(db.GetConnectionString()))
            using (var cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@id", int.Parse(parts[1]));

                bool wasOpen = (connection.State == System.Data.ConnectionState.Open);
                if (!wasOpen) connection.Open();
                try
                {
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0 ? "OK|Vehicle deleted" : "ERROR|Vehicle not found";
                }
                finally
                {
                    if (!wasOpen) connection.Close();
                }
            }
        }
    }

    public class SearchVehicleSqlCommand : ICommand
    {
        public string Execute(AppDbContext db, string[] parts)
        {
            string modelFilter = parts.Length > 1 ? parts[1] : "";
            string typeFilter = parts.Length > 2 ? parts[2] : "";
            string stationFilter = parts.Length > 3 ? parts[3] : "";
            string sql = "SELECT vehicle_id, type, model, hourly_rate FROM vehicles WHERE 1=1";
            var parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(modelFilter))
            {
                sql += " AND model LIKE @model";
                parameters.Add(new SqlParameter("@model", "%" + modelFilter + "%"));
            }
            if (!string.IsNullOrEmpty(typeFilter))
            {
                sql += " AND type = @type";
                parameters.Add(new SqlParameter("@type", typeFilter));
            }
            if (!string.IsNullOrEmpty(stationFilter) && int.TryParse(stationFilter, out int sid))
            {
                sql += " AND current_station_id = @stationId";
                parameters.Add(new SqlParameter("@stationId", sid));
            }

            using (var connection = new SqlConnection(db.GetConnectionString()))
            using (var cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.AddRange(parameters.ToArray());
                bool wasOpen = (connection.State == System.Data.ConnectionState.Open);
                if (!wasOpen) connection.Open();
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        var results = new List<string>();
                        while (reader.Read())
                            results.Add($"{reader[0]}|{reader[1]}|{reader[2]}|{reader[3]}");
                        return results.Any() ? $"OK|{string.Join(";", results)}" : "ERROR|No vehicles found";
                    }
                }
                finally
                {
                    if (!wasOpen) connection.Close();
                }
            }
        }
    }
}