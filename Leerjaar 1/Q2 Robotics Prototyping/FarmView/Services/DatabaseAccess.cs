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

  public void ReadMqttData(string typeData)
  {
    bool isTypeSensor = typeData == "sensor";

    using var connection = new SqlConnection(_connStr);
    {
      connection.Open();
      var command = connection.CreateCommand();
      command.Connection = connection;

      try
      {
        command.CommandText = isTypeSensor
          ? "SELECT SensorName, DataTimestamp, SensorData FROM SensorHistory WHERE SensorName LIKE 'cropbotics/sensor/'"
          : "SELECT CommandName, DataTimestamp, CommandData FROM CommandHistory WHERE CommandName LIKE 'cropbotics/command/'"; // TODO Add date range for SELECT

        var nameParam = isTypeSensor ? "@SensorName" : "@CommandName";
        var dataParam = isTypeSensor ? "@SensorData" : "@CommandData";

        var readerResult = command.ExecuteReader();

        if (readerResult.HasRows == false)
        {
          throw new Exception($"Reader got no results!");
        }

        // TODO Implement return of a list of sensor & command messages

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

  public void WriteMqttData(SimpleMqttMessage mqtt, string typeData)
  {
    string? _topic = mqtt.Topic;
    string? _data = mqtt.Message;
    bool isTypeSensor = typeData == "sensor";

    if (_topic == null)
    {
      throw new ArgumentNullException(_topic, "Topic cannot be null");
    }

    var sensorTypeMatch = Regex.Match(_topic, @"cropbotics/sensor/(\w+)", RegexOptions.IgnoreCase);

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

  public void WritePixelData(SimpleMqttMessage mqtt, string typeData, int pixelNumber)
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
}