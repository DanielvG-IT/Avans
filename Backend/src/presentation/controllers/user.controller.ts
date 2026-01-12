import { RequireAuth } from '../decorators/auth.decorator';
import { AuthenticatedSession } from '@/types/session.types';
import { SessionGuard } from '../guards/session.guard';
import {
  NotFoundException,
  ParseIntPipe,
  Controller,
  HttpStatus,
  UseGuards,
  HttpCode,
  Session,
  Delete,
  Inject,
  Param,
  Body,
  Post,
  Get,
} from '@nestjs/common';

// -- imports for users --
import { SubmitRecommendedDto } from '../dtos/userrecommended.dto';
import { IUserService } from '@/application/ports/user.port';
import { UserDTO } from '@/presentation/dtos/user.dto';
import { User } from '@/domain/user/user.model';

@Controller('user')
@UseGuards(SessionGuard)
export class UserController {
  constructor(
    @Inject('SERVICE.USER') private readonly userService: IUserService,
  ) {}

  @Get('profile')
  @HttpCode(HttpStatus.OK)
  async getProfile(
    @Session() session: AuthenticatedSession,
  ): Promise<{ user: UserDTO }> {
    const result = await this.userService.findById(session.user.id);
    if (result._tag === 'Failure' || !result.data) {
      throw new NotFoundException('User not found');
    }

    return { user: this.toUserDto(result.data) };
  }

  @Get('favorites')
  @RequireAuth('STUDENT')
  @HttpCode(HttpStatus.OK)
  async findFavorites(@Session() session: AuthenticatedSession) {
    return {
      favorites: await this.userService.findFavorites(session.user.id),
    };
  }

  @Get('recommended')
  @RequireAuth('STUDENT')
  @HttpCode(HttpStatus.OK)
  async getRecommended(@Session() session: AuthenticatedSession) {
    const MAX_RECENT_RECOMMENDED = 5;
    const modules = await this.userService.getRecommendedModules(
      session.user.id,
    );
    const recentModules = modules.slice(0, MAX_RECENT_RECOMMENDED);

    return {
      recommended: recentModules,
    };
  }

  @Post('recommended')
  @RequireAuth('STUDENT')
  @HttpCode(HttpStatus.CREATED)
  async submitRecommended(
    @Body() body: SubmitRecommendedDto,
    @Session() session: AuthenticatedSession,
  ) {
    await this.userService.setRecommendedModules(session.user.id, body.modules);
    return { success: true };
  }

  @Get('favorites/:moduleId')
  @RequireAuth('STUDENT')
  @HttpCode(HttpStatus.OK)
  async isModuleFavorited(
    @Param('moduleId', ParseIntPipe) moduleId: number,
    @Session() session: AuthenticatedSession,
  ) {
    return {
      isFavorited: await this.userService.isModuleFavorited(
        session.user.id,
        moduleId,
      ),
    };
  }

  @Post('favorites/:moduleId')
  @RequireAuth('STUDENT')
  @HttpCode(HttpStatus.CREATED)
  async favoriteModule(
    @Param('moduleId', ParseIntPipe) moduleId: number,
    @Session() session: AuthenticatedSession,
  ) {
    await this.userService.favoriteModule(session.user.id, moduleId);
    return { success: true };
  }

  @Delete('favorites/:moduleId')
  @RequireAuth('STUDENT')
  @HttpCode(HttpStatus.OK)
  async unfavoriteModule(
    @Param('moduleId', ParseIntPipe) moduleId: number,
    @Session() session: AuthenticatedSession,
  ) {
    await this.userService.unfavoriteModule(session.user.id, moduleId);
    return { success: true };
  }

  private toUserDto(user: User): UserDTO {
    const dto = new UserDTO();
    dto.id = user.id;
    dto.name = user.name;
    dto.email = user.email;
    dto.role = user.role;
    dto.createdAt = user.createdAt;
    dto.updatedAt = user.updatedAt;
    return dto;
  }
}
