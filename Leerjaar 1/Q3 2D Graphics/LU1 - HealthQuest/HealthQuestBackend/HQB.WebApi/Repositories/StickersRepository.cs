using Dapper;
using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.Data.SqlClient;

namespace HQB.WebApi.Repositories;

public class StickersRepository : IStickersRepository
{
  private readonly string _connectionString;

  public StickersRepository(string connectionString)
  {
    _connectionString = connectionString;
  }

  public async Task<IEnumerable<Sticker>> GetAllStickersAsync()
  {
    using var connection = new SqlConnection(_connectionString);
    string query = "SELECT * FROM Sticker";
    return await connection.QueryAsync<Sticker>(query);
  }

  public async Task<Sticker?> GetStickerByIdAsync(Guid id)
  {
    using var connection = new SqlConnection(_connectionString);
    string query = "SELECT * FROM Sticker WHERE ID = @ID";
    return await connection.QuerySingleOrDefaultAsync<Sticker>(query, new { ID = id });
  }

  public async Task<IEnumerable<Sticker>> GetUnlockedStickersByPatientId(Guid patientId)
  {
    using var connection = new SqlConnection(_connectionString);
    string query = @"
      SELECT s.* 
      FROM Sticker s
      INNER JOIN StickerCollection sc ON s.ID = sc.StickerID
      WHERE sc.PatientID = @PatientID";
    return await connection.QueryAsync<Sticker>(query, new { PatientID = patientId });
  }

  public async Task AddStickerAsync(Sticker sticker)
  {
    using var connection = new SqlConnection(_connectionString);
    string query = "INSERT INTO Sticker (ID, Name) VALUES (@ID, @Name)";
    await connection.ExecuteAsync(query, sticker);
  }

  public async Task UpdateStickerAsync(Sticker sticker)
  {
    using var connection = new SqlConnection(_connectionString);
    string query = "UPDATE Sticker SET Name = @Name WHERE ID = @ID";
    await connection.ExecuteAsync(query, sticker);
  }

  public async Task DeleteStickerAsync(Guid id)
  {
    using var connection = new SqlConnection(_connectionString);
    string query = "DELETE FROM Sticker WHERE ID = @ID";
    await connection.ExecuteAsync(query, new { ID = id });
  }
}