import { normalizeId, normalizeIds } from "../../src/application/utils/id-normalizer.util";

describe("id-normalizer utility", () => {
  describe("normalizeId", () => {
    it("should return the same value for a string ID", () => {
      const id = "507f1f77bcf86cd799439011";
      expect(normalizeId(id)).toBe(id);
    });

    it("should handle MongoDB ObjectId with toHexString method", () => {
      const mockObjectId = {
        toHexString: () => "507f1f77bcf86cd799439011",
      };

      expect(normalizeId(mockObjectId)).toBe("507f1f77bcf86cd799439011");
    });

    it("should extract hex from toString output", () => {
      const mockObjectId = {
        toString: () => "ObjectId('507f1f77bcf86cd799439011')",
      };

      expect(normalizeId(mockObjectId)).toBe("507f1f77bcf86cd799439011");
    });

    it("should use toString as fallback when no hex pattern found", () => {
      const mockValue = {
        toString: () => "some-custom-id",
      };

      expect(normalizeId(mockValue)).toBe("some-custom-id");
    });

    it("should handle numeric values", () => {
      expect(normalizeId(123)).toBe("123");
    });

    it("should handle null and undefined", () => {
      expect(normalizeId(null)).toBe("null");
      expect(normalizeId(undefined)).toBe("undefined");
    });

    it("should handle boolean values", () => {
      expect(normalizeId(true)).toBe("true");
      expect(normalizeId(false)).toBe("false");
    });

    it("should handle empty string", () => {
      expect(normalizeId("")).toBe("");
    });

    it("should prioritize toHexString over toString", () => {
      const mockObjectId = {
        toHexString: () => "507f1f77bcf86cd799439011",
        toString: () => "should-not-use-this",
      };

      expect(normalizeId(mockObjectId)).toBe("507f1f77bcf86cd799439011");
    });

    it("should handle hex string in different positions", () => {
      const mockValue = {
        toString: () => "prefix-507f1f77bcf86cd799439011-suffix",
      };

      expect(normalizeId(mockValue)).toBe("507f1f77bcf86cd799439011");
    });
  });

  describe("normalizeIds", () => {
    it("should normalize an array of string IDs", () => {
      const ids = ["507f1f77bcf86cd799439011", "507f191e810c19729de860ea"];
      expect(normalizeIds(ids)).toEqual(ids);
    });

    it("should normalize an array of ObjectId-like objects", () => {
      const ids = [
        { toHexString: () => "507f1f77bcf86cd799439011" },
        { toHexString: () => "507f191e810c19729de860ea" },
      ];

      expect(normalizeIds(ids)).toEqual(["507f1f77bcf86cd799439011", "507f191e810c19729de860ea"]);
    });

    it("should normalize mixed array of strings and objects", () => {
      const ids = [
        "507f1f77bcf86cd799439011",
        { toHexString: () => "507f191e810c19729de860ea" },
        { toString: () => "ObjectId('507f1f77bcf86cd799439012')" },
      ];

      expect(normalizeIds(ids)).toEqual([
        "507f1f77bcf86cd799439011",
        "507f191e810c19729de860ea",
        "507f1f77bcf86cd799439012",
      ]);
    });

    it("should handle empty array", () => {
      expect(normalizeIds([])).toEqual([]);
    });

    it("should handle array with null and undefined", () => {
      const ids = [null, undefined, "507f1f77bcf86cd799439011"];
      expect(normalizeIds(ids)).toEqual(["null", "undefined", "507f1f77bcf86cd799439011"]);
    });

    it("should handle array with numbers", () => {
      const ids = [123, 456, "789"];
      expect(normalizeIds(ids)).toEqual(["123", "456", "789"]);
    });

    it("should handle single element array", () => {
      const ids = ["507f1f77bcf86cd799439011"];
      expect(normalizeIds(ids)).toEqual(["507f1f77bcf86cd799439011"]);
    });
  });
});
