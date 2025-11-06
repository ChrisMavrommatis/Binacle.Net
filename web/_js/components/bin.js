class Bin {
    constructor(length, width, height) {
        this.length = length;
        this.width = width;
        this.height = height;
    }

    get id() {
        return `${this.length}x${this.width}x${this.height}`;
    }
}

export default Bin;
