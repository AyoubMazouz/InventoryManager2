using Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> Names()
    {
        var result = new List<Dictionary<string, object>>();
        var tableName = "Employees";

        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = $"SELECT * FROM {tableName}";
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
}