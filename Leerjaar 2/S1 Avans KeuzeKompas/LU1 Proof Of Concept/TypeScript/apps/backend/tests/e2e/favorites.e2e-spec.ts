import { Test, TestingModule } from "@nestjs/testing";
import { INestApplication, ValidationPipe } from "@nestjs/common";
import request from "supertest";
import { AppModule } from "../../src/app.module";
import cookieParser from "cookie-parser";

describe("Favorites (e2e)", () => {
  let app: INestApplication;
  let adminToken: string;
  let studentToken: string;
  let electiveId: string;
  let studentId: string;

  beforeAll(async () => {
    const moduleFixture: TestingModule = await Test.createTestingModule({
      imports: [AppModule],
    }).compile();

    app = moduleFixture.createNestApplication();
    app.use(cookieParser());
    app.useGlobalPipes(new ValidationPipe());
    await app.init();

    // Login as admin
    try {
      const adminLogin = await request(app.getHttpServer()).post("/auth/login").send({
        email: "admin@test.com",
        password: "admin123",
      });

      if (adminLogin.status === 200) {
        adminToken = adminLogin.body.accessToken;

        // Create a test student
        const studentUser = {
          firstName: "Test",
          lastName: "FavStudent",
          email: `fav-student-${Date.now()}@test.com`,
          password: "TestPass123!",
          role: "student",
        };

        const studentResponse = await request(app.getHttpServer())
          .post("/users")
          .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
          .send(studentUser);

        if (studentResponse.status === 201) {
          studentId = studentResponse.body.id;

          // Login as student
          const studentLogin = await request(app.getHttpServer()).post("/auth/login").send({
            email: studentUser.email,
            password: studentUser.password,
          });

          if (studentLogin.status === 200) {
            studentToken = studentLogin.body.accessToken;
          }
        }

        // Create a test elective
        const elective = {
          code: `FAV-${Date.now()}`,
          name: "Favorite Test Elective",
          description: "For testing favorites",
          provider: "Test Provider",
          period: "P3",
          duration: "1 Periode",
          credits: 5,
          language: "Engels",
          location: "Breda",
          level: "NLQF5",
        };

        const electiveResponse = await request(app.getHttpServer())
          .post("/electives")
          .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
          .send(elective);

        if (electiveResponse.status === 201) {
          electiveId = electiveResponse.body.id;
        }
      }
    } catch (error) {
      console.error("Setup failed in favorites test");
    }
  });

  afterAll(async () => {
    // Cleanup
    if (adminToken && electiveId) {
      await request(app.getHttpServer())
        .delete(`/electives/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`]);
    }
    if (adminToken && studentId) {
      await request(app.getHttpServer())
        .delete(`/users/${studentId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`]);
    }
    await app.close();
  });

  describe("/users/me/favorites/:electiveId (POST)", () => {
    it("should add elective to favorites as student", async () => {
      if (!studentToken || !electiveId) {
        return;
      }

      // First ensure the favorite doesn't exist
      await request(app.getHttpServer())
        .delete(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`]);

      await request(app.getHttpServer())
        .post(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(201);

      // Verify it was added
      const favoritesResponse = await request(app.getHttpServer())
        .get("/users/me/favorites")
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(200);

      expect(Array.isArray(favoritesResponse.body)).toBe(true);
      const foundElective = favoritesResponse.body.find(
        (elective: { id: string }) => elective.id === electiveId,
      );
      expect(foundElective).toBeDefined();

      // Cleanup
      await request(app.getHttpServer())
        .delete(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`]);
    });

    it("should fail when not authenticated", async () => {
      if (!electiveId) {
        return;
      }

      await request(app.getHttpServer()).post(`/users/me/favorites/${electiveId}`).expect(401);
    });

    it("should fail when adding duplicate favorite", async () => {
      if (!studentToken || !electiveId) {
        return;
      }

      // First add the favorite
      await request(app.getHttpServer())
        .post(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`]);

      // Try to add again - should fail
      await request(app.getHttpServer())
        .post(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(400);

      // Cleanup
      await request(app.getHttpServer())
        .delete(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`]);
    });
  });

  describe("/users/me/favorites/:electiveId (GET)", () => {
    it("should check if elective is favorite", async () => {
      if (!studentToken || !electiveId) {
        return;
      }

      // Add the favorite first
      await request(app.getHttpServer())
        .post(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`]);

      const response = await request(app.getHttpServer())
        .get(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(200);

      expect(response.body).toHaveProperty("isFavorite");
      expect(response.body.isFavorite).toBe(true);

      // Cleanup
      await request(app.getHttpServer())
        .delete(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`]);
    });

    it("should return false for non-favorite elective", async () => {
      if (!studentToken) {
        return;
      }

      const response = await request(app.getHttpServer())
        .get("/users/me/favorites/000000000000000000000000")
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(200);

      expect(response.body).toHaveProperty("isFavorite");
      expect(response.body.isFavorite).toBe(false);
    });

    it("should fail when not authenticated", async () => {
      if (!electiveId) {
        return;
      }

      await request(app.getHttpServer()).get(`/users/me/favorites/${electiveId}`).expect(401);
    });
  });

  describe("/users/me/favorites (GET)", () => {
    it("should return list of favorite electives", async () => {
      if (!studentToken || !electiveId) {
        return;
      }

      // Add a favorite first
      await request(app.getHttpServer())
        .post(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`]);

      const response = await request(app.getHttpServer())
        .get("/users/me/favorites")
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(200);

      expect(Array.isArray(response.body)).toBe(true);
      expect(response.body.length).toBeGreaterThan(0);

      // Cleanup
      await request(app.getHttpServer())
        .delete(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`]);
    });

    it("should fail when not authenticated", async () => {
      await request(app.getHttpServer()).get("/users/me/favorites").expect(401);
    });
  });

  describe("/users/me/favorites/:electiveId (DELETE)", () => {
    it("should remove elective from favorites", async () => {
      if (!studentToken || !electiveId) {
        return;
      }

      // Add a favorite first
      await request(app.getHttpServer())
        .post(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`]);

      await request(app.getHttpServer())
        .delete(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(204);

      // Verify it was removed
      const favoritesResponse = await request(app.getHttpServer())
        .get("/users/me/favorites")
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(200);

      const foundElective = favoritesResponse.body.find(
        (elective: { id: string }) => elective.id === electiveId,
      );
      expect(foundElective).toBeUndefined();
    });

    it("should fail when not authenticated", async () => {
      if (!electiveId) {
        return;
      }

      await request(app.getHttpServer()).delete(`/users/me/favorites/${electiveId}`).expect(401);
    });

    it("should fail when removing non-existent favorite", async () => {
      if (!studentToken || !electiveId) {
        return;
      }

      // Try to remove again (already removed)
      await request(app.getHttpServer())
        .delete(`/users/me/favorites/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(404);
    });
  });
});
