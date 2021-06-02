<%@ Page Title="" Language="C#" MasterPageFile="~/Calendar/Site.master" AutoEventWireup="True"
    CodeBehind="Activity.aspx.cs" EnableViewState="True" Inherits="Gcpe.Hub.Calendar.Activity" MaintainScrollPositionOnPostback="false" ClientIDMode="Static"%>

<%@ Import Namespace="Gcpe.Hub.Calendar" %>
<%@ OutputCache Duration="1" Location="None" NoStore="true" %>
<%@ MasterType VirtualPath="~/Calendar/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">

    <script type="text/javascript">

        /*
         * jQuery 1.9 support. browser object has been removed in 1.9  (needed for jquery.editable-select.js and jquery.maskedinput)
         */
        if (!$.browser) {
            function uaMatch(ua) {
                ua = ua.toLowerCase();

                var match = /(chrome)[ \/]([\w.]+)/.exec(ua) ||
                        /(webkit)[ \/]([\w.]+)/.exec(ua) ||
                        /(opera)(?:.*version|)[ \/]([\w.]+)/.exec(ua) ||
                        /(msie) ([\w.]+)/.exec(ua) ||
                        ua.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+)|)/.exec(ua) ||
                        [];

                return {
                    browser: match[1] || "",
                    version: match[2] || "0"
                };
            };

            var matched = uaMatch(navigator.userAgent);
            $.browser = {};

            if (matched.browser) {
                $.browser[matched.browser] = true;
                $.browser.version = matched.version;
            }

            // Chrome is Webkit, but Webkit is also Safari.
            if ($.browser.chrome) {
                $.browser.webkit = true;
            } else if ($.browser.webkit) {
                $.browser.safari = true;
            }
        }
    </script>

    <!-- The line before and after the jquery-ui include are needed to allow access to the bootstrap tooltip functionality  -->
    <script>var bootstrap_tooltip = jQuery.fn.tooltip;</script>

    <!-- This has to use this or the drop downs and multiselect jquery does not work -->
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-ui-1.11.4.js") %>"></script>

    <script>jQuery.fn.tooltip = bootstrap_tooltip;</script>

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/kendo/2016.2.504/kendo.ui.core.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/kendo/2016.2.504/kendo.popup.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/kendo/2016.2.504/kendo.data.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/kendo/2016.2.504/kendo.list.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/kendo/2016.2.504/kendo.multiselect.min.js") %>"></script>

    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/themes/base/all.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/kendo/2016.2.504/kendo.common.min.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/kendo/2016.2.504/kendo.default.min.css") %>" />

    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/uniform.default.css") %>" />
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.uniform.min.js") %>"></script>

    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/jquery.multiselect.css") %>" />
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.multiselect.js") %>"></script>

    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/jquery.multiselect.filter.css") %>" />
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.multiselect.filter.js") %>"></script>

    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/jquery.editable-select.css") %>" />
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.editable-select.js") %>"></script>

    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/jquery.alerts.css") %>" media="screen" />
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.alerts.js") %>"></script>

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.elastic.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.maskedinput-1.3.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.url.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/date.js") %>"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/navigator.sendbeacon"></script>

    <link rel="stylesheet" href="<%= ResolveUrl("~/Calendar/Content/Custom.css") %>" />
    <!-- RDK -->
    <script type="text/javascript" src="Scripts/activityhelper.js"></script>
    <script type="text/javascript" src="Scripts/ContextualHelp.js"></script>


    <script type="text/javascript">


        function IsCloneValid() {
            if ($('#CategoriesDropDownListRequiredFieldValidator').css('display') == "none") {
                return true;
            } else {

                return false;
            }
        }

        // --------------------------------------------
        // HANDLE BUTTON CLICKS AND VALIDATION
        // --------------------------------------------

        // VALIDATION ------------------------------------------------------------------------------------------------

        // Start by checking for blank required field first, if any are found, show the validator messages
        // The required fields are

        //Breaking this into smaller functions to help debugging

        // Check all mandatory fields are entered
        function RequiredFieldsCompleted() {

            if ($('#CategoriesDropDownList').val() == null
                || $('#ContactMinistryDropDownList option:selected').val() == ''
                || $('#TitleTextBox').val() == ''
                || $('#CommunicationContactRequiredFieldValidator').is(':visible')
                || $('#DetailsRequiredFieldValidator').is(':visible')
                || $('#SignificanceRequiredFieldValidator').is(':visible')
                || $('#StrategyIsRequiredFieldValidator').is(':visible')
                || $('#SchedulingFieldValidator').is(':visible')
                || $('#StartDate').val() == ''
                || $('#EndDate').val() == ''
                || $('#StartTime').val() == ''
                || $('#EndTime').val() == '') {

                return false;
            }
            return true;
        }

        function MarkRequiredFields() {
            if ($('#CategoriesDropDownList').val() == null) {
                $('#CategoriesDropDownListRequiredFieldValidator').show();
            }
        }

        // 3075 : For activities with a Category of Proposed Release or Approved Release, Origin, Distribution and Comm Materials need to be  Mandatory.
        function validateOriginForCategory(source, args) {
            if ($('#CategoriesDropDownList').val() == '12'
                || $('#CategoriesDropDownList').val() == '58')
            {
                if ($('#NROriginDropDownList').val() == null)
                {
                    console.log('validate origin');
                    args.IsValid = false;
                    return;
                }
            }
            args.IsValid = true;
        }

        // 3075 : For activities with a Category of Proposed Release or Approved Release, Origin, Distribution and Comm Materials need to be  Mandatory.
        function validateDistributionForCategory(source, args) {
            if ($('#CategoriesDropDownList').val() == '12'
                || $('#CategoriesDropDownList').val() == '58')
            {
                if ($('#NRDistributionDropDownList').val() == null)
                {
                    console.log('validate distribution');
                    args.IsValid = false;
                    return;
                }
            }
            args.IsValid = true;
        }

        // 3075 : For activities with a Category of Proposed Release or Approved Release, Origin, Distribution and Comm Materials need to be  Mandatory.
        function validateCommMaterialsForCategory(source, args) {
            if ($('#CategoriesDropDownList').val() == '12'
                || $('#CategoriesDropDownList').val() == '58')
            {
                if ($('#CommMaterialsDropDownList').val() == null)
                {
                    console.log('validate comm materials');
                    args.IsValid = false;
                    return;
                }
            }
            args.IsValid = true;
        }

        // 3076 : Creating a new activity defaults to no start and end date, Start Time and End Time Validation after removing the initial values
        function validateTime(source, args){
            if (args.Value == '')
            {
                args.IsValid = false;
                return;
            }
            args.IsValid = true;
        }

        function ValidateDate(date2check, endDate) {
            var jqDate = $('#' + date2check.replace(' ',''));
            var validatedDate = Date.parse(jqDate.val());
            if (validatedDate == null) {
                jAlert('This is not a valid ' + date2check + '. Please use MM/DD/YYYY (e.g., 01/31/2021) format.', 'Format Error');
                jqDate.focus();
                return null;
            }
            if (endDate && validatedDate.compareTo(endDate) > 0) {
                jAlert('The ' + date2check + ' cannot occur after the End Date.', 'Validation Error');
                jqDate.focus();
                return null;
            }
            return validatedDate;
        }

        var timeRegex = /^((([0]?[1-9]|1[0-2])(:|\.)(00|05|10|15|20|25|30|35|40|45|50|55)?( )?(AM|am|aM|Am|PM|pm|pM|Pm))|(([0]?[0-9]|1[0-9]|1[0-2])(:|\.)(00|05|10|15|20|25|30|35|40|45|50|55)?))$/;
        function ValidateTime(time2check) {
            var jqTime = $('#' + time2check.replace(' ', ''));
            var validatedTime = new Date();
            if (jqTime.val() != '') {
                var time = jqTime.val().match(/(\d+)(?::(\d\d))?\s*(p?)/i);
                if (time == null || time == undefined) {
                    jAlert('This is not a valid ' + time2check + '. Please use HH:MM TT (e.g., 12:00 AM) format.', 'Format Error');
                    jqTime.focus();
                    return null;
                } else {
                    var hour = parseInt(time[1]);
                    if (hour == 12 && (time[3] == null || time[3].length == 0)) hour = 0;
                    validatedTime.setHours(hour + ((time[3] && hour != 12) ? 12 : 0));
                    validatedTime.setMinutes(parseInt(time[2]) || 0);
                }
            } else {
                validatedTime.setHours(0);
                validatedTime.setMinutes(0);
            }

            var timeInput = $.trim(jqTime.val());
            if (!timeRegex.test(timeInput)) {
                jAlert('This is not a valid ' + time2check + '. Please use 5 minute increments', 'Format Error');
                jqTime.focus();
                return null;
            }

            if (timeInput.indexOf(' ') == -1) {
                var timeString = timeInput.toLowerCase();
                var splitTime;

                if (timeString.indexOf('am') != -1) {
                    splitTime = timeString.split('a');
                    jqTime.val($.trim(splitTime[0]) + ' AM');
                }
                else if (timeString.indexOf('pm') != -1) {
                    splitTime = timeString.split('p');
                    jqTime.val($.trim(splitTime[0]) + ' PM');
                }
            }

            return validatedTime;
        }

        function IsSaveValid() {
            if (warn_on_unload == "" && $.url().param('ActivityId') == undefined) return false; // prevent double clicking users to create 2 new activities

            var pageIsValid = Page_ClientValidate();

            MarkRequiredFields();

            if (!RequiredFieldsCompleted() || !pageIsValid) {
                jAlert('A required field is missing.', 'Validation Error');
                return false;
            }

            // Validate the start date/end date & start/end time
            var endDate = ValidateDate('End Date');
            var startDate = ValidateDate('Start Date', endDate);
            if (startDate == null || endDate == null)
                return false;


            console.log("Date: |" + $("#NRDate").val() + "| Time: |" + $("#NRTime").val() + "|");

            if ($("#NRDate").val() != "" || ($("#NRTime").val() != "")) {
                var nrDate = ValidateDate('NR Date', endDate);   //($('#NRDate').val() == "" || $('#NRDate').val() == null) ? null : ValidateDate('NR Date', endDate);
                if (nrDate == null)
                    return false;

                var nrTime = ValidateTime('NR Time', nrDate.compareTo(endDate) == 0 ? endTime : null);
                if (nrTime == null)
                    return false
            }

            var startTime = ValidateTime('Start Time');
            var endTime = ValidateTime('End Time');
            if (startTime == null || endTime == null)
                return false;

            if (startDate.compareTo(endDate) == 0 && startTime.compareTo(endTime) > 0) {
                jAlert('The Start Time cannot occur after the End Time.', 'Validation Error');
                $('#EndTime').focus();
                return false;
            }

            var potentialDates = $('#PotentialDatesTextBox').val().toUpperCase();
            if (potentialDates.indexOf("TBC") != -1 || potentialDates.indexOf("TBD") != -1 || potentialDates.match(/\d+/g)) {
                jAlert('The potential dates field cannot use TBC, TBD or numbers.\n Please use this field only for general timelines e.g: winter, late June.', 'Validation Error');
                $('#PotentialDatesTextBox').focus();
                return false;
            }

            /* All unconfirmed activities must span no more than 2 weeks.
            if (!$("#IsDateConfirmed")[0].checked && endDate > CalculateMaxEndDate(14)) {
                jAlert('Unconfirmed activities cannot span more than 2 weeks. \n Please change your end date.', 'Timespan Error');
                return false;
            }*/

            return true;
        }

    </script>

    <style type="text/css">
        .ui-datepicker {
            margin-left: -80px;
        }
        /*class="new-activity-date"
            editable-select new-activity-time
            */
        .editable-select {
            width: 81px;
        }
    </style>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">

    <asp:HiddenField runat="server" ID="timestamp" />

    <div style="margin: 0 auto">

        <div style="width: 100%; padding-bottom: 10px;">
            <div runat="server" id="InactivityNotice" class="ChangesNotice" style="background-color: #CCCCCC; border: 1px solid #CCCCCC;">
                <table style="margin: auto;">
                    <tr>
                        <td>
                            <img src="../images/alert-icon.png" alt="warning" /></td>
                        <td><span style="font-weight: bold;">Note:</span> This tab will be closed after 15 minutes of inactivity.</td>
                    </tr>
                </table>
            </div>
            <div id="ChangesPending" class="ChangesNotice-Modifier" style="display: none;">
                <table style="margin: auto;">
                    <tr>
                        <td style="padding-bottom: 8px;">
                            <img src="../images/alert-icon.png" alt="warning" /></td>
                        <td><span style="font-weight: bold;">Activity has changes that have not been saved</span>. Please save your work.</td>
                    </tr>
                </table>
                <table id="SecondaryAlert" style="margin: auto; width: 100%; background-color: #D9EAF7; color: #464849;">
                    <tr>
                        <td id="ChangesPendingAlert" style="padding-top: 8px; padding-bottom: 8px;">
                            <i class="fa fa-info-circle fa-fw" aria-hidden="true"></i> This tab will be closed after 15 minutes of inactivity.
                        </td>
                    </tr>
                </table>
            </div>
            <div runat="server" id="SavedSuccessfullyNotice" class="SaveNotice" style="display: none;">
                <table style="margin: auto;">
                    <tr>
                        <td>
                            <img src="../images/done.png" alt="success" style="width: 24px; height: 15px;" /></td>
                        <td><span style="font-weight: bold;">Activity saved</span></td>
                    </tr>
                </table>
            </div>
            <div runat="server" id="ErrorNotice"  class="ErrorNotice" style="display: none;">
                <table style="margin: auto;">
                    <tr>
                        <td><%--<img src="../images/alert-icon.png" alt="warning" />--%></td>
                        <td><span style="font-weight: bold;" id="ErrorNoticeText" runat="server">Activity was updated by another user, and your changes cannot be saved</span>. Please open the activity again in order to see the most recent changes.</td>
                    </tr>
                </table>
            </div>
        </div>

        <table class="activity">
            <tr>
                <td></td>
                <td colspan="2">
                    <div class="activity-top-row">
                        <asp:Label Visible="false" ID="ActiveIDLabel" runat="server" Text="Activity Id: " CssClass="activityidlabel"></asp:Label>
                        <asp:Label Visible="false" ID="ActiveID" runat="server" Text="Saved" CssClass="activityid"></asp:Label>

                        <div>
                            <asp:Label Visible="false" ID="LastUpdated" runat="server" Text="Saved" CssClass="lastupdated"></asp:Label>
                            <asp:Label Visible="false" ID="LastUpdatedLabel" runat="server" Text="Last Updated: " CssClass="lastupdatedlabel"></asp:Label>
                        </div>

                        <div class="checks" style="display: none;">
                            <asp:CheckBox ID="IsInternalCheckBoxNotUsed" runat="server" Text="Is Internal?" /></div>

                        <div style="display: inline-block; display: inline-block">
                            <span runat="server" id="FavoriteIcon"></span>
                            <asp:Button ID="FavoriteButton" runat="server" OnClick="FavoriteButtonClick" Text="Add to Watchlist" />
                        </div>

                    </div>
                </td>
                <td></td>
            </tr>
            <tr>
                <td style="vertical-align: top;">
                    <div class="leftHelpDiv"></div>
                </td>
                <td class="col1" style="width: 540px; padding-left: 8px; padding-right: 8px;">
                    <div>

                        <fieldset id="overviewFieldset">
                            <legend>Overview</legend>
                            <div class="helpLink"><a onclick="onQuestionClick(this,true);"><i class="fa fa-question-circle fa-lg"></i></a></div>
                            <table>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsCategoriesNeedsReview)%>">Category:</td>
                                    <td class="column-right">
                                        <div id="CategoriesDropDownContainer">
                                            <select id="CategoriesDropDownList" multiple="true" runat="server" style="display:none"/>
                                            <span id="CategoriesDropDownListRequiredFieldValidator" style="color: red" runat="server">Required</span>
                                            <div id="CategoriesSelectedTextRow" style="display: none">
                                                <div id="CategoriesSelectedText" class="panel">
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr class="row" <%= CurrentActiveActivity.IsConfidential ? "style='color:darkred'" : ""%>>
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsCategoriesNeedsReview)%>">Not for Look Ahead:</td>
                                    <td class="column-right">
                                        <div class="checks">
                                            <asp:CheckBox ID="IsConfidentialCheckBox" runat="server" Text="" /></div>
                                    </td>
                                </tr>

                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsTitleNeedsReview)%>">Title:</td>
                                    <td class="column-right">
                                        <asp:TextBox ID="TitleTextBox" CssClass="new-activity-textareas" ForeColor="#4d4d4d" TextMode="MultiLine" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator Display="Dynamic" ID="TitleRequiredFieldValidator" runat="server" EnableClientScript="true"
                                            ControlToValidate="TitleTextBox" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                                        <span style="float: right"><span id="charlimitinfo">Characters remaining: 100</span></span>
                                    </td>
                                </tr>
                                <tr class="row" id="SummaryRow">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsDetailsNeedsReview)%>">Summary:</td>
                                    <td class="column-right">
                                        <asp:TextBox id="DetailsTextBox" CssClass="new-activity-textareas" ForeColor="#4d4d4d" Style="height: 60px;" TextMode="MultiLine"
                                            runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator Display="Dynamic" ID="DetailsRequiredFieldValidator" runat="server" EnableClientScript="true"
                                            ControlToValidate="DetailsTextBox" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                                        <span style="float: right"><span id="charlimitinfo1">Characters remaining: 700</span></span>
                                    </td>
                                </tr>
                                <tr class="row" id="IssueRow" <%= CurrentActiveActivity.IsIssue ? "style='font-weight:bold'" : "" %>>
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsCategoriesNeedsReview)%>">Issue:</td>
                                    <td class="column-right">
                                        <div class="checks">
                                            <asp:CheckBox ID="IsIssueCheckBox" runat="server" Text="" /></div>
                                    </td>
                                </tr>
                                <tr class="row" runat="server" id="SignificanceRow">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td id="SignificanceTitle" class="column-left label-dark">Significance:</td>
                                    <td class="column-right">
                                        <div>
                                            <asp:TextBox
                                                id="SignificanceTextBox"
                                                CssClass="new-activity-textareas"
                                                Style="height: 60px;"
                                                ForeColor="#4d4d4d"
                                                TextMode="MultiLine"
                                                runat="server"></asp:TextBox>
                                        </div>
                                        <asp:RequiredFieldValidator Display="Dynamic" ID="SignificanceRequiredFieldValidator" runat="server" EnableClientScript="true"
                                            ControlToValidate="SignificanceTextBox" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                                        <span style="float: right"><span id="charlimitinfo7">Characters remaining: 500</span></span>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsLeadOrganizationNeedsReview)%>">Lead Organization:</td>
                                    <td class="column-right">
                                        <asp:TextBox ID="LeadOrganizationTextBox" TextMode="MultiLine" ForeColor="#4d4d4d" CssClass="new-activity-textareas" placeholder="Province of BC"
                                            runat="server"></asp:TextBox>
                                        <span style="float: right"><span id="charlimitinfo6">Characters remaining: 80</span></span>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsInitiativesNeedsReview)%>">HQ Initiatives:</td>
                                    <td class="column-right">
                                        <select id="InitiativeDropDownList" multiple="true" runat="server" style="display:none"/>
                                        <div id="InitiativesSelectedTextRow" style="display: none">
                                            <div id="InitiativesSelectedText" class="panel">
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsTagsNeedsReview)%>">HQ Tags:</td>
                                    <td class="column-right">
                                        <select id="KeywordList" multiple="true" runat="server" style="display:none"/>
                                        <asp:TextBox ID="KeywordsTextBox" CssClass="new-activity-textareas" width="95%" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                        <fieldset id="planningFieldset">
                            <legend>Planning</legend>
                            <div class="helpLink"><a onclick="onQuestionClick(this,true);"><i class="fa fa-question-circle fa-lg"></i></a></div>
                            <table style="max-width: 500px;">
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark">Comm. Contact:</td>
                                    <td class="column-right">
                                        <select id="CommContactDropDownList" multiple="true" runat="server" style="display:none" class="new-activity-dropdowns" />
                                        <asp:RequiredFieldValidator Display="Dynamic" ID="CommunicationContactRequiredFieldValidator"
                                            runat="server" ControlToValidate="CommContactDropDownList" ErrorMessage="Required"
                                            ForeColor="Red"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr class="row" style="display:none">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left">Key activity:</td>
                                    <td class="column-right">
                                        <div class="checks">
                                            <asp:CheckBox ID="IsMilestoneCheckBox" runat="server" Visible="false" Text="" /></div>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsStrategyNeedsReview)%>">Strategy:</td>
                                    <td class="column-right">
                                        <div>
                                            <asp:TextBox
                                                id="StrategyTextBox"
                                                CssClass="new-activity-textareas"
                                                Style="height: 60px;"
                                                ForeColor="#4d4d4d"
                                                TextMode="MultiLine"
                                                runat="server"></asp:TextBox>
                                        </div>
                                        <asp:RequiredFieldValidator Display="Dynamic" ID="StrategyIsRequiredFieldValidator" runat="server" EnableClientScript="true" ControlToValidate="StrategyTextBox" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                                        <span style="float: right"><span id="charlimitinfo9">Characters remaining: 500</span></span>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsCommMaterialsNeedsReview)%>">Comm. Materials:</td>
                                    <td class="column-right">
                                        <select id="CommMaterialsDropDownList" multiple="true" runat="server" style="display: none" />
                                        <div id="CommMaterialsSelectedTextRow" style="display: none">
                                            <div id="CommMaterialsSelectedText" class="panel">
                                            </div>
                                        </div>
                                        <asp:CustomValidator ID="CommMaterialsDropDownListValidator" runat="server" Display="Dynamic"
                                            EnableClientScript="true" ControlToValidate="CommMaterialsDropDownList" ShowMessageBox="true"
                                            ErrorMessage="Required" ForeColor="Red" ValidateEmptyText="True" Enabled="true"
                                            ClientValidationFunction="validateCommMaterialsForCategory">
                                        </asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-right label-dark <%=Markup(CurrentActiveActivity.IsInternalNotesNeedsReview)%>" colspan="3" style="padding-left: 12px;">Internal Notes:
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-right" colspan="3" style="padding-left: 12px;">
                                        <asp:TextBox ID="CommentsTextBox" CssClass="new-activity-textareas" Style="height: 100px;" ForeColor="#4d4d4d" TextMode="MultiLine"
                                            runat="server"></asp:TextBox>
                                        <span style="float: right"><span id="charlimitinfo3">Characters remaining: 4000</span></span>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>

                        <fieldset id="ministryFieldset">
                            <legend>Ministry</legend>
                            <div class="helpLink"><a onclick="onQuestionClick(this,true);"><i class="fa fa-question-circle fa-lg"></i></a></div>
                            <table>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark">Lead Ministry:</td>
                                    <td class="column-right">
                                        <select id="ContactMinistryDropDownList" multiple="true" runat="server" style="display:none" CssClass="new-activity-dropdowns" />
                                        <asp:RequiredFieldValidator Display="Dynamic" ID="ContactMinistryRequiredFieldValidator"
                                            runat="server" ControlToValidate="ContactMinistryDropDownList" ErrorMessage="Required"
                                            ForeColor="Red"></asp:RequiredFieldValidator>
                                    </td>

                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark">Cross-Government:</td>
                                    <td class="column-right">
                                        <div class="checks">
                                            <asp:CheckBox ID="IsCrossGovernmentCheckBox" runat="server" Text="" /></div>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark">Shared With:</td>
                                    <td class="column-right">
                                        <select id="SharedWithDropDownList" multiple="true" runat="server" style="display:none"/>
                                        <div id="SharedWithSelectedTextRow" style="display: none">
                                            <div id="SharedWithSelectedText" class="panel"></div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </div>
                </td>
                <td class="col2" style="width: 540px; padding-left: 8px; padding-right: 8px;">
                    <div>

                        <fieldset id="laFieldset" runat="server">
                            <legend>Look Ahead</legend>
                            <!-- <div class="helpLink"><a onclick="(this,false);"><i class="fa fa-question-circle fa-lg"></i></a></div> -->
                            <table>
                                <tr class="row" runat="server" id="LACommentsRow">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark">Executive Summary:</td>
                                    <td class="column-right">
                                        <asp:TextBox ID="LACommentsTextBox" CssClass="new-activity-textareas" Style="height: 60px;" ForeColor="#4d4d4d" TextMode="MultiLine"
                                            runat="server"></asp:TextBox>
                                        <asp:CustomValidator Display="Dynamic" ID="LACommentsCustomValidator" runat="server" ClientValidationFunction="EvaluateLAComments"
                                            Text="Required" ForeColor="Red"></asp:CustomValidator>
                                        <span style="float: right"><span id="charlimitinfo2">Characters remaining: 2000</span></span>
                                    </td>
                                </tr>
                                <tr class="row" runat="server" id="LAStatusRow">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark">Look Ahead Status:</td>
                                    <td class="column-right">
                                        <asp:RadioButtonList ID="LAStatusRadioButtonList" runat="server" RepeatDirection="Horizontal" />
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark">Look Ahead Section:</td>
                                    <td class="column-right">
                                        <asp:CheckBoxList ID="LASectionCheckBoxList" runat="server" RepeatDirection="Horizontal" />
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark">Release Time:</td>
                                    <td class="column-right">
                                        <input type="text" id="NRDate" title="Choose a release date and time" value="" runat="server" class="new-activity-date" style="width: 80px;" />
                                        <asp:DropDownList runat="server" ID ="NRTime" CssClass="editable-select" />
                                        <span class="PSTLabel">PST</span>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>

                        <fieldset id="scheduleFieldset">
                            <legend>Schedule</legend>
                            <div class="helpLink"><a onclick="onQuestionClick(this,false);"><i class="fa fa-question-circle fa-lg"></i></a></div>
                            <table>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsStartDateNeedsReview)%>">Start Date:</td>
                                    <td class="column-right">
                                        <div>
                                            <input type="text" id="StartDate" runat="server" class="new-activity-date" style="width: 80px;" />
                                            <asp:RequiredFieldValidator Display="Dynamic" ID="StartDateRequiredFieldValidator"
                                                runat="server" ControlToValidate="StartDate" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                                            &nbsp;&nbsp;
                                            <div id="StartTimeContainer" style="display: inline-block; vertical-align: top;" >
                                                <asp:DropDownList runat="server" ID ="StartTime" CssClass="editable-select" />
                                                <span id="StartTimePSTLabel" class="PSTLabel">PST</span>
                                                <span id="StartTimeValidator" style="color: red" runat="server">Required</span>
                                                <asp:CustomValidator id="StartTimeCustomValidator" runat="server" Display="Dynamic"
                                                    EnableClientScript="true" ControlToValidate="StartTime" ShowMessageBox="true"
                                                    ErrorMessage="Required" ForeColor="Red" ValidateEmptyText="True" Enabled="true"
                                                    ClientValidationFunction="validateTime">
                                                </asp:CustomValidator>
                                            </div>
                                        </div>
                                    </td>

                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsEndDateNeedsReview)%>">End Date:</td>
                                    <td class="column-right">
                                        <div>
                                            <input type="text" id="EndDate" value="" runat="server" class="new-activity-date" style="width: 80px;" />
                                            <asp:RequiredFieldValidator Display="Dynamic" ID="EndDateRequiredFieldValidator"
                                                runat="server" ControlToValidate="EndDate" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                                            &nbsp;&nbsp;
                                            <div id="EndTimeContainer" style="display: inline-block; vertical-align: top;">
                                                <asp:DropDownList runat="server" ID ="EndTime" CssClass="editable-select" />
                                                <span id="EndTimePSTLabel" class="PSTLabel">PST</span>
                                                <span id="EndTimeRequiredValidator" style="color: red" runat="server">Required</span>
                                                <asp:CustomValidator id="EndTimeCustomValidator" runat="server" Display="Dynamic"
                                                    EnableClientScript="true" ControlToValidate="EndTime" ShowMessageBox="true"
                                                    ErrorMessage="Required" ForeColor="Red" ValidateEmptyText="True" Enabled="true"
                                                    ClientValidationFunction="validateTime">
                                                </asp:CustomValidator>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr class="row">
                                    <td class="column-indicator"></td>
                                    <td class="column-left label-dark">
                                        <asp:Label ID="Label24" runat="server" Text="All Day Activity:"></asp:Label>
                                    </td>
                                    <td class="column-right">
                                        <asp:CheckBox ID="IsAllDayCheckBox" Style="float: left" runat="server" />
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark">
                                        <asp:Label ID="DateConfirmedLabel" runat="server" Text="Dates Confirmed:"></asp:Label>
                                    </td>
                                    <td class="column-right">
                                        <asp:CheckBox ID="IsDateConfirmed" Style="float: left;margin-right: 15px;" runat="server" />
                                        <asp:Label ID="PotentialDatesLabel" runat="server" Text="Potential Dates:" style="vertical-align:top"></asp:Label>
                                        <asp:TextBox ID="PotentialDatesTextBox" CssClass="new-activity-textareas" MaxLength="50" Style="width: 170px;" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="row" runat="server" id="ScheduleRow">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td id="SchedulingTitle" runat="server" class="column-left label-dark" style="line-height: 18px;">Scheduling
                                        Considerations & Approval Notes:</td>
                                    <td class="column-right">
                                        <asp:TextBox ID="SchedulingTextBox" CssClass="new-activity-textareas" Style="height: 60px;" ForeColor="#4d4d4d" TextMode="MultiLine"
                                            runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator Display="Dynamic" ID="SchedulingFieldValidator" runat="server" EnableClientScript="true"
                                            ControlToValidate="SchedulingTextBox" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                                        <span style="float: right"><span id="charlimitinfo8">Characters remaining: 500</span></span>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>

                        <fieldset id="releaseFieldset">
                            <legend>Release</legend>
                            <div class="helpLink"><a onclick="onQuestionClick(this,false);"><i class="fa fa-question-circle fa-lg"></i></a></div>
                            <table>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsOriginNeedsReview)%>">Origin:</td>
                                    <td class="column-right">
                                        <select id="NROriginDropDownList" multiple="true" runat="server" style="display:none"/>
                                        <asp:CustomValidator id="NROriginDropDownListValidator" runat="server" Display="Dynamic"
                                            EnableClientScript="true" ControlToValidate="NROriginDropDownList" ShowMessageBox="true"
                                            ErrorMessage="Required" ForeColor="Red" ValidateEmptyText="True" Enabled="true"
                                            ClientValidationFunction="validateOriginForCategory">
                                        </asp:CustomValidator>

                                    </td>

                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsDistributionNeedsReview)%>">Distribution:</td>
                                    <td class="column-right">
                                        <select id="NRDistributionDropDownList" multiple="true" runat="server" style="display:none"/>
                                        <asp:CustomValidator id="NRDistributionDropDownListValidator" runat="server" Display="Dynamic"
                                            EnableClientScript="true" ControlToValidate="NRDistributionDropDownList" ShowMessageBox="true"
                                            ErrorMessage="Required" ForeColor="Red" ValidateEmptyText="True" Enabled="true"
                                            ClientValidationFunction="validateDistributionForCategory">
                                        </asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsTranslationsRequiredNeedsReview)%>">Translations Required:</td>
                                    <td class="column-right">
                                        <select id="TranslationsRequired" multiple="true" runat="server" style="display:none"/>
                                        <asp:TextBox ID="TranslationsTextbox" CssClass="new-activity-textareas" width="95%" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="row" style="display:none">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left">Sectors:</td>
                                    <td class="column-right">
                                        <select id="SectorDropDownList" multiple="true" runat="server" style="display:none"/>
                                        <div id="SectorsSelectedTextRow" style="display: none">
                                            <div id="SectorsSelectedText" class="panel">
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark">Themes:</td>
                                    <td class="column-right">
                                        <select id="ThemeDropDownList" multiple="true" runat="server" style="display:none"/>
                                        <div id="ThemesSelectedTextRow" style="display: none">
                                            <div id="ThemesSelectedText" class="panel">
                                            </div>
                                        </div>
                                    </td>
                                </tr>

                                <%if (Model.Releases.Any())
                                { %>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left" colspan="2">BC Gov News:</td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left" colspan="2">
                                        <table class="release-list-item-row">
                                            <asp:Repeater ID="ActivityNewsList" runat="server" OnItemCommand="ReleaseLinksItems_Click" DataSource="<%# Model.Releases %>" ItemType="CorporateCalendar.ActivityModel.Release">
                                                <ItemTemplate>
                                                    <tr style="vertical-align: text-top">
                                                        <td style="padding-right: 8px;">
                                                            <span style='width: 6px; background-color: <%# Item.Color %>;'>&nbsp;</span>
                                                        </td>
                                                        <td style="width: 100%">
                                                            <asp:LinkButton runat="server" CommandArgument='<%# Item.Id %>' CommandName="ReleaseLinksItems_Click" Text='<%# Item.DocumentType %>'></asp:LinkButton>
                                                        </td>
                                                        <td style="width: auto; white-space: nowrap; padding-left: 8px; padding-right: 8px">
                                                            <asp:Label runat="server" Text='<%# Item.PublishStatus %>' />
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </table>
                                    </td>
                                </tr>
                                <% } %>
                            </table>
                        </fieldset>

                        <fieldset id="eventFieldset">
                            <legend>Event</legend>
                            <div class="helpLink"><a onclick="onQuestionClick(this,false);"><i class="fa fa-question-circle fa-lg"></i></a></div>
                            <table>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsPremierRequestedNeedsReview)%>">Premier Requested:</td>
                                    <td class="column-right">
                                        <select id="PremierRequestedDropDownList" multiple="true" runat="server" style="display:none"/>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsRepresentativeNeedsReview)%>">Representative:</td>
                                    <td class="column-right">
                                        <select id="RepresentativeDropDownList" multiple="true" runat="server" style="display:none" CssClass="new-activity-dropdowns"  />
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark">At BC Legislature:</td>
                                    <td class="column-right">
                                        <div class="checks">
                                            <asp:CheckBox ID="IsAtLegislatureCheckBox" Text="" runat="server" /></div>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsCityNeedsReview)%>">City:</td>
                                    <td class="column-right">
                                        <select id="CityDropDownList" multiple="true" runat="server" style="display:none" CssClass="new-activity-dropdowns"  />
                                    </td>
                                </tr>
                                <tr class="row" runat="server" id="OtherCityRow">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark">Other City:</td>
                                    <td class="column-right">
                                        <asp:TextBox ID="OtherCityTextBox" TextMode="MultiLine" MaxLength="50" ForeColor="#4d4d4d" CssClass="new-activity-dropdowns"
                                            runat="server"></asp:TextBox>
                                        <span style="float: right"><span id="charlimitinfo5">Characters remaining: 55</span></span>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsVenueNeedsReview)%>">Venue:</td>
                                    <td class="column-right">
                                        <asp:TextBox ID="VenueTextBox" TextMode="MultiLine" MaxLength="50" ForeColor="#4d4d4d" CssClass="new-activity-textareas"
                                            runat="server"></asp:TextBox>
                                        <span style="float: right"><span id="charlimitinfo4">Characters remaining: 55</span></span>
                                    </td>
                                </tr>
                                <tr id="EventPlannerContainer" class="row">
                                        <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                        <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsEventPlannerNeedsReview)%>">Event Planner:</td>
                                        <td class="column-right">
                                            <select id="EventPlannerDropDownList" multiple="true" runat="server" style="display:none"/>
                                        </td>
                                </tr>
                                <tr id="VideographerContainer" class="row">
                                    <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                    <td class="column-left label-dark <%=Markup(CurrentActiveActivity.IsDigitalNeedsReview)%>">Digital:</td>
                                    <td class="column-right">
                                        <select id="VideographerDropDownList" multiple="true" runat="server" style="display:none"/>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>

                        <%--TODO: Request Bruno's help organizing this code.--%>
                        <style type="text/css">
                            .activity-document-deleted {
                                text-decoration: line-through;
                                color: dimgray !important;
                            }
                        </style>
                        <asp:Panel runat="server" ID="RecordsSection">
                        <asp:HiddenField runat="server" ID="deletedDocuments" ClientIDMode="Static" Value="" />
                        <fieldset id="documentsFieldset">
                            <legend>Records</legend>
                            <asp:Repeater runat="server" ID="documentsRepeater" ClientIDMode="Predictable" ItemType="CorporateCalendar.Data.ActivityFile">
                                <HeaderTemplate>
                                    <table>
                                </HeaderTemplate>
                                <ItemTemplate>
                                        <tr class="row">
                                            <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                            <td style="width=100%" colspan="2" onmouseover="$('#MainContent_documentsRepeater_deleteBtn_<%# Container.ItemIndex %>').show();" onmouseout ="$('#MainContent_documentsRepeater_deleteBtn_<%# Container.ItemIndex %>').hide();"                                             >
                                                <asp:HyperLink runat="server" NavigateUrl='<%# ResolveUrl($"~/Calendar/ActivityFile.ashx?id={Item.Id}") %>'>
                                                    <asp:Label runat="server" Text="<%# Item.FileName %>" />
                                                </asp:HyperLink>

                                                <asp:Label  runat="server" Visible="<%# !IsUserReadOnly %>">
                                                <asp:Label ID="deleteBtn" runat="server" Style="display:none">
                                                <a href="#" style="color: dimgray" onclick="$(this).parent().parent().prev().addClass('activity-document-deleted'); $(this).hide(); $('#deletedDocuments').val($('#deletedDocuments').val() + ',<%# Item.Id %>'); return false;">
                                                    <span class="glyphicon glyphicon-remove-circle" aria-hidden="true"></span>
                                                </a>
                                                </asp:Label>
                                                </asp:Label>

                                            </td>
                                        </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                        <tr>
                                            <td colspan="3">&nbsp; </td>
                                        </tr>
                                        <asp:Panel ClientIDMode="Static" runat="server" Visible="<%# !IsUserReadOnly %>">
                                            <tr class="row" style="margin-top: 12px;">
                                                <td class="column-indicator"><span class="non-required-field">&nbsp;</span></td>
                                                <td class="column-left">Add Files:</td>
                                                <td class="column-right">
                                                    <asp:FileUpload runat="server" AllowMultiple="true" ID="documents" Width="95%" />
                                                </td>
                                            </tr>
                                        </asp:Panel>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </fieldset>
                        </asp:Panel>

                         <fieldset runat="server" id="ActionsFieldset" style="font-size: 10px;">

                             <%--There is a known issue (see below) that the icons won't show up on asp:buttons,
            so we have jquery buttons and use the postback event from the hidden asp:buttons
            so using a proxy button for the query buttons--%>

                            <div style="padding: 1px; padding-bottom: 5px; display: inline-block">
                                <asp:Button ID="SaveButton" runat="server" OnClick="SaveButtonClick" Text="Save and Close"/>

                                <asp:Button ID="CancelButton" runat="server" OnClick="CancelButton_Click" Text="Cancel" CssClass="hidden" CausesValidation="false" />
                            </div>

                            <div style="display: inline-block; display: inline-block">
                                <asp:Button ID="ReviewButton" runat="server" OnClick="ReviewButtonClick" Text="Save and Review" />
                            </div>
                            <div style="display: inline-block; display: inline-block">
                                <asp:Button ID="CloneButton" runat="server" OnClick="CloneButtonClick" Text="Clone" CausesValidation="false" />
                            </div>
                            <div style="display: inline-block; display: inline-block; padding-bottom: 5px;">
                                <asp:Button ID="DeleteButton" runat="server" OnClick="DeleteButton_Click" Text="Delete" CssClass="hidden" CausesValidation="false" />
                            </div>
                        </fieldset>
                     </div><div style="display: none;">

                        <asp:HiddenField ID="ActivityIdHiddenField" runat="server" />
                        <asp:HiddenField ID="UserFirstNameLastNameHiddenField" runat="server" />
                        <asp:HiddenField ID="UserIdHiddenField" runat="server" />
                        <asp:HiddenField ID="UserEmailHiddenField" runat="server" />
                        <asp:HiddenField ID="PageGUID" ViewStateMode="Enabled" runat="server" />
                        <asp:HiddenField ID="IsLockedHiddenField" runat="server" Value="False" />

                        <asp:TextBox ID="CategoryIdHiddenField" runat="server" Value="" /><br />
                        <asp:TextBox ID="SharedWithSelectedValues" runat="server" />
                        <asp:TextBox ID="SharedWithSelectedValuesServerSide" runat="server" />

                        <asp:TextBox ID="CommMaterialsSelectedValues" runat="server" />
                        <asp:TextBox ID="CommMaterialsSelectedValuesServerSide" runat="server" />
                        <asp:TextBox ID="SectorsSelectedValues" runat="server" />
                        <asp:TextBox ID="SectorsSelectedValuesServerSide" runat="server" />
                        <asp:TextBox ID="ThemesSelectedValues" runat="server" />
                        <asp:TextBox ID="ThemesSelectedValuesServerSide" runat="server" />
                        <asp:TextBox ID="InitiativesSelectedValues" runat="server" />
                        <asp:TextBox ID="InitiativesSelectedValuesServerSide" runat="server" />
                        <asp:TextBox ID="ApplicationOwnerOrganizations" runat="server" />
                        <asp:TextBox ID="IsPostBackHiddenField" runat="server" />
                        <asp:TextBox ID="IsCommunicationMaterialsDirty" runat="server" />
                        <asp:TextBox ID="IsNROriginsDirty" runat="server" />
                        <asp:TextBox ID="IsNRDistributionsDirty" runat="server" />

                        <asp:TextBox ID="IsSectorsDirty" runat="server" />
                        <asp:TextBox ID="IsThemesDirty" runat="server" />
                        <asp:TextBox ID="IsInitiativesDirty" runat="server" />

                        <input type="hidden" runat="server" id="ComContactSelectedValue" />

                    </div>

                </td>
                <td style="vertical-align: top;">
                    <div class="rightHelpDiv"></div>
                </td>
            </tr>
        </table>

    </div>

    <script type="text/javascript">

        var idleTime = 0;
        $(document).ready(function () {

            // Increment the idle time counter every minute.
            var idleInterval = setInterval(timerIncrement, 60000); // 1 minute

            // Zero the idle timer on mouse movement.
            $(this).mousemove(function (e) {
                idleTime = 0;
            });
            $(this).keypress(function (e) {
                idleTime = 0;
            });

            function timerIncrement() {
                idleTime = idleTime + 1;
                if (idleTime > 14) { // 15 minutes
                    window.onbeforeunload = null;
                    window.open('', '_self', '').close();
                }
            }

            function highlightChangedFields() {
                var significanceClass = "<%=Markup(CurrentActiveActivity.IsSignificanceNeedsReview)%>";
                if (significanceClass && significanceClass === "reviewed") {

                    var $significanceTitle = $("#SignificanceTitle");
                    if ($significanceTitle.length) {
                        $significanceTitle.addClass(significanceClass);
                    }
                }

                var schedulingClass = "<%=Markup(CurrentActiveActivity.IsSchedulingConsiderationsNeedsReview)%>";
                if (schedulingClass && schedulingClass === "reviewed") {

                    var $schedulingTitle = $("#SchedulingTitle");
                    if ($schedulingTitle.length) {
                        $schedulingTitle.addClass(schedulingClass);
                    }
                }
            }
            highlightChangedFields();

            // Add tool tips
            $("#DetailsTextBox").tooltip({ trigger: "hover", html: "true", placement: "right", delay: 250, title: "Describe this activity in the present tense, including <b>who</b> is participating (each spokesperson and their role), <b>what</b> is happening (key details of the activity) and <b>how much</b> is being funded.  The summary should contain all of the details that are needed for the look ahead report." });
            $("#SignificanceTextBox").tooltip({ trigger: "hover", html: "true", placement: "right", delay: 250, title: "Explain <b>why</b> this activity is included on the corporate calendar. What is its importance to government or your ministry?" });
            $("#StrategyTextBox").tooltip({ trigger: "hover", html: "true", placement: "right", delay: 250, title: "Describe how you will <b>promote</b> this activity or announcement." });
            $("#SchedulingTextBox").tooltip({ trigger: "hover", html: "true", placement: "left", delay: 250, title: "If dates and times are confirmed, please tick the <b>Confirmed</b> box" });
            $("#EventPlannerContainer").tooltip({ trigger: "hover", html: "true", placement: "left", delay: 250, title: "To book a GCPE event planner, email events@gov.bc.ca" });
            $("#VideographerContainer").tooltip({ trigger: "hover", html: "true", placement: "left", delay: 250, title: "To book a GCPE videographer, email videorequest@gov.bc.ca <br /> To book a GCPE photographer, email photorequest@gov.bc.ca" });
            $("#StartTimeContainer").tooltip({ trigger: "hover", html: "true", placement: "left", delay: 250, title: "If time is a <b>guess</b>, please leave as 8am-6pm default." });
            $("#EndTimeContainer").tooltip({ trigger: "hover", html: "true", placement: "left", delay: 250, title: "If time is a <b>guess</b>, please leave as 8am-6pm default." });
            $("#CategoriesDropDownContainer").tooltip({
                trigger: "hover", html: "true", placement: "right", delay: 250, title: "<p align='left'><b>Approved Items:</b> use only when Province is involved (releases, activities, events etc.); approval notes must also be added. </p>\
                                                                                        <p align='left'><b>Awareness Dates or Conferences:</b> list name, description and dates it is taking place. Separate entries are needed for spin-off events such as news releases, speeches or announcements.</p>\
                                                                                        <p align='left'><b>FYI Only:</b> use for third-party items with no provincial involvement other than a Minister’s message or quote. </p>\
                                                                                        <p align='left'><b>Proposed Items:</b> use only when Province is involved (releases, activities, events etc.) </p>\
                                                                                        <p align='left'><b>Speech/Remarks:</b> use for Minister’s routine speaking engagements. Start time is speaking time. </p>\
                                                                                        <p align='left'><b>TV/Radio:</b> use for interviews; set to date/time it will air.</p>" });


            function getParameterByName(name, url) {
                if (!url) url = window.location.href;
                name = name.replace(/[\[\]]/g, '\\$&');
                var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
                    results = regex.exec(url);
                if (!results) return null;
                if (!results[2]) return '';
                return decodeURIComponent(results[2].replace(/\+/g, ' '));
            }

            var saveBtnLabelText = 'Save';

            var activityIdQueryParam = getParameterByName('ActivityId');
            var isClonedQueryParam = getParameterByName('IsCloned');

            if (activityIdQueryParam !== null || isClonedQueryParam !== null) {
                saveBtnLabelText = 'Save and Close';
            }

            // hide the potential dates field for new activities
            var newQueryParam = getParameterByName('new');
            if (activityIdQueryParam !== null || activityIdQueryParam === null || newQueryParam == 1 || isClonedQueryParam == "true") {
                $('#PotentialDatesLabel').hide();
                $('#PotentialDatesTextBox').hide();
            }

            // Handle Save Button
            $('#SaveButton').button({ label: saveBtnLabelText }).click(function (event) {
                if (!IsSaveValid()) {
                    event.preventDefault();
                    return;
                }
                warn_on_unload = "";
                $('#IsPostBackHiddenField').val('true');
                // force the tab/window to close so ministry users are forced to re-open the activity and get any subsequent changes made by HQ
                // previously, ministry users would leave the tab open, HQ would make changes and ministry users would indavertently
                // overwrite those changes made by HQ with later edits to the open activity, causing data to be incorrect
                // window.close();
            });

            // Work around to get icons working
            // http://csharp-guide.blogspot.in/2012/07/aspnet-jquery-button-icons-converting.html

            // Setup cancel button
            $('#CancelButton').each(function () {
                $(this).hide().after('<button>').next().button
                ({
                    label: $(this).val()
                }).click(function (event) {
                    event.preventDefault();
                    var $this = $(this);
                    jConfirm("Are you sure you want to cancel these changes?", null, function (ok) {
                        if (!ok) return;
                        CancelEditing();
                        warn_on_unload = "";
                        $this.prev().click();
                    });
                });
            });
            $('#CancelButton').button();

            // Set up delete button
            $('#DeleteButton').each(function () {
                $(this).hide().after('<button>').next().button
                ({
                    icons: { primary: 'ui-icon-trash' },
                    label: $(this).val()
                }).click(function (event) {
                    event.preventDefault();
                    var $this = $(this);
                    var msg = "Please confirm and let the Corporate Calendar Team know the reason for deletion.";
                    jConfirm(msg, "Delete this activity", function (ok) {
                        if (!ok) return;
                        warn_on_unload = "";
                        $this.prev().click();
                    });
                });
            });
            $('#DeleteButton').button();

            // Set up clone button
            $('#CloneButton').each(function () {
                $(this).hide().after('<button>').next().button
                ({
                    icons: { primary: 'ui-icon-copy' },
                    label: $(this).val()
                }).click(function (event) {
                    event.preventDefault();
                    var $this = $(this);
                    jConfirm("Are you sure you want to clone this activity?", null, function (ok) {
                        if (ok && IsCloneValid()) {
                            warn_on_unload = "";
                            $('#IsPostBackHiddenField').val('true');
                            $this.prev().click();
                        }
                    });
                });
            });
            $('#CloneButton').button();

            // Set up review button
            $('#ReviewButton').each(function () {
                $(this).hide().after('<button>').next().button
                ({
                    icons: { primary: 'ui-icon-check' },
                    label: $(this).val()
                }).click(function (event) {
                    event.preventDefault();
                    if (!IsSaveValid()) {
                        return;
                    }
                    var $this = $(this);
                    jConfirm("Are you sure you want to review this activity?", null, function (ok) {
                        if (!ok) return;
                        warn_on_unload = "";
                        $('#IsPostBackHiddenField').val('true');
                        $this.prev().click();
                    });
                });
            });
            $('#ReviewButton').button();

            // Set up review button
            $('#FavoriteButton').each(function () {
                $(this).hide().after('<button>').next().button
                ({
                    //icons: { primary: 'ui-icon-star' },
                    label: $(this).val()
                }).click(function (event) {
                    event.preventDefault();
                    if (GetChanged()) {
                        jAlert("Save your changes to the activity first.", "");
                    }
                    else {
                        $('#IsPostBackHiddenField').val('true');
                        $(this).prev().click();
                    }
                });
            });
            $('#FavoriteButton').button();

            hideIrrelevantPanels($('#CategoriesDropDownList option:selected').text());
        });

    $(function () {

        var aDate = "2016/1/1 ";
        var previousStart = $('#StartTime').val();
        $('#StartTime').editableSelect({
            bg_iframe: true,

            onSelect: function (listItem) {
                // 3076: Creating a new activity defaults to no start and end date
                // the editable select textbox does not update to the selected value and selected option alway set to "", have to put 2 validation in order to validate on change and on save,
                // will need further investigation on those editable select
                if ( listItem.text() == '' || listItem.text() == null )
                {
                    $('#StartTimeValidator').show();
                    $('#StartTimeCustomValidator').show();
                }
                else
                {
                    $('#StartTimeValidator').hide();
                    $('#StartTimeCustomValidator').hide();
                }
                if (previousStart == "8:00 AM" && $('#EndTime').val() == "6:00 PM") {
                    var newStart = new Date(aDate + $('#StartTime').val());
                    newStart.setTime(newStart.getTime() + 1000 * 60 * 5); // advance the time 5 minutes.
                    $('#EndTime').val(newStart.toString("h:mm tt"));
                    previousStart = $('#StartTime').val();
                }
                SetChanged($.url().param('ActivityId'));
            }
        });

        $('#EndTime').editableSelect({
            bg_iframe: true,
            onSelect: function (listItem) {
                if ( listItem.text() == '' || listItem.text() == null )
                {
                    $('#EndTimeRequiredValidator').show();
                    $('#EndTimeCustomValidator').show();
                }
                else
                {
                    $('#EndTimeRequiredValidator').hide();
                    $('#EndTimeCustomValidator').hide();
                }
                SetChanged($.url().param('ActivityId'));
            }
        });

        $('#NRTime').editableSelect({
            bg_iframe: true,
            onSelect: function (listItem) {
                SetChanged($.url().param('ActivityId'));
            }
        });
        var select = $('.editable-select:first');
        var instances = select.editableSelectInstances();

        // --------------------------------------------
        // Handle all-day activities

        $("#IsAllDayCheckBox").click(function () {
            SetChanged($.url().param('ActivityId'));

            if ($('#IsAllDayCheckBox').is(':checked')) {
                $('#StartTime').val("12:00 AM");
                $('#EndTime').val("11:45 PM");
                $('#StartTime').hide();
                $('#EndTime').hide();
                $("#StartTimePSTLabel").hide();
                $("#EndTimePSTLabel").hide();
            } else {
                $('#StartTime').show();
                $('#EndTime').show();
                $("#StartTimePSTLabel").show();
                $("#EndTimePSTLabel").show();
                $('#StartTime').val("8:00 AM");
                $('#EndTime').val("6:00 PM");
                previousStart = $('#StartTime').val();
            }
            SetConfirmedLabel();
        });

    });


    function SetConfirmedLabel() {
        var allDay = $("#IsAllDayCheckBox").is(':checked');

        var startDate = $("#StartDate").datepicker("getDate");
        var endDate = $("#EndDate").datepicker("getDate");
        var multiDay = Number(startDate) != Number(endDate);

        $("#DateConfirmedLabel").text(multiDay ? "Dates Confirmed:" : allDay ? "Date Confirmed:" : "Time Confirmed:");
    }

    var hidden_offset;
    function moveHidden() {
        var hidden = $('#hidden');
        hidden.show();
        $('#toggle_hidden').val('Hide');
        if (!hidden_offset) {
            hidden_offset = hidden.offset();
            hidden
              .css('position', 'absolute')
              .css('top', hidden_offset.top)
              .css('left', hidden_offset.left)
              .animate({ top: 100, left: 400 })
            ;
        } else {
            hidden.animate({ top: hidden_offset.top, left: hidden_offset.left }, function () {
                hidden.css('position', 'static');
                hidden_offset = null;
            });
        }
    }
    function toggleHidden(btn) {
        var hidden = $('#hidden');
        if ($('#hidden').is(':visible')) {
            hidden.hide();
            $(btn).val('Display');
        } else {
            hidden.show();
            $(btn).val('Hide');
        }
    }

    </script>



    <script type="text/javascript">
        $(document).ready(function () {

            // --------------------------------------------
            // THIS BLOCK OF SCRIPT IS ALL ABOUT STYLE AND DEFAULT VALUES
            // --------------------------------------------

            // --------------------------------------------
            // Turn on uniform styling
            $("input:text, textarea").uniform();
            $('#IsAtLegislatureCheckBox').uniform();
            $('#IsDateConfirmed').uniform();
            $('#IsInternalCheckBoxNotUsed').uniform();
            $('#IsConfidentialCheckBox').uniform();
            $('#IsIssueCheckBox').uniform();
            $('#IsMilestoneCheckBox').uniform();
            $('#IsCrossGovernmentCheckBox').uniform();
            $('#IsAllDayCheckBox').uniform();
            $('#StartTime').uniform();
            $('#EndTime').uniform();

            // --------------------------------------------
            // Turn on elastic styling
            $('#TitleTextBox').elastic();
            $('#DetailsTextBox').elastic();
            $('#LACommentsTextBox').elastic();
            $('#CommentsTextBox').elastic();
            $('#VenueTextBox').elastic();
            $('#OtherCityTextBox').elastic();
            $('#LeadOrganizationTextBox').elastic();
            $('#SignificanceTextBox').elastic();
            $('#StrategyTextBox').elastic();
            $('#SchedulingTextBox').elastic();

            // --------------------------------------------
            // Setup date pickers
            $('#StartDate').datepicker({
                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonText: "",
                buttonImageOnly: true,
                onSelect: function (dateText, inst) {
                    $(this).change();
                    var activityId = $.url().param('ActivityId');
                    if (activityId < 0 || activityId === undefined) {
                        $('#EndDate').datepicker("setDate", $(this).datepicker("getDate"));
                        $('#EndDateRequiredFieldValidator').hide();
                    }
                    SetConfirmedLabel();
                }
            });

            $('#EndDate').datepicker({
                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonText: "",
                buttonImageOnly: true,
                onSelect: function () {
                    $(this).change();

                    SetConfirmedLabel();
                }
            });

            $('#NRDate').datepicker({
                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true,
                buttonText: 'Choose a release date',
                onSelect: function () {
                    $(this).change();
                }
            });

            $('#StartTime').css('width', '78px');
            $('#EndTime').css('width', '78px');
            $('#NRTime').css('width', '78px');

            // --------------------------------------------
            // Setup the input mask for start and end dates (__/__/____)
            $('#StartDate').mask("99/99/9999", { placeholder: 'MM/DD/YYYY' });
            $('#EndDate').mask("99/99/9999", { placeholder: 'MM/DD/YYYY' });;
            $('#NRDate').mask("99/99/9999", { placeholder: 'MM/DD/YYYY' });;

            $('#IsDateConfirmed').click(function () {
                ShowHidePotentialDates();
                SetLASection();
                SetChanged($.url().param('ActivityId'));
            });

            var startDate = $("#StartDate");
            var endDate = $("#EndDate");
            var nrDate = $("#NRDate");

            startDate.change(function () {
                SetChanged($.url().param('ActivityId'));
                SetLASection();
            });

            endDate.change(function () {
                SetChanged($.url().param('ActivityId'));
                SetLASection();
            });
            nrDate.change(function () {
                SetChanged($.url().param('ActivityId'));
            });

            // --------------------------------------------
            // Handle Is At Legislature Logic
            //  - Sets City to Victoria, Sets Venue to Provincial Legislature,
            //    Clear's Other City Values and hides that row
            $('#IsAtLegislatureCheckBox').click(function () {

                SetChanged($.url().param('ActivityId'));

                var cityDropDown = $('#CityDropDownList');
                var venueTextBox = $('#VenueTextBox');
                var otherCityTextBox = $('#OtherCityTextBox');

                if (this.checked) {
                    // Select Victoria (id = 322)
                    cityDropDown.val('322');
                    cityDropDown.multiselect("refresh");

                    // Set the venue
                    venueTextBox.val('Provincial Legislature');
                    $.uniform.update(venueTextBox);

                    // Clear Other City
                    otherCityTextBox.val('');
                    $.uniform.update(otherCityTextBox);
                    // Hide the Other City Row
                    $('#OtherCityRow').hide();

                } else {
                    cityDropDown.attr('selectedIndex', '-1');
                    cityDropDown.multiselect('refresh');
                    venueTextBox.val('');
                    $.uniform.update(venueTextBox);
                }
            });

            // --------------------------------------------
            // Handle Is Internal Logic

            $('#IsInternalCheckBoxNotUsed').click(function () {

                SetChanged($.url().param('ActivityId'));

                if (this.checked) {
                    var sharedWithMinistryAbbreviations = $("#SharedWithSelectedText").html(); // TO DO: change "abbreviations" to "codes"
                    var applicationOwnerOrganizations = $('#ApplicationOwnerOrganizations').val().split(',');

                    if ($('#IsCrossGovernmentCheckBox').is(':checked')) {
                        for (var i = 0; i < applicationOwnerOrganizations.length; i++) {
                            // remove it from the list
                            $('#SharedWithDropDownList').multiselect("widget").find(":checkbox[value='" + applicationOwnerOrganizations[i] + "']").each(function () {
                                this.click();
                            });
                        }
                    }

                    var crossGovernmentCheckBox = $('#IsCrossGovernmentCheckBox').removeAttr('checked');
                    $.uniform.update(crossGovernmentCheckBox);

                    $('#SharedWithDropDownList').multiselect("uncheckAll");
                }
            });

            // --------------------------------------------
            // Handle Is Cross-Government Logic

            $('#IsCrossGovernmentCheckBox').click(function () {
                SetChanged($.url().param('ActivityId'));

                if (this.checked) {
                    // check all shared-with options
                    $('#SharedWithDropDownList').multiselect("checkAll");
                } else {
                    // uncheck all Shared With options
                    $('#SharedWithDropDownList').multiselect("uncheckAll");
                }
            });


            $('#IsConfidentialCheckBox').click(function () {
                var elt = $(this);
                elt.closest("tr").css("color",elt.prop('checked') ? "darkred" : "");
                SetChanged($.url().param('ActivityId'));
                SetLASection();
            });


            $('#IsIssueCheckBox').click(function () {
                var elt = $(this);
                elt.closest("tr").css("font-weight", elt.prop('checked') ? "bold" : "");
                SetChanged($.url().param('ActivityId'));
            });

            $('#IsMilestoneCheckBox').click(function () {
                SetChanged($.url().param('ActivityId'));
            });

            // --------------------------------------------
            // Turn on multiselect for dropdownlists

            // Government representatives
            $('#RepresentativeDropDownList').multiselect({
                multiple: false,
                noneSelectedText: '',
                position: {
                    my: 'left bottom',
                    at: 'left top'
                },
                header: 'Select an option',
                selectedList: 1,
                height: 250,
                click: function (event, ui) {
                    SetChanged($.url().param('ActivityId'));
                }
            });

            var commContactDropDownList = $('#CommContactDropDownList');
            // ----------------------------------------------------------------
            // Contact ministries dropdownlist logic
            $('#ContactMinistryDropDownList').multiselect({
                multiple: false,
                noneSelectedText: '',
                position: {
                    my: 'left bottom',
                    at: 'left top'
                },
                header: 'Select an option',
                selectedList: 1,
                height: 250,
                click: function (event, ui) {
                    $('#ContactMinistryRequiredFieldValidator').hide();
                    commContactDropDownList.empty();
                    $.getJSON("MinistryHandler.ashx?Op=GetMinistryCommunicationContacts", { ministryCode: ui.text, sortOrder: 'RoleThenName' }, function (j) {
                        for (var k = 0; k < j[0].length; k++) { // There are 3 columns returned (from the view) for each row
                            var opt = $('<option />', {
                                value: j[0][k].SystemUserId,
                                text: j[0][k].NameAndNumber
                            });
                            opt.appendTo(commContactDropDownList);
                            commContactDropDownList.multiselect('refresh');
                        }
                    });
                },
                close: function () {
                    UpdateLeadOrgPlaceholder();
                    SetChanged($.url().param('ActivityId'));
                    SetLASection();
                }
            });

            // ------------------------------------------------------------------------
            // Communication contacts dropdownlist logic
            commContactDropDownList.multiselect({
                multiple: false,
                height: 250,
                minWidth: 300,
                noneSelectedText:'',
                header: 'Select an option',
                selectedList: 1,
                click: function (event, ui) {
                    $('#ComContactSelectedValue').val(ui.value);
                    $('#CommunicationContactRequiredFieldValidator').hide();
                    SetChanged($.url().param('ActivityId'));
                }
            });

            // ---------------------------------------------------------------------------------
            // Cities dropdownlist logic
            $('#CityDropDownList').multiselect({
                multiple: false,
                noneSelectedText: '',
                position: {
                    my: 'left bottom',
                    at: 'left top'
                },
                header: '',
                selectedList: 1,
                height: 200,
                create: function (event, ui) {
                    if ($('#CityDropDownList').val() == '311') {
                        $('#OtherCityRow').show();
                    } else {
                        $('#OtherCityRow').hide();
                    }
                },

                click: function (event, ui) {
                    var selectmenu = $("#CityDropDownList");

                    var isAtLegCheckBox = $('#IsAtLegislatureCheckBox');
                    if (isAtLegCheckBox.attr('checked') && ui.text != 'Victoria, BC') {

                        isAtLegCheckBox.removeAttr('checked');
                        $('#VenueTextBox').val('');

                        $.uniform.update(isAtLegCheckBox);
                        $.uniform.update($('#VenueTextBox'));
                    }

                    if (ui.text == 'Other...') {
                        $('#OtherCityRow').show();
                        $('#OtherCityTextBox').focus();

                        $.uniform.update(isAtLegCheckBox);
                        $.uniform.update($('#VenueTextBox'));
                    } else {
                        $('#OtherCityRow').hide();
                        $('#VenueTextBox').focus();
                    }
                    SetChanged($.url().param('ActivityId'));
                }
            }).multiselectfilter();

            // -------------------------------------------------------------------------
            // Shared-with multiselect dropdownlist logic
            var sharedWithTarget = $("#SharedWithSelectedText");
            var sharedWithHiddenTarget = $('#SharedWithSelectedValues');

            var totalMinistries = null;
            var ministriesChecked = null;

            $('#SharedWithDropDownList').multiselect({
                noneSelectedText:'',
                header: true,
                position: {
                    my: 'left bottom',
                    at: 'left top'
                },
                height: 200,

                selectedText: function (numChecked, numTotal, checkedItems) {
                    var crossGovernmentCheckBox;
                    if (numChecked < numTotal) { // this is not a cross-government activity
                        crossGovernmentCheckBox = $('#IsCrossGovernmentCheckBox').removeAttr('checked');
                        $.uniform.update(crossGovernmentCheckBox);
                    }
                    if (numChecked == numTotal) { // this is a cross-government activity
                        // Check the Is Cross-Government checkbox
                        crossGovernmentCheckBox = $('#IsCrossGovernmentCheckBox').attr('checked', true);
                        $.uniform.update(crossGovernmentCheckBox);
                        // Uncheck the Is Internal checkbox
                        var isInternalCheckBox = $('#IsInternalCheckBoxNotUsed').attr('checked', false);
                        $.uniform.update(isInternalCheckBox);
                    }

                    return numChecked + ' of ' + numTotal + ' ministries selected';
                },

                checkAll: function (e) {
                    var crossGovernmentCheckBox = $('#IsCrossGovernmentCheckBox').attr('checked', true);
                    $.uniform.update(crossGovernmentCheckBox);
                    var internalGovernmentCheckBox = $('#IsInternalCheckBoxNotUsed').removeAttr('checked');
                    $.uniform.update(internalGovernmentCheckBox);
                    $(this).multiselect("close");
                },

                uncheckAll: function (e) {
                    var crossGovernmentCheckBox = $('#IsCrossGovernmentCheckBox').removeAttr('checked');
                    $.uniform.update(crossGovernmentCheckBox);
                    $(this).multiselect("close");
                },

                close: function () {
                    if (sharedWithHiddenTarget.val() != $('#SharedWithSelectedValuesServerSide').val()) {
                        SetChanged($.url().param('ActivityId'));
                    }
                }
            }).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {

                // the getChecked method returns an array of DOM elements.
                // map over them to create a new array of just the values.
                // you could also do this by maintaining your own array of
                // checked/unchecked values, but this is just as easy.
                var checkedValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                });

                var checkedTitles = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.title;
                });

                // update the target based on how many are checked
                sharedWithTarget.html(checkedTitles.length ? checkedTitles.join(', ') : '');

                // Show / hide the target row
                if (checkedTitles.length > 0) {
                    $("#SharedWithSelectedTextRow").show();
                } else {
                    $("#SharedWithSelectedTextRow").hide();
                }

                sharedWithHiddenTarget.val(checkedValues.length ? checkedValues.join(',') : '');
            });

            // --------------------------------------------------------------------------------------
            // Category

            $('#IsIssueCheckBox').click(function () {
                UpdateCategoriesFYILabel();
                SetLASection();
            });

            $('#CategoriesDropDownList').multiselect({
                multiple: false,
                height: 350,
                minWidth: 250,
                noneSelectedText:'',
                selectedList: 1,
                header: 'Select an option',
                position: {
                    //my: 'center',
                    //at: 'center'

                    // only include the "of" property if you want to position
                    // the menu against an element other than the button.
                    // multiselect automatically sets "of" unless you explicitly
                    // pass in a value.
                },
                click: function (event, ui) {
                    $('#CategoriesDropDownListRequiredFieldValidator').hide();
                    hideIrrelevantPanels(ui.text);

                    /*if ($(this).multiselect("widget").find("input:checked").length > 1) {
                        jAlert("Please select a single category.", "Validation Error");
                        categoriesTarget.html(categoriesTarget.html().replace(ui.text, ''));
                        categoriesTarget.html(categoriesTarget.html().replace(/(^\s*,)|(,\s*$)/g, ''));
                        categoriesHiddenTarget.val(categoriesHiddenTarget.val().replace(ui.value, ''));
                        categoriesHiddenTarget.val(categoriesHiddenTarget.val().replace(/(^\s*,)|(,\s*$)/g, ''));
                        return false;
                    }*/
                    return true;
                },
                close: function () {
                    var selectedCategory = GetDropDownSelection('#CategoriesDropDownList');
                    var allDayCategory = selectedCategory == "Awareness Day / Week / Month" || selectedCategory == "Conference / AGM / Forum";
                    if (allDayCategory !== $("#IsAllDayCheckBox").is(':checked')) {
                        $("#IsAllDayCheckBox").click();
                    }

                    SetChanged($.url().param('ActivityId'));
                    SetLASection();
                }
            });

            // -------------------------------------------------------------------------------------
            // COMM MATERIALS

            var commMaterialsTarget = $('#CommMaterialsSelectedText');
            var commMaterialsHiddenTarget = $('#CommMaterialsSelectedValues');
            $('#CommMaterialsDropDownList').multiselect({
                height: 250,
                noneSelectedText:'',
                header: true,
                position: {
                    //my: 'center',
                    //at: 'center'

                    // only include the "of" property if you want to position
                    // the menu against an element other than the button.
                    // multiselect automatically sets "of" unless you explicitly
                    // pass in a value.
                },
                click: function (event, ui) {
                    $('#IsCommunicationMaterialsDirty').val('true');
                },
                checkAll: function (e) {
                    $('#IsCommunicationMaterialsDirty').val('true');
                    $(this).multiselect("close");
                },
                uncheckAll: function (e) {
                    $(this).multiselect("close");
                },
                selectedText: function (numChecked, numTotal, checkedItems) {
                    return numChecked + ' of ' + numTotal + ' comm. materials selected';
                },
                close: function () {
                    if (commMaterialsHiddenTarget.val() != $('#CommMaterialsSelectedValuesServerSide').val()) {
                        SetLASection();
                        SetChanged($.url().param('ActivityId'));
                    }
                }
            }).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {

                SetMultiSelectedValues($(this), commMaterialsTarget, commMaterialsHiddenTarget, null, $("#CommMaterialsSelectedTextRow"), $('#IsCommunicationMaterialsDirty').val());
            });

            // ------------------------------------------------------------------
            // NR Origin multiselect logic

            var nrOriginTarget = $("#NROriginsSelectedText");

            $('#NROriginDropDownList').multiselect({
                multiple: false,
                noneSelectedText:'',
                header: 'Select an option',
                selectedList: 1,
                click: function (event, ui) {
                    $('#IsNROriginsDirty').val('true');
                    return true;
                },
                close: function () {
                    SetLASection();
                    SetChanged($.url().param('ActivityId'));
                }
            });


            // -----------------------------------------------------------------------------------
            // NR distribution logic

            $('#NRDistributionDropDownList').multiselect({
                multiple: false,
                noneSelectedText:'',
                header: 'Select an option',
                selectedList: 1,
                click: function (event, ui) {
                    $('#IsNRDistributionsDirty').val('true');
                    SetChanged($.url().param('ActivityId'));
                }
            });


            // ----------------------------------------------------------------------
            // Sector multiselect logic

            var sectorsTarget = $("#SectorsSelectedText");
            var sectorsHiddenTarget = $('#SectorsSelectedValues');

            $('#SectorDropDownList').multiselect({
                noneSelectedText:'',
                header: true,
                checkAll: function () {
                    $('#IsSectorsDirty').val('true');
                    $("#callback").removeAttr('checked');
                    $(this).multiselect("close");
                },
                click: function (event, ui) {
                    $('#IsSectorsDirty').val('true');
                },
                uncheckAll: function (e) {
                    $(this).multiselect("close");
                },
                selectedText: function (numChecked, numTotal, checkedItems) {
                    return numChecked + ' of ' + numTotal + ' sectors selected';
                },
                position: {
                    //my: 'center',
                    //at: 'center'

                    // only include the "of" property if you want to position
                    // the menu against an element other than the button.
                    // multiselect automatically sets "of" unless you explicitly
                    // pass in a value.
                },
                close: function () {
                    if ($('#SectorsSelectedValuesServerSide').val() != $('#SectorsSelectedValues').val()) {
                        SetChanged($.url().param('ActivityId'));
                    }
                }
            }).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {

                SetMultiSelectedValues($(this), sectorsTarget, sectorsHiddenTarget, null, $("#SectorsSelectedTextRow"), $('#IsSectorsDirty').val());
            });


            // Theme multiselect logic

            var themesTarget = $("#ThemesSelectedText");
            var themesHiddenTarget = $('#ThemesSelectedValues');


            $('#ThemeDropDownList').multiselect({
                noneSelectedText:'',
                header: true,
                checkAll: function () {
                    $('#IsThemesDirty').val('true');
                    $("#callback").removeAttr('checked');
                    $(this).multiselect("close");
                },
                click: function (event, ui) {
                    $('#IsThemesDirty').val('true');
                },
                uncheckAll: function (e) {
                    $(this).multiselect("close");
                },
                selectedText: function (numChecked, numTotal, checkedItems) {
                    return numChecked + ' of ' + numTotal + ' themes selected';
                },
                position: {
                    //my: 'center',
                    //at: 'center'

                    // only include the "of" property if you want to position
                    // the menu against an element other than the button.
                    // multiselect automatically sets "of" unless you explicitly
                    // pass in a value.
                },
                close: function () {
                    if ($('#ThemesSelectedValuesServerSide').val() != $('#ThemesSelectedValues').val()) {
                        SetChanged($.url().param('ActivityId'));
                    }
                }
            }).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {

                SetMultiSelectedValues($(this), themesTarget, themesHiddenTarget, null, $("#ThemesSelectedTextRow"), $('#IsThemesDirty').val());
            });

            // Initiative multiselect logic

            var initiativesTarget = $("#InitiativesSelectedText");
            var initiativesHiddenTarget = $('#InitiativesSelectedValues');

            $('#InitiativeDropDownList').multiselect({
                noneSelectedText: '',
                header: true,
                checkAll: function () {
                    $('#IsInitiativesDirty').val('true');
                    $("#callback").removeAttr('checked');
                    $(this).multiselect("close");
                },
                click: function (event, ui) {
                    $('#IsInitiativesDirty').val('true');
                },
                uncheckAll: function (e) {
                    $(this).multiselect("close");
                },
                selectedText: function (numChecked, numTotal, checkedItems) {
                    return numChecked + ' of ' + numTotal + ' initiatives selected';
                },
                position: {
                    //my: 'center',
                    //at: 'center'

                    // only include the "of" property if you want to position
                    // the menu against an element other than the button.
                    // multiselect automatically sets "of" unless you explicitly
                    // pass in a value.
                },
                close: function () {
                    if ($('#InitiativesSelectedValuesServerSide').val() != $('#InitiativesSelectedValues').val()) {
                        SetChanged($.url().param('ActivityId'));
                    }
                }
            }).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {

                SetMultiSelectedValues($(this), initiativesTarget, initiativesHiddenTarget, null, $("#InitiativesSelectedTextRow"), $('#IsInitiativesDirty').val());
            });

            // ---------------------------------------------------------------
            // Premier requested
            $('#PremierRequestedDropDownList').multiselect({
                multiple: false,
                noneSelectedText:'',
                header: 'Select an option',
                selectedList: 1,
                click: function () {
                    SetChanged($.url().param('ActivityId'));
                }
            });

            // ----------------------------------------------------------------
            // Event Planner
            $('#EventPlannerDropDownList').multiselect({
                multiple: false,
                position: {
                    my: 'left bottom',
                    at: 'left top'
                },
                noneSelectedText:'',
                header: 'Select an option',
                selectedList: 1,
                click: function () {
                    SetChanged($.url().param('ActivityId'));
                }
            });

            // -----------------------------------------------------------------
            // Videographer
            $('#VideographerDropDownList').multiselect({
                multiple: false,
                position: {
                    my: 'left bottom',
                    at: 'left top'
                },
                noneSelectedText:'',
                header: 'Select an option',
                selectedList: 1,
                click: function () {
                    SetChanged($.url().param('ActivityId'));
                }
            });

            $('.ui-multiselect').css('width', '95%');

            //--------------------------------------------------------------------
            //setup the label for the "Date/Dates/Time Confirmed label
            SetConfirmedLabel();
        });
    </script>

    <script type="text/javascript">

        //window.onunload = CheckInActivity;
        window.onbeforeunload = ConfirmUnsavedChanges;

        window.onunload = CancelEditing;
        function CancelEditing() {
            var id = $.url().param('ActivityId') || '';
            DoCancel(id);
        }


        function CalculateMaxEndDate(numDays) {
            var startDate = new Date($('#StartDate').val());
            return startDate.add(numDays).days();
        }

        function GetDropDownSelection(dropdownId) {
            var selection = $(dropdownId).children().filter(function (i, option) {
                return option.selected;
            });
            return selection.length ? selection[0].text : null;
        }

        function ConfirmUnsavedChanges() {
            //CheckInActivity();
            if (GetChanged() && warn_on_unload != "") {
                return warn_on_unload;
            }
        }

        function ShowHidePotentialDates() {

            // disable toggling for new activities
            function getParameterByName(name, url) {
                if (!url) url = window.location.href;
                name = name.replace(/[\[\]]/g, '\\$&');
                var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
                    results = regex.exec(url);
                if (!results) return null;
                if (!results[2]) return '';
                return decodeURIComponent(results[2].replace(/\+/g, ' '));
            }
            var activityIdQueryParam = getParameterByName('ActivityId');
            var newQueryParam = getParameterByName('new');
            var isClonedQueryParam = getParameterByName('IsCloned');
            if (activityIdQueryParam !== null || activityIdQueryParam === null || newQueryParam == 1 || isClonedQueryParam == "true") return;


            if ($("#IsDateConfirmed")[0].checked) {
                $('#PotentialDatesLabel').hide();
                $('#PotentialDatesTextBox').hide();
            } else {
                $('#PotentialDatesLabel').show();
                $('#PotentialDatesTextBox').show();
            }
        }

        function UpdateCategoriesFYILabel() {
            var categoriesDropDownList = $('#CategoriesDropDownList');
            var categoriesMultiSelectMenu = categoriesDropDownList.parent().find(".ui-multiselect-menu");
            var fyiLabel = "FYI Only";

            if ($('#IsIssueCheckBox').is(":checked")) {
                fyiLabel = "FYI (Issue)";
            }

            categoriesDropDownList.find("option[value='9']").html(fyiLabel);
            categoriesMultiSelectMenu.find("input[value='9']").next("span").html(fyiLabel);
            categoriesMultiSelectMenu.find("input[value='9']").attr("title", fyiLabel);
            if (categoriesMultiSelectMenu.find("input[value='9']").is(":checked")) {
                $("#CategoriesSelectedText").html(fyiLabel);
            }
        }

        function UpdateOriginLabels() {
            var originDropDownList = $('#NROriginDropDownList');
            var originMultiSelectMenu = originDropDownList.parent().find(".ui-multiselect-menu");
            var thirdParty = "3rd party";
            var ministry = "Ministry";
            var thirdPartyLabel = "3rd party release";
            var ministryLabel = "Ministry release";
            var jointProvFedLabel = "Joint Provincial / Federal release";
            var jointProvThirdPartyLabel = "Joint Provincial / 3rd party release";

            if ($('#LeadOrganizationTextBox').val() != "") {
                thirdParty = $('#LeadOrganizationTextBox').val();
                //if (thirdParty.length > 10) {
                //    thirdParty = thirdParty.substring(0, 10) + "...";
                //}
                thirdPartyLabel = thirdParty + " release";
                jointProvThirdPartyLabel = "Joint Provincial /" + thirdParty + " release";
            }

            if ($('#ContactMinistryDropDownList').val() != "") {
                ministry = $('#ContactMinistryDropDownList').find("option:selected").text();
                ministryLabel = "Ministry (" + ministry + ") release";
                jointProvFedLabel = "Joint Ministry (" + ministry + ") / Federal release";
                jointProvThirdPartyLabel = "Joint Ministry (" + ministry + ") / " + thirdParty + " release";
            }

            originDropDownList.find("option[value='7']").html(thirdPartyLabel);
            originMultiSelectMenu.find("input[value='7']").next("span").html(thirdPartyLabel);
            originMultiSelectMenu.find("input[value='7']").attr("title", thirdPartyLabel);
            if (originMultiSelectMenu.find("input[value='7']").is(":checked")) {
                $("#NROriginsSelectedText").html(thirdPartyLabel);
            }

            originDropDownList.find("option[value='3']").html(ministryLabel);
            originMultiSelectMenu.find("input[value='3']").next("span").html(ministryLabel);
            originMultiSelectMenu.find("input[value='3']").attr("title", ministryLabel);
            if (originMultiSelectMenu.find("input[value='3']").is(":checked")) {
                $("#NROriginsSelectedText").html(ministryLabel);
            }

            originDropDownList.find("option[value='5']").html(jointProvFedLabel);
            originMultiSelectMenu.find("input[value='5']").next("span").html(jointProvFedLabel);
            originMultiSelectMenu.find("input[value='5']").attr("title", jointProvFedLabel);
            if (originMultiSelectMenu.find("input[value='5']").is(":checked")) {
                $("#NROriginsSelectedText").html(jointProvFedLabel);
            }

            originDropDownList.find("option[value='28']").html(jointProvThirdPartyLabel);
            originMultiSelectMenu.find("input[value='28']").next("span").html(jointProvThirdPartyLabel);
            originMultiSelectMenu.find("input[value='28']").attr("title", jointProvThirdPartyLabel);
            if (originMultiSelectMenu.find("input[value='28']").is(":checked")) {
                $("#NROriginsSelectedText").html(jointProvThirdPartyLabel);
            }
        }
        ShowHidePotentialDates();
        UpdateCategoriesFYILabel();
        UpdateOriginLabels();

        function EvaluateLAComments(validator, args) {
            args.IsValid = !$('#IsConfidentialCheckBox').prop('checked') || !laSectionInOverride || ValidatorTrim($("#LACommentsTextBox")[0].value).length != 0;
        }

        function DisplayOverride(override) {
            $("#LASectionCheckBoxList").closest("td").siblings(".column-left").css("background-color", override ? "gold" : "");
        }

        function SetLASection() {

            var newCheckedSection = InferLASection();
            if (newCheckedSection && !laSectionInOverride) {
                var oldCheckedSection = $("#LASectionCheckBoxList input:not(:last):checked");
                oldCheckedSection.prop("checked", "");
                newCheckedSection.prop("checked", true);
            }
        }

        function InferLASection() {
            var selectedCategory = GetDropDownSelection('#CategoriesDropDownList');

            var automaticSection = null;

            if (selectedCategory == "Awareness Day / Week / Month") {
                automaticSection = "Awareness Dates";
            } else if (GetDropDownSelection('#ContactMinistryDropDownList') == "CITENG") {
                automaticSection = "Consultations and Dialogues";
            }

            $("#LookAheadSectionRow").css("color", automaticSection ? "lightgrey" : '');
            $("#LASectionCheckBoxList td:last label").text(automaticSection ? automaticSection : "Long Term Outlook");
            DisplayOverride(!automaticSection && laSectionInOverride);
            if (automaticSection)
            {   // No HQ override possible
                $("#LASectionCheckBoxList td:not(:last)").hide();
                return null;
            }

            $("#LASectionCheckBoxList td").show();

            var laPos = 4; // Not For Look Ahead
            if (!$('#IsConfidentialCheckBox').prop('checked')) {
                var isConfirmed = $("#IsDateConfirmed")[0].checked;

                var shouldToggleLA = selectedCategory != "Approved Event or Activity"
                    && selectedCategory != "Approved Release"
                    && selectedCategory != "Proposed Event or Activity"
                    && selectedCategory != "Proposed Release"
                    && selectedCategory != "Speech / Remarks";

                if ($('#IsIssueCheckBox').prop('checked') && shouldToggleLA) {
                    laPos = 1;
                }
                else if (!isConfirmed && $('#CommMaterialsSelectedValues').val().indexOf("61") != -1) {
                    laPos = 1;
                }
                else if (isConfirmed || new Date($('#EndDate').val()) < CalculateMaxEndDate(2)) {
                    laPos = 3;
                    switch (selectedCategory) {
                        case "Approved Release":
                        case "Proposed Release":
                        case "Approved Event or Activity":
                        case "Proposed Event or Activity":
                        case "Speech":
                        case "Speech / Remarks":
                        case "HQ Placeholder":
                            laPos = 2;
                    }
                }
            }
            return $("#LASectionCheckBoxList [value='" + laPos + "']");
        }

        $("[id^='LASectionCheckBoxList_']").click(function () {
            if ($("#LASectionCheckBoxList td:visible").length > 1) {
                // not an automatic section
                var checkedSections = $("#LASectionCheckBoxList input:not(:last):checked");
                if (checkedSections.length > 1) {
                    checkedSections.prop("checked", "");
                    $(this).prop("checked", "checked");
                }
                else if (checkedSections.length == 0) {
                    $("#LASectionCheckBoxList [value='4']").prop("checked", "checked");
                }
                var newCheckedSection = InferLASection();
                laSectionInOverride = newCheckedSection && !newCheckedSection.prop('checked');
                DisplayOverride(laSectionInOverride);
                ValidatorValidate(LACommentsCustomValidator);
                SetChanged($.url().param('ActivityId'));
            }
        });

        var isExistingActivity = $("#ActiveID").length != 0;
        var sectionCheckBox = isExistingActivity ? InferLASection() : null;
        var laSectionInOverride = sectionCheckBox && !sectionCheckBox.prop('checked');
        DisplayOverride(laSectionInOverride);

    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            // disable start and end time if IsAllDay is checked onload
            if ($('#IsAllDayCheckBox').is(':checked')) {
                $('#StartTime').hide();
                $('#EndTime').hide();
                $("#StartTimePSTLabel").hide();
                $("#EndTimePSTLabel").hide();
            }
            // if (isPostBack) {

            var arrayValues;

            // Select this activity's shared with ministries

            arrayValues = $('#SharedWithSelectedValuesServerSide').val().split(',');
            MultiSelectReset($('#SharedWithDropDownList'));
            for (var i = 0; i < arrayValues.length; i++) {
                $('#SharedWithDropDownList').multiselect("widget").find(":checkbox[value='" + arrayValues[i] + "']").each(function () {
                    this.click();
                });
            }

            // Select this activity's communication materials

            arrayValues = $('#CommMaterialsSelectedValuesServerSide').val().split(',');
            MultiSelectReset($('#CommMaterialsDropDownList'));
            for (var i = 0; i < arrayValues.length; i++) {
                $('#CommMaterialsDropDownList').multiselect("widget").find(":checkbox[value='" + arrayValues[i] + "']").each(function () {
                    this.click();
                });
            }

            // Select this activity's sectors

            arrayValues = $('#SectorsSelectedValuesServerSide').val().split(',');
            MultiSelectReset($('#SectorDropDownList'));
            for (var i = 0; i < arrayValues.length; i++) {
                $('#SectorDropDownList').multiselect("widget").find(":checkbox[value='" + arrayValues[i] + "']").each(function () {
                    this.click();
                });
            }

            // Select this activity's themes

            arrayValues = $('#ThemesSelectedValuesServerSide').val().split(',');
            MultiSelectReset($('#ThemeDropDownList'));
            for (var i = 0; i < arrayValues.length; i++) {
                $('#ThemeDropDownList').multiselect("widget").find(":checkbox[value='" + arrayValues[i] + "']").each(function () {
                    this.click();
                });
            }

            // Select this activity's initiatives

            arrayValues = $('#InitiativesSelectedValuesServerSide').val().split(',');
            MultiSelectReset($('#InitiativeDropDownList'));
            for (var i = 0; i < arrayValues.length; i++) {
                $('#InitiativeDropDownList').multiselect("widget").find(":checkbox[value='" + arrayValues[i] + "']").each(function () {
                    this.click();
                });
            }

            $('#TitleTextBox').keypress(function (event) {
                if (event.keyCode == 10 || event.keyCode == 13)
                    event.preventDefault();
            });

            $('#TitleTextBox').keyup(function () {
                // var text = $('#TitleTextBox').val();
                //$('#TitleTextBox').val(text.replace(/(\r\n|\n|\r)/gm, ""));
                limitChars('TitleTextBox', 100, 'charlimitinfo');
                SetChanged($.url().param('ActivityId'));
            });

            $('#DetailsTextBox').keyup(function () {
                limitChars('DetailsTextBox', 700, 'charlimitinfo1');
                SetChanged($.url().param('ActivityId'));
            });

            var initialText = $("#LACommentsTextBox").val();

            $('#LACommentsTextBox').on('input', function () {
                limitChars('LACommentsTextBox', 2000, 'charlimitinfo2');
                var laStatusRadioButtonList = $('#LAStatusRadioButtonList');
                if (laStatusRadioButtonList.find("input[value='']").prop('checked') &&
                    initialText == "**" &&
                    $("#LACommentsTextBox").val().length > 0) {
                    laStatusRadioButtonList.find("input[value='7']").prop('checked', true).trigger("click");
                }
                SetChanged($.url().param('ActivityId'));
            });

            $('#CommentsTextBox').keyup(function () {
                limitChars('CommentsTextBox', 4000, 'charlimitinfo3');
                SetChanged($.url().param('ActivityId'));
            });

            $('#VenueTextBox').keyup(function () {
                limitChars('VenueTextBox', 55, 'charlimitinfo4');
                SetChanged($.url().param('ActivityId'));
            });

            $('#OtherCityTextBox').keyup(function () {
                limitChars('OtherCityTextBox', 55, 'charlimitinfo5');
                SetChanged($.url().param('ActivityId'));
            });

            $('#LeadOrganizationTextBox').keyup(function () {
                limitChars('LeadOrganizationTextBox', 80, 'charlimitinfo6');
                SetChanged($.url().param('ActivityId'));
            });

            var newPrefix = " (new ";
            var onSelectCustom = function (e) {
                if (e.item[0].textContent.indexOf(newPrefix) === -1) {
                    var dataItems = e.sender.dataSource.data();
                    if (dataItems[dataItems.length - 1].indexOf(newPrefix) !== -1) {
                        // remove the the new item in the popup so that the change function (below) does not select it when called
                        dataItems.splice(dataItems.length - 1, 1);
                    }
                }
                return false; // do not prevent the selection
            };
            var newItemText;
            var onFilteringCustom = function (e) { // user has typed a character, let see if we should display the new item menu
                var _prev = e.sender._prev;
                if (_prev && newItemText !== _prev) {
                    newItemText = _prev;
                    var newItemTextLower = newItemText.toLowerCase();
                    var dataItems = e.sender.dataSource.data();

                    var elementId = $(e.sender.element).prop("id");
                    var newItemPrefix = newPrefix + (elementId === "KeywordsTextBox"  ? "tag)" : "language)");
                    var addNewItemMenuItem = newItemText.length >= 3;
                    for (var i = 0; i < dataItems.length; i++) {
                        var dataItem = dataItems[i];
                        if (newItemTextLower === dataItem.toLowerCase()) {
                            addNewItemMenuItem = false; // exact match
                        } else if (dataItem.indexOf(newItemPrefix) !== -1) {
                            if (!addNewItemMenuItem) {
                                dataItems.splice(i, 1); // remove the new item in the popup
                            } else {
                                // replace the existing new item in the popup
                                addNewItemMenuItem = false;
                                dataItems[i] = newItemText + newItemPrefix;
                            }
                            break;
                        }
                    }
                    if (addNewItemMenuItem) {
                        dataItems.push(newItemText + newItemPrefix);
                    }
                }
            };

            var onChangeCustom = function (e) {
                var ctrl = e.sender;
                var dataItems = ctrl.dataSource.data();
                if (dataItems.length === 0) {
                    return;
                }
                var itemToAddToDb = "";

                var lastDataItemIdx = dataItems.length - 1;
                var lastDataItem = dataItems[lastDataItemIdx];

                let posPrefix = lastDataItem.indexOf(newPrefix);
                if (posPrefix !== -1) {
                    // new item selected: update the new item in the popup
                    itemToAddToDb = lastDataItem.substr(0, posPrefix);
                    dataItems[lastDataItemIdx] = itemToAddToDb;
                }
                var value = ctrl.value().slice(0); // make a copy if the array
                if (itemToAddToDb) {
                    ctrl.dataSource.filter({});
                    value[value.length - 1] = itemToAddToDb;
                    ctrl.value(value);
                }
                ctrl.element[0].value = value.join('~'); // for the post back to the server
            };


            var translationsTextbox = $('#TranslationsTextbox');
            translationsTextbox.kendoMultiSelect({
                dataSource: $('#TranslationsRequired option').map(function( i, elem ) {
                    return elem.text;
                }).get(),
                filter: "startswith",
                maxSelectedItems: 15,
                value: translationsTextbox[0].value.split('~'),
                filtering: onFilteringCustom,
                select: onSelectCustom,
                change: onChangeCustom
            });

            var keywordsTextBox = $('#KeywordsTextBox');
            keywordsTextBox.kendoMultiSelect({
                dataSource: $('#KeywordList option').map(function( i, elem ) {
                    return elem.text;
                }).get(),
                filter: "startswith",
                maxSelectedItems: 10,
                value: keywordsTextBox[0].value.split("~"),
                filtering: onFilteringCustom,
                select: onSelectCustom,
                change: onChangeCustom
            });

            var keywordsMultiSelect = keywordsTextBox.data("kendoMultiSelect");

            // assume that an empty activity title on page load means that we're creating a new activity
            var activityTitle = $("#TitleTextBox").val();
            if(!activityTitle) {
                // search the pre-populated list of tags for the 30-60-90 tag
                var defaultTag = $.map(keywordsMultiSelect.dataSource.data(), function(dataItem) {
                if(dataItem === "30-60-90")
                    return dataItem;
                });
                // set the tag as a selected value and trigger a change event to persist the selection
                keywordsMultiSelect.value([defaultTag]);
                keywordsMultiSelect.trigger("change");
            }

            keywordsTextBox.keyup(function () {
                SetChanged($.url().param('ActivityId'));
            });

            $('#SignificanceTextBox').keyup(function () {
                limitChars('SignificanceTextBox', 500, 'charlimitinfo7');
                SetChanged($.url().param('ActivityId'));
            });

            $('#StrategyTextBox').keyup(function () {
                limitChars('StrategyTextBox', 500, 'charlimitinfo9');
                SetChanged($.url().param('ActivityId'));
            });

            $('#SchedulingTextBox').keyup(function () {
                limitChars('SchedulingTextBox', 500, 'charlimitinfo8');
                SetChanged($.url().param('ActivityId'));
            });

            $('#PotentialDatesTextBox').keyup(function () {
                SetChanged($.url().param('ActivityId'));
            });

            var laStatusRadioButtonList = $('#LAStatusRadioButtonList');
            SetLAStatusColor(laStatusRadioButtonList.find("input:checked")[0]);

            laStatusRadioButtonList.click(function (evt) {
                laStatusRadioButtonList.find("input:not(checked)").parent().css("background", "");
                SetLAStatusColor(evt.target);
                SetChanged($.url().param('ActivityId'));
            });

            $('#LeadOrganizationTextBox').blur(function () {
                UpdateOriginLabels();
            });
            UpdateLeadOrgPlaceholder();
        });

        function SetLAStatusColor(elt) {
            if (elt && elt.value) {
                elt.parentNode.style.background = "orange";
            }
        }

        function UpdateLeadOrgPlaceholder() {
            var ministry = ministries[$('#ContactMinistryDropDownList').val()];
            if (ministry == undefined || ministry == "") {
                ministry = "Province of BC";
            }
            // Set LeadOrganizationTextBox placeholder to Long Name of ministry that was just selected
            $('#LeadOrganizationTextBox').attr("placeholder", ministry);
        }
    </script>


    <script type="text/javascript">

        // Show characters remaining
        limitChars('TitleTextBox', 100, 'charlimitinfo');
        limitChars('DetailsTextBox', 700, 'charlimitinfo1');
        limitChars('LACommentsTextBox', 2000, 'charlimitinfo2');
        limitChars('CommentsTextBox', 4000, 'charlimitinfo3');
        limitChars('VenueTextBox', 55, 'charlimitinfo4');
        limitChars('OtherCityTextBox', 55, 'charlimitinfo5');
        limitChars('LeadOrganizationTextBox', 80, 'charlimitinfo6');
        limitChars('SignificanceTextBox', 500, 'charlimitinfo7');
        limitChars('SchedulingTextBox', 500, 'charlimitinfo8');
        limitChars('StrategyTextBox', 500, 'charlimitinfo9');


    </script>
</asp:Content>
