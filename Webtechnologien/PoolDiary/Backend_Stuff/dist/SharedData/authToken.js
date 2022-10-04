"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AuthToken = void 0;
class AuthToken {
    constructor(username, passwordHash, isAuthenticated, message) {
        this.username = username;
        this.passwordHash = passwordHash;
        this.isAuthenticated = isAuthenticated;
        this.message = message;
    }
}
exports.AuthToken = AuthToken;
