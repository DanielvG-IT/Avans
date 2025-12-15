export type ServiceError = {
  code: string; // stable programmatic code
  message?: string; // short human-readable string (optional)
  meta?: Record<string, any>; // optional extra info for logs (never send secrets)
};

export type Result<T> = { ok: true; data: T } | { ok: false; error: ServiceError };

export const ok = <T>(data: T): Result<T> => ({ ok: true, data });

export const err = <T = never>(
  code: string,
  message?: string,
  meta?: Record<string, any>,
): Result<T> => ({
  ok: false,
  error: { code, message, meta },
});
