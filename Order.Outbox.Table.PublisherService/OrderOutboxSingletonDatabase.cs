﻿using Dapper;
using System.Data;
using System.Data.SqlClient;
namespace Order.Outbox.Table.PublisherService
{
    public static class OrderOutboxSingletonDatabase
    {
        static IDbConnection _connection;
        static bool _dataReaderState = true;
        static OrderOutboxSingletonDatabase()
            => _connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Outbox_OrderAPIDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        public static IDbConnection Connection
        {
            get
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                return _connection;
            }
        }
        public static async Task<IEnumerable<T>> QueryAsync<T>(string sql)
             => await _connection.QueryAsync<T>(sql);
        public static async Task<int> ExecuteAsync(string sql)
            => await _connection.ExecuteAsync(sql);
        public static void DataReaderReady()
            => _dataReaderState = true;
        public static void DataReaderBusy()
            => _dataReaderState = false;
        public static bool DataReaderState => _dataReaderState;
    }
}
