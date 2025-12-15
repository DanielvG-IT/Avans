import { Test, TestingModule } from "@nestjs/testing";
import { INestApplication, ValidationPipe } from "@nestjs/common";
import request from "supertest";
import { AppModule } from "../../src/app.module";
import cookieParser from "cookie-parser";

describe("AuthController (e2e)", () => {
  let app: INestApplication;
  let adminToken: string;
  let testUserEmail: string;

  beforeAll(async () => {
    const moduleFixture: TestingModule = await Test.createTestingModule({
      imports: [AppModule],
    }).compile();

    app = moduleFixture.createNestApplication();
    app.use(cookieParser());
    app.useGlobalPipes(new ValidationPipe());
    await app.init();

    // Create a test admin user for authentication
    testUserEmail = `admin-${Date.now()}@test.com`;
    const adminUser = {
      firstName: "Admin",
      lastName: "User",
      email: testUserEmail,
      password: "AdminPass123!",
      role: "admin",
    };

    // First, login with existing admin or create one
    // Note: You may need to seed an initial admin user in your database
    try {
      const loginResponse = await request(app.getHttpServer()).post("/auth/login").send({
        email: "admin@test.com",
        password: "admin123",
      });

      if (loginResponse.status === 200) {
        adminToken = loginResponse.body.accessToken;

        // Create test user
        await request(app.getHttpServer())
          .post("/users")
          .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
          .send(adminUser);
      }
    } catch (_error) {
      // If login fails, we'll handle it in the tests
    }
  });

  afterAll(async () => {
    await app.close();
  });

  describe("/auth/login (POST)", () => {
    it("should successfully login with valid credentials", async () => {
      const loginDto = {
        email: testUserEmail,
        password: "AdminPass123!",
      };

      const response = await request(app.getHttpServer())
        .post("/auth/login")
        .send(loginDto)
        .expect(200);

      expect(response.body).toHaveProperty("accessToken");
      expect(typeof response.body.accessToken).toBe("string");
      expect(response.body.accessToken.length).toBeGreaterThan(0);

      // Check if cookie is set
      const cookies = response.headers["set-cookie"];
      expect(cookies).toBeDefined();
      if (Array.isArray(cookies)) {
        const accessTokenCookie = cookies.find((cookie: string) =>
          cookie.startsWith("ACCESSTOKEN="),
        );
        expect(accessTokenCookie).toBeDefined();
      }
    });

    it("should fail login with invalid email", async () => {
      const loginDto = {
        email: "nonexistent@test.com",
        password: "SomePassword123!",
      };

      await request(app.getHttpServer()).post("/auth/login").send(loginDto).expect(401);
    });

    it("should fail login with invalid password", async () => {
      const loginDto = {
        email: testUserEmail,
        password: "WrongPassword123!",
      };

      await request(app.getHttpServer()).post("/auth/login").send(loginDto).expect(401);
    });

    it("should fail login with missing email", async () => {
      const loginDto = {
        password: "AdminPass123!",
      };

      await request(app.getHttpServer()).post("/auth/login").send(loginDto).expect(401);
    });

    it("should fail login with missing password", async () => {
      const loginDto = {
        email: testUserEmail,
      };

      await request(app.getHttpServer()).post("/auth/login").send(loginDto).expect(401);
    });

    it("should fail login with empty credentials", async () => {
      await request(app.getHttpServer()).post("/auth/login").send({}).expect(401);
    });
  });
});
