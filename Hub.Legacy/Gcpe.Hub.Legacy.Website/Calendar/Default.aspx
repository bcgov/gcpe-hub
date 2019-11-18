<%@ Page Title="" Language="C#" MasterPageFile="~/Calendar/Site.master" AutoEventWireup="True"
    CodeBehind="Default.aspx.cs" Inherits="Gcpe.Hub.Calendar.Default" ClientIDMode="Static"%>
<%@ Import Namespace="Gcpe.Hub.Calendar" %>
<%@ MasterType VirtualPath="~/Calendar/Site.master" %>
<%@ Register Src="~/Calendar/UCFlexiGrid.ascx" TagName="FlexiGrid" TagPrefix="UC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">

    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/themes/base/all.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/kendo/2016.2.504/kendo.common.min.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/kendo/2016.2.504/kendo.default.min.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/jquery.alerts.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/validationEngine.jquery.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/flexigrid.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/fullcalendar.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/uniform.default.css") %>" />

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-ui-1.11.4.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/kendo/2016.2.504/kendo.ui.core.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/kendo/2016.2.504/kendo.popup.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/kendo/2016.2.504/kendo.data.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/kendo/2016.2.504/kendo.list.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/kendo/2016.2.504/kendo.multiselect.min.js") %>"></script>

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/flexigrid.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/moment.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.uniform.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/fullcalendar.js") %>"></script>

    <script type="text/javascript" src="<%= ResolveUrl("~/Calendar/Scripts/activityList.js") %>"></script>

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.alerts.js") %>"></script>

    <link rel="stylesheet" href="<%= ResolveUrl("~/Calendar/Content/Custom.css") %>" />

</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">

    <script type="text/javascript" charset="utf-8">

        //----------------------------------------------------------------------
        // Used by Search and Save to get the URL with parameters
        function AddToFilter(filters, name, value) {
            if (value) {
                filters[name] = value;
            }
        }

        function AddSelect(filters, name, fselect) {
            // use select option that has the clear text (for snowplow)
            if (fselect.selectedIndex !== 0) {
                filters[name] = fselect[fselect.selectedIndex];
            }
        }

        function AddToFilters(filters) {
            AddSelect(filters, 'status', $("#fstatus")[0]);
            AddSelect(filters, 'category', $("#fcategory")[0]);
            AddSelect(filters, 'ministry', $("#fministry")[0]);
            AddSelect(filters, 'contact', $("#fcontact")[0]);
            AddSelect(filters, 'representative', $("#frepresentative")[0]);
            AddSelect(filters, 'initiative', $("#finitiative")[0]);
            AddSelect(filters, 'premierRequested', $("#fpremier")[0]);
            AddSelect(filters, 'distribution', $("#fdistribution")[0]);
            AddSelect(filters, 'isissue', $("#fissue")[0]);
            AddSelect(filters, 'dateConfirmed', $("#fdateConfirmed")[0]);

            var fkeyword = $("#fkeyword").getKendoMultiSelect();
            if (fkeyword) {
                AddToFilter(filters, 'keywords', fkeyword.dataItems().reduce(function (acc, item) {
                    return acc ? { value: acc.value + '~' + item.value, text: acc.text + ' or ' + item.text } : item;
                }, null));
            }
            AddToFilter(filters, 'datefrom', $("#StartDateTextBox").val());
            AddToFilter(filters, 'dateto', $("#EndDateTextBox").val());
            var display = $("#DisplayRadioButtonList input:checked")[0];
            AddToFilter(filters, 'display', display.value === '3' ? null : { value: display.value, text: display.nextSibling.textContent });
            AddToFilter(filters, 'thisdayonly', $("#ThisDayOnlyCheckBox")[0].checked);
        }

        function GetQuery(filters, txtSearch, forPost) { // Modifies filters too
            // lookahead is for User AND Corporate queries
            if ($("#lookaheadNo")[0].checked) {
                filters['lookahead'] = false;
            } else {
                AddToFilter(filters, 'lookahead', $("#lookaheadYes")[0].checked);
            }
            var filterArray = Object.keys(filters).map(function (key) {
                var val = filters[key];
                if (val.value !== undefined) {
                    filters[key] = val.text; // to be used later by snowplow
                    val = val.value;
                }
                return key + '=' + val;
            });
            if (txtSearch !== '') {
                filterArray.push('quickSearch=' + txtSearch); // do not put this in filters because snowplow has a different argument for it
            }
            return filterArray.join(forPost ? "|" : "&");
        }

        //----------------------------------------------------------------------
        // Resets all the filters options, likely more intuitive to only have 
        // one reset button...but is an existing functionality
        function ResetFilterOptions() {
            //UpdateCommunicationContacts();

            $("#fkeyword").getKendoMultiSelect().value([]);
            $.uniform.update($("#txtSearch").val(""));
            $.uniform.update($("#fmore").val("*"));
            $.uniform.update($("#fcategory").val("*"));

            ResetFilterOption($("#fstatus"));
            ResetFilterOption($("#fministry"));
            ResetFilterOption($("#fcontact"));
            ResetFilterOption($("#frepresentative"));
            ResetFilterOption($("#finitiative"));
            ResetFilterOption($("#fpremier"));
            ResetFilterOption($("#fdistribution"));
            ResetFilterOption($("#fissue"));
            ResetFilterOption($("#fdateConfirmed"));
            $("#DisplayRadioButtonList_1").next('label').text(myMinLabel);
            $("#DisplayRadioButtonList_2").next('label').text("My Activities Only");
        }

        //----------------------------------------------------------------------
        // Reset the date filters
        function ResetFilterOption(filter) {
            filter.parent().hide();
            $.uniform.update(filter.val("*"));
        }

        function ResetFilterDates() {
            var fdatefrom = $("#StartDateTextBox").val("");
            var fdateto = $("#EndDateTextBox").val("");
            $.uniform.update(fdatefrom);
            $.uniform.update(fdateto);
            var thisDayOnly = $("#ThisDayOnlyCheckBox")[0];
            thisDayOnly.checked = false;
            $.uniform.update(thisDayOnly);
        }


        //----------------------------------------------------------------------
        // This is called from the reset and from the change event of the
        // ministry drop down
        function UpdateCommunicationContacts() {

            $("#fcontact").empty();
            $.uniform.update("#fcontact");
            
            $.getJSON("MinistryHandler.ashx?Op=GetMinistryCommunicationContacts", { ministryCode: $("#fministry option:selected").text(), sortOrder: 'FirstName' }, function (j) {
                for (var k = 0; k < j[0].length; k++) { // There are 3 columns returned (from the view) for each row
                    var opt = $('<option />', {
                        value: j[0][k].SystemUserId,
                        text: j[0][k].Name
                    });
                    opt.appendTo($("#fcontact"));
                    $.uniform.update("#fcontact");
                }
            });
            
            $("#fcontact").prepend("<option value='*' selected='selected'>Select Comm. Contact</option>");
            $("#fcontact").val("*");
            $.uniform.update("#fcontact");

        }


        //----------------------------------------------------------------------
        // Update the Grid by the search Filters
        function search(filters) {
            var onMobile = !$("#NavigationMenu").is(':visible');
            var sidebarIsClosed = !$('#contentwrapper').hasClass("visible");
            if (onMobile !== sidebarIsClosed) {
                toggleSidebar(true);
            }

            $("#loadingPanel").modal('show');
            if (!filters) {
                filters = {};
                AddToFilters(filters);
            }
            var txtSearch = filters.op === 'rcc' ? '' : encodeURIComponent($("#txtSearch").val());;
            var queryParams = GetQuery(filters, txtSearch);

            var activityGridDiv = $("#activityGridDiv");
            var calendarDiv = $('#calendarDiv');
            initCalendar(calendarDiv);

            $('#ActivityGrid_tbl').flexOptions({
                url: 'ActivityListProvider.ashx' + (queryParams === '' ? queryParams : '?' + queryParams),
                newp: 1,
                preProcess: function (data) {
                    $('.popover').popover('hide'); // force the current tooltip to close so it does not get stuck
                    calendarDiv.fullCalendar('addEventSource', data.rows);

                    if (data.page === 1) {
                        setupInfiniteScrolling(activityGridDiv);
                    } else {
                        filters['page'] = data.page;
                    }
                    $("#lastLoadedDatetime").val(data.loadedTime);
                    $(".fTitle").text("B.C Government Activities: " + data.total);
                    if (queryParams !== '') {
                        window.snowplow('trackSiteSearch', txtSearch.split(' '), filters, data.total);
                    }
                    return data;
                },
                onToggleCol: function (cid) {
                    filters = {'cid2toggle': cid};
                    AddToFilters(filters);
                    search(filters);
                },
                onSuccess: function () {
                    if (activityGridDiv.css("display") === "none") {
                        if (getMoreActivitiesIfNeeded(calendarDiv)) return;
                    }

                    $("#loadingPanel").modal('hide');
                    $('div#loadmoreajaxloader').hide();
                },
                onError: function () { $("#loadingPanel").modal('hide'); }
            }).flexReload();
            $('#SelectedTextBox').val('');

            var gridIsHidden = activityGridDiv.css("display") === "none";
            if (onMobile !== gridIsHidden) // force grid view on desktop and calendar view on mobile
                toggleView();
        }

        function setupInfiniteScrolling(activityGridDiv) {
            // Infinite scroll
            $(window).scroll(function () {
                if (activityGridDiv.css("display") === "none") return;
                var scrollTop = $(window).scrollTop();
                //var sidebarIsVisible = $('#contentwrapper').hasClass("visible");
                //if (sidebarIsVisible ? scrollTop > 600 : scrollTop < 100)toggleSidebar();

                if (scrollTop > $(document).height() - $(window).height() - 30) {
                    var grid = $('#ActivityGrid_tbl')[0].grid;
                    grid.changePage('next');
                    if (grid.loading)
                        $('div#loadmoreajaxloader').show();
                    else
                        $('div#loadmoreajaxloader').hide();
                }
            });
        }

        var dateMin = null, dateMax = null;
        function toggleView() {
            var activityGridDiv = $("#activityGridDiv");
            var calendarDiv = $('#calendarDiv');
            if (activityGridDiv.css("display") === "none") {
                $(".toggleView").text("Calendar View");
                calendarDiv.hide();
                activityGridDiv.show();
            } else {
                $(".toggleView").text("Grid View");

                var th = $("th.sorted", activityGridDiv);
                if (th.prop("abbr") !== "StartEndDateTime" || $(".sasc", th).length === 0) {
                    var newSort = $("th[abbr=StartEndDateTime]");
                    dateMin = null, dateMax = null;
                    calendarDiv.fullCalendar('removeEvents');
                    $('#ActivityGrid_tbl')[0].grid.changeSort(newSort);
                }
                activityGridDiv.hide();
                calendarDiv.show();
                calendarDiv.fullCalendar('reportEventChange');
                getMoreActivitiesIfNeeded(calendarDiv);
            }
        }

        function getMoreActivitiesIfNeeded(calendarDiv, view) {
            if (!dateMax) return false;
            if (!view) view = calendarDiv.fullCalendar('getView');

            if (view.intervalStart <= new Date(dateMin)) {
                calendarDiv.find(".fc-prev-button").addClass('fc-state-disabled');
            }
            else {
                calendarDiv.find(".fc-prev-button").removeClass('fc-state-disabled');
            }
            var endDate = $("#EndDateTextBox").val();
            if (endDate)
            {
                if (view.intervalEnd >= new Date(endDate)) {
                    calendarDiv.find(".fc-next-button").addClass('fc-state-disabled');
                }
                else {
                    calendarDiv.find(".fc-next-button").removeClass('fc-state-disabled');
                }
            }

            if (view.intervalEnd < new Date(dateMax)) return false;
            $("#loadingPanel").modal('show');
            var grid = $('#ActivityGrid_tbl')[0].grid;
            grid.changePage('next');
            if (!grid.loading)
                $("#loadingPanel").modal('hide');

            return grid.loading;
        }

        function initCalendar(calendarDiv) {
            var tbSpan = "<b><span style='color:darkblue'>T";
            var defaultDate = $("#StartDateTextBox").val();

            if (dateMin || dateMax) {
                dateMin = null, dateMax = null;
                calendarDiv.fullCalendar('destroy');
                calendarDiv.show();
            }

            calendarDiv.fullCalendar({
                header: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'month,basicWeek,basicDay'
                },
                eventDataTransform: function (evt) {
                    if (!evt.cell) return evt;
                    //var color = evt.color;
                    evt = evt.cell;
                    //if (color)evt.color = color;
                    var posTB = evt.StartEndDateTime.lastIndexOf(tbSpan);
                    if (posTB !== -1)
                    {
                        var isTBC = evt.StartEndDateTime.indexOf("BC</span></b>", posTB + tbSpan.length) !== -1;
                        evt.title = tbSpan + (isTBC ? "BC" : "BD") + "</span></b> " + evt.title;
                    }
                    //else evt.color = '#F0FFF0'; // Green = Confirmed

                    if (dateMin === null || dateMin > evt.end)
                        dateMin = evt.end;
                    if (dateMax === null || dateMax < evt.start)
                        dateMax = evt.start;

                    return evt;
                },
                viewRender: function (view, element) {
                    getMoreActivitiesIfNeeded(calendarDiv, view);
                },
                eventRender: function (event, element) {
                    element.find('.fc-title').html(event.title); // to render the markup
                    element.popover({
                        title: event.title,
                        placement: 'top',
                        container: 'body',
                        html: true,
                        trigger:'hover',
                        content: event.StartEndDateTime + '<br /><br />' + event.Ministry + ' - ' + event.Id + '<br /><br />' + event.details,
                    });
                },
                eventClick: function(calEvent, jsEvent, view) {
                    window.open("Activity.aspx?ActivityId=" + calEvent.Id, '_blank');
                },
                defaultDate: defaultDate ? defaultDate : null,
                defaultView: 'basicWeek',
                businessHours: true, // display business hours
                editable: false,
                height: 'auto'
            });
            var onMobile = !$("#NavigationMenu").is(':visible');
            if (!onMobile)
                calendarDiv.hide();
        }


        //----------------------------------------------------------------------
        // Used to execute the Corporate Queries
        function runCorporateQuery() {

            var query = { 'op': 'rcc' };
            query.showAll = $('#rdCQAll').is(':checked');
            query.nbrDays = query.showAll ? '' : $("#txtNumberDays").val();

            query.statuses = '';
            query.hqStatuses = '';
            query.deletedYN = false;

            var items = $('#chkBxStatus input:checkbox');
            for (var i = 0; i < items.length; i++) {
                if (items[i].checked !== true) continue;

                if (items[i].value === 'Deleted') {
                    query.deletedYN = true;
                } else if (items[i].value.indexOf("LA ") != -1) {
                    if (query.hqStatuses) query.hqStatuses += ",";
                    query.hqStatuses += items[i].value.substr(3);
                } else {
                    if (query.statuses) query.statuses += ",";
                    query.statuses += items[i].value;
                }
            }

            search(query);
        }

        function ClearLAStatus() {
            var msg = "For how many days?";
            jPrompt(msg, 10, "Clear Look Ahead Status of Activities", function (result) {
                if (result !== null) {
                    $.ajax({
                        type: 'POST',
                        url: 'ActivityHandler.ashx?Op=ClearLAStatus&numberDays=' + result,
                        success: function (data) {
                            if (data !== null && data !== undefined && data.length > 0)
                                jAlert(data, 'Look Ahead Status Clear Error');
                            $('#ActivityGrid_tbl').flexOptions().flexReload();
                        }
                    });
                }
            });
        }

        //----------------------------------------------------------------------
        // Takes a url and loads the filter options into the Query Area
        // This does not execute the query.
        // This is only used when saving a My Query or running a My Query
        function SetFilter(filterString) {

            // Reset in the other context was not working, moving it directly to here
            var fParams = filterString.replace('ActivityListProvider.aspx?', '').split('|');
            for (var i = 0; i < fParams.length; i++) {
                var fieldAndvalue = fParams[i].split('=');
                var value = fieldAndvalue[1];
                if (value === "*" || value === '') continue;
                var filter = null;
                switch (fieldAndvalue[0])
                {
                    case "status":
                        filter = $("#fstatus").val(value);
                        break;
                    case "category":
                        filter = $("#fcategory").val(value);
                        break;
                    case "ministry":
                        filter = $("#fministry").val(value);
                        break;
                    case "contact":
                        filter = $("#fcontact").val(value);
                        break;
                    case "representative":
                        filter = $("#frepresentative").val(value);
                        break;
                    case "initiative":
                        filter = $("#finitiative").val(value);
                        break;
                    case "premierRequested":
                        $.uniform.update($("#fpremier").val(value));
                        break;
                   case "distribution":
                        $.uniform.update($("#fdistribution").val(value));
                        break;
                    case "datefrom":
                        filter = $("#StartDateTextBox").val(value);
                        break;
                    case "dateto":
                        filter = $("#EndDateTextBox").val(value);
                        break;
                    case "isissue":
                        filter = $("#fissue").val(value);
                        break;
                    case "dateConfirmed":
                        filter = $("#fdateConfirmed").val(value);
                        break;
                    case "keywords":
                        $("#fkeyword").getKendoMultiSelect().value(value.split('~'));
                        break;
                    case "quickSearch":
                        filter = $("#txtSearch").val(value);
                        break;
                    default:
                        break;
                }
                if (filter !== null) {
                    $.uniform.update(filter);
                    filter.parent().show();
                }
            }
        }

        //----------------------------------------------------------------------
        // Used to execute a "My Query"
        function runMyQuery(queryUrl) {

            // First reset all the filters - wonder if users would expect this
            $.ajaxSetup({ async: false }); // Need to do this here to make sure this next call is done BEFORE moving on.
            ResetFilterOptions();
            $.ajaxSetup({ async: true }); // set it back to async.
            ResetFilterDates();

            // Set up filter with settings from the My Query selected
            SetFilter(queryUrl);
            
            // Execute the search
            search();
        }

    </script>

    <div id="CalendarView" style="overflow: auto"></div>


    <div id="loadingPanel" class="modal fade">
        <div class="modal-dialog modal-sm">
        <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-body">
                    <img src="../images/flexgrid/load.gif" alt="" />
                    Loading&hellip;</div>
          </div>
      </div>
    </div>


    <div id="contentwrapper">
      <div id="leftcolumn" style="display:none">
        <div class="accordion">
            <h1><a href="#">Filter</a></h1>
            <div class="filter">

                <table style="width:100%;">
                    <tr>
                        <td style="font-weight:bold">Dates</td>
                        <td style="font-size: 0.8em; text-align: right; text-transform: none;"><button id="ResetFilterDates">Reset Dates</button></td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <div>From:</div>
                            <input type="text" id="StartDateTextBox" runat="server" class="dates" /> 
                        </td>
                        <td style="padding-left: 5px;">
                            <div>To:</div>
                            <input type="text" id="EndDateTextBox" runat="server" class="dates" />
                        </td>
                    </tr>
                </table>

                <div style="padding-top: 3px;">
                    <div class="thisdayonly"><asp:CheckBox ID="ThisDayOnlyCheckBox" Text="This day only" TextAlign="Right" runat="server" /></div>
                </div>


                <table style="width:100%;">
                    <tr>
                        <td style="font-weight:bold">Options</td>
                        <td style="font-size: 0.8em; text-align: right; text-transform: none;"><button id="ResetFilters">Reset Options</button></td>
                    </tr>
                </table>

                <div style="padding-bottom:5px">Search for:</div>
                <input type="text" id="txtSearch" style="margin-left:3px; width:195px; height:22px" runat="server" placeholder="ID, Keywords, City"/> 

                <select id="KeywordList" multiple="true" runat="server" style="display:none"/>
                <asp:TextBox ID="fkeyword" style="width:194px;margin:2px 0 0 2px" runat="server"></asp:TextBox>

                <select id="fissue" runat="server">
                    <option selected="selected" value="*">Select Issue</option>
                    <option value="true">Is an Issue</option>
                    <option value="false">Not an Issue</option>
                </select>
                <select id="fdateConfirmed" runat="server">
                    <option selected="selected" value="*">Select Date Confirmed</option>
                    <option value="true">Date is Confirmed</option>
                    <option value="false">Date is not Confirmed</option>
                </select>

                <select id="fstatus" runat="server">
                </select>
                <select id="fcategory" runat="server">
                </select>
                <select id="fministry" runat="server" onchange="ministryChange(this);">
                </select>
                <select id="fcontact" runat="server" onchange="comContactChange(this);">
                </select>
                <select id="frepresentative" runat="server">
                </select>
                <select id="finitiative" runat="server" title="Initiative">
                </select>
                <select id="fpremier" runat="server" title="Premier Requested">
                </select>
                <select id="fdistribution" runat="server" title="Distribution">
                </select>
                <select id="fmore" runat="server">
                    <option selected="selected" value="*">More...</option>
                    <option value="fissue">Issue</option>
                    <option value="fdateConfirmed">Date Confirmed</option>
                    <option value="fstatus">Status</option>
                    <option value="fministry">Lead Ministry</option>
                    <option value="fcontact">Communication Contact</option>
                    <option value="frepresentative">Lead Representative</option>
                    <option value="finitiative">Initiative</option>
                    <option value="fpremier">Premier Requested</option>
                    <option value="fdistribution">Distribution</option>
                </select>

                <table style="margin-top:10px;width:100%;">
                    <tr>
                        <td style="font-weight:bold">Display</td>
                        <td style="font-size: 0.8em; text-align: right;"></td>
                    </tr>
                </table>

                <asp:RadioButtonList ID="DisplayRadioButtonList" runat="server" RepeatLayout="Flow" RepeatDirection="Vertical">
                    <asp:ListItem Text="Show All" Value="3" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="My Ministries' Activities Only" Value="2"></asp:ListItem>
                    <asp:ListItem Text="My Activities Only" Value="4" onClick="MyActivitiesClick();"></asp:ListItem>
                    <asp:ListItem Text="My Favourites Only" Value="10"></asp:ListItem>
                </asp:RadioButtonList>

                <div style="padding-top: 10px; text-align: center; width:100%;">
                    <button class="primary" id="ExecuteSearchButton">&nbsp;&nbsp;Search&nbsp;&nbsp;</button>
                    <button id="SaveQuery">Save</button>
                </div>

            </div>
        </div>


        <div class="accordion">

            <h1><a href="#">Corporate Queries</a></h1>
            <div class="filter" id="corpQueryOptions">
                <div class="CorporateQueries">
                    <p>These queries provide all ministry activities for the specified time and status.</p>
                    <div>
                        Upcoming Activities to Show:
                    <div><asp:RadioButton runat="server" ID="rdCQAll" Text="Show all" GroupName="showall" /></div>
                    <div>
                        <asp:RadioButton runat="server" ID="rdCQWeeks" Text="For the next " Checked="true" GroupName="showall" />
                        <input type="text" id="txtNumberDays" name="txtNumberDays" style="width: 35px; padding: 5px;" value="10" /> days(s)
                    </div>
                    </div>

                    <div style="padding-top:10px">
                        With status of:<br />
                        <asp:CheckBoxList runat="server" ID="chkBxStatus" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" >
                            <asp:ListItem Text="New" Value="New" Selected="True" ></asp:ListItem>
                            <asp:ListItem Text="Changed" Value="Changed" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Reviewed" Value="Reviewed" ></asp:ListItem>
                            <asp:ListItem Text="Deleted" Value="Deleted" ></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>

                    <div style="padding-top: 10px; text-align: center; width:100%;">
                        <button class="primary" id="RunCorporateQuery">&nbsp;&nbsp;Search&nbsp;&nbsp;</button>
                        <button id="ClearLAStatus" runat="server" >&nbsp;&nbsp;Clear LA Status&nbsp;&nbsp;</button>
                    </div>

            </div>
        </div>
        </div>

        <div class="accordion">
            <h1><a href="#">My Queries</a></h1>
            <div class="filter MyQueries">
            <p>Use the Filter section above to create and save your own custom searches.</p>
            <ul id="MyQueryList">
                <asp:Repeater ID="MyQueriesRepeater" runat="server">
                    <ItemTemplate>
                        <li>
                            <span id="<%# DataBinder.Eval(Container.DataItem, "Id") %>">
                                <a href="javascript:runMyQuery('<%# DataBinder.Eval(Container.DataItem, "QueryString") %>');">
                                <%# DataBinder.Eval(Container.DataItem, "Name") %></a></span></li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>

            <div style="margin-top: 10px;width: 98%; text-align: right;"><a href="#" id="btnDialog" style="color: #999999;">Manage My Queries</a></div>
            </div>
        </div>

        <div id="accordionadmin" class="accordion" runat="server">
        <h1><a href="#">Admin Settings</a></h1>
           <div class="filter">
                <p>These settings affect the results of the Filter and Corporate Queries.</p>

            <div style="font-weight:bold">Look Ahead Filter:
                    <div>
                        <asp:RadioButton runat="server" ID="lookaheadAll" Text="Show All" Checked="true" GroupName="showConfidential" />
                    </div>
                    <div>
                        <asp:RadioButton runat="server" ID="lookaheadYes" Text="Look Ahead Only" GroupName="showConfidential" />
                    </div>
                    <div>
                        <asp:RadioButton runat="server" ID="lookaheadNo" Text="Not for Look Ahead Only" GroupName="showConfidential" />
                    </div>
                </div>
           </div>
      </div>
    </div>
    <div id="contentcolumn">
    <div class="innertube flexigrid">
        <UC:FlexiGrid ID="ActivityGrid" runat="server" DataType="json" ShowTableToggleButton="True"
        SortName="StartEndDateTime" SortOrder="asc" Title="B.C. Government Activities"
        UsePager="False" UseRP="False" OnRowSelected="rowSelected" Width="500" />
    </div>
    </div>
    </div>

    <asp:HiddenField ID="SelectedTextBox" runat="server" />

    <div id="myQueryDiv" class="modal fade" >
        <div class="modal-dialog" style="width:565px">
            <div class="modal-content">
                <div class="modal-header modal-header-gridHeader">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h6>Manage My Queries</h6>
                </div>

                <div class="modal-body modal-body-gridBody ">
                    <asp:HiddenField runat="server" ID="OrderIDs" />
                    <asp:HiddenField runat="server" ID="DeleteIDs" />

                    <h1>Manage My Queries</h1>
                    <p>Edit, Delete and Re-order your queries. <b>Click Save All</b> to submit and save all your changes.</p>

                    <div style="padding: 10px; text-align: center; width:100%;">
                        <button class="btn btn-sm" id="SaveOrder1" onclick="clickSave()">Save All</button>
                    </div>

                    <ul id="sortables">
                    </ul>

                    <div style="padding: 10px; text-align: center; width:100%;">
                        <button  class="btn btn-sm" id="SaveOrder2"  onclick="clickSave()">Save All</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">

        $(document).ready(function () {
            search();
        });

        var myMinLabel;
        function ministryChange(dropDown) {
            var selName = dropDown.options[dropDown.selectedIndex].text;
            myMinLabel = $("#DisplayRadioButtonList_1").next('label').text();
            $("#DisplayRadioButtonList_1").next('label').text(selName + " Activities Only");
        }

        function comContactChange(dropDown) {
            var selName = dropDown.options[dropDown.selectedIndex].text;
            if (selName == "Select Comm. Contact") {
                $("#DisplayRadioButtonList_2").next('label').text("My Activities Only");
            }
            else {
                $("#DisplayRadioButtonList_2").next('label').text(selName.split(' ')[0] + "'s Activities Only");
            }
        }

        function MyActivitiesClick() {
            var currentUserName = "<% =UserName %>";
          
            if ($("#fcontact").is(":visible")) {
                $('#fcontact option').each(function () {
                    if ($(this).text() === currentUserName) {
                        $(this).prop('selected', true).trigger('change');
                    }
                });
            }
        }

        function initFilters() {

            // Set the styles of the drop down lists
            var selects = $("select:visible");
            selects.uniform({ selectAutoWidth: false });
            selects.not("#fmore").not("#fcategory").parent().hide();

            // Set up accordion for filter
            $(".accordion").accordion({
                heightStyle: 'content',
                active: 0,
                collapsible: true  // http://jqueryui.com/demos/accordion/#collapsible -Requirements Change #5. Home Page, "Ability to collapse 4 top sections -DW 20120619
            });

            // Setup Date Picker for Start Date
            $("#StartDateTextBox").datepicker({
                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true
            });

            // Setup Date Picker for End Date
            $("#EndDateTextBox").datepicker({
                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true
            });

            // On This Day on selected, check dates are the same
            $("#ThisDayOnlyCheckBox").click(function () {
                if (this.checked) {
                    var error = checkThisDayOnly();
                    if (error) {
                        jAlert(error, 'Validation Error');
                     }
                 }
                //$("#StartDateTextBox").datepicker("option", "disabled", this.checked);
                $("#EndDateTextBox").datepicker("option", "disabled", this.checked);
             });

            $('#fkeyword').kendoMultiSelect({
                dataSource: $('#KeywordList option').map(function (i, elem) {
                    return { text: elem.text, value: elem.value };
                }).get(),
                dataTextField: "text",
                dataValueField: "value",
                filter: "startswith",
                placeholder: "Tags",
                maxSelectedItems: 10
            });

            $("#btnDialog").click(function () {
                var panelList = $('#sortables');
                var queryLi = "";
                queryLi += "<li exampleItemId='{QueryId}'>";
                queryLi += "  <div class='listitem ui-state-default'>";
                queryLi += "  <span class='ui-icon ui-icon-arrowthick-2-n-s' ></span>";
                queryLi += "  <input text='text' value='{QueryName}' style='width:80%'>";
                queryLi += "    <button class='btn btn-sm' onclick='return clickDelete(\"{QueryId}\",this)'>";
                queryLi += "      <span class='glyphicon glyphicon-trash' style='margin-right:5px'></span>Delete</button>";
                queryLi += "   </div></li>";

                panelList.empty();
                $('#MyQueryList span').each(function (index, elem) {
                    var id = elem.getAttribute("id");
                    var spy = queryLi.replace(/{QueryId}/g, id);
                    panelList.append(queryLi.replace("{QueryName}", elem.innerText).replace(/{QueryId}/g, id));
                });

                panelList.sortable({
                    // Only make the .panel-heading child elements support dragging.
                    // Omit this to make then entire <li>...</li> draggable.
                    handle: '.listitem',
                    update: function () {
                        $('.panel', panelList).each(function (index, elem) {
                            var $listItem = $(elem),
                                newIndex = $listItem.index();

                            // Persist the new indices.
                        });
                    }
                });

                $("#myQueryDiv").modal('show')
            });
        }

        // Style the Search Button and set up OnClick
        $("#RunCorporateQuery").button({
            // Uses the class Primary to give it the blue color
        }).click(function () {
            runCorporateQuery();
            return false;
        });

        // Style the Clear LA Status Button and set up OnClick
        $("#ClearLAStatus").button({
        }).click(function () {
            ClearLAStatus();
            return false;
        });


        // Style the Search Button and set up OnClick
        $("#ExecuteSearchButton").button({
            // Uses the class Primary to give it the blue color
        }).click(function () {
            var error = checkThisDayOnly();
            if (error) {
                jAlert(error, 'Validation Error');
            } else {
                search();
            }
            return false;
        });

        // Style the Save Button and Set up OnClick action
        $("#SaveQuery").button({
            icons: {
                primary: "ui-icon-disk"
            }
        }).click(function () {
            jPrompt('Type a name for your search query:', 'My Query', 'Save a Query', function (r) {
                if (r) {
                    var filters = {};
                    AddToFilters(filters);
                    var error = saveFilter(r, GetQuery(filters, $("#txtSearch").val(), true), function (error) {
                        var msg = error ? 'Sorry, there was an error saving your query. ' + error : 'Filter Saved.';
                        jAlert(msg, 'Save a Query');
                    });
                }
            });
            return false;
        });

        function clickSave() {
            var error = saveOrderClick(function (error) {
                var msg = error ? 'Sorry, there was an error saving your queries.' + error.Message : 'Changes Saved.';
                jAlert(msg, 'Manage My Queries');
            });
            event.preventDefault();
            return false;
        }

        var sidebarIsInitialized = false;
        //var userForced = false;
        function toggleSidebar(forced) {
            //if (forced) userForced = forced;
            //else if (userForced) return; // if user manually opened or closed the sidebar, don't do anything

            // Open/Close filter sidebar
            var panel = $('#contentwrapper');
            if (panel.hasClass("visible")) {
                panel.removeClass('visible');
                $('#leftcolumn').animate({ 'margin-left': '-242px' }, 500);
                $(".toggleSidebar").removeClass("closeIt").addClass("openIt").text("Open Sidebar");
            } else {
                panel.addClass('visible');
                $('#leftcolumn').show();
                $('#leftcolumn').animate({ 'margin-left': '0' }, 500);
                $(".toggleSidebar").removeClass("openIt").addClass("closeIt").text("Close Sidebar");
                if (!sidebarIsInitialized) initFilters();
                sidebarIsInitialized = true;
            }
        }

        // Styles the Reset Date Filters and Handles OnClick
        $("#ResetFilterDates").button({
            // Just Default Button
        }).click(function () {
            ResetFilterDates();
            return false;
        });

        // Styles the Reset Options Filter and Handles OnClick
        $("#ResetFilters").button({
            // Just Default Button
        }).click(function () {
            ResetFilterOptions();
            return false;
        });


        $("#fmore").change(function () {
            var $this = $(this);
            var filter = $("#" + $this.val());
            if ($this.val() === "fcontact") {
                UpdateCommunicationContacts();
            }
            if (filter !== null) {
                filter.parent().show();
                filter.uniform({ selectAutoWidth: false });
            }
            $this.val("*");
        });

        // Update List of Communication Contacts based on Ministry selection
        $("#fministry").change(function () {
            if ($("#fcontact:parent").is(':visible')) {
                UpdateCommunicationContacts();
            }
        });

    </script>


    <asp:HiddenField  ID="toBeDeletedIds" runat="server" /> 
    <div id="loadmoreajaxloader" style="display:none;text-align:center"><img src="../images/ajaxloader.gif" /></div>

</asp:Content>