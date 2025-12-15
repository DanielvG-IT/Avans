import * as bcrypt from "bcrypt";

/**
 * Password Utility
 * Centralized password hashing and comparison logic
 */
export class PasswordUtil {
  private static readonly SALT_ROUNDS = 10;

  /**
   * Hash a plain text password
   * @param plainPassword - The plain text password to hash
   * @returns The hashed password
   */
  static async hash(plainPassword: string): Promise<string> {
    return await bcrypt.hash(plainPassword, this.SALT_ROUNDS);
  }

  /**
   * Compare a plain text password with a hashed password
   * @param plainPassword - The plain text password
   * @param hashedPassword - The hashed password to compare against
   * @returns True if passwords match, false otherwise
   */
  static async compare(plainPassword: string, hashedPassword: string): Promise<boolean> {
    return await bcrypt.compare(plainPassword, hashedPassword);
  }
}
