using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
namespace ProkatTransporta_Server
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<TariffPlan> TariffPlans { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<FuelExpense> FuelExpenses { get; set; }
        public DbSet<CompanyIncome> CompanyIncomes { get; set; }
        public DbSet<CompanyExpense> CompanyExpenses { get; set; }
        public DbSet<VehicleSwap> VehicleSwaps { get; set; }

        private static string DbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");
        private static string MdfFile = Path.Combine(DbFolder, "Rent.mdf");
        string connectionString = $@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Rent;Integrated Security=True";
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
            }
        }

        public string GetConnectionString() => connectionString;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employees");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("employee_id");
                entity.Property(e => e.Full_Name).HasColumnName("full_name");
                entity.Property(e => e.Role).HasColumnName("role");
                entity.Property(e => e.Station_Id).HasColumnName("station_id");
            });

            // Station
            modelBuilder.Entity<Station>(entity =>
            {
                entity.ToTable("stations");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).HasColumnName("station_id");
                entity.Property(s => s.Name).HasColumnName("name");
                entity.Property(s => s.Address).HasColumnName("address");
                entity.Property(s => s.City).HasColumnName("city");
            });

            // Vehicle
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.ToTable("vehicles");
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Id).HasColumnName("vehicle_id");
                entity.Property(v => v.Type).HasColumnName("type");
                entity.Property(v => v.Model).HasColumnName("model");
                entity.Property(v => v.Fuel_Needed).HasColumnName("fuel_needed");
                entity.Property(v => v.Current_Station_Id).HasColumnName("current_station_id");
                entity.Property(v => v.Is_Available).HasColumnName("is_available");
                entity.Property(v => v.Hourly_Rate).HasColumnName("hourly_rate");
            });

            // Client
            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("clients");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).HasColumnName("client_id");
                entity.Property(c => c.Full_Name).HasColumnName("full_name");
                entity.Property(c => c.Passport_Or_Driver_License).HasColumnName("passport_or_driver_license");
                entity.Property(c => c.Phone).HasColumnName("phone");
                entity.Property(c => c.Registration_Date).HasColumnName("registration_date");
                entity.Property(c => c.Is_Profile_Confirmed).HasColumnName("is_profile_confirmed");
            });

            // TariffPlan
            modelBuilder.Entity<TariffPlan>(entity =>
            {
                entity.ToTable("tariff_plans");
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).HasColumnName("plan_id");
                entity.Property(t => t.Plan_Name).HasColumnName("plan_name");
                entity.Property(t => t.Allowed_Vehicle_Types).HasColumnName("allowed_vehicle_types");
                entity.Property(t => t.Price_Per_Hour).HasColumnName("price_per_hour");
                entity.Property(t => t.Can_Swap_Vehicles).HasColumnName("can_swap_vehicles");
                entity.Property(t => t.Max_Hours_Per_Day).HasColumnName("max_hours_per_day");
            });

            // Rental
            modelBuilder.Entity<Rental>(entity =>
            {
                entity.ToTable("rentals");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id).HasColumnName("rental_id");
                entity.Property(r => r.Client_Id).HasColumnName("client_id");
                entity.Property(r => r.Vehicle_Id).HasColumnName("vehicle_id");
                entity.Property(r => r.Tariff_Plan_Id).HasColumnName("tariff_plan_id");
                entity.Property(r => r.Start_Station_Id).HasColumnName("start_station_id");
                entity.Property(r => r.End_Station_Id).HasColumnName("end_station_id");
                entity.Property(r => r.Start_Time).HasColumnName("start_time");
                entity.Property(r => r.Expected_End_Time).HasColumnName("expected_end_time");
                entity.Property(r => r.Actual_End_Time).HasColumnName("actual_end_time");
                entity.Property(r => r.Total_Cost).HasColumnName("total_cost");
                entity.Property(r => r.Fine_Amount).HasColumnName("fine_amount");
                entity.Property(r => r.Status).HasColumnName("status");
            });

            // Fine
            modelBuilder.Entity<Fine>(entity =>
            {
                entity.ToTable("fines");
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Id).HasColumnName("fine_id");
                entity.Property(f => f.Rental_Id).HasColumnName("rental_id");
                entity.Property(f => f.Amount).HasColumnName("amount");
                entity.Property(f => f.Reason).HasColumnName("reason");
                entity.Property(f => f.Is_Paid).HasColumnName("is_paid");
                entity.Property(f => f.Paid_At).HasColumnName("paid_at");
            });

            // FuelExpense
            modelBuilder.Entity<FuelExpense>(entity =>
            {
                entity.ToTable("fuel_expenses");
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Id).HasColumnName("expense_id");
                entity.Property(f => f.Vehicle_Id).HasColumnName("vehicle_id");
                entity.Property(f => f.Station_Id).HasColumnName("station_id");
                entity.Property(f => f.Fuel_Liters).HasColumnName("fuel_liters");
                entity.Property(f => f.Cost_Per_Liter).HasColumnName("cost_per_liter");
                entity.Property(f => f.Total_Cost).HasColumnName("total_cost");
                entity.Property(f => f.Expense_Date).HasColumnName("expense_date");
                entity.Property(f => f.Technical_Specialist_Id).HasColumnName("technical_specialist_id");
            });

            // CompanyIncome
            modelBuilder.Entity<CompanyIncome>(entity =>
            {
                entity.ToTable("company_income");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).HasColumnName("income_id");
                entity.Property(c => c.Source_Type).HasColumnName("source_type");
                entity.Property(c => c.Source_Id).HasColumnName("source_id");
                entity.Property(c => c.Amount).HasColumnName("amount");
                entity.Property(c => c.Received_At).HasColumnName("received_at");
                entity.Property(c => c.Accountant_Confirmed_By).HasColumnName("accountant_confirmed_by");
            });

            // CompanyExpense
            modelBuilder.Entity<CompanyExpense>(entity =>
            {
                entity.ToTable("company_expenses");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).HasColumnName("expense_id");
                entity.Property(c => c.Category).HasColumnName("category");
                entity.Property(c => c.Amount).HasColumnName("amount");
                entity.Property(c => c.Expense_Date).HasColumnName("expense_date");
                entity.Property(c => c.Description).HasColumnName("description");
                entity.Property(c => c.Accountant_Confirmed_By).HasColumnName("accountant_confirmed_by");
            });

            // VehicleSwap
            modelBuilder.Entity<VehicleSwap>(entity =>
            {
                entity.ToTable("vehicle_swaps");
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Id).HasColumnName("swap_id");
                entity.Property(v => v.Rental_Id).HasColumnName("rental_id");
                entity.Property(v => v.Old_Vehicle_Id).HasColumnName("old_vehicle_id");
                entity.Property(v => v.New_Vehicle_Id).HasColumnName("new_vehicle_id");
                entity.Property(v => v.Swapped_At).HasColumnName("swapped_at");
                entity.Property(v => v.Operator_Station_Id).HasColumnName("operator_station_id");
                entity.Property(v => v.Performed_By).HasColumnName("performed_by");
            });
        }
    }
}
