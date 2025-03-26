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
    return await connection.QueryAsync<Sticker>("SELECT * FROM Sticker");
  }

  public async Task<Sticker?> GetStickerByIdAsync(Guid id)
  {
    using var connection = new SqlConnection(_connectionString);
    return await connection.QuerySingleOrDefaultAsync<Sticker>("SELECT * FROM Sticker WHERE ID = @ID", new { ID = id });
  }

  public async Task<IEnumerable<Sticker>> GetUnlockedStickersByPatientId(Guid patientId)
  {
    using var connection = new SqlConnection(_connectionString);
    return await connection.QueryAsync<Sticker>(
      @"SELECT s.* 
        FROM Sticker s
        INNER JOIN StickerCollection sc ON s.ID = sc.StickerID
        WHERE sc.PatientID = @PatientID", new { PatientID = patientId });
  }

  public async Task AddStickerAsync(Sticker sticker)
  {
    using var connection = new SqlConnection(_connectionString);
    await connection.ExecuteAsync(
      "INSERT INTO Sticker (ID, Name, ImageUrl) VALUES (@ID, @Name, @ImageUrl)", sticker);
  }

  public async Task UpdateStickerAsync(Sticker sticker)
  {
    using var connection = new SqlConnection(_connectionString);
    await connection.ExecuteAsync(
      "UPDATE Sticker SET Name = @Name, ImageUrl = @ImageUrl WHERE ID = @ID", sticker);
  }

  public async Task DeleteStickerAsync(Guid id)
  {
    using var connection = new SqlConnection(_connectionString);
    await connection.ExecuteAsync("DELETE FROM Sticker WHERE ID = @ID", new { ID = id });
  }
}