import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { forumEntry } from '../../../../Shared/entry';

@Injectable({
  providedIn: 'root'
})
export class FetchEntriesService {

  constructor(private httpClient : HttpClient) { }

public async fetchEntries() : Promise<forumEntry[]> {

  let returned : forumEntry[] = [];

  await this.httpClient.get('http://localhost:41005/getentries').toPromise()
  .then((result) =>{
    returned = result as forumEntry[];

    if (returned === null){
      alert('Error retrieving');
    }
  })
  .catch(() =>{
    alert('In catch');
  });

  return returned;
}

public postEntry(entry : forumEntry) {
  return this.httpClient.post('http://localhost:41005/postentry', entry).toPromise();
}

}
