export type UserRole = "student" | "teacher" | "admin";

export class baseUserDTO {
  id?: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  createdAt?: string;
  updatedAt?: string;
}

/** 👩‍🎓 Student */
export interface studentUserDTO extends baseUserDTO {
  role: "student";
  favorites: string[]; // list of Elective _id's
}

/** 👨‍🏫 Teacher */
export interface teacherUserDTO extends baseUserDTO {
  role: "teacher";
}

/** 🧑‍💼 Admin */
export interface adminUserDTO extends baseUserDTO {
  role: "admin";
}

/** 🎯 Discriminated Union */
export type userDTO = studentUserDTO | teacherUserDTO | adminUserDTO;

/** Create User DTO */
export class createUserDTO {
  firstName!: string;
  lastName!: string;
  email!: string;
  role!: UserRole;
  password!: string;
}

/** Update User DTO */
export class updateUserDTO {
  firstName?: string;
  lastName?: string;
  email?: string;
}
