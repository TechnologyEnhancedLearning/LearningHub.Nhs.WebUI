window.LHGlobal = {};

// code to toggle between nav bar and search in mobile view
$(function () {
    $('#navbar-toggler, .menu-header>span').on('click',
        function () {
            $('nav.navbar #collapsingNavbar').toggle();
            $('nav.navbar #navbar-toggler').toggleClass('open');
        }
    );
    $('#navbar-toggler').removeAttr('disabled');

    $('.panel-contactus').on('click', function () {
        $(this).find('a')[0].click();
    });

    $('#home-beta-link').on('click', function () {
        $('#home-link')[0].click();
    });
});

LHGlobal.sessionManager = new function () {

    var sm = this;

    var _idleTime = 0;
    var _warningTime = 2 * 60 * 1000;     // 15 minutes
    var _sessionTimeout = 3 * 60 * 1000;  // 20 minutes

    var _on = false;
    var _warningShown = false;
    var _interval = null;

    var _run = function () {

        _idleTime += 1000;

        // Show warning after 15 mins
        if (_idleTime >= _warningTime && !_warningShown) {
            _warningShown = true;
            sm.showWarning();
        }

        // Timeout after 20 mins
        if (_idleTime >= _sessionTimeout) {
            sm.showMessage();
        }
    };

    var _start = function () {

        if (_on && !_interval) {
            _interval = setInterval(_run, 1000);
        }
    };

    sm.init = function () {

        _on = true;

        // Reset inactivity timer on user activity
        [
            'click',
            'mousemove',
            'keypress',
            'scroll',
            'touchstart'
        ].forEach(function (event) {

            window.addEventListener(event, sm.reset);
        });
    };

    sm.reset = function () {

        _idleTime = 0;
        _warningShown = false;

        // Remove warning popup if exists
        var warning = document.getElementById('session-warning');

        if (warning) {
            warning.remove();
        }
    };

    sm.showWarning = function () {

        // Prevent duplicate popup
        if (document.getElementById('session-warning')) {
            return;
        }

        var modal = document.createElement('div');

        modal.id = 'session-warning';

        modal.innerHTML = `
            <div class="session-overlay">
                <div class="session-modal">

                    <h2 class="nhsuk-heading-l">You’re about to be logged out</h2>

                    <p>
                        You’ve been inactive for <b>15<b> minutes.
                        If you don’t take action, your session will end in 5 minutes.
                    </p>

                    <p>
                        <strong>Your progress will not be saved.</strong>
                    </p>

                    <button id="stay-logged-in">
                        Select anywhere to stay logged in
                    </button>

                </div>
            </div>
        `;

        document.body.appendChild(modal);

        // Stay logged in button
        document
            .getElementById('stay-logged-in')
            .addEventListener('click', function () {

                sm.reset();
            });

        // Click anywhere in overlay
        modal.addEventListener('click', function () {

            sm.reset();
        });
    };

    sm.showMessage = function () {
        // Stop interval
        if (_interval) {
            clearInterval(_interval);
            _interval = null;
        }

        // Remove warning popup
        var warning = document.getElementById('session-warning');

        if (warning) {
            warning.remove();
        }
        window.location.href =
            '/session-timeout?returnUrl=' +
            encodeURIComponent(window.location.href);
    };

    window.addEventListener('load', _start);

    return sm;
};

// Vuelidate expects to find the variable "window.process.env.BUILD"
// This was injected by earlier versions of webpack, but webpack 5 doesn't do this any more
// Until Vuelidate is updated to be compatible with webpack 5, this is a simple-but-hacky fix
if (!window.process) { window.process = {}; }
if (!window.process.env) { window.process.env = {}; }
window.process.env.BUILD = 'web';
