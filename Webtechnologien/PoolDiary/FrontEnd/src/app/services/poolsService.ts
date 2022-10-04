import { HttpClient } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { PoolEntry } from '../../../../SharedData/pool';

@Injectable()
export class poolsService{

    /**
     * Initializes a new instance of the poolsService class.
     * @param httpClient The http client used for connecting to and requesting data from the backend database server.
     */
    constructor(private httpClient : HttpClient){
    }

    public async getPoolEntries() : Promise<PoolEntry[]>{
        let returnedEntries : PoolEntry[];

        let result = await this.httpClient.get('http://localhost:6666/getPoolEntries')
        .toPromise()
        .then((result) => {
            let poolResult = result as PoolEntry[];

            if (!poolResult){
                console.error('Error during request.');
            }
            else{
                console.error('Successfully fetched pool data.');
                return result;
            }
        })
        .catch((error) =>{
            console.error('Error while fetching data: ', error);
        });

        return returnedEntries;
    }
}