using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace SmartNameplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly IConfiguration _config;
    public DatabaseController(IConfiguration config)
    {
        _config = config;
    }

    // GET: api/database/tables
    [HttpGet("tables")]
    public IActionResult GetTables()
    {
        var connStr = _config.GetConnectionString("DefaultConnection");
        var tables = new List<string>();
        using (var conn = new NpgsqlConnection(connStr))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand(@"SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' ORDER BY table_name", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
            }
        }
        return Ok(tables);
    }

    // GET: api/database/tables/{table}/columns
    [HttpGet("tables/{table}/columns")]
    public IActionResult GetTableColumns(string table)
    {
        var connStr = _config.GetConnectionString("DefaultConnection");
        var columns = new List<object>();
        using (var conn = new NpgsqlConnection(connStr))
        {
            conn.Open();
            var sql = @"SELECT column_name, data_type, is_nullable, column_default
                         FROM information_schema.columns
                         WHERE table_schema = 'public' AND lower(table_name) = lower(@table)
                         ORDER BY ordinal_position";
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@table", table);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columns.Add(new {
                            Name = reader.GetString(0),
                            Type = reader.GetString(1),
                            Nullable = reader.GetString(2) == "YES",
                            Default = reader.IsDBNull(3) ? null : reader.GetValue(3)
                        });
                    }
                }
            }
        }
        return Ok(columns);
    }

    // GET: api/database/tables/{table}/rows
    [HttpGet("tables/{table}/rows")]
    public IActionResult GetTableRows(string table)
    {
        var connStr = _config.GetConnectionString("DefaultConnection");
        var rows = new List<Dictionary<string, object>>();
        using (var conn = new NpgsqlConnection(connStr))
        {
            conn.Open();
            // 僅允許 public schema 下的表，並避免 SQL injection
            var tableName = table.Replace("\"", "");
            var sql = $"SELECT * FROM \"{tableName}\" LIMIT 20";
            using (var cmd = new NpgsqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    }
                    rows.Add(row);
                }
            }
        }
        return Ok(rows);
    }
} 