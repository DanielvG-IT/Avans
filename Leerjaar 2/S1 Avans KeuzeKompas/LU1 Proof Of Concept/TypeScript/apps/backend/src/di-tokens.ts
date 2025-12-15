// Central DI tokens (using Symbol for uniqueness)

export const REPOSITORIES = {
  USER: Symbol("REPOSITORIES.USER"),
  ELECTIVE: Symbol("REPOSITORIES.ELECTIVE"),
} as const;

export const SERVICES = {
  AUTH: Symbol("SERVICES.AUTH"),
  USER: Symbol("SERVICES.USER"),
  STUDENT: Symbol("SERVICES.STUDENT"),
  TEACHER: Symbol("SERVICES.TEACHER"),
  ELECTIVE: Symbol("SERVICES.ELECTIVE"),
} as const;
