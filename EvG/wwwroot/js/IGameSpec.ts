import { IMap } from "./IMap";
import { IUnitSpec } from "./IUnitSpec";
import { IUnit } from "./IUnit";
import { IPlayer } from "./IPlayer";

export interface IGameSpec {
    map: IMap;
    tilemap: string;
    units: IUnit[];
    players: IPlayer[];
    active: boolean;
}