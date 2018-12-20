import { Game } from './js/Game.js';
import { API } from './js/API.js';
import { ScoreBoard } from './js/ScoreBoard.js';
import { IPlayer } from './js/IPlayer.js';

let currentGame: Game | null = null;

function showGame() {
    document.querySelector('button').style.display = 'none';
    const game = (document.querySelector('#game') as HTMLDivElement);
    if (currentGame != null) {
        currentGame.destroy();
    }
    game.style.display = 'unset';

    currentGame = new Game(api);
}

const api = new API();
const scoreBoard = new ScoreBoard(document.querySelector('.players'))

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
            api.createGame();
        }, 3000);
    } else if (action.type.startsWith('player') && action.player != null) {
        console.log('Player updated');
        scoreBoard.createOrUpdatePlayer(action.player);
        if (scoreBoard.numberOfPlayers >= 2) {
            const preInfo = document.querySelector('.pre-game') as HTMLElement;
            if (preInfo.style.display == '') {
                preInfo.style.display = 'none'
                document.querySelector('button').style.display = 'unset';
            }
        }
    }
};

api.getGameSpec()
    .then((spec) => {
        if (spec) {
            showGame();
        }
    })
    .catch(() => { });

api.getPlayers()
    .then((players) => {
        players.forEach((p) => scoreBoard.createOrUpdatePlayer(p));
        if (scoreBoard.numberOfPlayers >= 2) {
            const preInfo = document.querySelector('.pre-game') as HTMLElement;
            if (preInfo.style.display == '') {
                preInfo.style.display = 'none'
                document.querySelector('button').style.display = 'unset';
            }
        }
    });

document.querySelector('button').addEventListener('click', () => {
    api.createGame();
})