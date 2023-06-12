using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DemoFunc.Database
{
    public interface IDatabaseCommander
    {
        Task<bool> AnyAsync(string query, params SqlParameter[] parameters);
        Task<bool> AnyAsync(string query, CommandType type, params SqlParameter[] parameters);
        Task<object> ExecuteScalarAsync(string query, params SqlParameter[] parameters);
        Task<object> ExecuteScalarAsync(string query, CommandType type, params SqlParameter[] parameters);
        Task<int> ExecuteScalarToIntAsync(string query, params SqlParameter[] parameters);
        Task<int> ExecuteScalarToIntAsync(string query, CommandType type, params SqlParameter[] parameters);
        Task<bool> NonQueryAnyAsync(string query, params SqlParameter[] parameters);
        Task<bool> NonQueryAnyAsync(string query, CommandType type, params SqlParameter[] parameters);
        Task<int> NonQueryAsync(string query, params SqlParameter[] parameters);
        Task<int> NonQueryAsync(string query, CommandType type, params SqlParameter[] parameters);
        Task<Dictionary<K, V>> QueryDictionaryAsync<K, V>(string query, Func<DbDataReader, K> getKey, Func<DbDataReader, V> getValue, params SqlParameter[] parameters);
        Task<Dictionary<K, V>> QueryDictionaryAsync<K, V>(string query, Func<DbDataReader, K> getKey, Func<DbDataReader, V> getValue, CommandType type, params SqlParameter[] parameters);
        Task<T> QueryFirstAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer, params SqlParameter[] parameters);
        Task<T> QueryFirstAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer, CommandType type, params SqlParameter[] parameters);
        Task<T> QueryFirstOrDefaultAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer, params SqlParameter[] parameters);
        Task<T> QueryFirstOrDefaultAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer, CommandType type, params SqlParameter[] parameters);
        Task<List<T>> QueryListAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer, params SqlParameter[] parameters);
        Task<List<T>> QueryListAsync<T>(string query, Func<SqlDataReader, T> dataReaderTransformer, CommandType type, params SqlParameter[] parameters);
    }
}