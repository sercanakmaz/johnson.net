using JohnsonNet.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace JohnsonNet.Operation
{
    public class DataOperation
    {
        public ConnectionStringSettings CurrentConnectionString;
        public int CommandTimeout { get; set; }

        public DataOperation()
            : this(null)
        {
        }
        public DataOperation(ConnectionStringSettings connectionString)
        {
            if (connectionString == null)
                connectionString = JohnsonManager.Config.Current.GetConnectionString("LocalSqlServer");

            this.CurrentConnectionString = connectionString;

        }

        /// <summary>
        /// SQL Server Managament Studio has a keyword named "GO", this keyword allows you to run several scripts in one file. Sometimes we need this on C#, this method helps you to that.
        /// </summary>
        /// <param name="query"></param>
        public void ExecuteBulk(string query)
        {
            string[] queries = query.Split(new[] { "\r\nGO", "\nGO", "\rGO", " GO", "\tGO", "GO\r\n", "GO\n", "GO\r", "GO ", "GO\t" }, StringSplitOptions.RemoveEmptyEntries);

            using (var conn = CurrentConnectionString.ToIDbConnection())
            {
                conn.Open();

                try
                {
                    foreach (string q in queries)
                    {
                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = q;
                            command.CommandType = CommandType.Text;
                            command.CommandTimeout = this.CommandTimeout;

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// You know this allready.
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual int ExecuteNonQuery(string proc, ParamDictionary parameters)
        {
            DateTime dateStart = DateTime.Now;

            using (var conn = CurrentConnectionString.ToIDbConnection())
            using (var command = conn.CreateCommand())
            {
                conn.Open();
                int result;

                try
                {

                    command.CommandText = proc;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = this.CommandTimeout;

                    if (parameters != null)
                    {
                        foreach (var item in parameters)
                        {
                            command.Parameters.Add(new SqlParameter(item.Key, item.Value));
                        }
                    }

                    this.WriteProfile(proc, parameters, dateStart);

                    result = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }

                return result;
            }
        }
        /// <summary>
        /// ExecuteReader method has 3 argument, first two procedure name and parameters. Third is a IDataReader Action, this allows you to get a your results with IDataRedeader.
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="parameters"></param>
        /// <param name="use"></param>
        public virtual void ExecuteReader(string proc, ParamDictionary parameters, Action<IDataReader> use)
        {
            DateTime dateStart = DateTime.Now;

            using (var conn = CurrentConnectionString.ToIDbConnection())
            using (var command = conn.CreateCommand())
            {
                conn.Open();
                try
                {
                    command.CommandText = proc;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = this.CommandTimeout;

                    if (parameters != null)
                    {
                        foreach (var item in parameters)
                        {
                            var param = new SqlParameter(item.Key, item.Value != null ? item.Value : DBNull.Value);
                            command.Parameters.Add(param);
                        }
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        use(reader);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }

            this.WriteProfile(proc, parameters, dateStart);
        }

        #region Execute Methods
        /// <summary>
        /// This method allows you to database results to a entity list.
        /// </summary>
        /// <typeparam name="T">entity</typeparam>
        /// <param name="proc">Procedure Name</param>
        /// <param name="parameters">Procedure Parameters</param>
        /// <returns></returns>
        public virtual List<T> Execute<T>(string proc, ParamDictionary parameters = null)
        {
            List<T> result = null;

            ExecuteReader(proc, parameters, (reader) => result = reader.ToList<T>());

            return result;
        }
        /// <summary>
        /// This method allows you to get 2 resultsets from your database and it converts to a entity list.
        /// </summary>
        /// <typeparam name="T">ResultSet1</typeparam>
        /// <typeparam name="T1">ResulSet2</typeparam>
        /// <param name="proc">Procedure Name</param>
        /// <param name="parameters">Procedure Parameters</param>
        /// <returns></returns>
        public virtual ResultSet<T, T1> Execute<T, T1>(string proc, ParamDictionary parameters = null)
        {
            var result = new ResultSet<T, T1>();

            ExecuteReader(proc, parameters, (reader) =>
            {
                result.Result1 = reader.ToList<T>();
                reader.NextResult();
                result.Result2 = reader.ToList<T1>();
            });

            return result;
        }
        public virtual ResultSet<T, T1, T2> Execute<T, T1, T2>(string proc, ParamDictionary parameters = null)
        {
            var result = new ResultSet<T, T1, T2>();

            ExecuteReader(proc, parameters, (reader) =>
            {
                result.Result1 = reader.ToList<T>();
                reader.NextResult();
                result.Result2 = reader.ToList<T1>();
                reader.NextResult();
                result.Result3 = reader.ToList<T2>();
            });

            return result;
        }
        public virtual ResultSet<T, T1, T2, T3> Execute<T, T1, T2, T3>(string proc, ParamDictionary parameters = null)
        {
            var result = new ResultSet<T, T1, T2, T3>();

            ExecuteReader(proc, parameters, (reader) =>
            {
                result.Result1 = reader.ToList<T>();
                reader.NextResult();
                result.Result2 = reader.ToList<T1>();
                reader.NextResult();
                result.Result3 = reader.ToList<T2>();
                reader.NextResult();
                result.Result4 = reader.ToList<T3>();
            });

            return result;
        }
        public virtual ResultSet<T, T1, T2, T3, T4> Execute<T, T1, T2, T3, T4>(string proc, ParamDictionary parameters = null)
        {
            var result = new ResultSet<T, T1, T2, T3, T4>();

            ExecuteReader(proc, parameters, (reader) =>
            {

                result.Result1 = reader.ToList<T>();
                reader.NextResult();
                result.Result2 = reader.ToList<T1>();
                reader.NextResult();
                result.Result3 = reader.ToList<T2>();
                reader.NextResult();
                result.Result4 = reader.ToList<T3>();
                reader.NextResult();
                result.Result5 = reader.ToList<T4>();
            });

            return result;
        }
        public virtual ResultSet<T, T1, T2, T3, T4, T5> Execute<T, T1, T2, T3, T4, T5>(string proc, ParamDictionary parameters = null)
        {
            var result = new ResultSet<T, T1, T2, T3, T4, T5>();

            ExecuteReader(proc, parameters, (reader) =>
            {
                result.Result1 = reader.ToList<T>();
                reader.NextResult();
                result.Result2 = reader.ToList<T1>();
                reader.NextResult();
                result.Result3 = reader.ToList<T2>();
                reader.NextResult();
                result.Result4 = reader.ToList<T3>();
                reader.NextResult();
                result.Result5 = reader.ToList<T4>();
                reader.NextResult();
                result.Result6 = reader.ToList<T5>();
            });

            return result;
        }
        #endregion

        private void WriteProfile(string proc, ParamDictionary parameters, DateTime dateStart)
        {
            bool writeExecuteLog = JohnsonManager.Config.Current.GetSetting<bool>("JonhsonNet:Profile");

            if (writeExecuteLog)
            {
                string executeLog = this.GetExecuteLog(proc, parameters);
                TimeSpan elapsedLog = DateTime.Now - dateStart;

                Debug.WriteLine(executeLog);
                Debug.WriteLine("Elapsed: {0}", elapsedLog.TotalMilliseconds);
            }
        }
        private string GetExecuteLog(string proc, ParamDictionary parameters = null)
        {
            if (parameters == null) parameters = new ParamDictionary();

            var parametersFiltered = parameters.Where(p => !string.IsNullOrEmpty(JohnsonManager.Convert.To<string>(p.Value)));

            return string.Format("EXEC {0} {1}", proc, string.Join(",", parameters.Select(p => string.Format("@{0} = '{1}'", p.Key, p.Value))));
        }
    }
}
