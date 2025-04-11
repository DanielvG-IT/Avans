using HQB.WebApi.Models;

namespace HQB.WebApi.Interfaces;

/// <summary>
/// Interface for managing stickers and their associations with patients.
/// </summary>
public interface IStickersRepository
{
  /// <summary>
  /// Retrieves all stickers.
  /// </summary>
  /// <returns>A task that represents the asynchronous operation. The task result contains a collection of stickers.</returns>
  Task<IEnumerable<Sticker>> GetAllStickersAsync();

  /// <summary>
  /// Retrieves a sticker by its unique identifier.
  /// </summary>
  /// <param name="id">The unique identifier of the sticker.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the sticker if found, otherwise null.</returns>
  Task<Sticker?> GetStickerByIdAsync(Guid id);

  /// <summary>
  /// Adds a new sticker.
  /// </summary>
  /// <param name="sticker">The sticker to add.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task AddStickerAsync(Sticker sticker);

  /// <summary>
  /// Updates an existing sticker.
  /// </summary>
  /// <param name="sticker">The sticker with updated information.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task UpdateStickerAsync(Sticker sticker);

  /// <summary>
  /// Deletes a sticker by its unique identifier.
  /// </summary>
  /// <param name="id">The unique identifier of the sticker to delete.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task DeleteStickerAsync(Guid id);

  /// <summary>
  /// Retrieves all unlocked stickers for a specific patient.
  /// </summary>
  /// <param name="patientId">The unique identifier of the patient.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains a collection of unlocked stickers.</returns>
  Task<IEnumerable<Sticker>> GetUnlockedStickersByPatientIdAsync(Guid patientId);

  /// <summary>
  /// Retrieves a specific unlocked sticker for a patient by its unique identifier.
  /// </summary>
  /// <param name="patientId">The unique identifier of the patient.</param>
  /// <param name="stickerId">The unique identifier of the sticker.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the unlocked sticker if found, otherwise null.</returns>
  Task<Sticker?> GetUnlockedStickerByIdAsync(Guid patientId, Guid stickerId);

  /// <summary>
  /// Associates a sticker with a patient.
  /// </summary>
  /// <param name="patientId">The unique identifier of the patient.</param>
  /// <param name="stickerId">The unique identifier of the sticker to associate.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task AddStickerToPatientAsync(Guid patientId, Guid stickerId);

  /// <summary>
  /// Removes the association of a sticker from a patient.
  /// </summary>
  /// <param name="patientId">The unique identifier of the patient.</param>
  /// <param name="stickerId">The unique identifier of the sticker to remove.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task DeleteStickerFromPatientAsync(Guid patientId, Guid stickerId);

  /// <summary>
  /// Checks if a sticker is unlocked for a specific patient.
  /// </summary>
  /// <param name="patientId">The unique identifier of the patient.</param>
  /// <param name="stickerId">The unique identifier of the sticker.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains true if the sticker is unlocked, otherwise false.</returns>
  Task<bool> IsStickerUnlockedByPatientAsync(Guid patientId, Guid stickerId);
}
