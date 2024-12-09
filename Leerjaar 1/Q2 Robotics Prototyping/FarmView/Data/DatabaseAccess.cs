using Microsoft.Data.SqlClient;
using SimpleMqtt;
using System;


static class DatabaseAccess
{
  private const string hostname = "aei-sql2.avans.nl";
  private const string port = "1443";
  private const string dbname = "DB2226789";
  private const string username = "ITI2226789";
  private const string passWrd = "H7lcQ2F0";
  private const bool trustcert = true;
  private readonly static string connStr = $"Server={hostname},{port};Database={dbname};UID={username};password={passWrd};TrustServerCertificate={trustcert};";



  public static void ExecuteQuery(string query) { }

  public static void ReadSensorHistory() { }
  public static void ReadCommandHistory() { }


  /// <summary>
  /// Writes the sensor data to the database.
  /// </summary>
  /// <remarks>
  /// This method is responsible for persisting sensor data into the database.
  /// Ensure that the data is correctly formatted and validated before calling this method.
  /// </remarks>
  public static void WriteSensorData(SimpleMqttMessage mqtt)
  {
    string? _topic = (string?)mqtt.Topic;
    string? _data = (string?)mqtt.Message;

    using (var connection = new SqlConnection(connStr))
    {
      connection.Open();

      var command = connection.CreateCommand();
      var transaction = connection.BeginTransaction();

      command.Connection = connection;
      command.Transaction = transaction;

      try
      {
        command.CommandText = "INSERT INTO SensorData (SensorName, DataTimestamp, SensorData) values (@SensorName, @DataTimestamp, @SensorData)";
        command.Parameters.AddWithValue("@SensorName", $"{_topic}");
        command.Parameters.AddWithValue("@DataTimestamp", DateTime.Now);
        command.Parameters.AddWithValue("@SensorData", $"{_data}");

        int nrOfRowsAffected = command.ExecuteNonQuery();
        Console.WriteLine($"Added sensordata! Rows Affected: {nrOfRowsAffected}");

        transaction.Commit();
      }

      catch (Exception ex)
      {
        Console.WriteLine($"Error writing data to database: {ex}");
        Console.WriteLine($"Trying transaction rollback...");

        try
        {
          transaction.Rollback();
          Console.WriteLine($"Transaction rollback succeeded!");
        }

        catch (Exception rollbackEx)
        {
          Console.WriteLine($"Error writing data to database: {rollbackEx}");
          Console.WriteLine($"Transaction rollback failed!");
        }
      }

      finally
      {
        connection.Close();
      }
    }
  }
}