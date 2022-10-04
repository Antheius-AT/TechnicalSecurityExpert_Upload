// Import required modules.
import * as nagel from "express";
import { myNagel } from "./services/myAwesomeService";
import * as bodyParser from "body-parser";
import * as cors from "cors";
import * as database from "rethinkdb-ts";
import { exception } from "console";
import { connect } from "http2";
import { r } from "rethinkdb-ts";
import { rejects } from "assert";
import { rethinkdbService } from "./services/databaseService";
import { PoolEntry } from "../SharedData/pool";
import { User } from "../SharedData/user";
import { AuthToken } from "../SharedData/authToken";
import { RegistrationToken } from "../SharedData/registrationToken";
import * as crypto from "crypto";

const app = nagel();
const port = 41005;
const myNagelVariable = new myNagel();
const databaseService = new rethinkdbService("PoolDiaryDatabase", [
  "Users",
  "Pools",
]);

// Configure Middleware.
app.use(cors());
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

//**Initialize the database. */
async function Initialize() {
  console.log("trying to connect.");
  databaseService.connect().then(async (success) => {
    if (success) {
      console.log("Connection to database established.");

      await databaseService.tryCreateDatabase().catch((error) => {
        console.error("The following error occurred: ", error);
      });

      await databaseService.tryCreateTables();
      console.log("Finished initializing database.");
    }
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

  let poolEntry = bodyData as PoolEntry;

  if (poolEntry === null) {
    console.error("Received object, but it was not of the correct type.");
    response.status(400).send("The specified object was malformed.");
  } else {
    databaseService
      .insertData<PoolEntry>("Pools", poolEntry)
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

  let user = request.body as User;

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
        let token = new AuthToken(
          user.username,
          user.password,
          true,
          "Authentication successful."
        );
        response.status(200).send(token);
      } else {
        console.log("User was not authenticated.");
        let token = new AuthToken(
          user.username,
          user.password,
          false,
          "Authentication denied."
        );
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

  let user = request.body as User;

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
      let token = new RegistrationToken(
        false,
        user.username,
        user.password,
        "User could not be registered, as user already exists."
      );
      response.status(400).send(token);
    } else {
      databaseService
        .insertData("Users", user)
        .then(() => {
          console.log("User was successfully registered.");
          let token = new RegistrationToken(
            true,
            user.username,
            user.password,
            "User successfully registered"
          );
          response.status(201).send(token);
        })
        .catch((error) => {
          console.error(
            "User could not be registered due to internal server error",
            error
          );
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
