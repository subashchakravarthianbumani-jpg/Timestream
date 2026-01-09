using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Interface
{
    public interface IMySqlHelper
    {
        DataSet ExecuteMutipleSelectCommand(string CommandName, CommandType cmdType);
        bool ExecuteNonQuery(string CommandName, CommandType cmdType);
        bool ExecuteNonQuery(string CommandName, CommandType cmdType, IDbDataParameter[] pars);
        DataSet ExecuteParamerizedMultipleSelectCommand(string CommandName, CommandType cmdType, IDbDataParameter[] param);
        DataTable ExecuteParamerizedSelectCommand(string CommandName, CommandType cmdType, IDbDataParameter[] param);
        string ExecuteScalar(string CommandName, CommandType cmdType, IDbDataParameter[] pars);
        DataTable ExecuteSelectCommand(string CommandName, CommandType cmdType);
        IDbDataParameter GetDataParameter(string parameter, object value);
    }
}
