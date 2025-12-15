export function cameraFov(aspectRatio: number) {
	if (aspectRatio < 0.60) {
		return 65;
	}
	if (aspectRatio < 1) {
		return 50;
	}
	return 40;
}

