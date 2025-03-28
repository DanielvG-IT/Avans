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
    const string query = "SELECT * FROM Sticker";
    return await connection.QueryAsync<Sticker>(query);
  }

  public async Task<Sticker?> GetStickerByIdAsync(Guid id)
  {
    using var connection = new SqlConnection(_connectionString);
    const string query = "SELECT * FROM Sticker WHERE ID = @ID";
    return await connection.QuerySingleOrDefaultAsync<Sticker>(query, new { ID = id });
  }

  public async Task AddStickerAsync(Sticker sticker)
  {
    using var connection = new SqlConnection(_connectionString);
    const string query = "INSERT INTO Sticker (ID, Name) VALUES (@ID, @Name)";
    await connection.ExecuteAsync(query, sticker);
  }

  public async Task UpdateStickerAsync(Sticker sticker)
  {
    using var connection = new SqlConnection(_connectionString);
    const string query = "UPDATE Sticker SET Name = @Name WHERE ID = @ID";
    await connection.ExecuteAsync(query, sticker);
  }

  public async Task DeleteStickerAsync(Guid id)
  {
    using var connection = new SqlConnection(_connectionString);
    const string query = "DELETE FROM Sticker WHERE ID = @ID";
    await connection.ExecuteAsync(query, new { ID = id });
  }

  // This method retrieves all unlocked stickers for a specific patient.

  public async Task<IEnumerable<Sticker>> GetUnlockedStickersByPatientIdAsync(Guid patientId)
  {
    using var connection = new SqlConnection(_connectionString);
    const string query = @"
      SELECT s.ID, s.Name, sc.UnlockedDate
      FROM Sticker s
      INNER JOIN StickerCollection sc ON s.ID = sc.StickerID
      WHERE sc.PatientID = @PatientID
      ORDER BY sc.UnlockedDate DESC";
    return await connection.QueryAsync<Sticker>(query, new { PatientID = patientId });
  }

  public async Task<Sticker?> GetUnlockedStickerByIdAsync(Guid patientId, Guid stickerId)
  {
    using var connection = new SqlConnection(_connectionString);
    const string query = @"
      SELECT s.ID, s.Name, sc.UnlockedDate
      FROM Sticker s
      INNER JOIN StickerCollection sc ON s.ID = sc.StickerID
      WHERE sc.PatientID = @PatientID AND sc.StickerID = @StickerID";
    return await connection.QuerySingleOrDefaultAsync<Sticker>(query, new { PatientID = patientId, StickerID = stickerId });
  }

  public async Task AddStickerToPatientAsync(Guid patientId, Guid stickerId)
  {
    using var connection = new SqlConnection(_connectionString);
    const string query = @"
      IF NOT EXISTS (
        SELECT 1 
        FROM StickerCollection 
        WHERE PatientID = @PatientID AND StickerID = @StickerID
      )
      BEGIN
        INSERT INTO StickerCollection (PatientID, StickerID, UnlockedDate) 
        VALUES (@PatientID, @StickerID, GETDATE())
      END";
    await connection.ExecuteAsync(query, new { PatientID = patientId, StickerID = stickerId });
  }

  public async Task DeleteStickerFromPatientAsync(Guid patientId, Guid stickerId)
  {
    using var connection = new SqlConnection(_connectionString);
    const string query = @"
      DELETE FROM StickerCollection 
      WHERE PatientID = @PatientID AND StickerID = @StickerID";
    await connection.ExecuteAsync(query, new { PatientID = patientId, StickerID = stickerId });
  }

  public async Task<bool> IsStickerUnlockedByPatientAsync(Guid patientId, Guid stickerId)
  {
    using var connection = new SqlConnection(_connectionString);
    const string query = @"
      SELECT COUNT(1)
      FROM StickerCollection
      WHERE PatientID = @PatientID AND StickerID = @StickerID";
    return await connection.ExecuteScalarAsync<int>(query, new { PatientID = patientId, StickerID = stickerId }) > 0;
  }
}