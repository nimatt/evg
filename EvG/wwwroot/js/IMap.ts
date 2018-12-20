import { ITileSet } from "./ITileSet";

export interface IMap {
    height: number;
    width: number;
    tileheight: number;
    tilewidth: number;
    tilesets: ITileSet[];
}