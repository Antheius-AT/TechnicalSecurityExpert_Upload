"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
// Import required modules.
const nagel = require("express");
const myAwesomeService_1 = require("./services/myAwesomeService");
const bodyParser = require("body-parser");
const cors = require("cors");
const databaseService_1 = require("./services/databaseService");
const authToken_1 = require("../SharedData/authToken");
const registrationToken_1 = require("../SharedData/registrationToken");
const crypto = require("crypto");
const app = nagel();
const port = 41005;
const myNagelVariable = new myAwesomeService_1.myNagel();
const databaseService = new databaseService_1.rethinkdbService("PoolDiaryDatabase", [
    "Users",
    "Pools",
]);
// Configure Middleware.
app.use(cors());
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));
//**Initialize the database. */
function Initialize() {
    return __awaiter(this, void 0, void 0, function* () {
        console.log("trying to connect.");
        databaseService.connect().then((success) => __awaiter(this, void 0, void 0, function* () {
            if (success) {
                console.log("Connection to database established.");
                yield databaseService.tryCreateDatabase().catch((error) => {
                    console.error("The following error occurred: ", error);
                });
                yield databaseService.tryCreateTables();
                console.log("Finished initializing database.");
            }
        }));
    });
}
// Begin of backend logic.
Initialize();
/**
 * Configures the URL to which to post new pool entries for storage
 * in the database.
 */
app.post("/postEntry/", function (request, response) {
    console.debug("Post entry request received.");
    let bodyData = request.body;
    let poolEntry = bodyData;
    if (poolEntry === null) {
        console.error("Received object, but it was not of the correct type.");
        response.status(400).send("The specified object was malformed.");
    }
    else {
        databaseService
            .insertData("Pools", poolEntry)
            .then(() => {
            console.log("Successfully inserted data into database.", poolEntry);
            response.status(201).send("Entry was successfully created.");
        })
            .catch(() => {
            console.error("could not insert data into database", poolEntry);
            response.status(500).send("Request handling failed, please try again.");
        });
    }
});
/**
 * Configures the URL from which to get pool entries.
 */
app.get("/getPoolEntries", function (request, response) {
    console.debug("Get request incoming.");
    databaseService
        .getAllEntries("Pools")
        .then((entries) => {
        response.status(200).send(entries);
        console.debug("Sent database entries to client.");
        console.debug(entries);
    })
        .catch((error) => {
        console.debug("Could not send database entries to client.");
        response.status(500).send("Request handling failed, please try again.");
    });
});
/**
 * Configures the URL to which to post authentication requests.
 */
app.post("/authenticate/", function (request, response) {
    console.debug("Trying to authenticate a user.");
    let user = request.body;
    if (user === null) {
        console.error("Received object, but it was not of the correct type.");
        response.status(400).send("The specified object was malformed.");
    }
    // Hash and replace the plaintext password with the hashed version.
    let passwordHash = crypto.createHash('sha256').update(user.password).digest('base64');
    user.password = passwordHash;
    databaseService
        .tryAuthenticateUser(user)
        .then((result) => {
        if (result) {
            console.log("User was successfully authenticated.");
            let token = new authToken_1.AuthToken(user.username, user.password, true, "Authentication successful.");
            response.status(200).send(token);
        }
        else {
            console.log("User was not authenticated.");
            let token = new authToken_1.AuthToken(user.username, user.password, false, "Authentication denied.");
            response.status(400).send(token);
        }
    })
        .catch((error) => {
        console.error("An error occurred during authentication", error);
        response
            .status(500)
            .send("Server error during handling the request. Please try again.");
    });
});
/**
 * Registers a URL to which to post registration requests to.
 */
app.post("/registerUser/", function (request, response) {
    console.debug("Trying to register user.");
    let user = request.body;
    if (user === null) {
        console.error("Received object, but it was not of the correct type.");
        response.status(400).send("The specified object was malformed.");
    }
    // Hash and replace the plaintext password with the hashed version.
    let passwordHash = crypto.createHash('sha256').update(user.password).digest('base64');
    user.password = passwordHash;
    // Maybe think about checking for usernames only, as to only allow
    // a username to be chosen once.
    databaseService.tryFindUser(user).then((result) => {
        if (result) {
            console.log("User could not be registered because user already existed.");
            let token = new registrationToken_1.RegistrationToken(false, user.username, user.password, "User could not be registered, as user already exists.");
            response.status(400).send(token);
        }
        else {
            databaseService
                .insertData("Users", user)
                .then(() => {
                console.log("User was successfully registered.");
                let token = new registrationToken_1.RegistrationToken(true, user.username, user.password, "User successfully registered");
                response.status(201).send(token);
            })
                .catch((error) => {
                console.error("User could not be registered due to internal server error", error);
            });
        }
    });
});
/**
 * Starts the server listener to accept incoming connections.
 */
app.listen(port, function () {
    console.log("App started...");
    console.log("Listening on port " + port);
});
