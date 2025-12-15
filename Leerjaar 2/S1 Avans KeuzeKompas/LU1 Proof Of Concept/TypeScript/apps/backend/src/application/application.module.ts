import { SERVICES } from "../di-tokens";
import { JwtModule } from "@nestjs/jwt";
import { jwtSecret } from "../constants";
import { AuthService } from "./services/auth.service";
import { UserService } from "./services/user.service";
import { StudentService } from "./services/student.service";
import { TeacherService } from "./services/teacher.service";
import { Module as NestModule } from "@nestjs/common";
import { ElectiveService } from "./services/elective.service";
import { InfrastructureModule } from "../infrastructure/infrastructure.module";

@NestModule({
  imports: [
    InfrastructureModule,
    JwtModule.register({
      global: true,
      secret: jwtSecret,
      signOptions: { expiresIn: "1h" },
    }),
  ],
  providers: [
    { provide: SERVICES.AUTH, useClass: AuthService },
    { provide: SERVICES.USER, useClass: UserService },
    { provide: SERVICES.STUDENT, useClass: StudentService },
    { provide: SERVICES.TEACHER, useClass: TeacherService },
    { provide: SERVICES.ELECTIVE, useClass: ElectiveService },
  ],
  exports: [SERVICES.AUTH, SERVICES.USER, SERVICES.STUDENT, SERVICES.TEACHER, SERVICES.ELECTIVE],
})
export class ApplicationModule {}
