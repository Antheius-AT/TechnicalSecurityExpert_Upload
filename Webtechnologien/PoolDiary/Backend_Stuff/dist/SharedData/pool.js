"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.PoolEntry = void 0;
const DatabaseAsset_1 = require("./DatabaseAsset");
class PoolEntry extends DatabaseAsset_1.databaseAsset {
    constructor(backflushDuration, phValue, comment, chlorineAmount, waterTemperature, airTemperature, date) {
        super();
        this.backflushDuration = backflushDuration;
        this.phValue = phValue;
        this.comment = comment;
        this.chlorineAmount = chlorineAmount;
        this.waterTemperature = waterTemperature;
        this.airTemperature = airTemperature;
        this.date = date;
    }
}
exports.PoolEntry = PoolEntry;
