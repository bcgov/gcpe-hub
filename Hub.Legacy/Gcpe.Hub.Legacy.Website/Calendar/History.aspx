<%@ Page Title="" Language="C#" MasterPageFile="~/Calendar/Site.master" AutoEventWireup="True"
    CodeBehind="History.aspx.cs" Inherits="Gcpe.Hub.Calendar.History" ClientIDMode="Static"%>

<%@ Import Namespace="Gcpe.Hub.Calendar" %>
<%@ MasterType VirtualPath="~/Calendar/Site.master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">

    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/themes/base/all.css") %>" />

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-ui-1.11.4.js") %>"></script>

    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/uniform.default.css") %>" />
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.uniform.min.js") %>"></script>

    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/jquery.alerts.css") %>" media="screen" />
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.alerts.js") %>"></script>


    <link rel="stylesheet" href="<%= ResolveUrl("~/Calendar/Content/Custom.css") %>" />
    <style type="text/css">
        h1 {
            margin-bottom: 10px;
        }
    </style>
    <script type="text/javascript" charset="utf-8">

        // This sets the styles of the inputs and drop down lists
        $(function () {
            $("select").uniform({ selectAutoWidth: false });
        });

        //
        $(function () {

            // Setup Date Picker for Start Date
            var updateStartDateTextBox = $("#UpdateStartDateTextBox");
            var updateEndDateTextBox = $("#UpdateEndDateTextBox");
            updateStartDateTextBox.datepicker({
                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true
            });
            updateStartDateTextBox.datepicker("setDate", -1);

            updateEndDateTextBox.datepicker({
                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true
            });
        });

    </script>

</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">

    <div id="contentwrapper">
        <div id="historycontentcolumn">
            <div class="innertube">

                <div id="loadingPanel" class="modal fade">
                    <div class="modal-dialog modal-sm">
                        <!-- Modal content-->
                        <div class="modal-content">
                            <div class="modal-body">
                                <img src="../images/flexgrid/load.gif" alt="" />
                                Loading&hellip;
                            </div>
                        </div>
                    </div>
                </div>

                <div id="result" style="padding-top: 0px; margin-top: 0px;">
                    <h1>
                        <asp:Label ID="ResultTitle" runat="server" /></h1>
                    <p>
                        Total
                        <asp:Label ID="ResultCount" runat="server" />
                        items
                    </p>
                </div>

            </div>
        </div>
    </div>

    <div id="leftcolumn">

        <div id="accordion">
            <h3><a href="#">Filter by Date Range</a></h3>
            <div class="filter">

                <table>
                    <tr>
                        <td>
                            <div>From:</div>
                            <input type="text" id="UpdateStartDateTextBox" runat="server" class="dates" />
                        </td>
                        <td style="padding-left: 5px;">
                            <div>To:</div>
                            <input type="text" id="UpdateEndDateTextBox" runat="server" class="dates" />
                        </td>
                    </tr>
                </table>
                <div style="padding: 10px 0px 10px 0px;">
                    <div>Update type:</div>
                    <select id="updateType" runat="server">
                        <option selected="selected" value="">All</option>
                        <option value="changed">Changed</option>
                        <option value="added">Added</option>
                        <option value="deleted">Deleted</option>
                        <option value="reviewed">Reviewed</option>
                        <option value="cloned">Cloned</option>
                    </select>

                </div>
                <div style="padding: 10px 0px 10px 0px;">
                    <div>Search for:</div>
                    <input type="text" id="txtKeyword" runat="server" maxlength="50" />

                </div>
                <div style="padding-top: 10px; text-align: center; width: 100%;">
                    <button class="primary" id="ExecuteSearchButton">&nbsp;&nbsp;Search&nbsp;&nbsp;</button>
                </div>

            </div>
        </div>

        <div id="SavedFilters" style="margin-top: 10px;">
            <h3><a href="#">Saved Filters</a></h3>
            <div class="filter">
                <div class="MyQueries">
                    <ul id="MyQueryList">
                        <li><a href="javascript:getLatestUpdates();">Latest 5 Updates</a></li>
                        <li><a href="javascript:getTodaysUpdates();">Today's updates</a></li>
                    </ul>
                </div>
            </div>
        </div>



    </div>


    <script type="text/javascript">

        // Do on page load
        $(function () {

            $("#txtKeyword").keypress(function (event) {
                if (event.keyCode == 13 || event.which == 13) {
                    $("#ExecuteSearchButton").click();
                }
            });

            // Set the styles of the drop down lists
            //$("select").uniform();

            // Set up accordion for filter
            $("#accordion").accordion({
                heightStyle: 'content',
                active: 0,
                collapsible: true
            });

            // Set up accordion for filter
            $("#SavedFilters").accordion({
                heightStyle: 'content',
                active: 0,
                collapsible: true
            });
            var activityId = '<%= Request.QueryString["ActivityID"]%>';
            if (activityId)
                getUpdatesForActivity(activityId);
            else
                getTodaysUpdates();
        });

        // Style the Search Button and set up OnClick
        $("#ExecuteSearchButton").button({
            // Uses the class Primary to give it the blue color
        }).click(function () {
            getUpdatesBetweenDates();
            return false;
        });

        function getTodaysUpdates() {
            $("#loadingPanel").modal('show');
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "CorporateCalendarUpdateWebService.asmx/GetTodaysUpdates",
                dataType: "json",
                success: function (data) {
                    $('#result').html(data.d);
                    $("#loadingPanel").modal('hide');
                },
                error: function (e) {
                    $('#result').html("An Error Occurred");
                    $("#loadingPanel").modal('hide');
                }
            });
        }
        function getLatestUpdates() {
            $("#loadingPanel").modal('show');
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "CorporateCalendarUpdateWebService.asmx/GetLatestUpdates",
                dataType: "json",
                success: function (data) {
                    $('#result').html(data.d);
                    $("#loadingPanel").modal('hide');
                },
                error: function (e) {
                    $('#result').html("An Error Occurred");
                    $("#loadingPanel").modal('hide');
                }
            });
        }
        function getUpdatesBetweenDates() {
            $("#loadingPanel").modal('show');
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "CorporateCalendarUpdateWebService.asmx/GetUpdatesBetweenDates",
                data: "{ fromDate: '" + $("#UpdateStartDateTextBox").val()
                    + "', toDate: '" + $("#UpdateEndDateTextBox").val() + "' "
                    + ", updateType: '" + $("#updateType").val() + "' "
                    + ", keyword: '" + $("#txtKeyword").val() + "'  }",
                dataType: "json",
                success: function (data) {
                    $('#result').html(data.d);
                    $("#loadingPanel").modal('hide');
                },
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    $('#result').html("An Error Occurred - " + err.Message, 20000);
                    // Usually because the JSON string is too long. Have increased the size to max in the web.config.
                    //$('#result').html("An Error Occurred");
                    $("#loadingPanel").modal('hide');
                }
            });
        }
        function getUpdatesForActivity(activityID) {
            $("#loadingPanel").modal('show');
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "CorporateCalendarUpdateWebService.asmx/GetUpdatesForActivity",
                data: "{ activityId: '" + activityID+ "'  }",
                dataType: "json",
                success: function (data) {
                    $('#result').html(data.d);
                    $("#loadingPanel").modal('hide');
                },
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    $('#result').html("An Error Occurred - " + err.Message, 20000);
                    // Usually because the JSON string is too long. Have increased the size to max in the web.config.
                    //$('#result').html("An Error Occurred");
                    $("#loadingPanel").modal('hide');
                }
            });
        }

    </script>

</asp:Content>
