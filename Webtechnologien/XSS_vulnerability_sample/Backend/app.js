const express = require('express');
const rethinkDB = require('rethinkdb');
const cors = require('cors');

const databaseName = "XSSDemoDatabase";
const app = express();
const port = 41005;

app.use(cors());
app.use(express.json());
app.use(express.urlencoded({extended : true}));

/**
 * Creates the tables and database if not already existing.
 */
async function initializeDatabase(){
    console.log('Initializing database..');

    var connection = null;
    
      await rethinkDB.connect({ host: 'localhost', port: 28015 })
      .then(conn =>{
          connection = conn;
          console.log('Database connection established.');
      })
      .catch((error) =>{
          console.error('An error occurred: ' + error);
      });

      createDatabase(connection, databaseName)
      .then(() =>{
          createTables(connection, databaseName, ["Users", "Entries"]);
        })
      .catch((error) =>{
          console.error('The following error occurred ' + error);
      });

      console.log('Database initialization complete..');
}

/**
 * This function attempts to create a database if no database using the specified name is in existence yet.
 * @param {*} connection The object with which to access the database.
 * @param {*} callback The callback that is called in case of an error.
 */
function createDatabase(connection, databaseName) {
    return rethinkDB.dbList()
    .contains(databaseName)
    .run(connection)
    .then((exists) =>{
        if (exists){
            console.log('Database was not created because it already exists.');
        }
        else{
            rethinkDB.dbCreate(databaseName)
            .run(connection)
            .then(() =>{
                console.log('Database was successfully created.');
            })
            .catch((error) =>{
                console.error('The following error occurred: ' + error);
            });
        }
    })
    .catch((error) =>{
        console.error('The following error occurred: ', error);
    });
}

/**
 * Creates tables in the specified database. The names of these tables can be specified via the @tableNames parameter.
 * @param {*} connection An object with which to access the database. 
 * @param {*} databaseName The name of the database in which to create the tables.
 * @param {*} tableNames An array containing the names of the tables.
 */
function createTables(connection, databaseName, tableNames){
    tableNames.forEach(async element => {
        await createTable(connection, databaseName, element);
    });
}

/**
 * Creates a table with the specified name in the specified database.
 * @param {*} connection An object with which to access the database. 
 * @param {*} databaseName The name of the database.
 * @param {*} tableName The name of the table to create.
 */
function createTable(connection, databaseName, tableName){
    rethinkDB.
    db(databaseName)
    .tableList()
    .contains(tableName)
    .run(connection)
    .then((exists) =>{
        if (exists){
            console.log('Table ' + tableName + ' already exists and therefore was not created.');
        }
        else{
            rethinkDB.db(databaseName).tableCreate(tableName).run(connection)
            .then(() =>{
                console.log('Table ' + tableName + ' was created successfully.');
            })
            .catch((error) =>{
                console.error('The following error occurred: ' + error);
            });
        }
    });
}

/**
 * Persists a user object in the database.
 * @param {*} data The data to persist in the database.
 * @param {*} databaseName The name of the database.
 * @param {*} tableName The name of the table.
 */
async function persistUser(data, databaseName, tableName){
    var connection = null;

    await rethinkDB.connect({ host: 'localhost', port: 28015 })
      .then(conn =>{
          connection = conn;
          console.log('Database connection established.');
      })
      .catch((error) =>{
          console.error('An error occurred: ' + error);
      });

      await rethinkDB.db(databaseName).table(tableName).insert(data).run(connection);
}

/**
 * Checks whether a user exists.
 * @param {*} data The data to check for existence. 
 * @param {*} databaseName The database name.
 * @param {*} tableName The name of the table in which to look for the data.
 */
async function doesExist(data, databaseName, tableName){
    var connection = null;
    var exists = true;

    await rethinkDB.connect({ host: 'localhost', port: 28015 })
      .then(conn =>{
          connection = conn;
          console.log('Database connection established.');
      })
      .catch((error) =>{
          console.error('An error occurred: ' + error);
      });

      var exists = await rethinkDB
      .db(databaseName)
      .table(tableName)
      .filter({"_fullName" : data._fullName, "_password" : data._password})
      .count()
      .run(connection)
      .then((result) =>{
          if (result == 1){
              console.log('Setting exists to true.');
              return true;
          }
      })
      .catch((error) =>{
          console.error('Database error occurred: ' + error);
          return false;
      });

      return exists;
}

/**
 * Gets all currently stored forum entries.
 * @param {*} databaseName The name of the database. 
 * @param {*} tableName The name of the table to get data from.
 * @returns A collection of entries.
 */
async function getAllEntries(databaseName, tableName){
    var connection = null;
    
    await rethinkDB.connect({ host: 'localhost', port: 28015 })
      .then(conn =>{
          connection = conn;
          console.log('Database connection established.');
      })
      .catch((error) =>{
          console.error('An error occurred: ' + error);
      });

      var entries = await rethinkDB
      .db(databaseName)
      .table(tableName)
      .run(connection);

      return entries.toArray();
}

/**
 * Stores a forum entry in the database.
 * @param {*} entry The entry to persist. 
 * @param {*} databaseName The name of the database.
 * @param {*} tableName The name of the table.
 */
async function storeEntry(entry, databaseName, tableName){
    var connection = null;

    await rethinkDB.connect({ host: 'localhost', port: 28015 })
      .then(conn =>{
          connection = conn;
          console.log('Database connection established.');
      })
      .catch((error) =>{
          console.error('An error occurred: ' + error);
      });

      await rethinkDB
      .db(databaseName)
      .table(tableName)
      .insert(entry)
      .run(connection);
}

app.post("/user/add", function(request, response){
    console.log('User is registrating..');    
    let bodyData = request.body;

    console.log('Username: ' + bodyData._fullName);
    console.log('Password: ' + bodyData._password);

    persistUser(bodyData, databaseName, 'Users')
    .then(() =>{
        console.log('Object persisted in database.');
        response.status(201).send('user was successfully registrated.');
    })
    .catch((error) =>{
        console.error('Could not persist object in database due to error: ' + error);
        response.status(400).send('Could not perform registration.');
    });
});

app.post("/user/authenticate", function(request, response){
    console.log('User is attempting to log in..');
    let bodyData = request.body;
    console.log(bodyData);

    doesExist(bodyData, databaseName, 'Users')
    .then((resp) =>{
        console.log(resp);
        
        if (resp){
            console.log('User found.');
            response.status(200).send();
        }
        else{
            console.log('User not found');
            response.status(401).send();
        }
    });
})

app.post("/postentry", function (request, response) {
    console.log('Incoming entry');
    let bodyData = request.body;

    storeEntry(bodyData, databaseName, "Entries")
    .then(() =>{
        console.log('Entry successfully persisted.');
        console.log(bodyData);
        response.status(201).send();
    })
    .catch((error) =>{
        console.error('Error during posting entry: ' + error);
        response.status(400).send();
    })
    console.log(bodyData);
});

app.get('/getEntries', function(request, response) {
    console.log('Requesting entries..');

    getAllEntries(databaseName, "Entries")
    .then((resp) =>{
        console.log(resp);
        response.status(200).send(resp);
    })
    .catch((error) =>{
        console.log('Error occurred while fetching entries: ' + error);
        response.status(404).send();
    })
});

initializeDatabase();
app.listen(port, () =>{
    console.log('Started listening on port ' + port);
});