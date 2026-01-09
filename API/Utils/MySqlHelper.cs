using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using Utils.Interface;

namespace Utils
{
    public class MySqlHelper : IMySqlHelper
    {
        private string _connectionString;

        private readonly IConfiguration _configuration;
        public MySqlHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("Default");
        }

        public DataTable ExecuteSelectCommand(string CommandName, CommandType cmdType)
        {
            DataTable table = null;

            using (MySqlConnection con = new MySqlConnection(this._connectionString))
            {
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            table = new DataTable();
                            da.Fill(table);
                        }

                    }
                    catch (MySqlException sqlex)
                    {
                        throw sqlex;
                    }
                    catch (DataException dataex)
                    {
                        throw dataex;
                    }
                    catch (Exception ex)
                    {
                        throw ex;

                    }
                    finally
                    {

                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();

                        }

                    }
                }
            }

            return table;
        }

        public DataTable ExecuteParamerizedSelectCommand(string CommandName, CommandType cmdType, IDbDataParameter[] param)
        {
            DataTable table = new DataTable();

            using (MySqlConnection con = new MySqlConnection(this._connectionString))
            {
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.Parameters.AddRange(param);

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(table);
                        }


                    }
                    catch (MySqlException sqlex)
                    {
                        throw sqlex;
                    }
                    catch (DataException dataex)
                    {
                        throw dataex;
                    }
                    catch (Exception ex)
                    {
                        throw ex;

                    }
                    finally
                    {

                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();

                        }
                    }
                }
            }

            return table;
        }

        public bool ExecuteNonQuery(string CommandName, CommandType cmdType, IDbDataParameter[] pars)
        {
            int result = 0;

            using (MySqlConnection con = new MySqlConnection(this._connectionString))
            {
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.Parameters.AddRange(pars);

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        result = cmd.ExecuteNonQuery();

                    }
                    catch (MySqlException sqlex)
                    {
                        throw sqlex;
                    }
                    catch (DataException dataex)
                    {
                        throw dataex;
                    }
                    catch (Exception ex)
                    {
                        throw ex;

                    }
                    finally
                    {

                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();

                        }
                    }
                }
            }

            return (result > 0);
        }

        public bool ExecuteNonQuery(string CommandName, CommandType cmdType)
        {
            int result = 0;

            using (MySqlConnection con = new MySqlConnection(this._connectionString))
            {
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        result = cmd.ExecuteNonQuery();

                    }
                    catch (MySqlException sqlex)
                    {
                        throw sqlex;
                    }
                    catch (DataException dataex)
                    {
                        throw dataex;
                    }
                    catch (Exception ex)
                    {
                        throw ex;

                    }
                    finally
                    {

                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();

                        }
                    }
                }
            }

            return (result > 0);
        }

        public string ExecuteScalar(string CommandName, CommandType cmdType, IDbDataParameter[] pars)
        {
            string result = "";

            using (MySqlConnection con = new MySqlConnection(this._connectionString))
            {
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.Parameters.AddRange(pars);

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        result = cmd.ExecuteScalar().ToString();

                    }
                    catch (MySqlException sqlex)
                    {
                        throw sqlex;
                    }
                    catch (DataException dataex)
                    {
                        throw dataex;
                    }
                    catch (Exception ex)
                    {
                        throw ex;

                    }
                    finally
                    {

                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();

                        }
                    }
                }
            }

            return result;
        }

        public DataSet ExecuteMutipleSelectCommand(string CommandName, CommandType cmdType)
        {
            DataSet dataset = null;
            using (MySqlConnection con = new MySqlConnection(this._connectionString))
            {
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            dataset = new DataSet();
                            da.Fill(dataset);
                        }
                    }
                    catch (MySqlException sqlex)
                    {
                        throw sqlex;
                    }
                    catch (DataException dataex)
                    {
                        throw dataex;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
            }
            return dataset;
        }

        public DataSet ExecuteParamerizedMultipleSelectCommand(string CommandName, CommandType cmdType, IDbDataParameter[] param)
        {
            DataSet dataset = new DataSet();

            using (MySqlConnection con = new MySqlConnection(this._connectionString))
            {
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.Parameters.AddRange(param);

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(dataset);
                        }
                    }
                    catch (MySqlException sqlex)
                    {
                        throw sqlex;
                    }
                    catch (DataException dataex)
                    {
                        throw dataex;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
            }
            return dataset;
        }

        public IDbDataParameter GetDataParameter(string parameter, object value)
        {
            throw new NotImplementedException();
        }
    }
}
