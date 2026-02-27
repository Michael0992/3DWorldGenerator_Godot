using Godot;
using Microsoft.Data.Sqlite;
using System;

public partial class DatabaseManager : Node
{
    private SqliteConnection _connection;
    private static DatabaseManager _instance;

    // Singleton - von überall erreichbar
    public static DatabaseManager Instance => _instance;

    public override void _Ready()
    {
        _instance = this;
        OpenDatabase();
        CreateTable();
        GD.Print("Datenbankmanager bereit");
    }

    private void OpenDatabase()
    {
        string dbPath = "res://Databases/world.db";
        // Godot res:// Pfad in absoluten Pfad umwandeln
        string absolutePath = ProjectSettings.GlobalizePath(dbPath);

        _connection = new SqliteConnection($"Data Source={absolutePath}");
        _connection.Open();
        GD.Print("Datenbank geöffnet: " + absolutePath);
    }

    private void CreateTable()
    {
        string sql1 = @"
            CREATE TABLE IF NOT EXISTS SceneObjects (
                guid        TEXT PRIMARY KEY,
                type        TEXT NOT NULL,
                pos_x       REAL NOT NULL,
                pos_y       REAL NOT NULL,
                pos_z       REAL NOT NULL,
                rot_x       REAL NOT NULL,
                rot_y       REAL NOT NULL,
                rot_z       REAL NOT NULL
            );";

        string sql2 = @"
        CREATE TABLE IF NOT EXISTS WorldSettings (
            key     TEXT PRIMARY KEY,
            value   TEXT NOT NULL
        );";




        using var cmd = new SqliteCommand(sql1, _connection);
        cmd.ExecuteNonQuery();
        GD.Print("Tabelle bereit");

        using var cmd2 = new SqliteCommand(sql2, _connection);
        cmd2.ExecuteNonQuery();
    }

    public string SaveObject(Node3D node, string type)
    {
        string guid = Guid.NewGuid().ToString();

        string sql = @"
            INSERT INTO SceneObjects 
                (guid, type, pos_x, pos_y, pos_z, rot_x, rot_y, rot_z)
            VALUES
                (@guid, @type, @px, @py, @pz, @rx, @ry, @rz);";

        using var cmd = new SqliteCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@guid", guid);
        cmd.Parameters.AddWithValue("@type", type);
        cmd.Parameters.AddWithValue("@px", node.Position.X);
        cmd.Parameters.AddWithValue("@py", node.Position.Y);
        cmd.Parameters.AddWithValue("@pz", node.Position.Z);
        cmd.Parameters.AddWithValue("@rx", node.Rotation.X);
        cmd.Parameters.AddWithValue("@ry", node.Rotation.Y);
        cmd.Parameters.AddWithValue("@rz", node.Rotation.Z);
        cmd.ExecuteNonQuery();

        return guid; // zurückgeben damit das Objekt seine ID kennt
    }

    public bool ChunkExistsAtPosition(float x, float z)
    {
        string sql = @"
            SELECT COUNT(*) FROM SceneObjects
            WHERE type = 'TerrainChunk'
              AND ABS(pos_x - @x) < 0.01
              AND ABS(pos_z - @z) < 0.01;";
        using var cmd = new SqliteCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@x", x);
        cmd.Parameters.AddWithValue("@z", z);
        var result = cmd.ExecuteScalar();
        return Convert.ToInt64(result) > 0;
    }

    public override void _ExitTree()
    {
        _connection?.Close();
    }


    public int LoadOrCreateWorldSeed()
    {
        // Prüfen ob Seed bereits existiert
        string selectSql = "SELECT value FROM WorldSettings WHERE key = 'world_seed';";
        using var selectCmd = new SqliteCommand(selectSql, _connection);
        var result = selectCmd.ExecuteScalar();
        GD.Print("Prüfe auf existierenden Welt-Seed...");

        if (result != null)
        {
            // Seed aus DB laden → gleiche Welt
            GD.Print("Welt-Seed geladen: " + result);
            return int.Parse(result.ToString());
        }
        else
        {
            // Neuen Seed generieren und speichern
            int newSeed = new Random().Next();
            string insertSql = "INSERT INTO WorldSettings (key, value) VALUES ('world_seed', @seed);";
            using var insertCmd = new SqliteCommand(insertSql, _connection);
            insertCmd.Parameters.AddWithValue("@seed", newSeed.ToString());
            insertCmd.ExecuteNonQuery();
            GD.Print("Neuer Welt-Seed erstellt: " + newSeed);
            return newSeed;
        }
    }
}