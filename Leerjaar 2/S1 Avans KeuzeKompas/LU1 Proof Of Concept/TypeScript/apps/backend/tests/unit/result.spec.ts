import { ok, err } from "../../src/domain/result";

describe("Result", () => {
  describe("ok", () => {
    it("should create a successful result with data", () => {
      const result = ok("test data");

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toBe("test data");
      }
    });

    it("should work with objects", () => {
      const data = { id: "123", name: "Test" };
      const result = ok(data);

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toEqual(data);
      }
    });

    it("should work with arrays", () => {
      const data = [1, 2, 3];
      const result = ok(data);

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toEqual(data);
      }
    });

    it("should work with null", () => {
      const result = ok(null);

      expect(result.ok).toBe(true);
      if (result.ok) {
        expect(result.data).toBeNull();
      }
    });
  });

  describe("err", () => {
    it("should create an error result with code only", () => {
      const result = err("ERROR_CODE");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ERROR_CODE");
        expect(result.error.message).toBeUndefined();
        expect(result.error.meta).toBeUndefined();
      }
    });

    it("should create an error result with code and message", () => {
      const result = err("ERROR_CODE", "Error message");

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ERROR_CODE");
        expect(result.error.message).toBe("Error message");
      }
    });

    it("should create an error result with code, message, and meta", () => {
      const result = err("ERROR_CODE", "Error message", { userId: "123", action: "delete" });

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.code).toBe("ERROR_CODE");
        expect(result.error.message).toBe("Error message");
        expect(result.error.meta).toEqual({ userId: "123", action: "delete" });
      }
    });

    it("should handle empty metadata", () => {
      const result = err("ERROR_CODE", "Error message", {});

      expect(result.ok).toBe(false);
      if (!result.ok) {
        expect(result.error.meta).toEqual({});
      }
    });
  });

  describe("type narrowing", () => {
    it("should allow accessing data when ok is true", () => {
      const result = ok(42);

      if (result.ok) {
        const value: number = result.data;
        expect(value).toBe(42);
      }
    });

    it("should allow accessing error when ok is false", () => {
      const result = err("TEST_ERROR", "Test message");

      if (!result.ok) {
        const code: string = result.error.code;
        expect(code).toBe("TEST_ERROR");
      }
    });
  });
});
