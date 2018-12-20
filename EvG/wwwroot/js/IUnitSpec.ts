import { IAnimationSpec } from "./IAnimationSpec";

export interface IUnitSpec {
    name: string;
    spriteMap: string;
    tileWidth: number;
    tileHeight: number;
    scale: number;
    healthBarOffset: number;
    animations: {
        left: IAnimationSpec;
        right: IAnimationSpec;
        hurtLeft: IAnimationSpec;
        hurtRight: IAnimationSpec;
        attackLeft: IAnimationSpec;
        attackRight: IAnimationSpec;
        attackUp: IAnimationSpec;
        attackDown: IAnimationSpec;
        deathLeft: IAnimationSpec;
        deathRight: IAnimationSpec;
    }
}