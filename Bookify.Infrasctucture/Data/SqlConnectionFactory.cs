﻿using Bookify.Application.Abstractions.Data;
using Npgsql;
using System.Data;

namespace Bookify.Infrasctucture.Data;
internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;        
    }

    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        return connection;
    }
}
