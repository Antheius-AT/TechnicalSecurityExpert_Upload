import { Component, OnInit } from '@angular/core';
import { PoolsService } from 'src/app/services/pools.service';
import { PoolEntry } from '../../../../../SharedData/pool';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-entry',
  templateUrl: './add-entry.component.html',
  styleUrls: ['./add-entry.component.scss']
})
export class AddEntryComponent implements OnInit {

  public airTempMaximum : number;
  public airTempMinimum : number;
  public waterTempMaximum : number;
  public chlorineMaximum : number;
  public phMinimum : number;

  public dateString : Date;
  public airTemperature : number;
  public waterTemperature : number;
  public chlorineAmount : number;
  public phValue : number;
  public backflushDuration : number;
  public comment : string;
  public daysAddedToDate : number;

  constructor(private poolsService : PoolsService, private router : Router) {
    this.airTempMaximum = 50;
    this.waterTempMaximum = 70;
    this.chlorineMaximum = 1;
    this.phMinimum = 5;
    this.airTempMinimum = -20;
  }

  ngOnInit(): void {
  }
  
  /**
   * Creates a new entry and requests for the pool service to store it in the backend database server.
   */
  public async tryAddEntry(){
    if (!this.comment || this.comment === ''){
      this.comment = 'Kein Kommentar';
    }

    this.router.navigate(['/home']);
    let poolEntry = new PoolEntry(this.backflushDuration, this.phValue, this.comment, this.chlorineAmount, this.waterTemperature, this.airTemperature, new Date().toDateString());
    await this.poolsService.postPoolEntry(poolEntry);
  }

  /**
   * Gets a value indicating whether the submit button for the add entry form is enabled.
   */
  isButtonEnabled(){
    // I am sure there is a way to properly bind the model and validate with the binding, but I didnt figure out how to do that.
    let isAirTempValid = this.airTemperature >= this.airTempMinimum && this.airTemperature <= this.airTempMaximum;
    let isWaterTempValid = this.waterTemperature >= 0 && this.waterTemperature <= this.waterTempMaximum;
    let isChlorineValueValid = this.chlorineAmount >= 0 && this.chlorineAmount <= this.chlorineMaximum;
    let isPhValueValid = this.phValue >= this.phMinimum && this.phValue <= 14;
    let isBackFlushDurationValid = this.backflushDuration > 0;

    return isAirTempValid && isWaterTempValid && isChlorineValueValid && isPhValueValid && isBackFlushDurationValid;
  }
}
