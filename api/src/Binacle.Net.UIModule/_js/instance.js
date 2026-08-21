// The presets list on the instance page. Relative on purpose: this page describes the instance serving it,
// never whichever API the demo is pointed at.
const container = document.getElementById('presets');

if (container) {
	const status = container.querySelector('[data-presets-status]');
	const count = container.querySelector('[data-presets-count]');
	const table = container.querySelector('[data-presets-table]');
	const body = container.querySelector('[data-presets-body]');

	const say = (message) => {
		status.textContent = message;
		status.hidden = false;
	};

	const render = (presets) => {
		const names = Object.keys(presets);

		count.textContent = names.length === 1 ? '1 loaded' : `${names.length} loaded`;

		if (names.length === 0) {
			say('None. Add them to Presets.json and restart, or pack against your own boxes in the packing demo.');
			return;
		}

		for (const name of names.sort()) {
			const bins = presets[name] || [];
			const row = document.createElement('tr');

			const nameCell = document.createElement('td');
			const code = document.createElement('code');
			code.textContent = name;
			nameCell.appendChild(code);

			const countCell = document.createElement('td');
			countCell.textContent = bins.length === 1 ? '1 box' : `${bins.length} boxes`;

			const binsCell = document.createElement('td');
			binsCell.className = 'right-align small-text';
			binsCell.textContent = bins.map(describe).join(', ');

			row.append(nameCell, countCell, binsCell);
			body.appendChild(row);
		}

		status.hidden = true;
		table.hidden = false;
	};

	const describe = (bin) => `${bin.id} ${bin.length}x${bin.width}x${bin.height}`;

	fetch('/api/v4/presets')
		.then((response) => {
			if (!response.ok) {
				throw new Error(`the presets endpoint answered ${response.status}`);
			}
			return response.json();
		})
		.then((payload) => render(payload.presets || {}))
		.catch((error) => say(`Could not read them: ${error.message}.`));
}
