using Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TableController : ControllerBase
{
    private readonly AppDbContext _context;

    public TableController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return Ok("Hello World");
    }

    [HttpGet("names")]
    public async Task<IActionResult> Names(int page = 1, int size = 1, string? filter = null)
    {
        var result = new List<Dictionary<string, object>>();
        var tableName = "Employees";

        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            // Build the SQL query with filtering and pagination
            var query = $"SELECT * FROM {tableName}";
            if (!string.IsNullOrEmpty(filter))
            {
                query += $" WHERE Name LIKE @filter";
                command.Parameters.Add(new SqlParameter("@filter", $"%{filter}%"));
            }
            query += $" ORDER BY EmployeeId OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
            command.CommandText = query;
            command.Parameters.Add(new SqlParameter("@offset", (page - 1) * size));
            command.Parameters.Add(new SqlParameter("@pageSize", size));

            _context.Database.OpenConnection();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }
                    result.Add(row);
                }
            }
        }

        return Ok(result);
    }

    [HttpGet("namesd")]
    public async Task<IActionResult> Namesd(int page = 1, int size = 10, string? filter = null)
    {
        var tableName = "Employees";
        var query = $"SELECT * FROM {tableName}";
        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(filter))
        {
            query += " WHERE Name LIKE @filter";
            parameters.Add("filter", $"%{filter}%");
        }

        query += " ORDER BY EmployeeId OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
        parameters.Add("offset", (page - 1) * size);
        parameters.Add("pageSize", size);

        using (var connection = _context.Database.GetDbConnection())
        {
            var result = await connection.QueryAsync(query, parameters);
            return Ok(result);
        }
    }

}