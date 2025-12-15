import { Module } from "@nestjs/common";
import { databaseUrl } from "./constants";
import { MongooseModule } from "@nestjs/mongoose";
import { InterfacesModule } from "./interfaces/interfaces.module";
import { ApplicationModule } from "./application/application.module";
import { InfrastructureModule } from "./infrastructure/infrastructure.module";

@Module({
  imports: [
    MongooseModule.forRoot(databaseUrl),
    InfrastructureModule,
    ApplicationModule,
    InterfacesModule,
  ],
})
export class AppModule {}
