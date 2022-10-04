import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PoolEntry } from '../../../../SharedData/pool';

@Injectable({
  providedIn: 'root'
})
export class PoolsService {

  constructor(private httpClient : HttpClient) { }

  /**
   * Posts a database asset to the server for storage.
   * @param poolEntry The pool entry to send to the server for storage.
   */
  public async postPoolEntry(poolEntry : PoolEntry){
    // await this.httpClient.get('http://localhost:41005/getEntries').subscribe();
    await this.httpClient.post<PoolEntry>('http://localhost:41005/postEntry/', poolEntry).toPromise();
}

/**
 * Fetches all available pool entries.
 */
public async getPoolEntriesAsync() : Promise<PoolEntry[]>{
   return this.httpClient.get<PoolEntry[]>('http://localhost:41005/getPoolEntries').toPromise();
}

}
