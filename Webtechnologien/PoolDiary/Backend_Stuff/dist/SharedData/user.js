"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.User = void 0;
const DatabaseAsset_1 = require("./DatabaseAsset");
class User extends DatabaseAsset_1.databaseAsset {
    constructor(username, passwordHash) {
        super();
        this.username = username;
        this.password = passwordHash;
    }
}
exports.User = User;
