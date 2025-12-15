/**
 * Test utilities and helper functions for e2e tests
 */

import request from "supertest";
import { INestApplication } from "@nestjs/common";

export interface TestUser {
  email: string;
  password: string;
  role: "student" | "teacher" | "admin";
  firstName: string;
  lastName: string;
}

export interface AuthTokens {
  accessToken: string;
}

/**
 * Login with credentials and return access token
 */
export async function loginUser(
  app: INestApplication,
  email: string,
  password: string,
): Promise<string | null> {
  try {
    const response = await request(app.getHttpServer())
      .post("/auth/login")
      .send({ email, password });

    if (response.status === 200 && response.body.accessToken) {
      return response.body.accessToken;
    }
  } catch (error) {
    console.error("Login failed:", error);
  }
  return null;
}

/**
 * Create a user and return the created user data
 */
export async function createUser(
  app: INestApplication,
  adminToken: string,
  user: TestUser,
): Promise<{ id: string; token?: string } | null> {
  try {
    const response = await request(app.getHttpServer())
      .post("/users")
      .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
      .send(user);

    if (response.status === 201) {
      const userId = response.body.id;

      // Login to get user token
      const token = await loginUser(app, user.email, user.password);

      return {
        id: userId,
        token: token || undefined,
      };
    }
  } catch (error) {
    console.error("User creation failed:", error);
  }
  return null;
}

/**
 * Create an elective and return the ID
 */
export async function createElective(
  app: INestApplication,
  adminToken: string,
  electiveData: Partial<{
    code: string;
    name: string;
    description: string;
    provider: string;
    period: string;
    duration: string;
    credits: number;
    language: string;
    location: string;
    level: string;
  }>,
): Promise<string | null> {
  try {
    const response = await request(app.getHttpServer())
      .post("/electives")
      .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
      .send(electiveData);

    if (response.status === 201) {
      return response.body.id;
    }
  } catch (error) {
    console.error("Elective creation failed:", error);
  }
  return null;
}

/**
 * Delete a user
 */
export async function deleteUser(
  app: INestApplication,
  adminToken: string,
  userId: string,
): Promise<boolean> {
  try {
    const response = await request(app.getHttpServer())
      .delete(`/users/${userId}`)
      .set("Cookie", [`ACCESSTOKEN=${adminToken}`]);

    return response.status === 204;
  } catch (error) {
    console.error("User deletion failed:", error);
    return false;
  }
}

/**
 * Delete an elective
 */
export async function deleteElective(
  app: INestApplication,
  adminToken: string,
  electiveId: string,
): Promise<boolean> {
  try {
    const response = await request(app.getHttpServer())
      .delete(`/electives/${electiveId}`)
      .set("Cookie", [`ACCESSTOKEN=${adminToken}`]);

    return response.status === 204;
  } catch (error) {
    console.error("Elective deletion failed:", error);
    return false;
  }
}

/**
 * Generate unique email for testing
 */
export function generateTestEmail(prefix: string): string {
  return `${prefix}-${Date.now()}@test.com`;
}

/**
 * Default admin credentials for testing
 */
export const DEFAULT_ADMIN = {
  email: "admin@test.com",
  password: "admin123",
};
