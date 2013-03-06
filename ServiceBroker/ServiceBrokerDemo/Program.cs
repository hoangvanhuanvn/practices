using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace ServiceBrokerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ServiceBrokerDB"].ConnectionString;
            bool isLoop = true;

            while (isLoop)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Console.WriteLine("Messages processed: " + CountMessages(connection));
                    Console.WriteLine("Press Y/N to continue/exists: ");

                    var input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input) && input.Trim().ToUpper().Equals("N"))
                    {
                        isLoop = false;
                    }

                    if (isLoop)
                    {
                        SendRequestMessage(connection);
                    }

                    connection.Close();
                }
            }
        }

        private static void SendRequestMessage(SqlConnection connection)
        {
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SendRequestMessages";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@body", "Hello, time now is " + DateTime.Now.ToString(CultureInfo.InvariantCulture));
                command.ExecuteNonQuery();
            }
        }

        private static int CountMessages(SqlConnection connection)
        {
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "select count(*) from ProcessedMessages";
                var result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }
    }
}
