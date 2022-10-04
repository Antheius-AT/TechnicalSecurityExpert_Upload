export class UserDto {
  private _fullName: string;
  private _password: string;
  private _token: string  |  undefined;

  public constructor (fullName : string, password : string){
      this._fullName = fullName;
      this._password = password; 
  }

  get fullName () : string {
    return this._fullName;
  }

  get password () : string {
      return this._password;
  }

  get token() : string {
    return this._token || "";
  } 

  set token(token: string) {
    if(token === "" || token ===  undefined){
         throw new Error("The string cannot be empty or undefined.");
    }
   
    this.token = token;
  }
}