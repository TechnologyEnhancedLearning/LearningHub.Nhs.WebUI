/**
 * Constructs and configures the control bar for the UI.
 * 
 * This function performs the following tasks:
 * 1. Selects the titlebar and controlbar elements from the DOM.
 * 2. Creates a playback toggle button with an initial "Play" state and appends it to the controlbar.
 * 3. Adds an event listener to the playback toggle button to handle play/pause functionality.
 * 4. Retrieves all buttons from the titlebar, aligns them to the right (except for the "Mute" button), and appends them to the controlbar.
 * 5. Selects the UI container element and sets up a MutationObserver to monitor changes in the container's class attribute.
 * 6. Updates the playback toggle button state based on the player's state (playing or paused) when the container's class changes.
 */

function buildControlbar(id, player) {
    var mediacontainerId = 'mediaContainer-' + id;

    // Select the titlebar and controlbar elements from the DOM
    let titlebar = document.querySelector(`#${mediacontainerId} .bmpui-ui-titlebar`);
    let controlbar = document.querySelector(`#${mediacontainerId} .bmpui-ui-controlbar`);

    // Check if both titlebar and controlbar elements exist
    if (titlebar && controlbar) {

        // Create a playback toggle button and set its initial state and appearance
        let playbackToggleButton = document.createElement('button');
        playbackToggleButton.classList.add('bmpui-ui-playbacktogglebutton', 'bmpui-off');
        playbackToggleButton.setAttribute('aria-label', 'Play');
        playbackToggleButton.innerHTML = '<span class="bmpui-label">Play</span>';
        playbackToggleButton.id = 'playback-toggle-btn-' + id;
        controlbar.appendChild(playbackToggleButton);

        // Add an event listener to the playback toggle button
        playbackToggleButton.addEventListener('click', function () {
            // Toggle playback state based on the current state
            if (player.isPlaying()) {
                player.pause();
                playbackToggleButton.classList.remove('bmpui-on');
                playbackToggleButton.classList.add('bmpui-off');
                playbackToggleButton.innerHTML = '<span class="bmpui-label">Play</span>';
            } else {
                player.play();
                playbackToggleButton.classList.remove('bmpui-off');
                playbackToggleButton.classList.add('bmpui-on');
                playbackToggleButton.innerHTML = '<span class="bmpui-label">Pause</span>';
            }
        });

        // Get all button elements from the titlebar
        let buttons = titlebar.querySelectorAll('button');

        // Reverse the button list and append each button to the controlbar
        Array.from(buttons).reverse().forEach(button => {
            if (button.textContent != "Mute") {
                button.classList.add('control-right'); // Add a class to align buttons to the right
            }
            controlbar.appendChild(button); // Append the button to the controlbar
        });

        // Select the UI container element
        let uiOverlayElement = document.querySelector(`#${mediacontainerId} .bmpui-ui-playbacktoggle-overlay`);
        uiOverlayElement.addEventListener('click', function () {
            let uiContainerElement = document.querySelector(`#${mediacontainerId} .bmpui-ui-uicontainer`);
            // Update the playback toggle button state based on the player's state
            if (uiContainerElement.classList.contains('bmpui-player-state-paused')) {
                playbackToggleButton.classList.remove('bmpui-off');
                playbackToggleButton.classList.add('bmpui-on');
            } else {
                playbackToggleButton.classList.remove('bmpui-on');
                playbackToggleButton.classList.add('bmpui-off');
            }
        });

    }
    else {
        console.error('UI container element not found');
    }
}
