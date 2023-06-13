using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
/// <summary>
/// Provides boilerplate for common database interactions
/// </summary>
namespace DemoFunc.Database
{
    public sealed class DatabaseLiveCommander : IDatabaseCommander, IDisposable
    {
        private readonly SqlConnection connection;
        private readonly int commandTimeoutSeconds;
        public DatabaseLiveCommander(
            SqlConnection connection,
            int commandTimeoutSeconds)
        {
            this.connection = connection;
            this.commandTimeoutSeconds = commandTimeoutSeconds;
        }

        public DatabaseLiveCommander(
            string connectionString,
            int commandTimeoutSeconds)
        {
            this.connection = new SqlConnection(connectionString);
            this.commandTimeoutSeconds = commandTimeoutSeconds;
        }

        public async Task<T> UseCommandAsync<T>(Func<SqlCommand, Task<T>> useCommand,
            string query, CommandType type, params SqlParameter[] parameters)
        {
            if (parameters != null)
                foreach (SqlParameter parameter in parameters)
                    if (parameter.Value == null)
                        parameter.Value = DBNull.Value;

            return await UseConnectionAsync(async (connection) =>
            {
                using var command = new SqlCommand(query, connection);
                command.CommandType = type;
                command.Parameters.AddRange(parameters);
                command.CommandTimeout = this.commandTimeoutSeconds;
                return await useCommand(command).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        public Task<T> UseCommandReaderAsync<T>(Func<SqlDataReader, Task<T>> useReaderAsync,
            string query, CommandType type, params SqlParameter[] parameters)
        {
            return UseCommandAsync(async (command) =>
            {
                using SqlDataReader reader = command.ExecuteReader();
                return await useReaderAsync(reader);
            }, query, type, parameters);
        }

        public async Task<T> UseConnectionAsync<T>(Func<SqlConnection, Task<T>> useConnection)
        {
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();
            return await useConnection(connection);
        }

        public Task<int> ExecuteScalarToIntAsync(string query, params SqlParameter[] parameters)
        {
            return ExecuteScalarToIntAsync(query, CommandType.Text, parameters);
        }

        public async Task<int> ExecuteScalarToIntAsync(string query, CommandType type, params SqlParameter[] parameters)
        {
            var result = await ExecuteScalarAsync(query, parameters);
            if (result is int count)
                return count;
            else
                throw new InvalidCastException($"Could not cast object {result} to int in {nameof(ExecuteScalarToIntAsync)}");
        }

        public Task<int> GetTableRowsCount(string tableName)
        {
            return ExecuteScalarToIntAsync($"SELECT COUNT(*) AS CNT FROM {tableName}");
        }

        /// <summary>
        /// queries the database and transform each row into a class
        /// </summary>
        /// <typeparam name="T">The type to map the row to</typeparam>
        public Task<List<T>> QueryListAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer, params SqlParameter[] parameters)
        {
            return QueryListAsync(query, dataReaderTransformer, CommandType.Text, parameters);
        }

        /// <summary>
        /// queries the database and transform each row into a class
        /// </summary>
        /// <typeparam name="T">The type to map the row to</typeparam>
        public async Task<List<T>> QueryListAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer,
            CommandType type, params SqlParameter[] parameters)
        {
            return await UseCommandReaderAsync(async (reader) =>
            {
                List<T> ret = new List<T>();
                while (await reader.ReadAsync())
                {
                    var res = dataReaderTransformer(reader);
                    if (res != null) ret.Add(res);
                }
                return ret;
            }, query, type, parameters);
        }

        public Task<List<T>> QueryManyListAsync<T>(string query, Func<DbDataReader, IEnumerable<T>> dataReaderTransformer,
            params SqlParameter[] parameters)
        {
            return QueryManyListAsync(query, dataReaderTransformer, CommandType.Text, parameters);
        }

        /// <summary>
        /// queries the database and transform each row into a class
        /// </summary>
        /// <typeparam name="T">The type to map the row to</typeparam>
        public Task<List<T>> QueryManyListAsync<T>(string query, Func<DbDataReader, IEnumerable<T>> dataReaderTransformer,
            CommandType type, params SqlParameter[] parameters)
        {
            return UseCommandReaderAsync(async (reader) =>
            {
                List<T> ret = new List<T>();
                while (await reader.ReadAsync())
                    ret.AddRange(dataReaderTransformer(reader));
                return ret;
            }, query, type, parameters);
        }

        /// <summary>
        /// Returns first row of first column
        /// </summary>
        public Task<object> ExecuteScalarAsync(string query, params SqlParameter[] parameters)
        {
            return UseCommandAsync((cmd) => cmd.ExecuteScalarAsync(), query, CommandType.Text, parameters);
        }

        /// <summary>
        /// Returns first row of first column
        /// </summary>
        public Task<object> ExecuteScalarAsync(string query,
            CommandType type, params SqlParameter[] parameters)
        {
            return UseCommandAsync((cmd) => cmd.ExecuteScalarAsync(), query, type, parameters);
        }

        /// <summary>
        /// queries the database and transforms each row into a dictionary
        /// </summary>
        public Task<Dictionary<K, V>> QueryDictionaryAsync<K, V>(string query,
            Func<DbDataReader, K> getKey,
            Func<DbDataReader, V> getValue,
            params SqlParameter[] parameters)
        {
            return QueryDictionaryAsync(query, getKey, getValue, CommandType.Text, parameters);
        }

        /// <summary>
        /// queries the database and transforms each row into a dictionary
        /// </summary>
        public Task<Dictionary<K, V>> QueryDictionaryAsync<K, V>(string query,
        Func<DbDataReader, K> getKey,
        Func<DbDataReader, V> getValue,
        CommandType type,
        params SqlParameter[] parameters)
        {
            return UseCommandReaderAsync(async (reader) =>
            {
                Dictionary<K, V> ret = new Dictionary<K, V>();
                while (await reader.ReadAsync())
                {
                    var key = getKey(reader);
                    if (key == null) continue;

                    var value = getValue(reader);
                    if (value == null) continue;

                    ret.Add(key, getValue(reader));
                }
                return ret;
            }, query, type, parameters);
        }

        /// <summary>
        /// Queries the database and transforms each row into a dictionary, where multiple
        /// values may be found per key. NOTE: If the value is null, it will not be added
        /// </summary>
        public Task<Dictionary<K, List<V>>> QueryDictionaryValuesListAsync<K, V>(string query,
            Func<DbDataReader, K> getKey,
            Func<DbDataReader, V> getValue,
            params SqlParameter[] parameters)
        {
            return QueryDictionaryValuesListAsync(query, getKey, getValue, CommandType.Text, parameters);
        }

        /// <summary>
        /// Queries the database and transforms each row into a dictionary, where multiple
        /// values may be found per key. NOTE: If the value is null, it will not be added
        /// </summary>
        public Task<Dictionary<K, List<V>>> QueryDictionaryValuesListAsync<K, V>(string query,
        Func<DbDataReader, K> getKey,
        Func<DbDataReader, V> getValue,
        CommandType type,
        params SqlParameter[] parameters)
        {
            return UseCommandReaderAsync(async (reader) =>
            {
                Dictionary<K, List<V>> ret = new Dictionary<K, List<V>>();
                while (await reader.ReadAsync())
                {
                    var key = getKey(reader);
                    try
                    {
                        if (key == null)
                            continue;

                        var value = getValue(reader);
                        if (ret.TryGetValue(key, out List<V> values))
                        {
                            if (value != null)
                                values.Add(value);
                        }
                        else if (value != null)
                        {
                            ret.Add(key, new List<V> { value });
                        }
                        else
                            ret.Add(key, new List<V>());
                    }
                    catch (ArgumentException ae)
                        when (ae.Message == "An item with the same key has already been added.")
                    {
                        var le = new ArgumentException($"An item with the same key '{key}' has already been added.", ae);
                        throw le;
                    }
                }
                return ret;
            }, query, type, parameters);
        }


        public Task<T> QueryFirstAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer,
            params SqlParameter[] parameters)
        {
            return QueryFirstAsync(query, dataReaderTransformer, CommandType.Text, parameters);
        }

        public Task<T> QueryFirstAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer,
            CommandType type, params SqlParameter[] parameters)
        {
            return UseCommandReaderAsync(async (dr) =>
            {
                if (await dr.ReadAsync())
                    return dataReaderTransformer(dr);

                throw new InvalidOperationException($"No rows found for query executed in {nameof(QueryFirstAsync)}");
            }, query, type, parameters);
        }


        public Task<T> QueryFirstOrDefaultAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer,
            params SqlParameter[] parameters)
        {
            return QueryFirstOrDefaultAsync(query, dataReaderTransformer, CommandType.Text, parameters);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer,
            CommandType type,
            params SqlParameter[] parameters)
        {
            return UseCommandReaderAsync(async (reader) =>
            {
                if (await reader.ReadAsync())
                    return dataReaderTransformer(reader);
                return default(T);
            }, query, type, parameters);
        }

        /// <summary>
        /// Asynchronously queries the database and transform each row into a class
        /// </summary>
        public Task<int> NonQueryAsync(string query,
            CommandType type,
            params SqlParameter[] parameters)
        {
            return UseCommandAsync((command) => command.ExecuteNonQueryAsync(), query, type, parameters);
        }

        /// <summary>
        /// Asynchronously queries the database and transform each row into a class
        /// </summary>
        public Task<int> NonQueryAsync(string query,
            params SqlParameter[] parameters)
        {
            return UseCommandAsync((command) => command.ExecuteNonQueryAsync(), query, CommandType.Text, parameters);
        }


        public Task<bool> NonQueryAnyAsync(string query, params SqlParameter[] parameters)
        {
            return NonQueryAnyAsync(query, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes a non-query, logging the error if no rows are effected
        /// </summary>
        /// <returns>True if 1 or more rows were effected, false if an error occured or if no rows were effected</returns>
        public async Task<bool> NonQueryAnyAsync(string query,
            CommandType type, params SqlParameter[] parameters)
        {
            var effectedRows = await NonQueryAsync(query, type, parameters);
            // NOTE: Oracle can return -1 as a true result
            bool didEffect = effectedRows != 0;
            return didEffect;
        }

        /// <summary>
        /// If any rows are found, return True. False otherwise.
        /// </summary>
        public Task<bool> AnyAsync(string query,
            params SqlParameter[] parameters)
        {
            return AnyAsync(query, CommandType.Text, parameters);
        }

        /// <summary>
        /// If any rows are found, return True. False otherwise.
        /// </summary>
        public Task<bool> AnyAsync(string query,
            CommandType type,
            params SqlParameter[] parameters)
        {
            return UseCommandReaderAsync((reader) => reader.ReadAsync(), query, type, parameters);
        }

        /// <summary>
        /// Checks if a query throws an exception or not
        /// </summary>
        public Task<bool> DoesQuerySucceedAsync(string query,
            CommandType type,
            params SqlParameter[] parameters)
        {
            return UseCommandAsync(async (command) =>
            {
                try
                {
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        await reader.ReadAsync();
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }, query, type, parameters);
        }


        public Task<bool> ToCSVFileAsync(string query, string fileName, CommandType type = CommandType.Text)
        {
            return UseCommandReaderAsync(async (reader) =>
            {
                string dirName = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    object[] output = new object[reader.FieldCount];

                    for (int i = 0; i < reader.FieldCount; i++)
                        output[i] = reader.GetName(i);

                    sw.WriteLine(string.Join(",", output));

                    while (await reader.ReadAsync())
                    {
                        reader.GetValues(output);
                        sw.WriteLine(string.Join(",", output));
                    }
                }
                return true;
            }, query, type);
        }

        public Task<DataTable> QueryDataTableAsync(string query,
            params SqlParameter[] parameters)
        {
            return QueryDataTableAsync(query, CommandType.Text, parameters);
        }

        public Task<DataTable> QueryDataTableAsync(string query,
            CommandType type,
            params SqlParameter[] parameters)
        {
            return UseCommandAsync(async (command) =>
            {
                DataTable ret = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(command);
                await Task.Run(() => da.Fill(ret));
                return ret;
            }, query, type, parameters);
        }

        public void Dispose()
        {
            connection?.Dispose();
        }

    }
}
