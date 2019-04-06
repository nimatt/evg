import { Unit } from "./Unit.js";
import { IUnitSpec } from "./IUnitSpec.js";
import { IAnimationSpec } from "./IAnimationSpec.js";
import { IGameSpec } from "./IGameSpec.js";
import { IMoveAction } from "./IMoveAction.js";
import { IAttackAction } from "./IAttackAction.js";
import { IGameEnded } from "./IGameEnded.js";
import { IPlayer } from "./IPlayer.js";

export class MainScene extends Phaser.Scene {
    public static readonly topBarHeight = 40;

    private tileSet: Phaser.Tilemaps.Tileset;
    private tileMap: Phaser.Tilemaps.Tilemap;
    private units: Map<string, Unit> = new Map();

    constructor(private gameSpec: IGameSpec, private unitSpecs: IUnitSpec[], private eventSource: EventSource) {
        super({
            key: "MainScene"
        });

        eventSource.onmessage = (event) => {
            if (event == null || event.data == null) {
                return;
            }
            const action: IMoveAction | IAttackAction | IGameEnded = JSON.parse(event.data);
            if (action.type === 'move' || action.type === 'attack') {
                const unit = action.unit;
                let actor: Unit | null = null;
                if (this.units.has(unit.id)) {
                    actor = this.units.get(unit.id);
                    if (actor != null && actor.health != unit.health) {
                        actor.damage(actor.health - unit.health);
                    }
                }
                if (action.type === 'move') {
                    if (actor != null) {
                        actor.move(unit.x, unit.y);
                    }
                } else if (action.type === 'attack') {
                    if (actor != null) {
                        actor.attack(action.target);
                    }
                    if (action.target != null && this.units.has(action.target.id)) {
                        const target = this.units.get(action.target.id);
                        this.units.get(action.target.id).damage(target.health - action.target.health);
                    }
                }
            } else if (action.type === 'game-ended') {
                this.displayWinner(action.winner);
            }
        };
    }

    preload(): void {
        this.gameSpec.map.tilesets.forEach((tileSet) => {
            this.load.image(tileSet.name, tileSet.image);
        });
        this.load.tilemapTiledJSON('map', this.gameSpec.tilemap);

        this.unitSpecs.forEach((unitSpec) => {
            this.load.spritesheet(unitSpec.name,
                unitSpec.spriteMap,
                { frameWidth: unitSpec.tileWidth, frameHeight: unitSpec.tileHeight }
            );
        })
    }

    create(): void {
        const map = this.make.tilemap({ key: "map" });
        this.gameSpec.map.tilesets.forEach((tileSet) => {
            const tileset = map.addTilesetImage(tileSet.name, tileSet.name);
            map.createStaticLayer(0, tileset, 0, MainScene.topBarHeight);
        });

        this.createAnimations();

        this.gameSpec.units.forEach((unit) => {
            const sprite = this.add.sprite(
                this.gameSpec.map.tilewidth * (unit.x + 0.5),
                this.gameSpec.map.tileheight * (unit.y + 0.5) + MainScene.topBarHeight,
                unit.type);
            const unitSpec = this.unitSpecs.find((us) => us.name === unit.type);
            sprite.setScale(unitSpec.scale, unitSpec.scale);
            this.units.set(unit.id, new Unit(
                sprite,
                this,
                unit.x,
                unit.y,
                this.gameSpec.map.tileheight,
                unit.type,
                unit.health,
                unitSpec.healthBarOffset
            ));
        })

        const mapSpec = this.gameSpec.map;
        this.showIntro(mapSpec.width * mapSpec.tilewidth, mapSpec.height * mapSpec.tileheight)
    }

    public destroy() {
        this.eventSource.close();
    }

    private displayWinner(winner?: IPlayer) {
        const mapSpec = this.gameSpec.map;
        const width = mapSpec.width * mapSpec.tilewidth;
        const height = mapSpec.height * mapSpec.tileheight;
        const background = this.add.graphics();
        background.depth = 100;
        background.fillStyle(0, 0.4);
        background.fillRect(0, MainScene.topBarHeight, width, height);
        const font = {
            fontSize: '60px',
            fontFamily: 'arcade',
            color: '#00cc00',
            maxLines: 2,
            wordWrap: {
                width: width - 50,
            },
            align: 'center'
        };
        if (winner == null) {
            const drawText = this.add.text(
                width / 2,
                height / 2 + MainScene.topBarHeight,
                'Draw',
                font
            )
            drawText.depth = 100;
            drawText.setOrigin(0.5, 0.5);
        } else {
            const winnerText = this.add.text(
                width / 2,
                height / 2 + MainScene.topBarHeight,
                winner.name,
                font
            )
            const wonText = this.add.text(
                width / 2,
                height / 2 + MainScene.topBarHeight,
                'Won',
                font
            )
            winnerText.depth = 100;
            wonText.depth = 100;
            winnerText.setOrigin(0.5, 1);
            wonText.setOrigin(0.5, 0);
            if (winnerText.height > 100) { // Two rows
                winnerText.setY(height / 2 + MainScene.topBarHeight + wonText.height / 2);
                wonText.setY(height / 2 + MainScene.topBarHeight + wonText.height / 2);
            }
        }
    }

    private async showIntro(width: number, height: number) {
        const playerFont = {
            fontSize: '20px',
            fontFamily: 'arcade',
            color: '#00cc00',
            maxLines: 2,
            wordWrap: {
                width: width / 2 - MainScene.topBarHeight,
            },
            align: 'center'
        };
        const players = this.gameSpec.players.map((p) => p.name);
        const extraSpace = 5;

        await this.showExpandingText(width, height, players[0], '100px');
        const p1UnitType = this.gameSpec.units[0].type;
        const p1Sprite = this.add.sprite(
            width / 2 - MainScene.topBarHeight / 2 - extraSpace,
            MainScene.topBarHeight / 2,
            p1UnitType
        );
        const p1UnitSpec = this.unitSpecs.find((us) => us.name === p1UnitType);
        p1Sprite.setScale(p1UnitSpec.scale, p1UnitSpec.scale)
        p1Sprite.anims.play(p1UnitType + '-right');
        const p1Text = this.add.text(
            width / 2 - MainScene.topBarHeight - extraSpace,
            MainScene.topBarHeight / 2, players[0],
            playerFont
        );
        p1Text.setOrigin(1, 0.5);

        await this.showExpandingText(width, height, 'VS');

        await this.showExpandingText(width, height, players[1], '100px');
        const p2UnitType = this.gameSpec.units.find((u) => u.type !== p1UnitType).type;
        const p2Sprite = this.add.sprite(
            MainScene.topBarHeight / 2 + width / 2 + extraSpace,
            MainScene.topBarHeight / 2, p2UnitType
        );
        const p2UnitSpec = this.unitSpecs.find((us) => us.name === p2UnitType);
        p2Sprite.setScale(p2UnitSpec.scale, p2UnitSpec.scale)
        p2Sprite.anims.play(p2UnitType + '-left');
        const p2Text = this.add.text(
            MainScene.topBarHeight + width / 2 + extraSpace,
            MainScene.topBarHeight / 2,
            players[1],
            playerFont
        );
        p2Text.setY(MainScene.topBarHeight / 2 - p2Text.height / 2);

        for (let i = 3; i > 0; i--) {
            await this.showExpandingText(width, height, i.toFixed(0));
        }
        await this.showExpandingText(width, height, 'GO');
    }

    private showExpandingText(width: number, height: number, text: string, fontSize = '150px') {
        return new Promise((resolve) => {
            const textObj = this.add.text(
                width / 2, height / 2 + MainScene.topBarHeight,
                text,
                {
                    fontSize: fontSize,
                    fontFamily: 'arcade',
                    color: '#00cc00',
                    maxLines: 3,
                    wordWrap: {
                        width: width,
                    },
                    align: 'center'
                });
            textObj.depth = 20;
            textObj.alpha = 0;
            textObj.setScale(0.2, 0.2)
            this.tweens.add(
                {
                    targets: textObj,
                    alpha: 1,
                    x: width / 2 - textObj.width / 2,
                    y: height / 2 - textObj.height / 2 + MainScene.topBarHeight,
                    scaleX: 1,
                    scaleY: 1,
                    duration: 1000,
                    yoyo: false,
                    repeat: 0,
                    onComplete: () => {
                        textObj.destroy();
                        resolve();
                    }
                });
        });
    }

    private createAnimations() {
        this.unitSpecs.forEach((unitSpec) => {
            for (let animationName in unitSpec.animations) {
                if (unitSpec.animations.hasOwnProperty(animationName)) {
                    var animation: IAnimationSpec = unitSpec.animations[animationName];
                    this.anims.create({
                        key: `${unitSpec.name}-${animationName}`,
                        frames: this.anims.generateFrameNumbers(unitSpec.name, { start: animation.start, end: animation.end }),
                        frameRate: animation.frameRate,
                        repeat: animation.repeat
                    })
                }
            }
        })
    }
}