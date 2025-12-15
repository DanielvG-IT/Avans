import { PasswordUtil } from "../../src/application/utils/password.util";
import * as bcrypt from "bcrypt";

jest.mock("bcrypt");

describe("PasswordUtil", () => {
  const mockBcrypt = bcrypt as jest.Mocked<typeof bcrypt>;

  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe("hash", () => {
    it("should hash a password with the correct salt rounds", async () => {
      const plainPassword = "mySecurePassword123";
      const hashedPassword = "hashedPassword123";

      mockBcrypt.hash.mockResolvedValue(hashedPassword as never);

      const result = await PasswordUtil.hash(plainPassword);

      expect(result).toBe(hashedPassword);
      expect(mockBcrypt.hash).toHaveBeenCalledWith(plainPassword, 10);
    });

    it("should handle empty password", async () => {
      const plainPassword = "";
      const hashedPassword = "hashedEmptyPassword";

      mockBcrypt.hash.mockResolvedValue(hashedPassword as never);

      const result = await PasswordUtil.hash(plainPassword);

      expect(result).toBe(hashedPassword);
      expect(mockBcrypt.hash).toHaveBeenCalledWith(plainPassword, 10);
    });

    it("should handle special characters in password", async () => {
      const plainPassword = "P@ssw0rd!#$%^&*()";
      const hashedPassword = "hashedSpecialPassword";

      mockBcrypt.hash.mockResolvedValue(hashedPassword as never);

      const result = await PasswordUtil.hash(plainPassword);

      expect(result).toBe(hashedPassword);
      expect(mockBcrypt.hash).toHaveBeenCalledWith(plainPassword, 10);
    });
  });

  describe("compare", () => {
    it("should return true when passwords match", async () => {
      const plainPassword = "mySecurePassword123";
      const hashedPassword = "hashedPassword123";

      mockBcrypt.compare.mockResolvedValue(true as never);

      const result = await PasswordUtil.compare(plainPassword, hashedPassword);

      expect(result).toBe(true);
      expect(mockBcrypt.compare).toHaveBeenCalledWith(plainPassword, hashedPassword);
    });

    it("should return false when passwords do not match", async () => {
      const plainPassword = "wrongPassword";
      const hashedPassword = "hashedPassword123";

      mockBcrypt.compare.mockResolvedValue(false as never);

      const result = await PasswordUtil.compare(plainPassword, hashedPassword);

      expect(result).toBe(false);
      expect(mockBcrypt.compare).toHaveBeenCalledWith(plainPassword, hashedPassword);
    });

    it("should handle empty plain password", async () => {
      const plainPassword = "";
      const hashedPassword = "hashedPassword123";

      mockBcrypt.compare.mockResolvedValue(false as never);

      const result = await PasswordUtil.compare(plainPassword, hashedPassword);

      expect(result).toBe(false);
      expect(mockBcrypt.compare).toHaveBeenCalledWith(plainPassword, hashedPassword);
    });

    it("should handle empty hashed password", async () => {
      const plainPassword = "mySecurePassword123";
      const hashedPassword = "";

      mockBcrypt.compare.mockResolvedValue(false as never);

      const result = await PasswordUtil.compare(plainPassword, hashedPassword);

      expect(result).toBe(false);
      expect(mockBcrypt.compare).toHaveBeenCalledWith(plainPassword, hashedPassword);
    });
  });
});
