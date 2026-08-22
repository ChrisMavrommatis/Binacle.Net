// three ships OrbitControls as ESM only and the commonjs transform cannot load it. It reaches the module
// graph because the plugin barrels import the visualizer; nothing that imports it here ever constructs one.
export class OrbitControls {
	constructor(..._args: unknown[]) {
	}

	update() {
	}

	dispose() {
	}
}
