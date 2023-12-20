var WebUI = WebUI || {};
WebUI.PagingRequest = { Page: 1, SortColumn: '', SortDirection: '', Filter: [] };

WebUI.pagingSetup = function (sortColumn, sortDirection, filter) {

    WebUI.PagingRequest.SortColumn = sortColumn;
    WebUI.PagingRequest.SortDirection = sortDirection;
    if (!filter) { filter = []; }
    WebUI.PagingRequest.Filter = filter;

    // Add Click events to column headers
    $('table.lh-datatable th.orderable').attr('onClick', 'WebUI.changeTableOrder(this);');

    // Add asc & desc class
    $('table.lh-datatable th[data-column="' + sortColumn + '"].orderable').addClass(sortDirection === 'D' ? 'desc' : 'asc');

    // Add filter row
    var columns = $('table.lh-datatable.filtered thead tr th');
    if (columns.length > 0) {
        row = document.createElement('tr');
        $(row).addClass('filter-row');

        for (var i = 0; i < columns.length; i++) {
            var columnName = $(columns[i]).attr('data-column');
            //var filterColumn = filter.find(x => x.Column === columnName); fails in IE11
            var filterColumn = filter.filter(function (x) { return x.Column === columnName; })[0];
            var filterValue = '';
            if (filterColumn) { filterValue = filterColumn.Value; }
            var inputType = 'text';
            if ($(columns[i]).attr('data-type')) { inputType = $(columns[i]).attr('data-type'); };

            if (columnName) {
                $(row).append('<td><div class="input-group"><input type="' + inputType + '" class="form-control" data-column="' + columnName + '" value="' + filterValue + '"><div class="input-group-append"><button class="btn" type="button" tabindex="-1"><i class="fas fa-search"></i></button></div></div></td>');
            } else {
                $(row).append('<td class="action-column"></td>');
            }
            //console.log('Add cell :' + i);
        }
        $('table.lh-datatable tbody').prepend(row);

        // Add click event to filter buttons
        $('table.lh-datatable tr.filter-row td button').attr('onClick', 'WebUI.filterTable();');
        $('table.lh-datatable tr.filter-row td input').keypress(function (e) {
            if (e.which === 13) {
                WebUI.filterTable();
                return false;
            }
        });
        $('table.lh-datatable tr.filter-row td input')[0].focus();
    }
};

WebUI.requestPage = function (page) {
    WebUI.PagingRequest.Page = page;
    WebUI.retrievePage();
};

WebUI.changeTableOrder = function (headerLink) {
    var sortDirection = 'A';
    var sortColumn = $(headerLink).attr('data-column');
    var currentSortDirection = $(headerLink).hasClass('asc');

    if (currentSortDirection) { sortDirection = 'D'; }

    WebUI.PagingRequest.SortDirection = sortDirection;
    WebUI.PagingRequest.SortColumn = sortColumn;

    WebUI.retrievePage();
};

WebUI.filterTable = function () {
    WebUI.PagingRequest.Filter = [];
    $('table.lh-datatable.filtered tbody tr.filter-row td input').each(function (index) {
        var filterColumn = $(this).attr('data-column');
        var filterValue = $(this).val();
        if ($(this).val() !== '') {
            WebUI.PagingRequest.Filter.push({ Column: filterColumn, Value: filterValue });
        }
    });
    WebUI.retrievePage();
};

WebUI.clearFilters = function () {
    WebUI.PagingRequest.Page = 1;
    WebUI.PagingRequest.Filter = [];
    WebUI.retrievePage();
    return false;
};

WebUI.retrievePage = function () {
    var url = window.location.pathname;

    var form = $(document.createElement('form'));
    $(form).attr("action", url);
    $(form).attr("method", "POST");

    var pagingRequestString = JSON.stringify(WebUI.PagingRequest);
    var input = $("<input>")
        .attr("type", "hidden")
        .attr("name", "pagingRequestModel")
        .val(pagingRequestString);

    $(form).append($(input));
    form.appendTo(document.body);
    $(form).submit();
};