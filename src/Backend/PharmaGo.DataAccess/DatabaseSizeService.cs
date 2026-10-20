using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PharmaGo.IDataAccess;

namespace PharmaGo.DataAccess;

[ExcludeFromCodeCoverage]
public class DatabaseSizeService(PharmacyGoDbContext _context) : IDatabaseSizeService
{

    public override async Task<long?> GetSpaceUsedAsync()
    {
        var connection = _context.Database.GetDbConnection() as SqlConnection;

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        using var command = new SqlCommand("sp_spaceused", connection);
        command.CommandType = CommandType.StoredProcedure;

        using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
        if (!await reader.ReadAsync())
            return null;

        return ParseSizeToKb(reader["database_size"].ToString());
    }

    public static long ParseSizeToKb(string? sizeStr)
    {
        if (string.IsNullOrWhiteSpace(sizeStr)) return 0;
        var parts = sizeStr.Trim().Split(' ');
        if (parts.Length != 2) return 0;

        var value = decimal.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
        return parts[1].ToUpper() switch
        {
            "KB" => (long)value,
            "MB" => (long)(value * 1024),
            "GB" => (long)(value * 1024 * 1024),
            _ => 0
        };
    }
}