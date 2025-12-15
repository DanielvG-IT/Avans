# 🏗️ Architecture Documentation

## Overview

This project is a **full-stack TypeScript application** for managing student
electives at Avans University. It follows **Clean Architecture** (Onion
Architecture) principles with clear separation between layers and
**Domain-Driven Design** for role-based user management.

The system consists of three main components:

- **Frontend**: React SPA with TypeScript
- **Backend**: NestJS REST API with TypeScript
- **Database**: MongoDB / Cosmos DB (NoSQL document database)

---

## 🎯 System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         FRONTEND (SPA)                          │
│                    React + TypeScript + Vite                    │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  • React Router for navigation                           │  │
│  │  • Tailwind CSS + shadcn/ui components                   │  │
│  │  • JWT-based authentication (HTTP-only cookies)          │  │
│  │  • Role-based access control (Student, Teacher, Admin)   │  │
│  │  • Custom hooks for data fetching & state management    │  │
│  └──────────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────────┘
                         │ HTTPS/REST API
                         │ (JSON + JWT Cookies)
┌────────────────────────▼────────────────────────────────────────┐
│                      BACKEND (REST API)                         │
│                     NestJS + TypeScript                         │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │                  Clean Architecture Layers                │  │
│  │  ┌────────────────────────────────────────────────────┐  │  │
│  │  │  Interfaces Layer (Controllers, DTOs, Guards)     │  │  │
│  │  └──────────────────┬─────────────────────────────────┘  │  │
│  │  ┌──────────────────▼─────────────────────────────────┐  │  │
│  │  │  Application Layer (Services, Ports, Use Cases)   │  │  │
│  │  └──────────────────┬─────────────────────────────────┘  │  │
│  │  ┌──────────────────▼─────────────────────────────────┐  │  │
│  │  │  Domain Layer (Entities, Business Rules)          │  │  │
│  │  └──────────────────┬─────────────────────────────────┘  │  │
│  │  ┌──────────────────▼─────────────────────────────────┐  │  │
│  │  │  Infrastructure Layer (Mongoose, Repositories)    │  │  │
│  │  └────────────────────────────────────────────────────┘  │  │
│  └──────────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────────┘
                         │ Mongoose ODM
                         │ (TCP Connection)
┌────────────────────────▼────────────────────────────────────────┐
│                       DATABASE (NoSQL)                          │
│                          MongoDB / Cosmos DB                    │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  Collections:                                            │  │
│  │  • users (with role discriminator)                       │  │
│  │    - StudentUser (with favorites array)                  │  │
│  │    - TeacherUser                                         │  │
│  │    - AdminUser                                           │  │
│  │  • electives (course/module information)                 │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🖥️ Frontend Architecture

### Technology Stack

- **Framework**: React 19.1.1
- **Language**: TypeScript 5.9.3
- **Build Tool**: Vite 7.1.7
- **Styling**: Tailwind CSS 4.1.14 + shadcn/ui components
- **Routing**: React Router DOM 7.9.4
- **State Management**: React hooks + custom hooks
- **Form Handling**: React Hook Form 7.65.0 + Zod validation
- **HTTP Client**: Native fetch API with custom wrapper

### Project Structure

```
frontend/
├── src/
│   ├── main.tsx              # Application entry point & router config
│   ├── main.css              # Global styles & Tailwind imports
│   ├── components/           # Reusable UI components
│   │   ├── auth/            # Authentication components
│   │   │   ├── ProtectedRoute.tsx
│   │   │   ├── RoleProtected.tsx
│   │   │   └── RoleProtectedRoute.tsx
│   │   ├── elective/        # Elective-related components
│   │   ├── recommendations/ # Recommendation wizard
│   │   ├── users/           # User management components
│   │   └── ui/              # shadcn/ui base components
│   ├── pages/               # Page components (route views)
│   │   ├── Home.tsx
│   │   ├── Auth/            # Login, Profile
│   │   ├── Electives/       # Browse & detail pages
│   │   ├── Recommendations/ # Recommendation wizard page
│   │   └── Admin/           # Admin dashboard & user management
│   ├── hooks/               # Custom React hooks
│   │   ├── useAuth.ts       # Authentication state & actions
│   │   ├── useElective.ts   # Single elective data
│   │   ├── useElectives.ts  # Electives list with filtering
│   │   ├── useFavorites.ts  # Favorite management
│   │   └── useUser.ts       # User profile data
│   ├── services/
│   │   └── api.service.ts   # API endpoints configuration
│   ├── lib/
│   │   ├── fetch.ts         # Fetch wrapper with auth handling
│   │   └── utils.ts         # Utility functions
│   ├── types/               # TypeScript type definitions
│   │   ├── Auth.d.ts        # Auth & user types
│   │   ├── Elective.d.ts    # Elective types
│   │   └── User.d.ts        # User-related types
│   └── layouts/             # Layout wrappers
│       ├── Layout.tsx       # Main app layout
│       └── AuthLayout.tsx   # Authentication pages layout
├── public/                   # Static assets
├── index.html               # HTML entry point
├── vite.config.ts           # Vite configuration
└── tailwind.config.ts       # Tailwind CSS configuration
```

### Key Features

- **Authentication**: JWT tokens stored in HTTP-only cookies
- **Role-Based Access Control**: Student, Teacher, Admin roles
- **Protected Routes**: Route guards for authenticated & role-specific pages
- **Responsive Design**: Mobile-first approach with Tailwind CSS
- **Dark Mode**: Theme toggle support with next-themes
- **Form Validation**: Type-safe forms with Zod schemas
- **Error Handling**: Centralized error states & loading indicators

### Environment Variables

```bash
VITE_BACKEND_URL=     # Backend API URL
VITE_BASE=            # Base path for routing
VITE_APP_NAME=        # Application name
VITE_PORT=            # Development server port
APP_ENV=              # Environment (development/production)
```

---

## 🔧 Backend Architecture

### Technology Stack

- **Framework**: NestJS 11.0.1
- **Language**: TypeScript 5.7.3
- **Runtime**: Node.js
- **Database ORM**: Mongoose 8.19.0
- **Authentication**: @nestjs/jwt 11.0.1 + bcrypt 6.0.0
- **Validation**: Class-validator & DTOs
- **API Documentation**: Swagger (development only)

### Clean Architecture Layers

#### 1. **Interfaces Layer** (Presentation)

```
interfaces/
├── controllers/           # HTTP request handlers
│   ├── auth.controller.ts    # Login, logout endpoints
│   ├── user.controller.ts    # User CRUD operations
│   └── elective.controller.ts # Elective endpoints
├── guards/                # Route protection
│   ├── auth.guard.ts         # JWT authentication
│   └── roles.guard.ts        # Role-based authorization
├── decorators/
│   └── roles.decorator.ts    # @Roles() decorator
└── dtos/                  # Data Transfer Objects
    ├── login.dto.ts
    ├── user.dto.ts
    └── favorites.dto.ts
```

#### 2. **Application Layer** (Business Logic)

```
application/
├── services/              # Use case implementations
│   ├── auth.service.ts       # Authentication logic
│   ├── user.service.ts       # User management
│   ├── student.service.ts    # Student-specific logic
│   ├── teacher.service.ts    # Teacher-specific logic
│   └── elective.service.ts   # Elective management
├── ports/                 # Interface definitions
│   ├── auth.port.ts
│   ├── user.port.ts
│   ├── student.port.ts
│   ├── teacher.port.ts
│   └── elective.port.ts
└── utils/
    ├── password.util.ts      # Password hashing
    └── id-normalizer.util.ts # ID normalization
```

#### 3. **Domain Layer** (Core Business Entities)

```
domain/
├── user/
│   ├── user.ts                  # User entity (discriminated union)
│   │   ├── StudentUser         # role: "student" + favorites[]
│   │   ├── TeacherUser         # role: "teacher"
│   │   └── AdminUser           # role: "admin"
│   └── user.repository.interface.ts
├── elective/
│   ├── elective.ts             # Elective entity
│   └── elective.repository.interface.ts
└── result.ts                   # Result<T> type for error handling
```

**User Domain Model** (Discriminated Union):

```typescript
export type User = StudentUser | TeacherUser | AdminUser;

interface StudentUser extends BaseUser {
  role: "student";
  favorites: string[]; // Array of elective IDs
}

interface TeacherUser extends BaseUser {
  role: "teacher";
}

interface AdminUser extends BaseUser {
  role: "admin";
}
```

#### 4. **Infrastructure Layer** (External Integrations)

```
infrastructure/
└── mongoose/
    ├── schemas/              # Mongoose schemas
    │   ├── user.schema.ts       # User schema with discriminator
    │   ├── student.schema.ts
    │   ├── teacher.schema.ts
    │   └── elective.schema.ts
    └── repositories/         # Repository implementations
        ├── mongoose-user.repository.ts
        └── mongoose-elective.repository.ts
```

### Dependency Injection

```typescript
// di-tokens.ts
export const REPOSITORIES = {
  USER: Symbol("IUserRepository"),
  ELECTIVE: Symbol("IElectiveRepository"),
};
```

### API Structure

- **Base Path**: `/api`
- **Versioning**: Header-based (`X-API-Version: 1`)
- **Authentication**: JWT tokens in HTTP-only cookies
- **CORS**: Configurable via `CORS_ORIGIN` environment variable

### Environment Variables

```bash
NODE_ENV=              # development | production | test
PORT=                  # API server port (e.g., 3000)
LOG_LEVEL=             # trace | debug | info | warn | error
DATABASE_URL=          # MongoDB connection string
JWT_SECRET=            # Secret for JWT signing
CORS_ORIGIN=           # Allowed frontend origin(s)
```

---

## 🗄️ Database Architecture

### Database Type

**MongoDB** - NoSQL document database

### Connection

- **ODM**: Mongoose 8.19.0
- **Connection String**: Configured via `DATABASE_URL` environment variable
- **Format**: `mongodb://[user:pass@]host:port/database[?options]`

### Collections

#### 1. **users** Collection

Single collection with role-based discriminator pattern.

**Schema**:

```typescript
{
  _id: ObjectId,
  firstName: string,
  lastName: string,
  email: string (unique),
  passwordHash: string,
  role: "student" | "teacher" | "admin",  // Discriminator

  // Student-specific fields
  favorites?: ObjectId[],  // References to electives

  createdAt: Date,
  updatedAt: Date
}
```

**Indexes**:

- `email` (unique)
- `role` (for filtered queries)

**Discriminator Pattern**:

- Base schema: `User`
- Discriminators: `StudentUser`, `TeacherUser`, `AdminUser`
- All stored in same collection with `__t` field

#### 2. **electives** Collection

Stores course/module information.

**Schema**:

```typescript
{
  _id: ObjectId,
  code: string (unique),
  name: string,
  description: string,
  provider: string,        // e.g., "Technische Bedrijfskunde"
  period: string,          // e.g., "P3"
  duration: string,        // e.g., "1 Periode"
  credits: number,         // e.g., 15 or 30
  language: string,        // "Nederlands" or "Engels"
  location: string,        // e.g., "Breda"
  level: string,           // e.g., "NLQF5" or "NLQF6"
  tags: string[],          // For filtering/recommendations
  teachers: ObjectId[],    // References to User (teachers)

  createdAt: Date,
  updatedAt: Date
}
```

**Indexes**:

- `code` (unique)
- `name` (text index for search)
- `credits`, `level`, `tags` (for filtering)

### Relationships

- **Many-to-Many**: Students ↔ Electives (via `favorites` array in StudentUser)
- **Many-to-Many**: Teachers ↔ Electives (via `teachers` array in Elective)

---

## 🔐 Security Architecture

### Authentication Flow

```
1. User submits credentials → POST /api/auth/login
2. Backend validates credentials (bcrypt password comparison)
3. Backend generates JWT token
4. Token stored in HTTP-only cookie (secure, sameSite: strict)
5. Frontend includes cookie automatically in subsequent requests
6. Backend validates JWT on protected routes via AuthGuard
```

### Authorization

- **Guards**: `AuthGuard` (authentication) + `RolesGuard` (authorization)
- **Decorators**: `@Roles('admin', 'teacher')` on controller methods
- **Frontend**: `ProtectedRoute` and `RoleProtectedRoute` components

### Password Security

- **Hashing**: bcrypt with salt rounds
- **Storage**: Only hashed passwords stored in database
- **Validation**: Password requirements enforced via DTOs

### CORS Configuration

- **Origin**: Configurable via environment variable
- **Credentials**: Enabled for cookie-based auth
- **Methods**: GET, POST, PUT, PATCH, DELETE

---

## 📊 Data Flow

### Example: Student Adds Elective to Favorites

```
┌─────────────┐
│   Frontend  │
│  (Student)  │
└──────┬──────┘
       │ 1. User clicks "Add to Favorites"
       │
       ▼
┌─────────────────────────────────────┐
│  POST /api/users/me/favorites       │
│  Body: { electiveId: "..." }       │
│  Cookie: JWT token                  │
└──────┬──────────────────────────────┘
       │ 2. AuthGuard validates JWT
       │
       ▼
┌─────────────────────────────────────┐
│  UserController.addFavorite()       │
│  Extracts user ID from JWT          │
└──────┬──────────────────────────────┘
       │ 3. Calls service
       │
       ▼
┌─────────────────────────────────────┐
│  StudentService.addFavorite()       │
│  Business logic validation          │
└──────┬──────────────────────────────┘
       │ 4. Repository call
       │
       ▼
┌─────────────────────────────────────┐
│  MongooseUserRepository              │
│  Updates user document              │
└──────┬──────────────────────────────┘
       │ 5. MongoDB update
       │
       ▼
┌─────────────────────────────────────┐
│  MongoDB                             │
│  db.users.updateOne(                │
│    { _id: userId },                 │
│    { $addToSet: { favorites } }     │
│  )                                  │
└──────┬──────────────────────────────┘
       │ 6. Success response
       │
       ▼
┌─────────────┐
│   Frontend  │
│  UI updates │
└─────────────┘
```

---

## 🚀 Deployment Architecture

### Frontend Deployment

- **Platform**: Azure App Service / Static Web Apps (recommended)
- **Build Output**: Static files (HTML, CSS, JS)
- **Build Command**: `npm run build`
- **Output Directory**: `dist/`
- **Environment**: Production environment variables

### Backend Deployment

- **Platform**: Azure App Service / Container Apps (recommended)
- **Runtime**: Node.js
- **Start Command**: `npm run start:prod`
- **Build Command**: `npm run build`
- **Health Check**: GET `/api/health` (if implemented)

### Database Hosting

- **Platform**: MongoDB Atlas (recommended) or Azure Cosmos DB
- **Connection**: Secure connection string with authentication
- **Backup**: Automated backups configured

### Environment Separation

```
┌─────────────────────────────────────────────────────────┐
│  PRODUCTION                                             │
│  ├─ Frontend: https://app.example.com                   │
│  ├─ Backend:  https://api.example.com                   │
│  └─ Database: MongoDB Atlas (prod cluster)             │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│  DEVELOPMENT                                            │
│  ├─ Frontend: http://localhost:4200                     │
│  ├─ Backend:  http://localhost:3000                     │
│  └─ Database: MongoDB local or Atlas (dev cluster)     │
└─────────────────────────────────────────────────────────┘
```

---

## 🧪 Testing Strategy

### Backend Tests

```
tests/
├── unit/          # Unit tests for services & utilities
├── system/        # Integration tests for modules
└── e2e/           # End-to-end API tests
```

### Frontend Tests

```
tests/
├── Unit tests for components & hooks
├── Integration tests for user flows
└── E2E tests (if configured)
```

### Test Commands

- **Backend**: `npm test` (Jest)
- **Frontend**: `npm test` (if configured)

---

## 📈 Scalability Considerations

### Horizontal Scaling

- **Frontend**: CDN distribution for static assets
- **Backend**: Stateless API servers (multiple instances)
- **Database**: MongoDB replica sets / sharding

### Caching Strategy

- **Frontend**: Browser caching, service workers (future)
- **Backend**: Response caching for read-heavy operations
- **Database**: Indexed queries for performance

### Performance Optimization

- **Frontend**: Code splitting, lazy loading, optimized bundles
- **Backend**: Connection pooling, efficient queries
- **Database**: Proper indexing, query optimization

---

## 🔄 Development Workflow

### Monorepo Structure

```
L2S1LU1-TypeScript/
├── apps/
│   ├── frontend/    # React application
│   └── backend/     # NestJS application
├── ARCHITECTURE.md  # This document
├── README.md        # Project overview
└── package.json     # Root package (workspace management)
```

### Running the Application

**Backend**:

```bash
cd apps/backend
npm install
npm run dev        # Development mode with hot reload
```

**Frontend**:

```bash
cd apps/frontend
npm install
npm run dev        # Development server
```

### Development Tools

- **TypeScript**: Type safety across stack
- **ESLint**: Code quality & consistency
- **Prettier**: Code formatting
- **Git**: Version control

---

## 📚 Key Design Patterns

### Backend Patterns

1. **Clean Architecture**: Separation of concerns across layers
2. **Repository Pattern**: Abstraction over data access
3. **Dependency Injection**: NestJS built-in DI container
4. **Discriminated Union**: Type-safe role handling
5. **Result Pattern**: Functional error handling

### Frontend Patterns

1. **Component Composition**: Reusable UI components
2. **Custom Hooks**: Shared logic extraction
3. **Protected Routes**: HOC for route protection
4. **Render Props**: Flexible component APIs
5. **Context API**: Theme & auth state management

---

## 🎯 Future Enhancements

### Potential Improvements

- [ ] GraphQL API for more flexible data fetching
- [ ] WebSocket support for real-time updates
- [ ] Advanced recommendation algorithm (ML-based)
- [ ] File upload for user profiles & elective materials
- [ ] Notification system (email, push)
- [ ] Analytics dashboard for admins
- [ ] Multi-language support (i18n)
- [ ] Progressive Web App (PWA) features
- [ ] Automated testing coverage > 80%
- [ ] Performance monitoring & logging (APM)

---

## 📝 Documentation

- **ARCHITECTURE.md**: This document - system architecture overview
- **IMPLEMENTATION.md**: Implementation details & technical decisions
- **README.md**: Project setup & getting started guide
- **API Documentation**: Swagger UI (available at `/api/docs` in development)

---

**Last Updated**: October 19, 2025  
**Version**: 0.2.0  
**Maintained By**: Daniël van Ginneken

---

## 🎯 Design Decisions

### 1. **Consolidated Controller, Separated Services**

**Controller Layer (✅ Consolidated):**

```typescript
UserController
├─ GET    /users/me                 → Any authenticated user
├─ GET    /users/me/favorites       → Students only
├─ POST   /users/me/favorites/:id   → Students only
├─ DELETE /users/me/favorites/:id   → Students only
└─ GET    /users/me/electives       → Teachers only
```

**Why?**

- ✅ Single API entry point (RESTful)
- ✅ Consistent URL structure
- ✅ Role checks happen in the controller

**Service Layer (✅ Separated):**

```typescript
UserService      → Generic user operations (CRUD)
StudentService   → Student-specific business logic (favorites)
TeacherService   → Teacher-specific business logic (electives)
```

**Why?**

- ✅ **Single Responsibility Principle** - Each service handles one domain
- ✅ **Easier to test** - Independent unit tests per role
- ✅ **Easier to extend** - Add new student/teacher features without touching
  other code
- ✅ **Clear separation of concerns** - Business logic organized by domain

---

### 2. **Single Collection with Discriminated Unions**

**Database Structure:**

```typescript
// Single MongoDB collection: "users"
{
  _id: ObjectId,
  role: "student" | "teacher" | "admin",
  firstName: string,
  lastName: string,
  email: string,
  passwordHash: string,

  // Student-specific (only if role = "student")
  favorites?: string[],

  // Teacher-specific (only if role = "teacher")
  modulesGiven?: string[],
}
```

**TypeScript Type:**

```typescript
type User = StudentUser | TeacherUser | AdminUser; // Discriminated union
```

**Benefits:**

- ✅ One database query for any user lookup
- ✅ TypeScript automatically narrows types based on role
- ✅ No complex joins or multiple collections
- ✅ Easier to add new roles in the future

---

### 3. **Result<T> Pattern for Error Handling**

```typescript
type Result<T> =
  | { ok: true; data: T }
  | { ok: false; error: { code: string; message: string; details?: any } };
```

**Benefits:**

- ✅ Explicit error handling (no throwing in services)
- ✅ Type-safe error codes
- ✅ Controllers can map errors to appropriate HTTP responses
- ✅ Better testability

---

## 📁 Project Structure

```
apps/backend/src/
├── application/                  # Application Layer
│   ├── ports/                    # Service interfaces (dependency inversion)
│   │   ├── user.port.ts
│   │   ├── student.port.ts
│   │   └── teacher.port.ts
│   ├── services/                 # Service implementations
│   │   ├── user.service.ts       # Generic CRUD operations
│   │   ├── student.service.ts    # Student business logic
│   │   └── teacher.service.ts    # Teacher business logic
│   └── utils/                    # Shared utilities
│       └── id-normalizer.util.ts # MongoDB ObjectId normalization
│
├── domain/                       # Domain Layer
│   ├── user/
│   │   ├── user.ts              # User domain entities (discriminated union)
│   │   └── user.repository.interface.ts
│   ├── elective/
│   │   ├── elective.ts
│   │   └── elective.repository.interface.ts
│   └── result.ts                # Result<T> error handling pattern
│
├── infrastructure/               # Infrastructure Layer
│   └── mongoose/
│       ├── repositories/
│       │   └── mongoose-user.repository.ts
│       └── schemas/
│           ├── user.schema.ts
│           ├── student.schema.ts  # Discriminator
│           └── teacher.schema.ts  # Discriminator
│
└── interfaces/                   # Presentation Layer
    ├── controllers/
    │   ├── user.controller.ts     # ⭐ Consolidated REST API
    │   ├── auth.controller.ts
    │   └── elective.controller.ts
    ├── guards/
    │   ├── auth.guard.ts          # JWT authentication
    │   └── roles.guard.ts         # Role-based authorization
    └── dtos/
        ├── user.dto.ts
        └── login.dto.ts
```

---

## 🔐 Authentication & Authorization Flow

### 1. **Authentication (AuthGuard)**

```typescript
Request → AuthGuard
         ├─ Extract JWT from cookie
         ├─ Verify JWT signature
         ├─ Attach authClaims to request
         └─ Allow/Deny access
```

### 2. **Authorization (Role Checks in Controller)**

```typescript
Controller Method
├─ Extract { sub: userId, role } from authClaims
├─ Check if role matches required role
│  ├─ If match → Proceed to service
│  └─ If no match → Return 403 Forbidden
└─ Call appropriate service method
```

**Example:**

```typescript
@Get("me/favorites")
public async getFavorites(@Req() req: RequestWithCookies) {
  const { sub: userId, role } = this.getAuthClaims(req);

  if (role !== "student") {
    throw new ForbiddenException("Only students can access favorites");
  }

  return await this.studentService.getFavorites(userId);
}
```

---

## 🚀 API Endpoints

### User Endpoints

| Method   | Endpoint                      | Role Required | Description                     |
| -------- | ----------------------------- | ------------- | ------------------------------- |
| `GET`    | `/api/users/me`               | Any           | Get current user profile        |
| `GET`    | `/api/users/me/favorites`     | Student       | Get favorite electives          |
| `GET`    | `/api/users/me/favorites/:id` | Student       | Check if elective is favorited  |
| `POST`   | `/api/users/me/favorites/:id` | Student       | Add elective to favorites       |
| `DELETE` | `/api/users/me/favorites/:id` | Student       | Remove elective from favorites  |
| `GET`    | `/api/users/me/electives`     | Teacher       | Get electives taught by teacher |

### Auth Endpoints

| Method | Endpoint          | Role Required | Description       |
| ------ | ----------------- | ------------- | ----------------- |
| `POST` | `/api/auth/login` | None          | Login and get JWT |

---

## 🧪 Testing Strategy

### Unit Tests

```
UserService      → Test generic CRUD operations
StudentService   → Test favorites logic (add, remove, check)
TeacherService   → Test electives retrieval
```

### Integration Tests

```
UserController   → Test all endpoints with proper auth
AuthGuard        → Test JWT validation
Role checks      → Test 403 responses for wrong roles
```

### E2E Tests

```
Login → Get Profile → Add Favorite → Remove Favorite
Login → Get Electives (as teacher)
```

---

## 📚 Key Patterns Used

1. **Clean Architecture** - Clear layer separation
2. **Domain-Driven Design** - Role-based domain modeling
3. **Dependency Inversion** - Services depend on interfaces (ports)
4. **Repository Pattern** - Data access abstraction
5. **Result Pattern** - Explicit error handling
6. **Discriminated Unions** - Type-safe role handling
7. **Single Table Inheritance** - One collection for all user types

---

## 🎓 Benefits of This Architecture

✅ **Maintainable** - Clear separation of concerns  
✅ **Testable** - Each layer can be tested independently  
✅ **Scalable** - Easy to add new roles or features  
✅ **Type-safe** - TypeScript ensures correctness at compile time  
✅ **RESTful** - Follows REST best practices  
✅ **Secure** - Proper authentication and authorization  
✅ **Clean** - No code duplication or "god classes"

---

## 🔄 Adding New Features

### Example: Adding a new role "Admin"

1. **Update Domain:**

```typescript
// domain/user/user.ts
export interface AdminUser extends BaseUser {
  role: "admin";
  permissions: string[]; // New property
}

export type User = StudentUser | TeacherUser | AdminUser;
```

2. **Create Admin Service:**

```typescript
// application/services/admin.service.ts
@Injectable()
export class AdminService implements IAdminService {
  async manageUsers() {
    /* ... */
  }
}
```

3. **Add Endpoints to UserController:**

```typescript
@Get("me/admin-panel")
public async getAdminData(@Req() req: RequestWithCookies) {
  const { sub: userId, role } = this.getAuthClaims(req);

  if (role !== "admin") {
    throw new ForbiddenException("Only admins can access this");
  }

  return await this.adminService.getAdminData(userId);
}
```

That's it! No other code needs to change. 🎉

---

## 📖 Further Reading

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [NestJS Documentation](https://docs.nestjs.com/)
- [TypeScript Discriminated Unions](https://www.typescriptlang.org/docs/handbook/2/narrowing.html#discriminated-unions)
