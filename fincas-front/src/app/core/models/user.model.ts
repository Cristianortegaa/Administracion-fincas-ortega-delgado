export interface UserDto {
  id: number;
  name: string;
  email: string;
  role: string;
  createdAt: string;
}

export interface CreateUserDto {
  name: string;
  email: string;
  password: string;
}

export interface UpdateUserDto {
  name: string;
  newPassword: string; // vacío = no cambiar
}

export interface ChangePasswordDto {
  newPassword: string;
}
