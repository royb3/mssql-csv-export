/*
 * MSSQL CSV export tool
 * Created by Roy Buitenhuis, April 2014
 */

using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace Database_export_tool
{
    class Program
    {
        static String store = "C:\\exporter\\test\\";
        static void Main(string[] args)
        {
            //Building the connection string by asking the user to type in data.
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
            Console.Write("Server/Datasource: ");
            String server = Console.ReadLine();           
            sqlConnectionStringBuilder.DataSource = server;
            Console.Write("UserID: ");
            String userId = Console.ReadLine();
            sqlConnectionStringBuilder.UserID = userId;
            Console.Write("Password: ");
            String password = Console.ReadLine();
            sqlConnectionStringBuilder.Password = password;
            Console.Write("Database: ");
            String database = Console.ReadLine();
            sqlConnectionStringBuilder.InitialCatalog = database;
            Console.Write("Path: ");
            String path = Console.ReadLine();
            store = path;
            
            //Creating folders for storing csv files;
            if(!Directory.Exists(store))
            {
                Directory.CreateDirectory(store);
            }
            //open database connection and list all tables
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionStringBuilder.ToString());
            sqlConnection.Open();
            List<String> tables = new List<String>();
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
            SqlDataReader reader = command.ExecuteReader();
            
            while(reader.Read())
            {
                tables.Add((String)reader[0]);
            }
            reader.Close();
            foreach( String table in tables)
            {
                //create csv files for all tables.
                FileStream tableFileStream = new FileStream(store + table + ".csv", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter tableFileStreamWriter = new StreamWriter(tableFileStream);
                //list all column names for the table, and write it into the csv file.
                SqlCommand tableHeadersCommand = sqlConnection.CreateCommand();
                tableHeadersCommand.CommandText = String.Format("select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='{0}'", table);
                SqlDataReader tableHeadersReader = tableHeadersCommand.ExecuteReader();
                bool firstTableHeaderValue = true;
                while(tableHeadersReader.Read())
                {
                    if (!firstTableHeaderValue)
                        tableFileStreamWriter.Write(',');
                    tableFileStreamWriter.Write(tableHeadersReader[0]);
                    firstTableHeaderValue = false;
                }
                tableFileStreamWriter.Write('\n');
                tableHeadersReader.Close();
                //aqquire all records from table, and write them into the csv file.
                SqlCommand tableCommand = sqlConnection.CreateCommand();
                tableCommand.CommandText = String.Format("SELECT * FROM {0}", table);
                SqlDataReader tableReader = tableCommand.ExecuteReader();
                while(tableReader.Read())
                {
                    for(int i = 0; i < tableReader.FieldCount; i++)
                    {
                        tableFileStreamWriter.Write(tableReader[i].ToString() + (i == tableReader.FieldCount - 1 ? "\n" : ","));
                    }
                }
                //close the file handle, and the table reader for the records.
                tableFileStreamWriter.Close();
                tableReader.Close();
            }
            Console.WriteLine("export successfull, press any key to continue.");
            Console.Read();
        }
    }
}
