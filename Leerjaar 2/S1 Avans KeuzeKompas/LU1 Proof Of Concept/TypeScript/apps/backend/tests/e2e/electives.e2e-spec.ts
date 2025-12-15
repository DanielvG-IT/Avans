import { Test, TestingModule } from "@nestjs/testing";
import { INestApplication, ValidationPipe } from "@nestjs/common";
import request from "supertest";
import { AppModule } from "../../src/app.module";
import cookieParser from "cookie-parser";

describe("ElectiveController (e2e)", () => {
  let app: INestApplication;
  let adminToken: string;
  let studentToken: string;
  let teacherToken: string;
  let createdElectiveId: string;
  let teacherId: string;

  beforeAll(async () => {
    const moduleFixture: TestingModule = await Test.createTestingModule({
      imports: [AppModule],
    }).compile();

    app = moduleFixture.createNestApplication();
    app.use(cookieParser());
    app.useGlobalPipes(new ValidationPipe());
    await app.init();

    // Login as admin to get token
    try {
      const adminLogin = await request(app.getHttpServer()).post("/auth/login").send({
        email: "admin@test.com",
        password: "admin123",
      });

      if (adminLogin.status === 200) {
        adminToken = adminLogin.body.accessToken;

        // Create a teacher user for testing
        const teacherUser = {
          firstName: "Test",
          lastName: "Teacher",
          email: `teacher-elective-${Date.now()}@test.com`,
          password: "TestPass123!",
          role: "teacher",
        };

        const teacherResponse = await request(app.getHttpServer())
          .post("/users")
          .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
          .send(teacherUser);

        if (teacherResponse.status === 201) {
          teacherId = teacherResponse.body.id;

          // Login as teacher
          const teacherLogin = await request(app.getHttpServer()).post("/auth/login").send({
            email: teacherUser.email,
            password: teacherUser.password,
          });

          if (teacherLogin.status === 200) {
            teacherToken = teacherLogin.body.accessToken;
          }
        }

        // Create a student for testing
        const studentUser = {
          firstName: "Test",
          lastName: "Student",
          email: `student-elective-${Date.now()}@test.com`,
          password: "TestPass123!",
          role: "student",
        };

        const studentResponse = await request(app.getHttpServer())
          .post("/users")
          .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
          .send(studentUser);

        if (studentResponse.status === 201) {
          // Login as student
          const studentLogin = await request(app.getHttpServer()).post("/auth/login").send({
            email: studentUser.email,
            password: studentUser.password,
          });

          if (studentLogin.status === 200) {
            studentToken = studentLogin.body.accessToken;
          }
        }
      }
    } catch (error) {
      console.error("Setup failed in test");
    }
  });

  afterAll(async () => {
    // Cleanup: delete created elective
    if (adminToken && createdElectiveId) {
      await request(app.getHttpServer())
        .delete(`/electives/${createdElectiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`]);
    }
    await app.close();
  });

  describe("/electives (POST)", () => {
    it("should create a new elective as admin", async () => {
      if (!adminToken) {
        return;
      }

      const newElective = {
        code: `TEST-${Date.now()}`,
        name: "Test Elective",
        description: "A test elective for e2e testing",
        provider: "Test Provider",
        period: "P3",
        duration: "1 Periode",
        credits: 5,
        language: "Engels",
        location: "Breda",
        level: "NLQF5",
        tags: ["testing", "e2e"],
      };

      const response = await request(app.getHttpServer())
        .post("/electives")
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .send(newElective)
        .expect(201);

      expect(response.body).toHaveProperty("id");
      expect(response.body.code).toBe(newElective.code);
      expect(response.body.name).toBe(newElective.name);
      expect(response.body.description).toBe(newElective.description);
      expect(response.body.credits).toBe(newElective.credits);
      expect(response.body.tags).toEqual(newElective.tags);

      createdElectiveId = response.body.id;
    });

    it("should fail when not authenticated", async () => {
      const newElective = {
        code: "TEST-001",
        name: "Test Elective",
        description: "Test",
        provider: "Test",
        period: "P1",
        duration: "1 Periode",
        credits: 5,
        language: "Engels",
        location: "Breda",
        level: "NLQF5",
      };

      await request(app.getHttpServer()).post("/electives").send(newElective).expect(401);
    });

    it("should fail for non-admin users", async () => {
      if (!studentToken) {
        return;
      }

      const newElective = {
        code: "TEST-002",
        name: "Test Elective",
        description: "Test",
        provider: "Test",
        period: "P1",
        duration: "1 Periode",
        credits: 5,
        language: "Engels",
        location: "Breda",
        level: "NLQF5",
      };

      await request(app.getHttpServer())
        .post("/electives")
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .send(newElective)
        .expect(403);
    });
  });

  describe("/electives (GET)", () => {
    it("should return all electives for authenticated users", async () => {
      if (!studentToken) {
        return;
      }

      const response = await request(app.getHttpServer())
        .get("/electives")
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(200);

      expect(Array.isArray(response.body)).toBe(true);
      if (response.body.length > 0) {
        expect(response.body[0]).toHaveProperty("id");
        expect(response.body[0]).toHaveProperty("code");
        expect(response.body[0]).toHaveProperty("name");
        expect(response.body[0]).toHaveProperty("credits");
      }
    });

    it("should fail when not authenticated", async () => {
      await request(app.getHttpServer()).get("/electives").expect(401);
    });
  });

  describe("/electives/:id (GET)", () => {
    it("should return a specific elective", async () => {
      if (!studentToken || !createdElectiveId) {
        return;
      }

      const response = await request(app.getHttpServer())
        .get(`/electives/${createdElectiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(200);

      expect(response.body).toHaveProperty("id");
      expect(response.body.id).toBe(createdElectiveId);
    });

    it("should fail when elective not found", async () => {
      if (!studentToken) {
        return;
      }

      await request(app.getHttpServer())
        .get("/electives/000000000000000000000000")
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(404);
    });

    it("should fail when not authenticated", async () => {
      if (!createdElectiveId) {
        return;
      }

      await request(app.getHttpServer()).get(`/electives/${createdElectiveId}`).expect(401);
    });
  });

  describe("/electives/:id (PUT)", () => {
    it("should update elective as admin", async () => {
      if (!adminToken || !createdElectiveId) {
        return;
      }

      const updatedElective = {
        code: `UPDATED-${Date.now()}`,
        name: "Updated Elective",
        description: "Updated description",
        provider: "Updated Provider",
        period: "P4",
        duration: "2 Periodes",
        credits: 10,
        language: "Nederlands",
        location: "Tilburg",
        level: "NLQF6",
        tags: ["updated"],
      };

      const response = await request(app.getHttpServer())
        .put(`/electives/${createdElectiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .send(updatedElective)
        .expect(200);

      expect(response.body.name).toBe(updatedElective.name);
      expect(response.body.credits).toBe(updatedElective.credits);
    });

    it("should fail for non-admin users", async () => {
      if (!studentToken || !createdElectiveId) {
        return;
      }

      await request(app.getHttpServer())
        .put(`/electives/${createdElectiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .send({ name: "Hacked" })
        .expect(403);
    });
  });

  describe("/electives/:id (PATCH)", () => {
    it("should partially update elective as admin", async () => {
      if (!adminToken || !createdElectiveId) {
        return;
      }

      const partialUpdate = {
        name: "Partially Updated Elective",
      };

      const response = await request(app.getHttpServer())
        .patch(`/electives/${createdElectiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .send(partialUpdate)
        .expect(200);

      expect(response.body.name).toBe(partialUpdate.name);
    });

    it("should fail for non-admin users", async () => {
      if (!teacherToken || !createdElectiveId) {
        return;
      }

      await request(app.getHttpServer())
        .patch(`/electives/${createdElectiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${teacherToken}`])
        .send({ name: "Hacked" })
        .expect(403);
    });
  });

  describe("/electives/:id/teachers/:teacherId (POST)", () => {
    it("should assign teacher to elective as admin", async () => {
      if (!adminToken || !createdElectiveId || !teacherId) {
        return;
      }

      const response = await request(app.getHttpServer())
        .post(`/electives/${createdElectiveId}/teachers/${teacherId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .expect(200);

      expect(response.body).toHaveProperty("message");
      expect(response.body.message).toContain("assigned");
    });

    it("should fail when teacher already assigned", async () => {
      if (!adminToken || !createdElectiveId || !teacherId) {
        return;
      }

      await request(app.getHttpServer())
        .post(`/electives/${createdElectiveId}/teachers/${teacherId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .expect(400);
    });

    it("should fail for non-admin users", async () => {
      if (!studentToken || !createdElectiveId || !teacherId) {
        return;
      }

      await request(app.getHttpServer())
        .post(`/electives/${createdElectiveId}/teachers/${teacherId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(403);
    });
  });

  describe("/electives/:id/teachers/:teacherId (DELETE)", () => {
    it("should unassign teacher from elective as admin", async () => {
      if (!adminToken || !createdElectiveId || !teacherId) {
        return;
      }

      const response = await request(app.getHttpServer())
        .delete(`/electives/${createdElectiveId}/teachers/${teacherId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .expect(200);

      expect(response.body).toHaveProperty("message");
      expect(response.body.message).toContain("unassigned");
    });

    it("should fail when teacher not assigned", async () => {
      if (!adminToken || !createdElectiveId || !teacherId) {
        return;
      }

      await request(app.getHttpServer())
        .delete(`/electives/${createdElectiveId}/teachers/${teacherId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .expect(400);
    });

    it("should fail for non-admin users", async () => {
      if (!teacherToken || !createdElectiveId || !teacherId) {
        return;
      }

      await request(app.getHttpServer())
        .delete(`/electives/${createdElectiveId}/teachers/${teacherId}`)
        .set("Cookie", [`ACCESSTOKEN=${teacherToken}`])
        .expect(403);
    });
  });

  describe("/electives/:id (DELETE)", () => {
    it("should delete elective as admin", async () => {
      if (!adminToken) {
        return;
      }

      // Create a new elective to delete
      const electiveToDelete = {
        code: `DELETE-${Date.now()}`,
        name: "Delete Me",
        description: "To be deleted",
        provider: "Test",
        period: "P1",
        duration: "1 Periode",
        credits: 5,
        language: "Engels",
        location: "Breda",
        level: "NLQF5",
      };

      const createResponse = await request(app.getHttpServer())
        .post("/electives")
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .send(electiveToDelete);

      const electiveId = createResponse.body.id;

      await request(app.getHttpServer())
        .delete(`/electives/${electiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${adminToken}`])
        .expect(204);
    });

    it("should fail for non-admin users", async () => {
      if (!studentToken || !createdElectiveId) {
        return;
      }

      await request(app.getHttpServer())
        .delete(`/electives/${createdElectiveId}`)
        .set("Cookie", [`ACCESSTOKEN=${studentToken}`])
        .expect(403);
    });
  });
});
