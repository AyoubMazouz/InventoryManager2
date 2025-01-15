using Dapper;
using Dal.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TableController : ControllerBase
{
    private readonly DapperContext _context;

    public TableController(DapperContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int size = 10)
    {
        var query = $"SELECT * FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
        var parameters = new DynamicParameters();

        parameters.Add("offset", (page - 1) * size);
        parameters.Add("pageSize", size);

        using (var connection = _context.CreateConnection())
        {
            var result = await connection.QueryAsync(query, parameters);
            return Ok(result);
        }
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> Get(string name, string? orderBy, bool asc = true, int page = 1, int size = 2)
    {
        var sortDir = asc ? "ASC" : "DESC";

        var columnsParameters = new DynamicParameters();
        var columnsQuery = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName";
        columnsParameters.Add("tableName", name);
        var columns = new List<string>();
        using (var connection = _context.CreateConnection())
        {
            var result = await connection.QueryAsync<string>(columnsQuery, columnsParameters);
            columns = result.ToList();
        }

        if (orderBy == null)
        {
            orderBy = columns[0];
        }
        else if (!columns.Contains(orderBy))
        {
            return BadRequest($"Invalid column name: {orderBy}");
        }

        var parameters = new DynamicParameters();
        var query = $@"
            SELECT * FROM {name} 
            ORDER BY {orderBy} {sortDir} 
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
        parameters.Add("pageSize", size);
        parameters.Add("offset", (page - 1) * size);

        using (var connection = _context.CreateConnection())
        {
            var result = await connection.QueryAsync(query, parameters);
            return Ok(result);
        }
    }
}