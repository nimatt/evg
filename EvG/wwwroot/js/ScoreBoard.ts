import { IPlayer } from "./IPlayer.js";

export class ScoreBoard {
    private static readonly playerHeight = 30;

    private playerMap: Map<string, HTMLDivElement> = new Map();

    constructor(private container: HTMLElement) { }

    public get numberOfPlayers(): number {
        return this.playerMap.size;
    }

    public createOrUpdatePlayer(player: IPlayer) {
        let playerElem = this.playerMap.get(player.id);
        if (playerElem == null) {
            playerElem = this.createPlayerElem(player);
        } else {
            (playerElem.querySelector('.player-name') as HTMLDivElement).innerText = player.name;
            this.updatePlayerScore(player, playerElem);
        }
    }

    private updatePlayerScore(player: IPlayer, playerElem: HTMLElement) {
        let players = Array.from(this.container.querySelectorAll('.player') as NodeListOf<HTMLElement>)
            .map((pe) => {
                return {
                    score: Number.parseInt(pe.querySelector('.player-score').innerHTML),
                    position: Number.parseInt(/translateY\((\d+)/.exec(pe.style.transform)[1]),
                    elem: pe
                };
            });
        players = players.sort((a, b) => {
            if (a.score === b.score) {
                return a.position - b.position;
            }
            return b.score - a.score;
        });
        const playerIndex = players.findIndex((pe) => pe.elem === playerElem);
        let newIndex = playerIndex;
        while (newIndex > 0 && players[newIndex - 1].score < player.score) {
            newIndex--;
        }
        if (newIndex < playerIndex) {
            playerElem.classList.add('hidden');
            setTimeout(() => {
                for (let i = newIndex; i < playerIndex; i++) {
                    players[i].elem.style.transform = `translateY(${(i + 1) * ScoreBoard.playerHeight}px)`;
                }
                playerElem.style.transform = `translateY(${newIndex * ScoreBoard.playerHeight}px)`;
                playerElem.querySelector('.player-score').innerHTML = player.score.toFixed(0);
                setTimeout(() => {
                    playerElem.classList.remove('hidden');
                }, 250)
            }, 250);
        } else {
            playerElem.querySelector('.player-score').innerHTML = player.score.toFixed(0);
        }
    }

    private createPlayerElem(player: IPlayer) {
        const playerElem = this.getDivWithClass('player');
        const name = this.getDivWithClass('player-name');
        name.innerText = player.name;
        playerElem.appendChild(name);
        const score = this.getDivWithClass('player-score');
        score.innerText = player.score.toFixed(0);
        playerElem.appendChild(score);
        playerElem.style.transform = `translateY(${this.numberOfPlayers * ScoreBoard.playerHeight}px)`;
        this.playerMap.set(player.id, playerElem);
        this.container.appendChild(playerElem);
        return playerElem;
    }

    private getDivWithClass(className: string): HTMLDivElement {
        const elem = document.createElement('div');
        elem.classList.add(className);
        return elem;
    }
}