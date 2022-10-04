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
exports.rethinkdbService = void 0;
const console_1 = require("console");
const rethinkdb_ts_1 = require("rethinkdb-ts");
class rethinkdbService {
    constructor(databaseName, tableNames) {
        this.databaseName = databaseName;
        this.tableNames = tableNames;
    }
    /**
     * Connects to the database server.
     * Returns a promise handling the logic of connecting, and returning a boolean
     * indicating whether the connection was successfully established.
     */
    connect() {
        return __awaiter(this, void 0, void 0, function* () {
            this.connection = yield rethinkdb_ts_1.r.connect({ host: "localhost", port: 28015 });
            if (this.connection) {
                return true;
            }
            else {
                return false;
            }
        });
    }
    /**
     * Tries to create the database, returning a promise containing a boolean in its result
     * on termination, indicating whether the database was created successfully.
     */
    tryCreateDatabase() {
        return __awaiter(this, void 0, void 0, function* () {
            var exists = yield rethinkdb_ts_1.r
                .dbList()
                .contains(this.databaseName)
                .run(this.connection);
            let creationSuccessful;
            if (exists) {
                console.log("Database was not created because it already exists.");
                return false;
            }
            else {
                rethinkdb_ts_1.r.dbCreate(this.databaseName)
                    .run(this.connection)
                    .then(() => {
                    console.log("Database successfully created.");
                    creationSuccessful = true;
                })
                    .catch((error) => {
                    console.error("Database creation failed due to error: ", error);
                    creationSuccessful = false;
                });
                return creationSuccessful;
            }
        });
    }
    /**
     * Tries to create the tables using the table names array passed to this class
     * on initialization.
     */
    tryCreateTables() {
        return __awaiter(this, void 0, void 0, function* () {
            let doesDatabaseExist = yield rethinkdb_ts_1.r
                .dbList()
                .contains(this.databaseName)
                .run(this.connection);
            if (!doesDatabaseExist) {
                throw new console_1.exception("Can not create tables on a database that does not exist.");
            }
            this.tableNames.forEach((element) => __awaiter(this, void 0, void 0, function* () {
                let exists = yield this.checkTableExists(element);
                if (!exists) {
                    rethinkdb_ts_1.r.db(this.databaseName)
                        .tableCreate(element)
                        .run(this.connection)
                        .then(() => {
                        console.log("Created table " + element);
                    })
                        .catch((error) => {
                        console.error("Table creation failed", error);
                    });
                }
                else {
                    // I would prefer to use string interpolation here, but I cant figure out the typescript syntax.
                    console.log("Did not create table " + element + " because table already existed.");
                }
            }));
        });
    }
    /**
     * This method checks whether a specified table already exists in the database.
     * @param tableName The name of the table to check for existence.
     */
    checkTableExists(tableName) {
        return __awaiter(this, void 0, void 0, function* () {
            return rethinkdb_ts_1.r
                .db(this.databaseName)
                .tableList()
                .contains(tableName)
                .run(this.connection);
        });
    }
    /**
     * Inserts data into a specified table.
     * @param tableName The table name into which to insert data.
     * @param insertedData The data to insert.
     */
    insertData(tableName, insertedData) {
        return __awaiter(this, void 0, void 0, function* () {
            let exists = yield this.checkTableExists(tableName);
            if (!exists) {
                throw new console_1.exception("The specified table did not exist, thus inserting data was impossible: " + tableName);
            }
            rethinkdb_ts_1.r.db(this.databaseName)
                .table(tableName)
                .insert(insertedData)
                .run(this.connection)
                .then(() => {
                console.log('Inserted data into database.');
            })
                .catch((error) => {
                console.error('Failed to insert data into table due to error: ', error);
            });
        });
    }
    /**
     * Gets all entries stored in a specified table.
     * @param tableName The name of the table to query.
     */
    getAllEntries(tableName) {
        return __awaiter(this, void 0, void 0, function* () {
            let result = yield rethinkdb_ts_1.r.db(this.databaseName)
                .table(tableName)
                .run(this.connection);
            if (result === null) {
                console.error('Database assets could not be retrieved', result);
                return Promise.reject();
            }
            else {
                console.log('Database assets successfully retrieved.');
                return Promise.resolve(result);
            }
        });
    }
    /**
     * Tries to authenticate a user by checking whether this user exists.
     * @param user The user trying to authenticate.
     */
    tryAuthenticateUser(user) {
        return __awaiter(this, void 0, void 0, function* () {
            let exists = yield this.tryFindUser(user);
            console.debug('Does user exist: ' + exists);
            if (exists) {
                console.log(user.username + ' tried to authenticate and was successfully authenticated.');
                return true;
            }
            else {
                console.log(user.username + ' tried to authenticate and could not be authenticated.');
                return false;
            }
        });
    }
    /**
     * Tries to find a user in the database. Returns a result based on whether
     * the user was found.
     * @param user The user object to find in the database.
     */
    tryFindUser(user) {
        return __awaiter(this, void 0, void 0, function* () {
            return rethinkdb_ts_1.r.db(this.databaseName)
                .table('Users')
                .filter({ "username": user.username, "password": user.password })
                .count()
                .run(this.connection)
                .then((result) => {
                console.log('Found entry, amount: ' + result);
                return result == 1;
            })
                .catch((error) => {
                console.error('Server error occured during find user operation.');
                return false;
            });
        });
    }
}
exports.rethinkdbService = rethinkdbService;
