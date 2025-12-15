import { Module } from "@nestjs/common";
import { REPOSITORIES } from "../di-tokens";
import { MongooseModule } from "@nestjs/mongoose";
import { UserSchema } from "./mongoose/schemas/user.schema";
import { ElectiveSchema } from "./mongoose/schemas/elective.schema";
import { MongooseUserRepository } from "./mongoose/repositories/mongoose-user.repository";
import { MongooseElectiveRepository } from "./mongoose/repositories/mongoose-elective.repository";

@Module({
  imports: [
    MongooseModule.forFeature([
      { name: "User", schema: UserSchema },
      { name: "Elective", schema: ElectiveSchema },
    ]),
  ],
  providers: [
    { provide: REPOSITORIES.USER, useClass: MongooseUserRepository },
    { provide: REPOSITORIES.ELECTIVE, useClass: MongooseElectiveRepository },
  ],
  exports: [REPOSITORIES.USER, REPOSITORIES.ELECTIVE],
})
export class InfrastructureModule {}
