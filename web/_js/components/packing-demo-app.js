export default (base_url) => ({

	getResults(request){
		console.log('Getting results from Packing Demo App', request);
		fetch(`${base_url}/api/v3/pack/by-custom`, {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(request)
		})
			.then(response => response.json())
			.then(data => {
				console.log('Packing results:', data);
			})
			.catch(error => {
				console.error('Error fetching packing results:', error);
			});

	},

});


