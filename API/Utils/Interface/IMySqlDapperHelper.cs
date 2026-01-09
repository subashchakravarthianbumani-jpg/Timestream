using System.Data;

namespace Utils.Interface
{
    public interface IMySqlDapperHelper
    {
        IEnumerable<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionId = "Default");
        int SaveData<T>(string storedProcedure, T parameters, string connectionId = "Default");
        public IDbConnection GetConnection(string connectionId = "Default");
    }
}
