/// <reference path="../types/phaser.d.ts" />

import { API } from "./API.js";
import { IGameSpec } from "./IGameSpec.js";
import { TileType } from "./TileType.js";
import { MainScene } from './MainScene.js';
import { IUnitSpec } from "./IUnitSpec.js";

export class Game {
    public engine: Phaser.Game;
    public spec: IGameSpec;
    private config: GameConfig;
    private unitSpecs: IUnitSpec[];

    constructor(private api: API) {
        this.getSpecs()
            .then(() => {
                console.log('Creating visual game')
                var map = this.spec.map;
                this.config = {
                    type: Phaser.AUTO,
                    parent: 'game',
                    width: map.width * map.tilewidth,
                    height: map.height * map.tileheight + MainScene.topBarHeight,
                    scene: new MainScene(this.spec, this.unitSpecs, api.getEventSource())
                };

                this.engine = new Phaser.Game(this.config);
            })
            .then(() => {
                setTimeout(() => api.startGame(), 7000);
            });
    }

    public destroy() {
        if (this.engine != null) {
            this.engine.destroy(true, false);
        }
        if (this.config != null) {
            (this.config.scene as MainScene).destroy();
        }
    }

    private async getSpecs() {
        this.spec = await this.api.getGameSpec();
        this.unitSpecs = await this.api.getUnitSpecs();
    }
}