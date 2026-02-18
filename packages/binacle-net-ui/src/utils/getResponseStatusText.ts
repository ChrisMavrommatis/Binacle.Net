export function getResponseStatusText(status: number): string {
	const statusTexts: Record<number, string> ={
		400: 'Bad Request',
		401: 'Unauthorized',
		403: 'Forbidden',
		404: 'Not Found',
		422: 'Unprocessable Entity',
		429: 'Too Many Requests',
		500: 'Internal Server Error',
		502: 'Bad Gateway',
		503: 'Service Unavailable'
	};
	return statusTexts[status] || 'Error';
}
