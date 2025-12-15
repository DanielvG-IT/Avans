/**
 * ID Normalization Utility
 *
 * Handles conversion of various ID types (MongoDB ObjectId, strings, etc.)
 * to normalized string format for consistent comparison and storage.
 */

/**
 * Normalizes a single ID value to a string.
 * Handles MongoDB ObjectId with toHexString() and other ID types.
 *
 * @param val - The ID value to normalize (can be string, ObjectId, or any object)
 * @returns Normalized string representation of the ID
 *
 * @example
 * normalizeId("507f1f77bcf86cd799439011") // "507f1f77bcf86cd799439011"
 * normalizeId(new ObjectId("507f1f77bcf86cd799439011")) // "507f1f77bcf86cd799439011"
 */
export function normalizeId(val: unknown): string {
  if (typeof val === "string") {
    return val;
  }

  // Handle MongoDB ObjectId with toHexString method
  if (val && typeof (val as { toHexString?: unknown }).toHexString === "function") {
    return (val as { toHexString: () => string }).toHexString();
  }

  // Fallback to toString and extract hex if present
  if (val && typeof (val as { toString?: unknown }).toString === "function") {
    const str = (val as { toString: () => string }).toString();
    const hexMatch = str.match(/([0-9a-fA-F]{24})/);
    return hexMatch ? hexMatch[1] : str;
  }

  return String(val);
}

/**
 * Normalizes an array of ID values to strings.
 *
 * @param vals - Array of ID values to normalize
 * @returns Array of normalized string IDs
 *
 * @example
 * normalizeIds([objectId1, objectId2, "stringId"]) // ["507f...", "507f...", "stringId"]
 */
export function normalizeIds(vals: unknown[]): string[] {
  return vals.map((v) => normalizeId(v));
}
