using HQB.WebApi.Models;

namespace HQB.WebApi.Interfaces;
/// <summary>
/// Interface for managing sticker-related operations.
/// </summary>
public interface IStickersRepository
{
  /// <summary>
  /// Retrieves all stickers asynchronously.
  /// </summary>
  /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of stickers.</returns>
  Task<IEnumerable<Sticker>> GetAllStickersAsync();

  /// <summary>
  /// Retrieves a sticker by its unique identifier asynchronously.
  /// </summary>
  /// <param name="id">The unique identifier of the sticker.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the sticker if found; otherwise, null.</returns>
  Task<Sticker?> GetStickerByIdAsync(Guid id);

  /// <summary>
  /// Retrieves all unlocked stickers for a specific patient asynchronously.
  /// </summary>
  /// <param name="patientId">The unique identifier of the patient.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of unlocked stickers.</returns>
  Task<IEnumerable<Sticker>> GetUnlockedStickersByPatientId(Guid patientId);

  /// <summary>
  /// Adds a new sticker asynchronously.
  /// </summary>
  /// <param name="sticker">The sticker to add.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task AddStickerAsync(Sticker sticker);

  /// <summary>
  /// Updates an existing sticker asynchronously.
  /// </summary>
  /// <param name="sticker">The sticker to update.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task UpdateStickerAsync(Sticker sticker);

  /// <summary>
  /// Deletes a sticker by its unique identifier asynchronously.
  /// </summary>
  /// <param name="id">The unique identifier of the sticker to delete.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task DeleteStickerAsync(Guid id);
}