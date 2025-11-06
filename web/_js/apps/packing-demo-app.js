import Bin from '../components/bin.js'

export default () =>({
    bins: [],
    removeBin(index) {
        this.bins.splice(index, 1);
    },
    addBin(){
        this.bins.push(new Bin(0,0,0));
    },
    clearAllBins(){
        this.bins = [];
    },
    randomizeBinsFromSamples(){
        this.bins = [
            new Bin(60,40,30),
            new Bin(60,40,40),
            new Bin(60,40,60),
        ];
    },
    init(){
        this.randomizeBinsFromSamples();
    }

});