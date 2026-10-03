using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Voidwell.DaybreakGames.Data.Extensions;

public static class ModelBuilderExtensions
{
    private static readonly Func<string, string> _tableFormat = value => ToSnakeCase(value);
    private static readonly Func<string, string> _primaryKeyFormat = value => $"p_k_{ToSnakeCase(value)}";
    private static readonly Func<string, string> _foreignKeyFormat = value => $"f_k_{ToSnakeCase(value)}";
    private static readonly Func<string, string> _indexFormat = value => $"i_x_{ToSnakeCase(value)}";

    public static void ConvertToSnakeCaseConvention(this ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            // Replace table names
            entity.SetTableName(_tableFormat(entity.GetTableName()!));

            // Replace column names
            entity.GetProperties().ToList().ForEach(a => a.SetColumnName(ToSnakeCase(a.Name)));

            // Replace primary key names
            entity.GetKeys().ToList().ForEach(a => a.SetName(_primaryKeyFormat(a.GetName()!)));

            // Replace foreign key names
            entity.GetForeignKeys().ToList().ForEach(a => a.SetConstraintName(_foreignKeyFormat(a.GetConstraintName()!)));

            // Replace index names
            entity.GetIndexes().ToList().ForEach(a => a.SetDatabaseName(_indexFormat(a.GetDatabaseName()!.RemovePrefix("IX_"))));
        }
    }

    private static string ToSnakeCase(string input)
    {
        var matches = Regex.Matches(input, "([A-Z][A-Z0-9]*(?=$|[A-Z][a-z0-9])|[A-Za-z][a-z0-9]+)");
        var result = string.Join("_", matches.Select(a => a.Value));

        return result.ToLower();
    }

    private static string RemovePrefix(this string s, string prefix)
    {
        return Regex.Replace(s, "^" + prefix, String.Empty);
    }
}
