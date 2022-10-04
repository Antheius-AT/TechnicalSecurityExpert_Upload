export class AuthToken {
  public readonly username: string;
  public readonly passwordHash: string;
  public readonly isAuthenticated: boolean;
  public readonly message : string;

  constructor(username: string, passwordHash: string, isAuthenticated: boolean, message : string) {
    this.username = username;
    this.passwordHash = passwordHash;
    this.isAuthenticated = isAuthenticated;
    this.message = message;
  }
}
