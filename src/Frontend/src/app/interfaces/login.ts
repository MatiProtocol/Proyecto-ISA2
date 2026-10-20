export class LoginRequest {
  userName = '';
  password = '';

  constructor(userName: string, password: string) {
    this.userName = userName;
    this.password = password;
  }
}

export interface LoginResponse {
  token: string;
  role: string;
  userName: string;
}
