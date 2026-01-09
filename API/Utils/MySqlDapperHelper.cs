using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Interface;

namespace Utils
{
    public class MySqlDapperHelper : IMySqlDapperHelper
    {
        private readonly IConfiguration _configuration;
        public MySqlDapperHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionId = "Default")
        {
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            return connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public int SaveData<T>(string storedProcedure, T parameters, string connectionId = "Default")
        {
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            return connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public IDbConnection GetConnection(string connectionId = "Default")
        {
            return new MySqlConnection(_configuration.GetConnectionString(connectionId));
        }
    }
}
