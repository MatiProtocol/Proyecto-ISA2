export class UserRequest {
  userName = '';
  userCode = '';
  email = '';
  password = '';
  address = '';
  registrationDate = '';

  constructor(
    userName: string,
    userCode: string,
    email: string,
    password: string,
    address: string,
    registrationDate: string,
  ) {
    this.userName = userName;
    this.userCode = userCode;
    this.email = email;
    this.password = password;
    this.address = address;
    this.registrationDate = registrationDate;
  }
}

export interface UserResponse {
  userName: string;
  email: string;
  address: string;
  registrationDate: string;
}
