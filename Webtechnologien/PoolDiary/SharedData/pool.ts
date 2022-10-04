import { databaseAsset } from './DatabaseAsset'

export class PoolEntry extends databaseAsset {
    constructor(backflushDuration : number, phValue : number, comment : string, chlorineAmount : number, waterTemperature : number, airTemperature : number, date : string){
       super();
        this.backflushDuration = backflushDuration;
        this.phValue = phValue;
        this.comment = comment;
        this.chlorineAmount = chlorineAmount;
        this.waterTemperature = waterTemperature;
        this.airTemperature = airTemperature;
        this.date = date;
    }

    public backflushDuration : number;
    public phValue : number;
    public comment : string;
    public chlorineAmount : number;
    public waterTemperature : number;
    public airTemperature : number;
    public date : string;
}