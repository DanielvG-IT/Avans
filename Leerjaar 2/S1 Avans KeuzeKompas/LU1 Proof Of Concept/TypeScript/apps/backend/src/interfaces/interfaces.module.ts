import { Module } from "@nestjs/common";
import { AuthController } from "./controllers/auth.controller";
import { UserController } from "./controllers/user.controller";
import { ApplicationModule } from "../application/application.module";
import { ElectiveController } from "./controllers/elective.controller";

@Module({
  imports: [ApplicationModule],
  controllers: [AuthController, ElectiveController, UserController],
})
export class InterfacesModule {}
