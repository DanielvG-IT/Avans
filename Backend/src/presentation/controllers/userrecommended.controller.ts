import { Controller, Get, Post, Body, Inject, Session } from '@nestjs/common';
import { SessionData } from '@/types/session.types';
import { UserRecommendedService } from '@/application/services/userrecommended.service';
import { SubmitRecommendedDto } from '@/presentation/dtos/userrecommended.dto';

@Controller('user/recommended')
export class UserRecommendedController {
  constructor(
    @Inject('SERVICE.USER_RECOMMENDED')
    private readonly service: UserRecommendedService,
  ) {}

  // GET /user/recommended
  @Get()
  async getRecommended(@Session() session: SessionData) {
    if (!session?.user) {
      throw new Error('No active session');
    }

    const MAX_RECENT_RECOMMENDED = 5;
    const moduleIds = await this.service.getRecommendedModuleIds(session.user.id);
    const recentIds = moduleIds.slice(0, MAX_RECENT_RECOMMENDED);

    return {
      recommended: recentIds.map((choiceModuleId) => ({ choiceModuleId })),
    };
  }

  // POST /user/recommended
  @Post()
  async submitRecommended(
    @Body() body: SubmitRecommendedDto,
    @Session() session: SessionData,
  ) {
    if (!session?.user) {
      throw new Error('No active session');
    }

    await this.service.setRecommendedModules(session.user.id, body.moduleIds);
    return { success: true };
  }
}
