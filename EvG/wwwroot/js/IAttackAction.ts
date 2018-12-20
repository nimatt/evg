import { IUnit } from "./IUnit";

export interface IAttackAction {
    type: 'attack';
    unit: IUnit;
    target: IUnit;
}