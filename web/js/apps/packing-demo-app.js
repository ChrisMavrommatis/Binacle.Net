document.addEventListener('alpine:init', () => {
    Alpine.data('packingdemoapp', () => ({
        bins: [
            {length: 60, width: 40, height: 30},
            {length: 60, width: 40, height: 40},
            {length: 60, width: 40, height: 60},

        ],
        removeBin(index) {
            this.bins.splice(index, 1);
        },
        addBin(){
            this.bins.push({length: 0, width: 0, height: 0});
        },
        clearAllBins(){
            this.bins = [];
        },
        randomizeBinsFromSamples(){
            this.bins = [
                {length: 60, width: 40, height: 30},
                {length: 60, width: 40, height: 40},
                {length: 60, width: 40, height: 60},

            ];
        }
    }))
});