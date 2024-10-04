using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using ASPTestRustami.Models;

namespace ASPTestRustami
{
    public class DatabaseAccess
    {
        public const string CONNECTION_STRING = "Data Source=(localdb)\\MSSQLLocalDB;database=Rustami;Integrated Security=False;Persist Security Info=False;Pooling=False;" +
                "Encrypt=False;";

        public List<Employee> ReadEmployess()
        {
            List<Employee> empployeeList = new List<Employee>();
            string query = "SELECT * FROM Employees";
            using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Employee employee = new Employee();
                                employee.Id = Convert.ToInt32(reader["Id"]);
                                employee.PayrollNumber = reader[nameof(Employee.PayrollNumber)].ToString();
                                employee.Forenames = reader[nameof(Employee.Forenames)].ToString();
                                employee.Surname = reader[nameof(Employee.Surname)].ToString();
                                employee.DateOfBirth = Convert.ToDateTime(reader[nameof(Employee.DateOfBirth)]);
                                employee.Telephone = reader[nameof(Employee.Telephone)].ToString();
                                employee.Mobile = reader[nameof(Employee.Mobile)].ToString();
                                employee.Adress = reader[nameof(Employee.Adress)].ToString();
                                employee.Adress2 = reader[nameof(Employee.Adress2)].ToString();
                                employee.Postcode = reader[nameof(Employee.Postcode)].ToString();
                                employee.Email = reader[nameof(Employee.Email)].ToString();
                                employee.StartDate = Convert.ToDateTime(reader[nameof(Employee.StartDate)].ToString());
                                empployeeList.Add(employee);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        Debug.WriteLine("SQL Error: " + ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return empployeeList;
        }

        // Adds employee list to actual database
        public int AddEmployees(List<Employee> employees)
        {
            int addedCount = 0;

            string query = $"INSERT INTO Employees ({nameof(Employee.PayrollNumber)}, {nameof(Employee.Forenames)}, {nameof(Employee.Surname)}, " +
                    $"{nameof(Employee.DateOfBirth)}, {nameof(Employee.Telephone)}, {nameof(Employee.Mobile)}, {nameof(Employee.Adress)}," +
                    $"{nameof(Employee.Adress2)}, {nameof(Employee.Postcode)}, {nameof(Employee.Email)}, {nameof(Employee.StartDate)})" +
                    $"VALUES (@value1, @value2, @value3, @value4, @value5, @value6, @value7, @value8, @value9, @value10, @value11)";

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                for (int i = 0; i < employees.Count; i++)
                {
                    Employee employee = employees[i];
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@value1", employee.PayrollNumber);
                        command.Parameters.AddWithValue("@value2", employee.Forenames);
                        command.Parameters.AddWithValue("@value3", employee.Surname);
                        command.Parameters.AddWithValue("@value4", employee.DateOfBirth);
                        command.Parameters.AddWithValue("@value5", employee.Telephone);
                        command.Parameters.AddWithValue("@value6", employee.Mobile);
                        command.Parameters.AddWithValue("@value7", employee.Adress);
                        command.Parameters.AddWithValue("@value8", employee.Adress2);
                        command.Parameters.AddWithValue("@value9", employee.Postcode);
                        command.Parameters.AddWithValue("@value10", employee.Email);
                        command.Parameters.AddWithValue("@value11", employee.StartDate);
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            Debug.WriteLine("SQL Error: " + ex.Message);
                        }
                    }
                    addedCount += 1;
                }
                connection.Close();
            }
            return addedCount;
        }

        public void UpdateEmployee(Employee employee)
        {
            string query = $"UPDATE Employees SET {nameof(Employee.PayrollNumber)} = @PayrollNumber, {nameof(Employee.Forenames)} = @Forenames, {nameof(Employee.Surname)} = @Surname," +
                $"{nameof(Employee.DateOfBirth)} = @DateOfBirth, {nameof(Employee.Telephone)} = @Telephone, {nameof(Employee.Mobile)} = @Mobile, {nameof(Employee.Adress)} = @Adress," +
                $"{nameof(Employee.Adress2)} = @Adress2, {nameof(Employee.Postcode)} = @Postcode, {nameof(Employee.Email)} = @Email, {nameof(Employee.StartDate)} = @StartDate " +
                $"WHERE Id = @Id";
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PayrollNumber", employee.PayrollNumber);
                    command.Parameters.AddWithValue("@Forenames", employee.Forenames);
                    command.Parameters.AddWithValue("@Surname", employee.Surname);
                    command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                    command.Parameters.AddWithValue("@Telephone", employee.Telephone);
                    command.Parameters.AddWithValue("@Mobile", employee.Mobile);
                    command.Parameters.AddWithValue("@Adress", employee.Adress);
                    command.Parameters.AddWithValue("@Adress2", employee.Adress2);
                    command.Parameters.AddWithValue("@Postcode", employee.Postcode);
                    command.Parameters.AddWithValue("@Email", employee.Email);
                    command.Parameters.AddWithValue("@StartDate", employee.StartDate);
                    command.Parameters.AddWithValue("@Id", employee.Id);
                    
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Debug.WriteLine("SQL Error: " + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Adds to database Employees table from .csv file
        /// </summary>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public int AddCSVForEmployees(byte[] fileContent)
        {
            List<Employee> employeeList = new List<Employee>();
            try
            {
                // Read csv file, first line is header that contains column names
                using (MemoryStream memoryStream = new MemoryStream(fileContent))
                {
                    using (StreamReader reader = new StreamReader(memoryStream))
                    {
                        bool isColumnNameLine = true;
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (isColumnNameLine)
                            {
                                isColumnNameLine = false;
                                continue;
                            }

                            string[] values = line.Split(',');
                            Employee employee = new Employee();

                            employee.PayrollNumber = values[0];
                            employee.Forenames = values[1];
                            employee.Surname = values[2];
                            employee.DateOfBirth = Convert.ToDateTime(values[3]);
                            employee.Telephone = values[4];
                            employee.Mobile = values[5];
                            employee.Adress = values[6];
                            employee.Adress2 = values[7];
                            employee.Postcode = values[8];
                            employee.Email = values[9];
                            employee.StartDate = Convert.ToDateTime(values[10]);
                            employeeList.Add(employee);
                        }
                    }
                }
                return AddEmployees(employeeList);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}