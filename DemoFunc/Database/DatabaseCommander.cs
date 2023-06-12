using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DemoFunc.Database
{
    /// <summary>
    /// Provides boilerplate for common database interactions
    /// </summary>
    public class DatabaseCommander : IDatabaseCommander
    {
        private readonly int commandTimeoutSeconds;
        private readonly string connectionString;

        public DatabaseCommander(string connectionString, int commandTimeoutSeconds = 60)
        {
            this.commandTimeoutSeconds = commandTimeoutSeconds;
            this.connectionString = connectionString;
        }

        public DatabaseLiveCommander GetDatabaseLiveCommander()
        {
            return new DatabaseLiveCommander(this.connectionString, this.commandTimeoutSeconds);
        }


        /// <summary>
        /// queries the database and transforms each row into a dictionary
        /// </summary>
        public async Task<Dictionary<K, V>> QueryDictionaryAsync<K, V>(string query,
            Func<DbDataReader, K> getKey,
            Func<DbDataReader, V> getValue,
            params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.QueryDictionaryAsync(query, getKey, getValue, parameters);
        }


        /// <summary>
        /// Queries the database and transforms each row into a dictionary, where multiple
        /// values may be found per key. NOTE: If the value is null, it will not be added
        /// </summary>
        public async Task<Dictionary<K, List<V>>> QueryDictionaryValuesListAsync<K, V>(string query,
            Func<DbDataReader, K> getKey,
            Func<DbDataReader, V> getValue,
            params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.QueryDictionaryValuesListAsync(query, getKey, getValue, parameters);
        }

        /// <summary>
        /// Queries the database and transforms each row into a dictionary, where multiple
        /// values may be found per key. NOTE: If the value is null, it will not be added
        /// </summary>
        public async Task<Dictionary<K, List<V>>> QueryDictionaryValuesListAsync<K, V>(string query,
            Func<DbDataReader, K> getKey,
            Func<DbDataReader, V> getValue,
            CommandType type,
            params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.QueryDictionaryValuesListAsync(query, getKey, getValue, type, parameters);
        }

        /// <summary>
        /// queries the database and transforms each row into a dictionary
        /// </summary>
        public async Task<Dictionary<K, V>> QueryDictionaryAsync<K, V>(string query,
            Func<DbDataReader, K> getKey,
            Func<DbDataReader, V> getValue,
            CommandType type,
            params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.QueryDictionaryAsync(query, getKey, getValue, type, parameters);
        }

        /// <summary>
        /// Asynchronously queries the database and transform each row into a class
        /// </summary>
        /// <typeparam name="T">The type to map the row to</typeparam>
        /// <param name="query">The SQL Query</param>
        /// <param name="dataReaderTransformer">The function which takes a data reader and returns an instance of the class</param>
        /// <param name="parameters">Parameters for the <see cref="SqlCommand"/></param>
        /// <returns>The list of rows mapped to type <typeparamref name="T"/></returns>
        public async Task<List<T>> QueryListAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.QueryListAsync(query, dataReaderTransformer, parameters);
        }

        public async Task<List<T>> QueryListAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer,
            CommandType type, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.QueryListAsync(query, dataReaderTransformer, type, parameters);
        }

        public async Task<T> QueryFirstAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.QueryFirstAsync(query, dataReaderTransformer, parameters);
        }

        public async Task<T> QueryFirstAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer,
            CommandType type, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.QueryFirstAsync(query, dataReaderTransformer, type, parameters);
        }


        public async Task<object> ExecuteScalarAsync(string query,
            params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.ExecuteScalarAsync(query, parameters);
        }

        public async Task<object> ExecuteScalarAsync(string query,
             CommandType type, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.ExecuteScalarAsync(query, type, parameters);
        }

        public async Task<int> ExecuteScalarToIntAsync(string query, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.ExecuteScalarToIntAsync(query, parameters);
        }

        public async Task<int> ExecuteScalarToIntAsync(string query,
             CommandType type, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.ExecuteScalarToIntAsync(query, type, parameters);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.QueryFirstOrDefaultAsync(query, dataReaderTransformer, parameters);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer,
             CommandType type, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.QueryFirstOrDefaultAsync(query, dataReaderTransformer, type, parameters);
        }

        /// <summary>
        /// Asynchronously queries the database and transform each row into a class
        /// </summary>
        /// <param name="query">The SQL Query</param>
        /// <param name="parameters">Parameters for the <see cref="SqlCommand"/></param>
        /// <returns>The number of rows effected</returns>
        public async Task<int> NonQueryAsync(string query, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.NonQueryAsync(query, parameters);
        }

        public async Task<int> NonQueryAsync(string query,
            CommandType type, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.NonQueryAsync(query, type, parameters);
        }

        /// <summary>
        /// Executes a non-query, logging the error if no rows are effected
        /// </summary>
        /// <param name="query">The SQL Query</param>
        /// <param name="parameters">Parameters for the <see cref="SqlCommand"/></param>
        /// <returns>The Task of the non-query. Returns true if 1 or more rows were updated.</returns>
        public async Task<bool> NonQueryAnyAsync(string query, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.NonQueryAnyAsync(query, parameters);
        }

        public async Task<bool> NonQueryAnyAsync(string query,
            CommandType type, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.NonQueryAnyAsync(query, type, parameters);
        }

        public async Task<bool> AnyAsync(string query,
            params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.AnyAsync(query, parameters);
        }

        public async Task<bool> AnyAsync(string query,
            CommandType type, params SqlParameter[] parameters)
        {
            using var liveCommander = GetDatabaseLiveCommander();
            return await liveCommander.AnyAsync(query, type, parameters);
        }


    }

}
