using Microsoft.Data.SqlClient;
using SimpleMqtt;

/// <summary>
/// Initializes a new instance of the <see cref="DatabaseAccess"/> class.
/// </summary>
/// <param name="connectionString">The connection string used to connect to the database.</param>
public class DatabaseAccess(string connectionString) : IDatabaseAccess
{
  private string _connStr { get; set; } = connectionString;


  /// <summary>
  /// Reads the mqtt messages from the database.
  /// </summary>
  /// <remarks>
  /// This method is responsible for getting sensor data from the database.
  /// </remarks>
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



  /// <summary>
  /// Writes a mqtt message to the database.
  /// </summary>
  /// <remarks>
  /// This method is responsible for persisting sensor data into the database.
  /// Ensure that the data is correctly formatted and validated before calling this method.
  /// </remarks>
  public void WriteMqttData(SimpleMqttMessage mqtt, string typeData)
  {
    string? _topic = (string?)mqtt.Topic;
    string? _data = (string?)mqtt.Message;
    bool isTypeSensor = typeData == "sensor";

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

        command.Parameters.AddWithValue(nameParam, _topic);
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


}