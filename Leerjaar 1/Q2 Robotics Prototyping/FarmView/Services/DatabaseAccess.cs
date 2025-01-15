using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using SimpleMqtt;

/// <summary>
/// Initializes a new instance of the <see cref="DatabaseAccess"/> class.
/// </summary>
/// <param name="connectionString">The connection string used to connect to the database.</param>
public class DatabaseAccess(string connectionString) : IDatabaseAccess
{
  private string _connStr { get; set; } = connectionString;

  public List<Sensor> ReadSensorData(DateTime date)
  {
    var sensors = new List<Sensor>();

    try
    {
      using var connection = new SqlConnection(_connStr);
      connection.Open();
      using var sqlcommand = connection.CreateCommand();
      sqlcommand.CommandText = @"
            SELECT SensorName, DataTimestamp, SensorData 
            FROM SensorHistory 
            WHERE CONVERT(DATE, DataTimestamp) = @DataTimestamp";

      sqlcommand.Parameters.AddWithValue("@DataTimestamp", date);

      using var reader = sqlcommand.ExecuteReader();

      if (!reader.HasRows)
      {
        Console.WriteLine("DEBUG: No sensor data found for the given date.");
        return sensors;
      }

      while (reader.Read())
      {
        sensors.Add(new Sensor
        {
          SensorName = reader.GetString(0),
          DataTimestamp = reader.GetDateTime(1),
          SensorData = reader.GetString(2)
        });
      }
      Console.WriteLine($"DEBUG: Found {sensors.Count} readings for date {date.Date:yyyy-MM-dd}");
    }
    catch (SqlException sqlEx)
    {
      Console.WriteLine($"Database error: {sqlEx.Message}");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Unexpected error: {ex.Message}");
    }

    return sensors;
  }

  public List<Command> ReadCommandData(DateTime date)
  {
    var commands = new List<Command>();

    try
    {
      using var connection = new SqlConnection(_connStr);
      connection.Open();
      using var command = connection.CreateCommand();
      command.CommandText = @"
            SELECT CommandName, DataTimestamp, CommandData 
            FROM CommandHistory 
            WHERE CONVERT(DATE, DataTimestamp) = @DataTimestamp";

      command.Parameters.AddWithValue("@DataTimestamp", date);

      using var reader = command.ExecuteReader();

      if (!reader.HasRows)
      {
        Console.WriteLine("DEBUG: No command data found for the given date.");
        return commands;
      }

      while (reader.Read())
      {
        commands.Add(new Command
        {
          CommandName = reader.GetString(0),
          DataTimestamp = reader.GetDateTime(1),
          CommandData = reader.GetString(2)
        });
      }
      Console.WriteLine($"DEBUG: Found {commands.Count} commands for date {date.Date:yyyy-MM-dd}");
      return commands;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Unexpected error: {ex.Message}");
      return commands;
    }

  }

  public List<Pixel> ReadPixelData(DateTime date)
  {
    var pixels = new List<Pixel>();
    using var connection = new SqlConnection(_connStr);
    {
      connection.Open();
      using var command = connection.CreateCommand();
      command.CommandText = @"
            SELECT PixelNumber, DataTimestamp, PixelData 
            FROM PixelHistory 
            WHERE CONVERT(date, DataTimestamp) = @DataTimestamp";

      command.Parameters.AddWithValue("@DataTimestamp", date.Date);

      using var reader = command.ExecuteReader();
      while (reader.Read())
      {
        pixels.Add(new Pixel
        {
          PixelNumber = reader.GetInt32(0),
          DataTimestamp = reader.GetDateTime(1),
          PixelData = reader.GetString(2)
        });
      }
      connection.Close();
    }
    Console.WriteLine($"Found {pixels.Count} pixels for date {date.Date:yyyy-MM-dd}");
    return pixels;
  }

  public void WriteMqttData(SimpleMqttMessage mqtt, string typeData)
  {
    string? _topic = mqtt.Topic;
    string? _data = mqtt.Message;
    bool isTypeSensor = typeData == "sensor";

    if (_topic == null)
    {
      throw new ArgumentNullException(_topic, "Topic cannot be null");
    }

    var sensorTypeMatch = Regex.Match(_topic, @"CropBotics/sensor/(\w+)", RegexOptions.IgnoreCase);

    if (!sensorTypeMatch.Success)
    {
      throw new ArgumentException("Invalid topic format for sensor data");
    }

    string sensorType = sensorTypeMatch.Groups[1].Value;

    using var connection = new SqlConnection(_connStr);
    {
      connection.Open();
      var command = connection.CreateCommand();
      command.Connection = connection;

      try
      {
        command.CommandText = isTypeSensor
          ? "INSERT INTO SensorHistory (SensorName, DataTimestamp, SensorData) VALUES (@SensorName, @DataTimestamp, @SensorData)"
          : "INSERT INTO CommandHistory (CommandName, DataTimestamp, CommandData) VALUES (@CommandName, @DataTimestamp, @CommandData)";

        var nameParam = isTypeSensor ? "@SensorName" : "@CommandName";
        var dataParam = isTypeSensor ? "@SensorData" : "@CommandData";

        command.Parameters.AddWithValue(nameParam, sensorType);
        command.Parameters.AddWithValue("@DataTimestamp", DateTime.Now);
        command.Parameters.AddWithValue(dataParam, _data);

        int nrOfRowsAffected = command.ExecuteNonQuery();
        Console.WriteLine($"Added {typeData} data! Rows Affected: {nrOfRowsAffected}");

        if (nrOfRowsAffected == 0)
        {
          throw new Exception($"{nrOfRowsAffected} rows affected!");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error writing data to database: {ex}");
      }
      finally
      {
        connection.Close();
      }
    }
  }

  public void WritePixelData(SimpleMqttMessage mqtt, int pixelNumber)
  {
    using var connection = new SqlConnection(_connStr);
    {
      connection.Open();
      var command = connection.CreateCommand();
      command.Connection = connection;
      try
      {
        command.CommandText = "INSERT INTO PixelHistory (PixelNumber, DataTimestamp, PixelData) VALUES (@PixelNumber, @DataTimestamp, @PixelData)";

        command.Parameters.AddWithValue("@PixelNumber", pixelNumber);
        command.Parameters.AddWithValue("@DataTimestamp", DateTime.Today);
        command.Parameters.AddWithValue("@PixelData", mqtt.Message);
        int nrOfRowsAffected = command.ExecuteNonQuery();
        Console.WriteLine($"Added pixel data! Rows Affected: {nrOfRowsAffected}");

        if (nrOfRowsAffected == 0)
        {
          throw new Exception($"{nrOfRowsAffected} rows affected!");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error writing data to database: {ex}");
      }
      finally
      {
        connection.Close();
      }
    }
  }

}