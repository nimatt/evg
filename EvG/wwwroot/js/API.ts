import { IGameSpec } from './IGameSpec.js';
import { IUnitSpec } from './IUnitSpec.js';
import { IPlayer } from './IPlayer.js';

export class API {
    private actionEventSource: EventSource | null = null;

    public async createGame(): Promise<void> {
        console.log('Creating game');
        await fetch('/api/game', {
            headers: [['Content-Type', 'application/json']],
            method: 'POST',
            body: '"new"'
        });
    }

    public async startGame(): Promise<void> {
        console.log('Starting game');
        await fetch('/api/game', {
            headers: [['Content-Type', 'application/json']],
            method: 'POST',
            body: '"start"'
        });
    }

    public async getGameSpec(): Promise<IGameSpec> {
        console.debug('Getting game spec');
        const response = await fetch('/api/game/spec');
        return response.json();
    }

    public async getPlayers(): Promise<IPlayer[]> {
        console.debug('Getting players')
        const response = await fetch('/api/players');
        return response.json();
    }

    public async getUnitSpecs(): Promise<IUnitSpec[]> {
        console.debug('Getting unit specs')
        const response = await fetch('/api/units');
        return response.json();
    }

    public getEventSource(): EventSource {
        return new EventSource('/api/game/actions');
    }
}