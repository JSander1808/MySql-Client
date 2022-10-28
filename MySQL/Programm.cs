using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;

namespace MySQL {
    public class Programm {

        public static void Main(string[] args) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ____            _            ____   _   _                  _   \r\n / ___|    __ _  | |          / ___| | | (_)   ___   _ __   | |_ \r\n \\___ \\   / _` | | |  _____  | |     | | | |  / _ \\ | '_ \\  | __|\r\n  ___) | | (_| | | | |_____| | |___  | | | | |  __/ | | | | | |_ \r\n |____/   \\__, | |_|          \\____| |_| |_|  \\___| |_| |_|  \\__|\r\n             |_|                                                 ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("----------------------------------------------------[SETUP]-------------------------------------------------------");
            string user, password, ip, database;
            Console.WriteLine("Enter the User:");
            user = Console.ReadLine();
            Console.WriteLine("Enter the password:");
            password = Console.ReadLine();
            Console.WriteLine("Enter the Server ip-address:");
            ip = Console.ReadLine();
            Console.WriteLine("Enter the database:");
            database = Console.ReadLine();
            MySqlHandle mySql = new MySqlHandle(ip, database, user, password);
            Console.WriteLine("----------------------------------------------------[End-SETUP]---------------------------------------------------");
            Console.WriteLine("\n\n\n\n");

            Console.WriteLine("use 'help' to show commands...");
            while (true) {
                Console.Write("Server/MySql/"+MySqlHandle.database+">");
                string[] command = Console.ReadLine().Split(' ');
                if (command[0] == "help") {
                    Console.WriteLine("\n");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" __    __   _______  __      .______   \r\n|  |  |  | |   ____||  |     |   _  \\  \r\n|  |__|  | |  |__   |  |     |  |_)  | \r\n|   __   | |   __|  |  |     |   ___/  \r\n|  |  |  | |  |____ |  `----.|  |      \r\n|__|  |__| |_______||_______|| _|      ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("----------------------------------------------------[HELP]-------------------------------------------------------");
                    Console.WriteLine("help  -  show the list of commands");
                    Console.WriteLine("direct <SqlCommand>  -  send a directly command to the server");
                    Console.WriteLine("request <number of columns> <SqlCommand>  -  send a request to the server and shows in the Console");
                    Console.WriteLine("switchip <ip>  -  switch the ip");
                    Console.WriteLine("switchdb <database>  -  switch the database");
                    Console.WriteLine("switchacc <user> <password>  -  switch the account");
                    Console.WriteLine("info  -  list all infomations of the session");
                    Console.WriteLine("list <table>  -  list all entries in the table");
                    Console.WriteLine("add <table>  -  add a row in the table");
                    Console.WriteLine("new  -  create new DataBase/Tables");
                    Console.WriteLine("showdb  -  show all databases");
                    Console.WriteLine("showtb  -  show tablles");
                    Console.WriteLine("exit  -  Closed the programm");
                    Console.WriteLine("open - öffnet den phpmyadmin Dienst");
                    Console.WriteLine("----------------------------------------------------[End-HELP]-------------------------------------------------------");
                    Console.WriteLine("\n");
                } else if (command[0] == "direct") {
                    string commandString = "";
                    for (int i = 1; i < command.Length; i++) {
                        commandString = commandString + command[i] + " ";
                    }
                    MySqlHandle.executeDirectly(commandString);
                } else if (command[0] == "request") {
                    string commandString = "";
                    for (int i = 1; i < command.Length; i++) {
                        commandString = commandString + command[i] + " ";
                    }
                    MySqlDataReader reader = MySqlHandle.execute(commandString);
                    if (reader != null) {
                        while (reader.Read()) {
                            int temp = 0;
                            while (temp < reader.FieldCount) {
                                Console.Write(reader.GetString(temp) + " ");
                                temp++;
                            }
                            Console.Write("\n");
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Received successful all data...");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                } else if (command[0] == "add") {
                    MySqlDataReader reader = MySqlHandle.execute("SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + command[1] +"'");
                    List<string> columnName = new List<string>();
                    List<string> dataType = new List<string>();
                    if (reader != null) {
                        int temp = 0;
                        while (reader.Read()) {
                            columnName.Add(reader.GetString(0));
                            dataType.Add(reader.GetString(1));
                            temp++;
                        }
                    }
                    List<string> values = new List<string>();
                    for(int i = 1; i < columnName.Count; i++) {
                        Console.WriteLine("Enter value for " + columnName[i]+"  (" + dataType[i] +" required)");
                        if (dataType[i] == "date") {
                            Console.WriteLine("(Example: 12 Juni 2008)");
                        }
                        values.Add(Console.ReadLine());
                    }
                    StringBuilder sb = new StringBuilder();
                    sb.Append("INSERT INTO `" + command[1] + "` (`" + columnName[1] + "`");
                    for(int i = 2; i < columnName.Count; i++) {
                        sb.Append(",`" + columnName[i] + "`");
                    }

                    sb.Append(") VALUES (");

                    if (dataType[1] == "varchar") {
                        sb.Append("'" + values[0] + "'");
                    } else if (dataType[1] == "int") {
                        sb.Append("" + values[0]);
                    } else if (dataType[1] == "text") {
                        sb.Append("'" + values[0] + "'");
                    } else if (dataType[1] == "date") {
                        var cultureInfo = new CultureInfo("de-DE");
                        var dateTime = DateTime.Parse(values[0], cultureInfo);
                        sb.Append("" + values[0]);
                    }
                    for(int i = 1; i < values.Count; i++) {
                        if (dataType[i+1] == "varchar") {
                            sb.Append(",'" + values[i] + "'");
                        } else if (dataType[i+1] == "int") {
                            sb.Append("," + values[i]);
                        } else if (dataType[i+1] == "text") {
                            sb.Append(",'" + values[i] + "'");
                        } else if (dataType[i+1] == "date") {
                            var cultureInfo = new CultureInfo("de-DE");
                            var dateTime = DateTime.Parse(values[i], cultureInfo);
                            sb.Append("," + values[i]);
                        }
                    }
                    sb.Append(");");

                    MySqlHandle.executeDirectly(sb.ToString());
                } else if (command[0] == "switchip") {
                    if (command[1] != null && command[1] != "") {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Ip successfully changed...  (" + MySqlHandle.ip + " => " + command[1] + ")");
                        Console.ForegroundColor = ConsoleColor.White;
                        MySqlHandle.ip = command[1];
                    } else {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("This command is not right. switchip <ip>");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                } else if (command[0] == "switchdb") {
                    try {
                        if (command[1] != null && command[1] != "") {
                            MySqlHandle.database = command[1];
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Database successfully changed...");
                            Console.ForegroundColor = ConsoleColor.White;
                        } else {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("This command is not right. switchdb <database>");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    } catch (Exception e) {
                        string commandString = "";
                        for (int i = 0; i < command.Length; i++) {
                            commandString = commandString + command[i] + " ";
                        }
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("The command '" + commandString + "' is not right. Please Check spelling and try again.");
                        Console.WriteLine(e.Message);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                } else if (command[0] == "info") {
                    Console.WriteLine("\n");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(" __  .__   __.  _______   ______   \r\n|  | |  \\ |  | |   ____| /  __  \\  \r\n|  | |   \\|  | |  |__   |  |  |  | \r\n|  | |  . `  | |   __|  |  |  |  | \r\n|  | |  |\\   | |  |     |  `--'  | \r\n|__| |__| \\__| |__|      \\______/  \r\n                                   ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("----------------------------------------------------[INFO]-------------------------------------------------------");
                    Console.WriteLine("Accountinfomations:");
                    Console.WriteLine("     :User => " + MySqlHandle.user);
                    Console.WriteLine("     :Password => " + MySqlHandle.password);
                    Console.WriteLine("\nServerinformations:");
                    Console.WriteLine("     :Server-Ip => " + MySqlHandle.ip);
                    Console.WriteLine("     :DataBase => " + MySqlHandle.database);
                    Console.WriteLine("----------------------------------------------------[End-INFO]-------------------------------------------------------");
                    Console.WriteLine("\n");
                } else if (command[0] == "new") {
                    Console.WriteLine("\n");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  ______ .______       _______      ___      .___________.  ______   .______      \r\n /      ||   _  \\     |   ____|    /   \\     |           | /  __  \\  |   _  \\     \r\n|  ,----'|  |_)  |    |  |__      /  ^  \\    `---|  |----`|  |  |  | |  |_)  |    \r\n|  |     |      /     |   __|    /  /_\\  \\       |  |     |  |  |  | |      /     \r\n|  `----.|  |\\  \\----.|  |____  /  _____  \\      |  |     |  `--'  | |  |\\  \\----.\r\n \\______|| _| `._____||_______|/__/     \\__\\     |__|      \\______/  | _| `._____|\r\n                                                                                  ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("----------------------------------------------------[New]-------------------------------------------------------");
                    Console.WriteLine("[0] : Create new DataBase");
                    Console.WriteLine("[1] : Create new Tables");
                    Console.WriteLine("[2] : Exit");
                    Console.WriteLine();
                    string mode = Console.ReadLine();
                    if (mode == "0") {
                        Console.WriteLine("Enter the name for the DataBase. Allowed symbols [0-9,a-z,A-z]");
                        string databaseName = Console.ReadLine();
                        string databaseNameFinal = Regex.Replace(databaseName, @"[^0-9a-zA-Z]", string.Empty);
                        MySqlHandle.executeDirectly("CREATE DATABASE " + databaseNameFinal);
                        Console.WriteLine("Will you change into the new DataBase? [Y/n]");
                        string temp = Console.ReadLine();
                        if (temp == "y" || temp == "Y") {
                            MySqlHandle.database = databaseNameFinal;
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("DataBase " + databaseName + " successful added...");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("----------------------------------------------------[End-New]-------------------------------------------------------");
                        Console.WriteLine();
                    } else if (mode == "1") {
                        Console.WriteLine("Enter a Name for the Table");
                        string tableName = Console.ReadLine();
                        Console.WriteLine("Enter the amount of columns:");
                        int columns = int.Parse(Console.ReadLine());
                        string[] columnsConfiguration = new string[columns];
                        if (columns != 0) {
                            for (int i = 0; i < columns; i++) {
                                Console.WriteLine("Configuration for column " + i);
                                Console.WriteLine("Configurationformat: <name> <typ> <length>");
                                columnsConfiguration[i] = Console.ReadLine();
                            }
                        }
                        StringBuilder sb = new StringBuilder();
                        sb.Append("CREATE TABLE ").Append(tableName).Append("(id int NOT NULL AUTO_INCREMENT,");
                        for (int i = 0; i < columnsConfiguration.Length; i++) {
                            string[] temp = columnsConfiguration[i].Split(' ');
                            sb.Append(temp[0] + " ").Append(temp[1] + " ").Append("(" + temp[2] + "),");
                        }
                        sb.Append("PRIMARY KEY (id));");
                        MySqlHandle.executeDirectly(sb.ToString());
                    } else if (mode == "2") {
                        Console.WriteLine("----------------------------------------------------[End-New]-------------------------------------------------------");
                        Console.WriteLine();
                    }
                } else if (command[0] == "showdb") {
                    MySqlDataReader reader = MySqlHandle.execute("SHOW DATABASES");
                    if (reader != null) {
                        while (reader.Read()) {
                            int temp = 0;
                            while (temp < reader.FieldCount) {
                                Console.Write(reader.GetString(temp) + " ");
                                temp++;
                            }
                            Console.Write("\n");
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Received successful all data...");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                } else if (command[0] == "showtb") {
                    MySqlDataReader reader = MySqlHandle.execute("SHOW TABLES FROM " + MySqlHandle.database + ";");
                    if (reader != null) {
                        while (reader.Read()) {
                            int temp = 0;
                            while (temp < reader.FieldCount) {
                                Console.Write(reader.GetString(temp) + " ");
                                temp++;
                            }
                            Console.Write("\n");
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Received successful all data...");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                } else if (command[0] == "list") {
                    MySqlDataReader reader = MySqlHandle.execute("SELECT * FROM " + command[1] + ";");
                    if (reader != null) {
                        while (reader.Read()) {
                            int temp = 0;
                            while (temp < reader.FieldCount) {
                                Console.Write(reader.GetString(temp) + " ");
                                temp++;
                            }
                            Console.Write("\n");
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Received successful all data...");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                } else if (command[0]=="open") {
                    System.Diagnostics.Process.Start("cmd", "/c " + "start http://"+MySqlHandle.ip+"/phpmyadmin");
                } else if (command[0] == "switchacc") {
                    if (command.Length >= 2 && command[1] != null && command[2] != null && command[1] != "" && command[2] != "") {
                        MySqlHandle.user = command[1];
                        MySqlHandle.password = command[2];
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("User and password successfully changed...");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                } else if (command[0] == "exit") {
                    Environment.Exit(0);
                } else {
                    string commandString = "";
                    for (int i = 0; i < command.Length; i++) {
                        commandString = commandString + command[i] + " ";
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The command '" + commandString + "' is not right. Please Check spelling and try again.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

        }
    }
}
