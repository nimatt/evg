import { IPlayer } from "./IPlayer";

export interface IGameEnded {
    type: 'game-ended';
    winner?: IPlayer;
}