window.LHGlobal = window.LHGlobal || {};

LHGlobal.searchClickTelemetry = new function () {
    var endpoint = '/api/Search/RecordResultClickTelemetry';
    var selector = 'a[data-search-click-telemetry="true"]';
    var lastEventKey = null;
    var lastEventAt = 0;

    var isNewTab = function (e, link) {
        return e.button === 1 || e.ctrlKey || e.metaKey || e.shiftKey || link.target === '_blank';
    };

    var isMatchingElement = function (element) {
        if (!element || element.nodeType !== 1) {
            return false;
        }

        if (element.matches) {
            return element.matches(selector);
        }

        if (element.msMatchesSelector) {
            return element.msMatchesSelector(selector);
        }

        return false;
    };

    var findMatchingLink = function (element) {
        var currentElement = element;
        while (currentElement) {
            if (isMatchingElement(currentElement)) {
                return currentElement;
            }

            currentElement = currentElement.parentElement;
        }

        return null;
    };

    var createEventKey = function (link, e, openInNewTab, interactionType) {
        return [
            link.getAttribute('href') || '',
            link.dataset.correlationId || '',
            link.dataset.resultRank || '',
            e.button,
            openInNewTab ? '1' : '0',
            interactionType
        ].join('|');
    };

    var emitTelemetry = function (link, e, interactionType) {
        if (!link || !link.dataset) {
            return;
        }

        var openInNewTab = isNewTab(e, link);
        var eventKey = createEventKey(link, e, openInNewTab, interactionType);
        var now = Date.now();

        if (lastEventKey === eventKey && (now - lastEventAt) < 500) {
            return;
        }

        lastEventKey = eventKey;
        lastEventAt = now;

        var body = JSON.stringify({
            correlationId: link.dataset.correlationId || '',
            sessionId: link.dataset.sessionId || '',
            queryText: link.dataset.queryText || '',
            queryMode: link.dataset.queryMode || '',
            resultUrl: link.getAttribute('href') || '',
            resultTitle: (link.textContent || '').trim(),
            resultRank: Number(link.dataset.resultRank || 0),
            resourceReferenceId: Number(link.dataset.resourceReferenceId || 0),
            nodePathId: Number(link.dataset.nodePathId || 0),
            resultType: link.dataset.resultType || '',
            openInNewTab: openInNewTab,
            interactionType: interactionType
        });

        if (navigator.sendBeacon) {
            var blob = new Blob([body], { type: 'application/json; charset=UTF-8' });
            navigator.sendBeacon(endpoint, blob);
            return;
        }

        if (window.XMLHttpRequest) {
            var xhr = new XMLHttpRequest();
            xhr.open('POST', endpoint, true);
            xhr.setRequestHeader('Content-Type', 'application/json; charset=UTF-8');
            xhr.send(body);
        }
    };

    document.addEventListener('click', function (e) {
        var link = findMatchingLink(e.target);
        if (!link) {
            return;
        }

        var interactionType = e.detail === 0 ? 'keyboard' : 'click';
        emitTelemetry(link, e, interactionType);
    }, true);

    document.addEventListener('auxclick', function (e) {
        if (e.button !== 1) {
            return;
        }

        var link = findMatchingLink(e.target);
        if (!link) {
            return;
        }

        emitTelemetry(link, e, 'auxclick');
    }, true);
};
