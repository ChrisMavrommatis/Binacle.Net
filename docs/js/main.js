document.addEventListener('DOMContentLoaded', function () {
    
    const versionSelects = document.querySelectorAll('[data-versionselect]');
    
    if(!!versionSelects){
        versionSelects.forEach(versionSelect => {
            versionSelect.addEventListener('change', function (event) {
                const url = versionSelect.dataset.versionselect;
                const selectedVersion = event.target.value;
                if (selectedVersion) {
                    window.location.href = url + selectedVersion;
                }
            });
        });
    }
    
    // Find the active span
    const activeSpans = document.querySelectorAll('span[data-active]');
    if (!!activeSpans){
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
    }

    
    
});
