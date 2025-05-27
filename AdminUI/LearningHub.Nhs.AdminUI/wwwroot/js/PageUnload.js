        // This function will be called when the browser window is closed or unloaded
    function tellServerBrowserClosed() {
        // Send an asynchronous request to the server when the browser is closed
        fetch('/api/user/browser-close', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ message: 'Browser closed' })
        })
            .then(response => response.json())
            .catch(error => console.error('Error sending data to server:', error));
        }