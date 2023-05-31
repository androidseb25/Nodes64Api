using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using Swashbuckle.AspNetCore.Annotations;

namespace Nodes64Api.Models.Database
{
    public abstract class DBBase
    {
        private ConnectionStrings? _ConnString = null;
        private MySqlConnection? publicConnection = null;
        private LogWriter? logWriter = null;

        //[JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        [SwaggerSchema(ReadOnly = true)]
        [JsonIgnore]
        public ConnectionStrings? ConnString
        {
            get
            {
                return _ConnString;
            }
            set
            {
                _ConnString = value;
            }
        }

        public DBBase(ConnectionStrings connString)
        {
            _ConnString = connString;
        }

        public DBBase() { }

        public async Task<dynamic> SelectFromSql(string sql)
        {
            logWriter = new LogWriter("DBBase:SelectFromSql");
            string? connectionString = _ConnString?.DefaultConnection;
            logWriter?.WriteLine($"{connectionString}");
            logWriter?.WriteLine($"{sql}");
            dynamic? items = null;
            try
            {
                using (var connection = new MySqlConnection())
                {
                    connection.ConnectionString = connectionString?.ToString();
                    logWriter?.WriteLine(connection.ConnectionString);
                    logWriter?.WriteLine(connection.State.ToString());
                    await connection.OpenAsync();   //vs  connection.Open();
                    logWriter?.WriteLine(connection.State.ToString());
                    logWriter?.WriteLine(connection.ConnectionTimeout.ToString());
                    items = await connection.QueryAsync(sql, commandTimeout: connection.ConnectionTimeout);
                    logWriter?.WriteLine(items.Count);
                    //connection.Close();
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
                logWriter?.WriteLine(e.Message);
                logWriter?.WriteLine("");
                logWriter?.WriteLine(e.StackTrace);
                logWriter?.CloseLog();
            }
            logWriter?.CloseLog();
            return items;
        }

        public async Task<int> Insert()
        {
            string sql = CreateInsertString();
            string connectionString = _ConnString?.DefaultConnection!;
            int insertedItem = 0;
            using (var connection = new MySqlConnection())
            {
                connection.ConnectionString = connectionString.ToString();
                await connection.OpenAsync();   //vs  connection.Open();
                insertedItem = await connection.ExecuteScalarAsync<int>(sql, this);
                Console.WriteLine(insertedItem);
                connection.Close();
            }
            return insertedItem;
        }

        private string CreateInsertString()
        {
            string classname = GetType().Name.ToLower();
            PropertyInfo[] propertyInfoList = GetType().GetProperties();
            string columns = "";
            string values = "";
            string pkColumn = "";
            int count = 0;

            foreach (PropertyInfo info in propertyInfoList)
            {
                if (info.PropertyType == typeof(Int32?) && count == 0)
                {
                    pkColumn = info.Name;
                }

                if (info.Name == "ConnString")
                    continue;

                if (columns.Length == 0)
                    columns += info.Name;
                else
                    columns += $", {info.Name}";

                if (values.Length == 0)
                    values += $"@{info.Name}";
                else
                    values += $", @{info.Name}";

                count++;
            }

            return $"INSERT INTO {classname}({columns}) Values({values}); select * from {classname} where {pkColumn} = LAST_INSERT_ID();"; ;
        }

        public async Task<bool> Update()
        {
            string sql = CreateUpdateString();
            string connectionString = _ConnString?.DefaultConnection!;
            bool isSuccess = false;
            using (var connection = new MySqlConnection())
            {
                connection.ConnectionString = connectionString.ToString();
                await connection.OpenAsync();   //vs  connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var affectedRows = connection.Execute(sql, this, transaction: transaction);
                    transaction.Commit();
                    if (affectedRows > 0)
                        isSuccess = true;
                }
                connection.Close();
            }
            return isSuccess;
        }

        private string CreateUpdateString()
        {
            string classname = GetType().Name.ToLower();
            PropertyInfo[] propertyInfoList = GetType().GetProperties();
            string columnValue = "";
            string pkColumn = "";
            string pkValue = "";
            int count = 0;

            foreach (PropertyInfo info in propertyInfoList)
            {
                if (info.PropertyType == typeof(Int32?) && count == 0)
                {
                    pkColumn = info.Name;
                    pkValue = $"@{info.Name}";
                }

                if (info.Name == "ConnString")
                    continue;

                if (columnValue.Length == 0)
                    columnValue += $"{info.Name} = @{info.Name}";
                else
                    columnValue += $", {info.Name} = @{info.Name}";

                count++;
            }

            return $"UPDATE {classname} SET {columnValue} WHERE {pkColumn} = {pkValue};"; ;
        }

        public async Task<bool> Delete(string pk, int? id)
        {
            string sql = CreateDeleteString(pk, id);
            string connectionString = _ConnString?.DefaultConnection!;
            bool isSuccess = false;
            using (var connection = new MySqlConnection())
            {
                connection.ConnectionString = connectionString.ToString();
                await connection.OpenAsync();   //vs  connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var affectedRows = connection.Execute(sql, this, transaction: transaction);
                    transaction.Commit();
                    if (affectedRows > 0)
                        isSuccess = true;
                }
                connection.Close();
            }
            return isSuccess;
        }

        private string CreateDeleteString(string pk, int? id)
        {
            string classname = GetType().Name.ToLower();
            return $"DELETE FROM {classname} WHERE {pk} = {id};"; ;
        }
    }
}
