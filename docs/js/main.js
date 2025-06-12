document.addEventListener('DOMContentLoaded', function () {
    // Find the active span
    const activeSpans = document.querySelectorAll('span[data-active]');
    if (!activeSpans) return;

    activeSpans.forEach(activeSpan => {
        // Start from the active span and walk up, opening all parent <details>
        let parent = activeSpan.closest('details');
        let count = 0;
        while (parent && count < 10) { // Limit to 10 levels to prevent infinite loop
            parent.open = true;
            parent = parent.parentElement.closest('details');
            count++;
        }
    });
    
});
