import { databaseAsset } from "./DatabaseAsset";

export class User extends databaseAsset{
    public username : string;
    public password : string;

    constructor(username : string, passwordHash : string){
        super();
        this.username = username;
        this.password = passwordHash;
    }
}