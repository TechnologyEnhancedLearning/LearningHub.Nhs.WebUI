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
    var _on = false;
    var _sessionTimeout = 0;

    var _run = function () {
        _idleTime += 1000;
        if (_idleTime > _sessionTimeout) {
            sm.showMessage();
        }
    }

    var _start = function () {
        if (_on) {
            setInterval(_run, 1000);
            //console.log(new Date().toLocaleTimeString(), 'session started..');
        }
    }

    sm.init = function (timeoutInMin) {
        _sessionTimeout = timeoutInMin * 60000;
        _on = true;
    }

    sm.reset = function () {
        _idleTime = 0;
        //console.log(new Date().toLocaleTimeString(), 'session reset..');
    }

    sm.showMessage = function () {
        window.location.href = '/session-timeout?returnUrl=' + encodeURIComponent(window.location.href);
    }

    window.addEventListener('load', _start);

    return sm;
};

// Vuelidate expects to find the variable "window.process.env.BUILD"
// This was injected by earlier versions of webpack, but webpack 5 doesn't do this any more
// Until Vuelidate is updated to be compatible with webpack 5, this is a simple-but-hacky fix
if (!window.process) { window.process = {}; }
if (!window.process.env) { window.process.env = {}; }
window.process.env.BUILD = 'web';
