using JohnsonNet.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace JohnsonNet.Base.Data
{
    public class BaseDataOperation
    {
        public ConnectionStringSettings CurrentConnectionString;

        public BaseDataOperation()
            : this(null)
        {
        }
        public BaseDataOperation(ConnectionStringSettings connectionString)
        {
            if (connectionString == null)
                connectionString = ConfigurationFactory.Current.GetConnectionString("LocalSqlServer");

            this.CurrentConnectionString = connectionString;

        }
        public virtual int ExecuteNonQuery(string proc, ParamDictionary parameters)
        {
            using (var conn = Helpers.GetConnection(CurrentConnectionString))
            using (var command = conn.CreateCommand())
            {
                conn.Open();
                int result;

                try
                {

                    command.CommandText = proc;
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (var item in parameters)
                        {
                            command.Parameters.Add(new SqlParameter(item.Key, item.Value));
                        }
                    }

                    result = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }

                return result;
            }
        }
        /// <summary>
        /// Bu metodu kullandıktan sonra connection'i close etmelisiniz!
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual void ExecuteReader(string proc, ParamDictionary parameters, Action<IDataReader> use)
        {
            using (var conn = Helpers.GetConnection(CurrentConnectionString))
            using (var command = conn.CreateCommand())
            {
                conn.Open();
                try
                {

                    command.CommandText = proc;
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (var item in parameters)
                        {
                            command.Parameters.Add(new SqlParameter(item.Key, item.Value));
                        }
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        use(reader);
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
            }
        }

        #region Execute Methods
        public virtual List<T> Execute<T>(string proc, ParamDictionary parameters = null)
        {
            List<T> result = null;

            ExecuteReader(proc, parameters, (reader) => result = reader.ToList<T>());

            return result;
        }
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
    }
}
