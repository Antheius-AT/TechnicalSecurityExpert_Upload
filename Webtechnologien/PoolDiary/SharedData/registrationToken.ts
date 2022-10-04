export class RegistrationToken{
    readonly registrationSuccessful : boolean;
    readonly username : string;
    readonly passwordHash : string;
    readonly message : string;

    constructor(registrationSuccessful : boolean, username : string, passwordHash : string, message : string){
        this.registrationSuccessful = registrationSuccessful;
        this.username = username;
        this.passwordHash = passwordHash;
        this.message = message;
    }
}