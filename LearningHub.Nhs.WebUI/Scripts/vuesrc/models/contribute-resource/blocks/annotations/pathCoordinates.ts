export class PathCoordinates {
    x: number = undefined;
    y: number = undefined;

    constructor(init?: Partial<PathCoordinates>) {
        if (init) {
            this.x = init.x as number;
            this.y = init.y as number;
        }
    }
}
