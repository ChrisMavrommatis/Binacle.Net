// The two declaration files carry ambient augmentations - Window.binacle, Alpine's $logger magic and
// _x_fieldPrefix - and nothing imports them, so without these references a host never adds them to its
// program and every use of the three reports as a missing property.
/// <reference path="./src/types/global.d.ts" />
/// <reference path="./src/types/alpine.d.ts" />

export {packingDemoPlugin} from "./src/packingDemoPlugin";
export {protocolDecoderPlugin} from "./src/protocolDecoderPlugin";
