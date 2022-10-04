import { Component, OnInit } from '@angular/core';
import { PoolsService } from 'src/app/services/pools.service';
import { poolsService } from 'src/app/services/poolsService';
import { PoolEntry } from '../../../../../SharedData/pool';

@Component({
  selector: 'app-pool-diary',
  templateUrl: './pool-diary.component.html',
  styleUrls: ['./pool-diary.component.scss']
})
export class PoolDiaryComponent implements OnInit {

public pools : PoolEntry[];

  constructor(private poolsService : PoolsService) {
    // Configure table to be displayed dynamically.
  }

  ngOnInit(): void {
    this.poolsService.getPoolEntriesAsync()
    .then((result)=>{
      if (result){
        this.pools = result;
      }
      else{
        // Debugging.
        this.pools = [new PoolEntry(5, 5, 'In Else branch', 5, 5, 5, new Date().toDateString())]
      }
    })
    .catch(()=>{
      // Debugging
      this.pools = [new PoolEntry(5, 5, 'In error branch', 5, 5, 5, new Date().toDateString())]

      // Log error if possible.
    });
    }
}
