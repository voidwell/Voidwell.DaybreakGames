namespace Voidwell.DaybreakGames.Data;

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public int PoolSize { get; set; } = 100;
    public int? CommandTimeout { get; set; }
}
