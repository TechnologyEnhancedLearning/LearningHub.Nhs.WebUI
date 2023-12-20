var AdminUI = AdminUI || {};

AdminUI.PagingRequest = { Page: 1, SortColumn: '', SortDirection: '', Filter: [], UseAjax: false };

AdminUI.pagingSetupAjax = function (sortColumn, sortDirection, filter, presetFilter, ajaxUrl, resultsElementId) {
    if (!presetFilter) { presetFilter = []; }
    AdminUI.PagingRequest.PresetFilter = presetFilter;

    AdminUI.PagingRequest.UseAjax = true;
    AdminUI.PagingRequest.AjaxUrl = ajaxUrl;
    AdminUI.PagingRequest.ResultsElementId = resultsElementId;
    AdminUI.pagingSetup(sortColumn, sortDirection, filter);
}

AdminUI.url = window.location.pathname;

AdminUI.pagingSetup = function (sortColumn, sortDirection, filter, selectOptions, $table, url) {
    if (url) {
        AdminUI.url = url;
    }
    AdminUI.PagingRequest.SortColumn = sortColumn;
    AdminUI.PagingRequest.SortDirection = sortDirection;

    if (!filter) { filter = []; }
    AdminUI.PagingRequest.Filter = filter;

    if (!$table) {
        $table = $('table.lh-datatable');
    }
    AdminUI.initialiseTable($table, selectOptions);
};

AdminUI.initialiseTable = function ($table, selectOptions) {
    // Add Click events to column headers
    $table.find('th.orderable').attr('onClick', 'AdminUI.changeTableOrder(this);');

    // Add asc & desc class
    $table.find('th[data-column="' + AdminUI.PagingRequest.SortColumn + '"].orderable').addClass(AdminUI.PagingRequest.SortDirection === 'D' ? 'desc' : 'asc');

    // Add filter row
    //var columns = $('table.lh-datatable.filtered thead tr th');
    if ($table.hasClass('filtered')) {
        var columns = $table.find('thead tr th');
        row = document.createElement('tr');
        $(row).addClass('filter-row');

        for (var i = 0; i < columns.length; i++) {
            var columnName = $(columns[i]).attr('data-column');
            //var filterColumn = filter.find(x => x.Column === columnName); fails in IE11
            var filterColumn = AdminUI.PagingRequest.Filter.filter(function (x) { return x.Column === columnName; })[0];
            var filterValue = '';

            if (filterColumn) { filterValue = filterColumn.Value; }
            var inputType = 'text';
            if ($(columns[i]).attr('data-type')) { inputType = $(columns[i]).attr('data-type'); };



            if (columnName && inputType != "select") {
                $(row).append('<td><div class="input-group"><input type="' + inputType + '" class="form-control" data-column="' + columnName + '" value="' + filterValue + '"><div class="input-group-append"><button class="btn" type="button" tabindex="-1"><i class="fas fa-search"></i></button></div></div></td>');
            }
            else if (columnName && inputType == "select") {
                let selectOptionsGroup = selectOptions.filter(function (o) { return o.Type == columnName; });
                var allselectOptions = "";

                var k = 0;
                do {
                    allselectOptions += "<option value='" + selectOptionsGroup[k].Name + "'>";
                    k++;
                }
                while (k > 0 && selectOptionsGroup[k] != "" && k < selectOptionsGroup.length)

                var inp = '<td><div class="input-group"><input id="inputSelect' + columnName + '" onclick="fn_inpuSelect(this)" data-column="' + columnName + '" list="' + columnName + '" name="' + columnName + '" type="text" class="form-control" data-column="' + columnName + '" value="' + filterValue + '">';
                inp += '<datalist id="' + columnName + '">';
                inp += allselectOptions;
                inp += '</datalist>';
                inp += '<div class="input-group-append" > <button class="btn" type="button" tabindex="-1"><i class="fas fa-search"></i></button></div ></div></td>';
                $(row).append(inp);
            } else {
                $(row).append('<td class="action-column"></td>');
            }
        }
        $table.find('tbody').prepend(row);

        // Add click event to filter buttons
        $table.find('tr.filter-row td button').on('click', function () { AdminUI.filterTable($table); });
        $table.find('tr.filter-row td input').keypress(function (e) {
            if (e.which === 13) {
                AdminUI.filterTable($table);
                return false;
            }
        });
        $table.find('tr.filter-row td input')[0].focus();
    }
};

AdminUI.requestPage = function (page) {
    AdminUI.PagingRequest.Page = page;
    AdminUI.retrievePage();
};

AdminUI.changeTableOrder = function (headerLink) {
    var sortDirection = 'A';
    var sortColumn = $(headerLink).attr('data-column');
    var currentSortDirection = $(headerLink).hasClass('asc');

    if (currentSortDirection) { sortDirection = 'D'; }

    AdminUI.PagingRequest.SortDirection = sortDirection;
    AdminUI.PagingRequest.SortColumn = sortColumn;

    if (AdminUI.PagingRequest.UseAjax) {
        AdminUI.PagingRequest.Page = 1;
    }
    AdminUI.retrievePage();
};

AdminUI.filterTable = function ($table) {
    AdminUI.PagingRequest.Filter = [];
    $table.find('tbody tr.filter-row td input').each(function (index) {
        var filterColumn = $(this).attr('data-column');
        var filterValue = $(this).val();
        var dataType = $(this).attr('type');
        if ($(this).val() !== '') {
            AdminUI.PagingRequest.Filter.push({ Column: filterColumn, Value: filterValue, Type: dataType });
        }
    });
    if (AdminUI.PagingRequest.UseAjax) {
        AdminUI.PagingRequest.Page = 1;
    }
    AdminUI.retrievePage();
};

AdminUI.clearFilters = function () {
    AdminUI.PagingRequest.Page = 1;
    AdminUI.PagingRequest.Filter = [];
    AdminUI.retrievePage();
    return false;
};

AdminUI.retrievePage = function () {
    if (AdminUI.PagingRequest.UseAjax) {
        var pagingRequestString = JSON.stringify(AdminUI.PagingRequest);
        var url = window.location.origin + '/' + AdminUI.PagingRequest.AjaxUrl;
        $.ajax({
            cache: false,
            type: "POST",
            url: url,
            data: { pagingRequestModel: pagingRequestString },
            success: function (data) {
                $(AdminUI.PagingRequest.ResultsElementId).html(data);
                $table = $(AdminUI.PagingRequest.ResultsElementId + ' table.lh-datatable');

                AdminUI.initialiseTable($table);
                if (typeof pagingAjax_PageChanged === "function") {
                    pagingAjax_PageChanged();
                }
            },
            error: function () {
                $(AdminUI.PagingRequest.ResultsElementId).html('<div class="alert alert-danger">Error loading page</div>');
            }
        });
    }
    else {
        var form = $(document.createElement('form'));
        $(form).attr("action", AdminUI.url);
        $(form).attr("method", "POST");

        var pagingRequestString = JSON.stringify(AdminUI.PagingRequest);
        var input = $("<input>")
            .attr("type", "hidden")
            .attr("name", "pagingRequestModel")
            .val(pagingRequestString);

        $(form).append($(input));
        form.appendTo(document.body);
        $(form).submit();
    }
};

(function () {
    function AdminDatatable(config, request) {
        var self = this;
        self.request = {
            Page: 1,
            SortColumn: '',
            SortDirection: '',
            Filter: []
        };
        self.response = {
            Page: 1,
            TotalItemCount: 0
        };
        self.request = $.extend({}, self.request, request);
        if (!self.request.Filter) { self.request.Filter = []; }
        self.$table = config.table;
        self.url = config.url;
        self.ajax = config.ajax;
        self.templates = config.templates;
        self.templateValues = config.templateValues;
        // Column header click events for ordering
        self.$table.find('th.orderable').on('click', function () { self.changeTableOrder(this) });

        // Asc/Desc classes
        self.$table.find('th[data-column="' + self.request.SortColumn + '"].orderable').addClass(self.request.SortDirection === 'D' ? 'desc' : 'asc');

        if (self.$table.hasClass('filtered')) {
            var columns = self.$table.find('thead tr th');
            row = document.createElement('tr');
            $(row).addClass('filter-row');

            for (var i = 0; i < columns.length; i++) {
                var columnName = $(columns[i]).data('column');
                //var filterColumn = filter.find(x => x.Column === columnName); fails in IE11
                var filterColumn = self.request.Filter.filter(function (x) { return x.Column === columnName; })[0];
                var filterValue = '';
                if (filterColumn) { filterValue = filterColumn.Value; }
                var inputType = 'text';
                if ($(columns[i]).data('type')) { inputType = $(columns[i]).data('type'); };

                if (columnName) {
                    $(row).append('<td><div class="input-group"><input type="' + inputType + '" class="form-control datatable-search" data-column="' + columnName + '" value="' + filterValue + '"><div class="input-group-append"><button class="btn" type="button" tabindex="-1"><i class="fas fa-search"></i></button></div></div></td>');
                } else {
                    $(row).append('<td class="action-column"></td>');
                }
                //console.log('Add cell :' + i);
            }
            self.$table.find('tbody').prepend(row);

            // Add click event to filter buttons
            self.$table.find('tr.filter-row td button').on('click', function () { self.filterTable(); });
            self.$table.find('tr.filter-row td input').keypress(function (e) {
                if (e.which === 13) {
                    self.filterTable();
                    return false;
                }
            });
            self.$table.find('tr.filter-row td input')[0].focus();
        }

        self.changeTableOrder = function (columnHeader) {
            var $columnHeader = $(columnHeader);

            self.request.SortColumn = $columnHeader.data('column')
            self.request.SortDirection = $columnHeader.hasClass('asc') ? 'D' : 'A';
            self.updateTable();
        }

        self.filterTable = function () {
            self.$table.find('tbody tr.filter-row td input').each(function (index, e) {
                var $e = $(e);
                var filterColumn = $e.data('column');
                var filterValue = $e.val();
                if (filterValue !== '') {
                    self.request.Filter.push({ Column: filterColumn, Value: filterValue });
                }
            });
            self.request.Page = 1;
            self.updateTable();
        }

        self.clearFilter = function () {
            self.request.Page = 1;
            self.request.Filter = [];
            self.$table.find('tbody tr:not(.filter-row)').remove();
            $('.datatable-search').val("");
            self.$table.find('tbody').append("<tr><td colspan='6'>To search for users, type into the search boxes above.</td></tr>");
        }

        self.updateTable = function () {
            self.$table.find('tbody tr:not(.filter-row)').remove();
            self.$table.find('tbody').append('<tr><td colspan="6" style="display: flex; justify-content: center; align-items: middle;"><i class="fa fa-spinner fa-spin"></i></td></tr>');
            $.post(self.url, self.request)
                .then(function (response) {
                    var resultItems = response.results.items;
                    self.$table.find('tbody tr:not(.filter-row)').remove();
                    var columns = self.$table.find('th.orderable');
                    var columnNamesOrdered = [];
                    columns.each(function (i, e) {
                        columnNamesOrdered[i] = $(e).data('property');
                    });
                    for (var i = 0; i < resultItems.length; i++) {
                        var item = resultItems[i];
                        var values = getTemplateValues(self.templateValues, item);
                        var $row = $('<tr></tr>');

                        for (var j = 0; j < columnNamesOrdered.length; j++) {
                            var columnName = columnNamesOrdered[j];
                            var value = item[columnName];
                            var template = self.templates[columnName];
                            if (template) {
                                value = replaceTemplateValues(template, values, value);
                            }
                            $row.append('<td>' + value + '</td>');
                        }
                        $row.append('<td><input style="height: 20px !important; width: 20px;" type="checkbox" class="admin-checkbox" value="' + item.userId + '"/></td>')
                        self.$table.find('tbody').append($row);
                    }
                    self.response.Page = response.paging.currentPage;
                    self.response.TotalItemCount = response.results.totalItemCount;
                    self.displayPagination();
                });
        }

        function getTemplateValues(templateValues, model) {
            var output = {};
            for (var value in templateValues) {
                output[value] = templateValues[value](model);
            }
            return output;
        }

        function replaceTemplateValues(template, templateValues, value) {
            template = template.replace("{value}", value);
            var regex = /\{(\w+)\}/;
            var groups = regex.exec(template) || [];
            for (var i = 1; i < groups.length; i++) {
                var group = groups[i];
                template = template.replace("{" + group + "}", templateValues[group]);
            }
            return template;
        }

        self.navigatePage = function (page) {
            self.request.Page = page;
            self.updateTable();
        }

        self.displayPagination = function () {
            // clear old pagination if it exists
            self.$table.parent().find('.paging-container').remove();
            
            var currentPage = self.response.Page;
            var minPage = currentPage == 1 ? 1 : currentPage - 1;
            var totalPages = Math.ceil(self.response.TotalItemCount / self.request.PageSize);
            if (currentPage == totalPages) {
                minPage = currentPage - 2;
            }
            var maxPage = currentPage == 1 ? currentPage + 2 : currentPage + 1;

            console.log(currentPage, totalPages, minPage, maxPage);

            // Showing X - Y of Z
            var listHeader = self.$table.parent().find('.admin-list-header');
            var showingText = listHeader.children(':first-child()');
            var from = ((currentPage - 1) * self.request.PageSize) + 1;
            var to = currentPage * self.request.PageSize;
            var filtered = self.request.Filter.length > 0;
            var filteredText = filtered ? ' (filtered)' : '';
            showingText.text('Showing ' + from + ' - ' + to + ' of ' + self.response.TotalItemCount + filteredText);
            var clearFilters = listHeader.find('.right-side');
            if (!clearFilters.find('a').length) {
                var clearFiltersButton = $('<a href="#">Clear filters</a>');
                clearFiltersButton.on('click', function () {
                    self.clearFilter();
                });
                clearFilters.append(clearFiltersButton);
            }

            if (self.response.TotalItemCount < self.request.PageSize) {
                return;
            }

            var pagination = $('<div class="row paging-container d-flex justify-content-between"></div>');
            var prevPag = $('<div class="text-left navigate"></div>');
            pagination.append(prevPag);
            if (currentPage > 1) {
                prevPag.append($('<i class="fa fa-arrow-left"></i>'));
                var prevLink = $('<a href="#">Previous page</a>');
                prevLink.on('click', function () {
                    self.navigatePage(currentPage - 1);
                });
                prevPag.append(prevLink);
            }
            var paginationCenter = $('<div class="text-center"></div>');
            pagination.append(paginationCenter);
            var paginationCenterList = $('<ul class="paging-page-list"></ul>')
            paginationCenter.append(paginationCenterList);
            if (minPage > 1) {
                var oneLink = $('<a href="#">1</a>')
                oneLink.on('click', function () { self.navigatePage(1) });
                var oneLinkListItem = $('<li></li>');
                oneLinkListItem.append(oneLink);
                paginationCenterList.append(oneLinkListItem);
                if (minPage > 2) {
                    paginationCenterList.append($('<li>...</li>'));
                }
            }
            for (var i = minPage; i <= maxPage; i++) {
                if (i > 0 && i <= totalPages) {
                    if (i == currentPage) {
                        paginationCenterList.append($('<li class="active">' + i + '</li>'));
                    } else {
                        var link = $('<a href="#">' + i + '</a>');
                        link.on('click', function () { self.navigatePage(i) });
                        var listItem = $('<li></li>');
                        listItem.append(link);
                        paginationCenterList.append(listItem);
                    }
                }
            }
            if (totalPages > (currentPage + 1)) {
                if ((maxPage + 1) < totalPages) {
                    paginationCenterList.append($('<li>...</li>'));
                }
                if (maxPage < totalPages) {
                    var link = $('<a href="#">' + totalPages + '</a>');
                    link.on('click', function () {
                        self.navigatePage(totalPages);
                    });
                    var listItem = $('<li></li>');
                    listItem.append(link);
                    paginationCenterList.append(listItem);
                }
            }
            var nextPag = $('<div class="text-right navigate"></div>');
            if (currentPage < totalPages) {
                var link = $('<a href="#">Next page</a>');
                link.on('click', function () { self.navigatePage(currentPage + 1); });
                var arrow = $('<i class="fa fa-arrow-right"></i>');
                nextPag.append(link);
                nextPag.append(arrow);
            }
            pagination.append(nextPag);
            self.$table.after(pagination);

        }
    }
    window.AdminDatatable = AdminDatatable;
}());