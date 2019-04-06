import { Game } from './js/Game.js';
import { API } from './js/API.js';
import { ScoreBoard } from './js/ScoreBoard.js';
import { IPlayer } from './js/IPlayer.js';

let currentGame: Game | null = null;
let continuePlaying: boolean = true;
const api = new API();
const scoreBoard = new ScoreBoard(document.querySelector('.players'))
const startButton = document.querySelector('#start-button') as HTMLButtonElement;
const stopButton = document.querySelector('#stop-button') as HTMLButtonElement;
const game = (document.querySelector('#game') as HTMLDivElement);

function showGame() {
    startButton.style.display = 'none';
    if (currentGame != null) {
        currentGame.destroy();
    }
    stopButton.style.display = 'unset';
    game.style.display = 'unset';

    currentGame = new Game(api);
}

function endGaming() {
    currentGame.destroy();
    currentGame = null;
    startButton.style.display = 'unset';
    stopButton.style.display = 'none';
    game.style.display = 'none';
}

const eventSource = new EventSource('/api/game');
eventSource.onmessage = (event) => {
    if (event == null || event.data == null) {
        return;
    }

    const action: { type: string, player?: IPlayer, winner?: IPlayer } = JSON.parse(event.data);
    console.log(action.type);

    if (action.type === 'game-created') {
        console.log('Game has been created');
        showGame();
    } else if (action.type === 'game-ended') {
        console.log('Game ended');
        setTimeout(() => {
            if (continuePlaying) {
                api.createGame();
            } else {
                endGaming();
            }
        }, 3000);
    } else if (action.type.startsWith('player') && action.player != null) {
        console.log('Player updated');
        scoreBoard.createOrUpdatePlayer(action.player);
        if (scoreBoard.numberOfPlayers >= 2) {
            const preInfo = document.querySelector('.pre-game') as HTMLElement;
            if (preInfo.style.display == '') {
                preInfo.style.display = 'none'
                startButton.style.display = 'unset';
            }
        }
    }
};

api.getGameSpec()
    .then((spec) => {
        if (spec != null && spec.active) {
            startButton.style.display = 'none';
            showGame();
        }
    })
    .catch(() => { });

api.getPlayers()
    .then((players) => {
        players = (players || []).sort((a: IPlayer, b: IPlayer) => b.score - a.score);
        players.forEach((p) => scoreBoard.createOrUpdatePlayer(p));
        if (scoreBoard.numberOfPlayers >= 2) {
            const preInfo = document.querySelector('.pre-game') as HTMLElement;
            if (preInfo.style.display == '') {
                preInfo.style.display = 'none'
                startButton.style.display = 'unset';
            }
        }
    });

startButton.addEventListener('click', () => {
    continuePlaying = true;
    api.createGame();
});

stopButton.addEventListener('click', () => {
    continuePlaying = false;
    stopButton.disabled = true;
});
