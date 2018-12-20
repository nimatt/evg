import { IUnit } from "./IUnit";

export interface IMoveAction {
    type: 'move';
    unit: IUnit;
}