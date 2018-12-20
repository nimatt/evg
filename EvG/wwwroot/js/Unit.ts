import { MainScene } from "./MainScene.js";
import { IUnit } from "./IUnit.js";

export class Unit {
    private healthBar: Phaser.GameObjects.Graphics;
    private maxHealth: number;
    private facingLeft = true;

    constructor(
        private sprite: Phaser.GameObjects.Sprite,
        private scene: Phaser.Scene,
        private x: number,
        private y: number,
        private stepLength: number,
        private name: string,
        public health: number,
        private healthBarOffset: number
    ) {
        this.maxHealth = health;
        this.healthBar = scene.add.graphics({
            x: this.stepLength * (this.x),
            y: this.stepLength * (this.y - healthBarOffset) + MainScene.topBarHeight
        });
        this.healthBar.depth = 10;
        this.drawHealth();

        sprite.anims.play(`${this.name}-left`);
    }

    public move(x: number, y: number) {
        if (x < this.x) {
            this.sprite.anims.play(`${this.name}-left`);
            this.facingLeft = true;
        }
        else if (x > this.x) {
            this.sprite.anims.play(`${this.name}-right`);
            this.facingLeft = false;
        }
        this.x = x;
        this.y = y;

        this.scene.tweens.add({
            targets: this.sprite,
            x: this.stepLength * (this.x + 0.5),
            y: this.stepLength * (this.y + 0.5) + MainScene.topBarHeight,
            ease: 'Cubic.easeOut',
            duration: 300,
            yoyo: false,
            repeat: 0
        });
        this.scene.tweens.add({
            targets: this.healthBar,
            x: this.stepLength * (this.x),
            y: this.stepLength * (this.y - this.healthBarOffset) + MainScene.topBarHeight,
            ease: 'Cubic.easeOut',
            duration: 300,
            yoyo: false,
            repeat: 0
        });
    }

    public attack(target: IUnit) {
        let attackDir = 'Left';
        if (target != null && target.x < this.x) {
            this.facingLeft = true;
        } else if (target != null && target.x > this.x) {
            attackDir = 'Right';
            this.facingLeft = false;
        } else if (target != null && target.y < this.y) {
            attackDir = 'Up';
        } else if (target != null && target.y > this.y) {
            attackDir = 'Down';
        }
        this.sprite.anims.play(`${this.name}-attack${attackDir}`);
        this.sprite.once('animationcomplete', () => {
            this.sprite.anims.play(`${this.name}-${this.facingLeft ? 'left' : 'right'}`);
        }, this);
    }

    public damage(damage: number) {
        const text = this.scene.add.text(
            this.stepLength * (this.x + 0.5),
            this.stepLength * (this.y + 0.5) + MainScene.topBarHeight,
            damage.toString()
        );
        this.scene.tweens.add({
            targets: text,
            y: this.stepLength * (this.y - 1) + MainScene.topBarHeight,
            ease: 'Cubic.easeOut',
            duration: 700,
            onComplete: () => {
                text.destroy();
            }
        });
        this.health = Math.max(0, this.health - damage);
        if (this.health === 0) {
            this.healthBar.destroy();
            const isLeft = this.sprite.anims.currentAnim.key.endsWith('left');
            this.sprite.anims.play(`${this.name}-death${isLeft ? 'Left' : 'Right'}`);
            this.sprite.once('animationcomplete', () => {
                this.scene.tweens.add({
                    targets: this.sprite,
                    alpha: 0,
                    ease: 'Cubic.easeIn',
                    duration: 3000,
                    yoyo: false,
                    repeat: 0,
                    onComplete: () => {
                        this.sprite.destroy();
                    }
                })
            }, this);
        } else {
            this.drawHealth();
            //this.sprite.anims.play(`${this.name}-hurt${this.facingLeft ? 'Left' : 'Right'}`);
            //this.sprite.once('animationcomplete', () => {
            //    this.sprite.anims.play(`${this.name}-${this.facingLeft ? 'left' : 'right'}`);
            //}, this);
        }
    }

    private drawHealth() {
        const healthLength = this.stepLength * (this.health / this.maxHealth);
        this.healthBar.clear();
        if (this.health <= this.maxHealth / 3) {
            this.healthBar.fillStyle(0x440000, 0.7);
        } else {
            this.healthBar.fillStyle(0x004400, 0.7);
        }
        this.healthBar.fillRect(0, 0, healthLength, 5);

        if (this.health <= this.maxHealth / 3) {
            this.healthBar.fillStyle(0xcc0000, 0.7);
        } else {
            this.healthBar.fillStyle(0x00cc00, 0.7);
        }
        this.healthBar.fillRect(1, 1, healthLength - 2, 3);
        this.healthBar.fillRect(2, 3, healthLength - 4, 1);
    }
}