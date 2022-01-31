<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="UCFlexiGrid.ascx.cs" Inherits="UCFlexiGrid" %>

<script>
    function ReviewHandler() {
        
        if ($('#SelectedTextBox').val() == '') {
            $("#dialogNoActivitiesSelect").dialog("open");
        } else {
            var lst = $('#SelectedTextBox').val().slice(0, -1);
            
            var count = lst.split(',').length;

            var msg = "";
            if (count == 1) {
                msg = "<p style='font-weight:bold;'><strong>" + count + " activity selected.</strong></p><p>Are you sure you want to mark the selected activity as reviewed?</p>";
            } else {
                msg = "<p style='font-weight:bold;'><strong>" + count + " activities selected.</strong></p><p>Are you sure you want to mark selected activities as reviewed?</p>";
            }
            
            var formatedLst = lst.replace(/,/g, ', ');

            $("#ActivityReviewConfirmation").html(msg);

            $("#ActivityList").html(formatedLst);
            $("#dialog").dialog("open");

            // Remove focus on all buttons within the
            // div with class ui-dialog
            $('.ui-dialog :button').blur();
        }
    }

    function GetFilter(filters)
    {
        var txtSearch = encodeURIComponent($("#txtSearch").val());
        AddToFilters(filters);
        var filter = GetQuery(filters, txtSearch);
        return filter;
    }

    function download() {
        var filter = GetFilter({'Op':'ExportSelected'});
        window.location = 'ActivityHandler.ashx?' + filter;
    }

    function planningReport() {
        var filter = GetFilter({'Op':'PlanningReport'});
        window.open('ActivityHandler.ashx?' + filter);
    }

    function lookAheadReport() {
        var filter = GetFilter({'Op':'LookAheadReport'});
        window.open('ActivityHandler.ashx?' + filter);
    }

    function execLookAheadReport() {
        var filter = GetFilter({ 'Op': 'LookAheadReport' });
        window.open('ActivityHandler.ashx?' + filter + "&" + "Detailed=True");
    }

    function main30_60_90Report() {
        var filter = GetFilter({'Op':'Main30_60_90Report'});
        window.open('ActivityHandler.ashx?' + filter);
    }

    $(document).ready(function () {

        // --------------------------------------------
        // THIS BLOCK OF SCRIPT IS ALL ABOUT STYLE AND DEFAULT VALUES
        // --------------------------------------------


        $("#dialog").dialog({
            autoOpen: false,
            modal: true,
            width: 400,
            height: 220,
            buttons: {
                "Yes": {
                    text: "Yes, Mark as Reviewed",
                    "class": "primary",
                    click: function () {
                        var queryString = '?Op=ReviewSelected&ids=' + $('#SelectedTextBox').val().slice(0, -1);
                        var lastLoadedTime = $('#lastLoadedDatetime').val();
                        var filter = 'ActivityHandler.ashx' + queryString;
                        $.ajax({
                            type: 'POST',
                            //contentType: 'application/json; charset=utf-8',
                            url: 'ActivityHandler.ashx' + queryString + "&lastLoadedDatetime=" + lastLoadedTime,
                            //dataType: 'json',
                            success: function (data) {
                                if (data != null && data != undefined && data.length > 0)
                                    jAlert(data, 'Review Activities Error');
                                $('#ActivityGrid_tbl').flexOptions().flexReload();
                                $('#SelectedTextBox').val('');
                            }
                        });

                        $(this).dialog("close");
                    }
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            }
        });

        $("#dialogNoActivitiesSelect").dialog({
            autoOpen: false,
            modal: true,
            buttons: {
                "Ok": function () {
                    $(this).dialog("close");
                }
            }
        });

    });

</script>

        <div class="mDiv">
            <div class="fTitle"><%=Title %></div>
        </div>
        <div class="tDiv">
            <div class="tDiv2">
                <%=AppendButton("toggleSidebar", "Open Sidebar", "toggleSidebar(true)", "open_sidebar_button")%>
                <div class='btnseparator'></div>
                <%=AppendButton("toggleView", "Calendar View", "toggleView()", "calendar_view_button")%>
                <%if (DisplayReviewButton) { %>
                <div class='btnseparator hidden-xs'></div>
                <%=AppendButton("review hidden-xs", "Review", "ReviewHandler()", "review_button")%>
                <% } %>
                <div class='btnseparator hidden-xs'></div>
                <%=AppendButton("export hidden-xs", "Excel Export", "download()", "excel_export_button")%>
                <%=AppendButton("export hidden-xs", "Look Ahead Report", "lookAheadReport()", "look_ahead_report_button")%>
                <%if (DisplayExecLADetailedReportButton) { %>
                <%=AppendButton("export hidden-xs", "Exec Look Ahead", "execLookAheadReport()", "exec_look_ahead_report_button")%>
                <% } %>
                <%=AppendButton("export hidden-xs", "30/60/90 Report", "main30_60_90Report()", "main_30_60_90_report_button")%>
                <div class='btnseparator hidden-xs'></div>
                <%=AppendButton("export hidden-xs", "Planning Report", "planningReport()", "planning_report_button")%>
                <div class='btnseparator hidden-xs'></div>
            </div>
        </div>
        <div id="activityGridDiv">
    <input type="hidden" id="lastLoadedDatetime" name="lastLoadedDatetime"/>
<div id="dialog" title="Review Activities">
                <div id="ActivityReviewConfirmation"></div>
    <div id="ActivityList"></div>
</div>

<div id="dialogNoActivitiesSelect" title="Review Activities">
    <p>
        <span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 50px 0;"></span>
        No activities selected. You must select at least one activity to mark as reviewed.
    </p>
</div>
            <table id="ActivityGrid_tbl" style="height: auto; display: none; padding-left:0px; margin-left:0px"></table>
        </div>
        <div id="calendarDiv"></div>
