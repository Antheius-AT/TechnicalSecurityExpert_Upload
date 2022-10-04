import { exception } from "console";
import { Connection, r } from "rethinkdb-ts";
import { databaseAsset } from "../../SharedData/DatabaseAsset";
import { User } from "../../SharedData/user";

export class rethinkdbService {
  private databaseName: string;
  private tableNames: string[];
  private connection: Connection;

  constructor(databaseName: string, tableNames: string[]) {
    this.databaseName = databaseName;
    this.tableNames = tableNames;
  }

  /**
   * Connects to the database server.
   * Returns a promise handling the logic of connecting, and returning a boolean
   * indicating whether the connection was successfully established.
   */
  public async connect(): Promise<boolean> {
    this.connection = await r.connect({ host: "localhost", port: 28015 });

    if (this.connection) {
      return true;
    } else {
      return false;
    }
  }

  /**
   * Tries to create the database, returning a promise containing a boolean in its result
   * on termination, indicating whether the database was created successfully.
   */
  public async tryCreateDatabase(): Promise<boolean> {
    var exists = await r
      .dbList()
      .contains(this.databaseName)
      .run(this.connection);
    let creationSuccessful: boolean;

    if (exists) {
      console.log("Database was not created because it already exists.");
      return false;
    } else {
      r.dbCreate(this.databaseName)
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
  }

  /**
   * Tries to create the tables using the table names array passed to this class
   * on initialization.
   */
  public async tryCreateTables() {
    let doesDatabaseExist = await r
      .dbList()
      .contains(this.databaseName)
      .run(this.connection);

    if (!doesDatabaseExist) {
      throw new exception(
        "Can not create tables on a database that does not exist."
      );
    }

    this.tableNames.forEach(async (element) => {
      let exists = await this.checkTableExists(element);

      if (!exists) {
        r.db(this.databaseName)
          .tableCreate(element)
          .run(this.connection)
          .then(() => {
            console.log("Created table " + element);
          })
          .catch((error) => {
            console.error("Table creation failed", error);
          });
      } else {
        // I would prefer to use string interpolation here, but I cant figure out the typescript syntax.
        console.log(
          "Did not create table " + element + " because table already existed."
        );
      }
    });
  }

  /**
   * This method checks whether a specified table already exists in the database.
   * @param tableName The name of the table to check for existence.
   */
  private async checkTableExists(tableName: string): Promise<boolean> {
    return r
      .db(this.databaseName)
      .tableList()
      .contains(tableName)
      .run(this.connection);
  }

  /**
   * Inserts data into a specified table.
   * @param tableName The table name into which to insert data.
   * @param insertedData The data to insert.
   */
  public async insertData<T extends databaseAsset>(tableName: string, insertedData : T) {
      let exists = await this.checkTableExists(tableName);

      if (!exists){
          throw new exception("The specified table did not exist, thus inserting data was impossible: " + tableName);
      }

      r.db(this.databaseName)
      .table(tableName)
      .insert(insertedData)
      .run(this.connection)
      .then(() =>{
          console.log('Inserted data into database.');
      })
      .catch((error) =>{
          console.error('Failed to insert data into table due to error: ', error);
      });
  }

  /**
   * Gets all entries stored in a specified table.
   * @param tableName The name of the table to query.
   */
  public async getAllEntries(tableName : string) : Promise<databaseAsset[]>{
     let result = await r.db(this.databaseName)
    .table(tableName)
    .run(this.connection) as databaseAsset[];

    if (result === null){
      console.error('Database assets could not be retrieved', result);
      return Promise.reject();
    }
    else{
      console.log('Database assets successfully retrieved.');
      return Promise.resolve(result);
    }
  }

  /**
   * Tries to authenticate a user by checking whether this user exists.
   * @param user The user trying to authenticate.
   */
  public async tryAuthenticateUser(user : User) : Promise<boolean> {
   
    let exists = await this.tryFindUser(user);

    console.debug('Does user exist: ' + exists);

    if (exists){
      console.log(user.username + ' tried to authenticate and was successfully authenticated.');
      return true;
    }
    else{
      console.log(user.username + ' tried to authenticate and could not be authenticated.');
      return false;
    }
  }

  /**
   * Tries to find a user in the database. Returns a result based on whether
   * the user was found.
   * @param user The user object to find in the database.
   */
  public async tryFindUser(user : User) : Promise<boolean>{
    return r.db(this.databaseName)
    .table('Users')
    .filter({"username" : user.username, "password" : user.password})
    .count()
    .run(this.connection)
    .then((result)=>{
      console.log('Found entry, amount: ' + result);
      return result == 1;
    })
    .catch((error)=>{
      console.error('Server error occured during find user operation.');
      return false;
    })
  }
}
