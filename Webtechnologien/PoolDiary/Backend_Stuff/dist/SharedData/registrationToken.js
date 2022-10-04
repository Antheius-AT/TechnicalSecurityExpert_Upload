"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.RegistrationToken = void 0;
class RegistrationToken {
    constructor(registrationSuccessful, username, passwordHash, message) {
        this.registrationSuccessful = registrationSuccessful;
        this.username = username;
        this.passwordHash = passwordHash;
        this.message = message;
    }
}
exports.RegistrationToken = RegistrationToken;
