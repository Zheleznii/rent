using System;

namespace ProkatTransporta
{
    public class Employee
    {
        public int Id { get; set; }
        public string Full_Name { get; set; }
        public string Role { get; set; }
        public int? Station_Id { get; set; }
    }

    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
    }

    public class Vehicle
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public bool Fuel_Needed { get; set; }
        public int? Current_Station_Id { get; set; }
        public bool Is_Available { get; set; }
        public decimal Hourly_Rate { get; set; }
    }

    public class Client
    {
        public int Id { get; set; }
        public string Full_Name { get; set; }
        public string Passport_Or_Driver_License { get; set; }
        public string Phone { get; set; }
        public DateTime Registration_Date { get; set; }
        public bool Is_Profile_Confirmed { get; set; }
    }

    public class TariffPlan
    {
        public int Id { get; set; }
        public string Plan_Name { get; set; }
        public string Allowed_Vehicle_Types { get; set; }
        public decimal? Price_Per_Hour { get; set; }
        public bool Can_Swap_Vehicles { get; set; }
        public int? Max_Hours_Per_Day { get; set; }
    }

    public class Rental
    {
        public int Id { get; set; }
        public int Client_Id { get; set; }
        public int Vehicle_Id { get; set; }
        public int Tariff_Plan_Id { get; set; }
        public int Start_Station_Id { get; set; }
        public int? End_Station_Id { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime Expected_End_Time { get; set; }
        public DateTime? Actual_End_Time { get; set; }
        public decimal? Total_Cost { get; set; }
        public decimal Fine_Amount { get; set; }
        public string Status { get; set; }
    }

    public class Fine
    {
        public int Id { get; set; }
        public int Rental_Id { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public bool Is_Paid { get; set; }
        public DateTime? Paid_At { get; set; }
    }

    public class FuelExpense
    {
        public int Id { get; set; }
        public int Vehicle_Id { get; set; }
        public int Station_Id { get; set; }
        public decimal Fuel_Liters { get; set; }
        public decimal Cost_Per_Liter { get; set; }
        public decimal Total_Cost { get; set; }
        public DateTime Expense_Date { get; set; }
        public int Technical_Specialist_Id { get; set; }
    }

    public class CompanyIncome
    {
        public int Id { get; set; }
        public string Source_Type { get; set; }
        public int Source_Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Received_At { get; set; }
        public int Accountant_Confirmed_By { get; set; }
    }

    public class CompanyExpense
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Expense_Date { get; set; }
        public string Description { get; set; }
        public int Accountant_Confirmed_By { get; set; }
    }

    public class VehicleSwap
    {
        public int Id { get; set; }
        public int Rental_Id { get; set; }
        public int Old_Vehicle_Id { get; set; }
        public int New_Vehicle_Id { get; set; }
        public DateTime Swapped_At { get; set; }
        public int Operator_Station_Id { get; set; }
        public int Performed_By { get; set; }
    }
}