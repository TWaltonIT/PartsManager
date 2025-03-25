using Microsoft.Data.Sqlite;
using PartsManager.Models;
using System.Collections.Generic;

namespace PartsManager.Data
{
  public class Database
  {
    private readonly string _connectionString;

    public Database(string dbPath)
    {
      _connectionString = $"Data Source={dbPath}";
      InitializeDatabase();
    }

    private void InitializeDatabase()
    {
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      const string createTVModelsTable = @"
                CREATE TABLE IF NOT EXISTS TVModel (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                );";

      const string createPartsTable = @"
                CREATE TABLE IF NOT EXISTS Part (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PartNumber TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Location TEXT NOT NULL
                );";

      const string createTVModelPartTable = @"
        CREATE TABLE IF NOT EXISTS TVModelPart (
          TVModelId INTEGER NOT NULL,
          PartId INTEGER NOT NULL,
          PRIMARY KEY (TVModelId, PartId),
          FOREIGN KEY (TVModelId) REFERENCES TVModel(Id) ON DELETE CASCADE,
          FOREIGN KEY (PartId) REFERENCES Part(Id) ON DELETE CASCADE
        );";

      using var command = connection.CreateCommand();
      command.CommandText = $"{createTVModelsTable}{createPartsTable}{createTVModelPartTable}";
      command.ExecuteNonQuery();
    }

    // Add TV Model
    public void AddTVModel(string name)
    {
      ExecuteNonQuery("INSERT INTO TVModel (Name) VALUES (@name)",
          new Dictionary<string, object> { { "@name", name } });
    }

    // Get TV Models
    public List<string> GetTVModels()
    {
      return ExecuteQuery("SELECT Name FROM TVModel", reader => reader.GetString(0));
    }

    public List<TVModel> GetAllTVModels()
    {
      return ExecuteQuery("SELECT Id, Name FROM TVModel", reader => new TVModel
      {
        Id = reader.GetInt32(0),
        Name = reader.GetString(1)
      });
    }

    public void DeleteTVModel(int id)
    {
      ExecuteNonQuery("DELETE FROM TVModel WHERE Id = @id",
          new Dictionary<string, object> { { "@id", id } });
    }

    // Add Part
    public void AddPart(string partNumber, string name, int quantity, string location)
    {
      const string query = @"
                INSERT INTO Part (PartNumber, Name, Quantity, Location) 
                VALUES (@partNumber, @name, @quantity, @location)";

      ExecuteNonQuery(query, new Dictionary<string, object>
            {
                { "@partNumber", partNumber },
                { "@name", name },
                { "@quantity", quantity },
                { "@location", location }
            });
    }

        // Get Parts
        public List<Part> GetParts()
        {
            return ExecuteQuery("SELECT Id, PartNumber, Name, Quantity, Location FROM Part", reader => new Part
            {
                Id = reader.GetInt32(0),
                PartNumber = reader.GetString(1),
                Name = reader.GetString(2),
                Quantity = reader.GetInt32(3),
                Location = reader.GetString(4)
            });
        }

        // Helper Methods
        private void ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
    {
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      using var command = new SqliteCommand(query, connection);
      AddParameters(command, parameters);
      command.ExecuteNonQuery();
    }

    // Link a Part to a TV Model
    public void LinkPartToTVModel(int tvModelId, int partId)
    {
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      using var command = connection.CreateCommand();
      command.CommandText = @"
        INSERT INTO TVModelPart (TVModelId, PartId) 
        VALUES (@tvModelId, @partId)";
      command.Parameters.AddWithValue("@tvModelId", tvModelId);
      command.Parameters.AddWithValue("@partId", partId);
      command.ExecuteNonQuery();
    }

    // Unlink a Part from a TV Model
    public void UnlinkPartFromTVModel(int tvModelId, int partId)
    {
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      using var command = connection.CreateCommand();
      command.CommandText = "DELETE FROM TVModelPart WHERE TVModelId = @tvModelId AND PartId = @partId";
      command.Parameters.AddWithValue("@tvModelId", tvModelId);
      command.Parameters.AddWithValue("@partId", partId);
      command.ExecuteNonQuery();
    }

    // Get Parts for a Specific TV Model
    public List<Part> GetPartsForTVModel(int tvModelId)
    {
      var parts = new List<Part>();

      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      using var command = connection.CreateCommand();
      command.CommandText = @"
        SELECT p.Id, p.PartNumber, p.Name, p.Quantity, p.Location
        FROM Part p
        INNER JOIN TVModelPart tp ON p.Id = tp.PartId
        WHERE tp.TVModelId = @tvModelId";
      command.Parameters.AddWithValue("@tvModelId", tvModelId);

      using var reader = command.ExecuteReader();
      while (reader.Read())
      {
        parts.Add(new Part
        {
          Id = reader.GetInt32(0),
          PartNumber = reader.GetString(1),
          Name = reader.GetString(2),
          Quantity = reader.GetInt32(3),
          Location = reader.GetString(4)
        });
      }

      return parts;
    }

    private List<T> ExecuteQuery<T>(string query, Func<SqliteDataReader, T> map, Dictionary<string, object> parameters = null)
    {
      var results = new List<T>();

      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      using var command = new SqliteCommand(query, connection);
      AddParameters(command, parameters);

      using var reader = command.ExecuteReader();
      while (reader.Read())
      {
        results.Add(map(reader));
      }

      return results;
    }

    private void AddParameters(SqliteCommand command, Dictionary<string, object> parameters)
    {
      if (parameters == null) return;

      foreach (var param in parameters)
      {
        command.Parameters.AddWithValue(param.Key, param.Value);
      }
    }
  }
}