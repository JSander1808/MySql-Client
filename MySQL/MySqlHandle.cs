using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQL {
    public class MySqlHandle {

        public static string user;
        public static string password;
        public static string database ;
        public static string ip;
        public static string connectionString;
        public MySqlHandle(string tempIp, string tempDatabase, string tempUser, string tempPassword) {
            user = tempUser;
            ip= tempIp;
            database = tempDatabase;
            password = tempPassword;
            if (database == null || database == "") {
                connectionString = "Server=" + ip + ";User Id=" + user + ";Password=" + password + ";";
            } else {
                connectionString = "Server=" + ip + "Database="+database+";;User Id=" + user + ";Password=" + password + ";";
            }
            Console.WriteLine("Setup closed...");
        }

        public static MySqlDataReader execute(string SqlCommand) {
            try {
                connectionString = @"Data Source=" + ip + ";Initial Catalog=" + database + "; User ID=" + user + "; Password=" + password;
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand command = new MySqlCommand(SqlCommand, connection);
                return command.ExecuteReader();
            } catch(MySqlException e) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;
                return null;
            }
        }

        public static void executeDirectly(string SqlCommand) {
            try {
                connectionString = @"Data Source=" + ip + ";Initial Catalog=" + database + "; User ID=" + user + "; Password=" + password;
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand command = new MySqlCommand(SqlCommand, connection);
                command.ExecuteNonQuery();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Send data successful to the server...");
                Console.ForegroundColor = ConsoleColor.White;
            } catch(MySqlException e) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
