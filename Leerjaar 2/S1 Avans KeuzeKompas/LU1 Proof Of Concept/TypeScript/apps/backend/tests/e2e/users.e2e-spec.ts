import { Test, TestingModule } from "@nestjs/testing";
import { INestApplication, ValidationPipe } from "@nestjs/common";
import request from "supertest";
import { AppModule } from "../../src/app.module";
import cookieParser from "cookie-parser";

describe("UserController (e2e)", () => {
  let app: INestApplication;
  let adminToken: string;
  let studentToken: string;
  let teacherToken: string;
  let createdUserId: string;
  let createdStudentId: string;

  beforeAll(async () => {
    const moduleFixture: TestingModule = await Test.createTestingModule({
      imports: [AppModule],
    }).compile();

    app = moduleFixture.createNestApplication();
    app.use(cookieParser());
    app.useGlobalPipes(new ValidationPipe());
    await app.init();

    // Login as admin to get token
    // Note: This assumes you have seeded an admin user
    try {
      const adminLogin = await request(app.getHttpServer()).post("/auth/login").send({
        email: "admin@test.com",
        password: "admin123",
      });

      if (adminLogin.status === 200) {
        adminToken = adminLogin.body.accessToken;
      }
    } catch (error) {
      console.error("Admin login failed in test setup");
    }
  });

  afterAll(async () => {
    // Cleanup: delete created test users
    if (adminToken && createdUserId) {
      await request(app.getHttpServer())
        .delete(`/users/${createdUserId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`]);
    }
    if (adminToken && createdStudentId) {
      await request(app.getHttpServer())
        .delete(`/users/${createdStudentId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`]);
    }
    await app.close();
  });

  describe("/users/me (GET)", () => {
    it("should return current user profile when authenticated", async () => {
      if (!adminToken) {
        return; // Skip if no token
      }

      const response = await request(app.getHttpServer())
        .get("/users/me")
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .expect(200);

      expect(response.body).toHaveProperty("id");
      expect(response.body).toHaveProperty("email");
      expect(response.body).toHaveProperty("firstName");
      expect(response.body).toHaveProperty("lastName");
      expect(response.body).toHaveProperty("role");
      expect(response.body).not.toHaveProperty("passwordHash");
    });

    it("should fail when not authenticated", async () => {
      await request(app.getHttpServer()).get("/users/me").expect(401);
    });
  });

  describe("/users (GET)", () => {
    it("should return all users for admin", async () => {
      if (!adminToken) {
        return;
      }

      const response = await request(app.getHttpServer())
        .get("/users")
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .expect(200);

      expect(Array.isArray(response.body)).toBe(true);
      if (response.body.length > 0) {
        expect(response.body[0]).toHaveProperty("id");
        expect(response.body[0]).toHaveProperty("email");
        expect(response.body[0]).not.toHaveProperty("passwordHash");
      }
    });

    it("should fail when not authenticated", async () => {
      await request(app.getHttpServer()).get("/users").expect(401);
    });
  });

  describe("/users (POST)", () => {
    it("should create a new student user as admin", async () => {
      if (!adminToken) {
        return;
      }

      const newUser = {
        firstName: "Test",
        lastName: "Student",
        email: `student-${Date.now()}@test.com`,
        password: "TestPass123!",
        role: "student",
      };

      const response = await request(app.getHttpServer())
        .post("/users")
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .send(newUser)
        .expect(201);

      expect(response.body).toHaveProperty("id");
      expect(response.body.email).toBe(newUser.email);
      expect(response.body.firstName).toBe(newUser.firstName);
      expect(response.body.lastName).toBe(newUser.lastName);
      expect(response.body.role).toBe("student");
      expect(response.body).toHaveProperty("favorites");
      expect(response.body).not.toHaveProperty("passwordHash");

      createdStudentId = response.body.id;

      // Login with new user to get student token
      const loginResponse = await request(app.getHttpServer()).post("/auth/login").send({
        email: newUser.email,
        password: newUser.password,
      });

      if (loginResponse.status === 200) {
        studentToken = loginResponse.body.accessToken;
      }
    });

    it("should create a new teacher user as admin", async () => {
      if (!adminToken) {
        return;
      }

      const newUser = {
        firstName: "Test",
        lastName: "Teacher",
        email: `teacher-${Date.now()}@test.com`,
        password: "TestPass123!",
        role: "teacher",
      };

      const response = await request(app.getHttpServer())
        .post("/users")
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .send(newUser)
        .expect(201);

      expect(response.body).toHaveProperty("id");
      expect(response.body.email).toBe(newUser.email);
      expect(response.body.role).toBe("teacher");
      expect(response.body).not.toHaveProperty("favorites");
      expect(response.body).not.toHaveProperty("passwordHash");

      createdUserId = response.body.id;

      // Login with new user to get teacher token
      const loginResponse = await request(app.getHttpServer()).post("/auth/login").send({
        email: newUser.email,
        password: newUser.password,
      });

      if (loginResponse.status === 200) {
        teacherToken = loginResponse.body.accessToken;
      }
    });

    it("should fail to create user with missing required fields", async () => {
      if (!adminToken) {
        return;
      }

      const invalidUser = {
        firstName: "Test",
        email: `test-${Date.now()}@test.com`,
        // Missing lastName, password, role
      };

      await request(app.getHttpServer())
        .post("/users")
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .send(invalidUser)
        .expect(400);
    });

    it("should fail when not authenticated", async () => {
      const newUser = {
        firstName: "Test",
        lastName: "User",
        email: `test-${Date.now()}@test.com`,
        password: "TestPass123!",
        role: "student",
      };

      await request(app.getHttpServer()).post("/users").send(newUser).expect(401);
    });
  });

  describe("/users/:userId (PATCH)", () => {
    it("should update user as admin", async () => {
      if (!adminToken || !createdUserId) {
        return;
      }

      const updateData = {
        firstName: "Updated",
        lastName: "Name",
      };

      const response = await request(app.getHttpServer())
        .patch(`/users/${createdUserId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .send(updateData)
        .expect(200);

      expect(response.body.firstName).toBe(updateData.firstName);
      expect(response.body.lastName).toBe(updateData.lastName);
    });

    it("should fail when not authenticated", async () => {
      if (!createdUserId) {
        return;
      }

      await request(app.getHttpServer())
        .patch(`/users/${createdUserId}`)
        .send({ firstName: "Test" })
        .expect(401);
    });
  });

  describe("/users/:userId (DELETE)", () => {
    it("should delete user as admin", async () => {
      if (!adminToken) {
        return;
      }

      // Create a user to delete
      const userToDelete = {
        firstName: "Delete",
        lastName: "Me",
        email: `delete-${Date.now()}@test.com`,
        password: "TestPass123!",
        role: "student",
      };

      const createResponse = await request(app.getHttpServer())
        .post("/users")
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .send(userToDelete);

      const userId = createResponse.body.id;

      await request(app.getHttpServer())
        .delete(`/users/${userId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .expect(204);
    });

    it("should fail when not authenticated", async () => {
      await request(app.getHttpServer()).delete("/users/someId").expect(401);
    });
  });

  describe("/users/me/favorites (GET)", () => {
    it("should return favorites for student", async () => {
      if (!studentToken) {
        return;
      }

      const response = await request(app.getHttpServer())
        .get("/users/me/favorites")
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(200);

      expect(Array.isArray(response.body)).toBe(true);
    });

    it("should fail when not authenticated", async () => {
      await request(app.getHttpServer()).get("/users/me/favorites").expect(401);
    });

    it("should fail for non-student users", async () => {
      if (!teacherToken) {
        return;
      }

      await request(app.getHttpServer())
        .get("/users/me/favorites")
        .set("Cookie", [`ACCESSTOKEN=${teacherToken}`])
        .expect(403);
    });
  });

  describe("/users/me/electives (GET)", () => {
    it("should return electives for teacher", async () => {
      if (!teacherToken) {
        return;
      }

      const response = await request(app.getHttpServer())
        .get("/users/me/electives")
        .set("Cookie", [`ACCESSTOKEN=${teacherToken}`])
        .expect(200);

      expect(Array.isArray(response.body)).toBe(true);
    });

    it("should fail when not authenticated", async () => {
      await request(app.getHttpServer()).get("/users/me/electives").expect(401);
    });

    it("should fail for non-teacher users", async () => {
      if (!studentToken) {
        return;
      }

      await request(app.getHttpServer())
        .get("/users/me/electives")
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(403);
    });
  });
});
