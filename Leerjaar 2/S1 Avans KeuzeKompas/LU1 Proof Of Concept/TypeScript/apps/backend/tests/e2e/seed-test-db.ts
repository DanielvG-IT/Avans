import { NestFactory } from "@nestjs/core";
import { AppModule } from "../../src/app.module";
import { SERVICES } from "../../src/di-tokens";
import { type IUserService } from "../../src/application/ports/user.port";
import { type IElectiveService } from "../../src/application/ports/elective.port";

/**
 * Seed script for e2e test database
 * Run this before running e2e tests to set up initial test data
 */
async function seedTestDatabase() {
  console.log("🌱 Seeding test database...");

  const app = await NestFactory.createApplicationContext(AppModule);

  try {
    const userService = app.get<IUserService>(SERVICES.USER);
    const electiveService = app.get<IElectiveService>(SERVICES.ELECTIVE);

    // Check if admin user already exists
    console.log("Checking for existing admin user...");
    const existingAdmin = await userService.getUserByEmail("admin@test.com");

    if (!existingAdmin.ok) {
      // Create admin user for testing
      console.log("Creating admin user...");
      const adminResult = await userService.createUser({
        firstName: "Admin",
        lastName: "User",
        email: "admin@test.com",
        passwordHash: "admin123", // Will be hashed by service
        role: "admin",
      });

      if (adminResult.ok) {
        console.log("✅ Admin user created:", adminResult.data.email);
      } else {
        console.log("⚠️ Failed to create admin user:", adminResult.error.message);
      }
    } else {
      console.log("✅ Admin user already exists");
    }

    // Create sample teacher
    console.log("Creating sample teacher...");
    const teacherResult = await userService.createUser({
      firstName: "John",
      lastName: "Teacher",
      email: "teacher@test.com",
      passwordHash: "teacher123",
      role: "teacher",
    });

    if (teacherResult.ok) {
      console.log("✅ Sample teacher created:", teacherResult.data.email);
    } else {
      console.log("⚠️ Sample teacher already exists or failed to create");
    }

    // Create sample student
    console.log("Creating sample student...");
    const studentResult = await userService.createUser({
      firstName: "Jane",
      lastName: "Student",
      email: "student@test.com",
      passwordHash: "student123",
      role: "student",
      favorites: [],
    });

    if (studentResult.ok) {
      console.log("✅ Sample student created:", studentResult.data.email);
    } else {
      console.log("⚠️ Sample student already exists or failed to create");
    }

    // Create sample electives
    console.log("Creating sample electives...");
    const electives = [
      {
        code: "AI-101",
        name: "Introduction to Artificial Intelligence",
        description: "Learn the basics of AI and machine learning",
        provider: "Computer Science",
        period: "P3",
        duration: "1 Periode",
        credits: 5,
        language: "Engels",
        location: "Breda",
        level: "NLQF5",
        tags: ["AI", "Programming"],
      },
      {
        code: "WEB-201",
        name: "Advanced Web Development",
        description: "Deep dive into modern web technologies",
        provider: "Software Engineering",
        period: "P4",
        duration: "1 Periode",
        credits: 5,
        language: "Nederlands",
        location: "Breda",
        level: "NLQF6",
        tags: ["Web", "JavaScript"],
      },
      {
        code: "DB-301",
        name: "Database Design",
        description: "Learn database design principles and SQL",
        provider: "Data Engineering",
        period: "P3",
        duration: "2 Periodes",
        credits: 10,
        language: "Engels",
        location: "Tilburg",
        level: "NLQF6",
        tags: ["Database", "SQL"],
      },
    ];

    for (const elective of electives) {
      const result = await electiveService.createElective(elective);
      if (result.ok) {
        console.log(`✅ Elective created: ${result.data.name}`);
      } else {
        console.log(`⚠️ Elective ${elective.name} already exists or failed to create`);
      }
    }

    console.log("\n✨ Test database seeding complete!");
  } catch (error) {
    console.error("❌ Error seeding database:", error);
    process.exit(1);
  } finally {
    await app.close();
  }
}

void seedTestDatabase();
