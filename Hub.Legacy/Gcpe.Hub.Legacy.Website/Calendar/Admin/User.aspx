<%@ Page Title="" Language="C#" MasterPageFile="~/Calendar/Admin/Site.master" AutoEventWireup="true"
    CodeBehind="User.aspx.cs" Inherits="CorporateCalendarAdmin.User" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .marker { width: 27px; vertical-align:top; color: red }
        .field { width:225px;vertical-align:top; text-wrap: none }
        .widefield { width: auto }
    </style>

	<link href="<%= ResolveClientUrl("~/Calendar/Admin/Scripts/css/custom-theme/jquery-ui-1.9.2.custom.css") %>" rel="stylesheet" />

	<script src="<%= ResolveClientUrl("~/Calendar/Admin/Scripts/jquery-1.8.3.js") %>" type="text/javascript"></script>
	<script src="<%= ResolveClientUrl("~/Calendar/Admin/Scripts/jquery-ui-1.9.2.custom.js") %>" type="text/javascript"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <asp:Label ID="ErrorLabel" runat="server" Style="color: red; font: .8em Tahoma, Arial, Sans-Serif;"></asp:Label>

    <fieldset id="GeneralFieldSet">
        <span style="font: 0.8em Tahoma, Arial, Sans-Serif;">General Information</span>
        <asp:Button ID="VerifyButton" runat="server"
            Text="Check Name" OnClick="VerifyButton_Click" />



        <table style="width: 100%; color: #666; border: solid 1px #dbddff; font: .7em Tahoma, Arial, Sans-Serif;">
            <tr>
                <td class="marker">*</td>
                <td class="field">Account Name:</td>
                <td class="widefield">
                    <asp:TextBox ID="AccountNameTextBox" CssClass="DDTextBox" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="marker">*</td>
                <td class="field">First Name:</td>
                <td class="widefield">
                    <asp:TextBox ID="FirstNameTextBox" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="marker">*</td>
                <td class="field">Last Name:</td>
                <td class="widefield">
                    <asp:TextBox ID="LastNameTextBox" runat="server"></asp:TextBox>
                    <asp:TextBox ID="DisplayNameTextBox" runat="server" style="display:none"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="marker"></td>
                <td class="field">Phone:</td>
                <td class="widefield">
                    <asp:TextBox ID="PhoneTextBox" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="marker"></td>
                <td class="field">Mobile:</td>
                <td class="widefield">
                    <asp:TextBox ID="MobileTextBox" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="marker"></td>
                <td class="field">Email:</td>
                <td class="widefield">
                    <asp:TextBox ID="EmailTextBox" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="marker"></td>
                <td class="field">Job Title:</td>
                <td class="widefield">
                    <asp:TextBox ID="JobTitleTextBox" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset id="AdvancedFieldset">
        <span style="font: 0.8em Tahoma, Arial, Sans-Serif;">Advanced Information</span>
        <table style="width: 100%; color: #666; border: solid 1px #dbddff; font: .7em Tahoma, Arial, Sans-Serif;">
            <tr>
                <td class="marker">*</td>
                <td class="field">
                    <div>Type of Communication Contact?</div>
                    <asp:RadioButtonList ID="CommContactTypeSortRadioButtonList" runat="server" RepeatDirection="Vertical">
                        <asp:ListItem ID="ListItem1" runat="server" Text="Is not a Communication Contact" Value="0"></asp:ListItem>
                        <asp:ListItem ID="ListItem2" runat="server" Text="Comm. Director" Value="1"></asp:ListItem>
                        <asp:ListItem ID="ListItem3" runat="server" Text="Comm. Manager" Value="2"></asp:ListItem>
                        <asp:ListItem ID="ListItem4" runat="server" Text="Sr. PAO" Value="3"></asp:ListItem>
                        <asp:ListItem ID="ListItem5" runat="server" Text="PAO" Value="4"></asp:ListItem>
                        <asp:ListItem ID="ListItem6" runat="server" Text="Jr. PAO" Value="5"></asp:ListItem>
                        <asp:ListItem ID="ListItem7" runat="server" Text="Other" Value="6"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td class="field">
                    Ministry Membership(s):
                    <div class="instructions">Select at least one.</div> 
                    <asp:ListBox ID="ContactMinistryListBox" SelectionMode="Multiple" runat="server" Style="width: 200px; height: 200px; color: #666;"></asp:ListBox>
                </td>
                <td class="field">
                    <div class="instructions">Comments/Notes:<span id="count" style="font-weight: bold; display: inline-block; margin-left: 265px;"></span></div>
                    <asp:TextBox Width="500" Height="200" TextMode="MultiLine" ID="DescriptionTextBox" style="color: #666; font: 13px Tahoma, Arial, Sans-Serif;" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset id="AccessControlFieldset">
        <span style="font: 0.8em Tahoma, Arial, Sans-Serif;">Access Control</span>
        <table style="width: 100%; color: #666; border: solid 1px #dbddff; font: .7em Tahoma, Arial, Sans-Serif;">
            <tr>
                <td class="marker">*</td>
                <td class="widefield">
                    Role:
                    <asp:RadioButtonList ID="RoleRadioButtonList" runat="server" RepeatDirection="Vertical">
                        <asp:ListItem ID="ReadOnlyRadioButtonListItem" runat="server" Text="Read Only" Value="1"></asp:ListItem>
                        <asp:ListItem ID="EditorRadioButtonListItem" runat="server" Text="Editor" Value="2"></asp:ListItem>
                        <asp:ListItem ID="AdvancedRadioButtonListItem" runat="server" Text="Advanced" Value="3"></asp:ListItem>
                        <asp:ListItem ID="AdministratorRadioButtonListItem" runat="server" Text="Administrator" Value="4"></asp:ListItem>
                        <asp:ListItem ID="SystemAdministratorRadioButtonListItem" runat="server" Text="System Administrator" Value="5"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset id="ActivtyFieldset">
        <span style="font: 0.8em Tahoma, Arial, Sans-Serif;">Status</span>
        <table style="width: 100%; color: #666; border: solid 1px #dbddff; font: .7em Tahoma, Arial, Sans-Serif;">
            <tr>
                <td class="marker">*</td>
                <td class="widefield">
                    <div>Is Active?</div>
                    <asp:RadioButtonList ID="IsActiveRadioButtonList" ClientIDMode="Static" runat="server" RepeatDirection="Horizontal" onclick="CheckForActivitiesOnInactive(this)">
                        <asp:ListItem ID="YesIsActiveRadioButtonListItem" runat="server" Text="Yes" Value="true"></asp:ListItem>
                        <asp:ListItem ID="NoIsActiveRadioButtonListItem" runat="server" Text="No" Value="false"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset id="ActionsFieldset">
        <span style="font: 0.8em Tahoma, Arial, Sans-Serif;">Actions</span>
        <asp:Button ID="CancelButton" runat="server" Text="Cancel" />
        <asp:Button ID="SaveButton" runat="server" Text="Save"
            OnClick="SaveButton_Click" />&nbsp;<asp:Label ID="Label2" runat="server" Style="font: .8em Tahoma, Arial, Sans-Serif;"></asp:Label>
    </fieldset>


    <div id="dialog" title="Basic dialog" style="font-size:0.9em;width: 400px;">
        <h1 style="font-size: 14px;">This user is the contact for the following activities and so cannot be deleted:</h1>
        <div id="message_content" style="font-weight:normal;min-height: 350px">This is an animated dialog which is useful for displaying information. The dialog window can be moved, resized and closed with the 'x' icon.</div>

    </div>

    <script>
        $(function () {
            $("#dialog").dialog({
                autoOpen: false,
                height: 350,
                width: 500,
                modal: true,
                buttons: {
                    Ok: function () {
                        $(this).dialog("close");
                    }
                }
            });
        });
    </script>

    <script type="text/javascript">

        $("#<% = SaveButton.ClientID %>").click(function () { });

        $("#<%= CancelButton.ClientID%>").click(function () { window.close(); });

        function CheckForActivitiesOnInactive() {

            var queryString = '?Op=GetUserActivity&uid=<%= Request.QueryString["SystemUserId"] %>'; // send operation type and user ID in query string 


            if($("#IsActiveRadioButtonList_1").is(":checked"))
            {

                $.ajax({
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    url: "/admin/api/Activity/<%= Request.QueryString["SystemUserId"] %>", // "http://localhost:50181/ashx/ActivityHandler.ashx" + queryString, // "/admin/api/Activity/<%= Request.QueryString["SystemUserId"] %>", // "http://localhost:50181/ashx/ActivityHandler.ashx" + queryString, // url: "http://localhost:50181/CorporateCalendarUpdateWebService.asmx/getUserActivityCount",
                    data: "{}", // "{ uid: '<%= Request.QueryString["SystemUserId"] %>' }",
                    dataType: "json",
                    success: function (data) {


                        if (data == null) {
                            alert("There was an issue looking up the user's activities. There may be an error with the application so please contact the appropriate person to have a look.");

                        } else {

                        var content = '';
                            var title = '';
                        var activityCount = 0;

                            for (var j in data) {
                            activityCount++;
                                activity = data[j];

                                title = activity.title == 'undefined' ? 'No Title Found' : activity.title;

                                content += '<li>' + activity.ActivityID + ' ' + title + ' (starts ' + formatDate(activity.startDate.toString()) + ') </li>';  // new Date(parseInt(activity.startDate.substr(6))).toDateString() + ') </li>';
                            }
                        }

                        if (activityCount > 0) {

                            // alert user
                            $("#dialog").find("#message_content").html('<ul>' + content + '</ul>');
                            $("#dialog").dialog("open");

                            $('#<%=IsActiveRadioButtonList.ClientID %>').find("input[value='true']").attr("checked", "checked");

                        }
                    },
                    error: function (xhr, status, error) {
                        var i = -1;
                        var err = eval("(" + xhr.responseText + ")");

                        alert("There was an issue making sure that this user is not a contact for\nany active activities. If this message keeps appearing, contact\nthe owner or manager of this application and let them know\nyou received this message.");
                    }
                });
            }
            return false;
        }

        // take date string, of the form "2012-03-19T09:00:00" and make it of the form "mm/dd/yyyy"
        function formatDate(inDate) { 
            var strMonth = inDate.toString().substring(5, 7);
            var strDay = inDate.toString().substring(8, 10);
            var strYear = inDate.toString().substring(0, 4);

            return strMonth + "/" + strDay + "/" + strYear;
        }

        //  

        $('#<%=DescriptionTextBox.ClientID %>').keyup(function () {
            var count = 1950 - this.value.length;
            var adjustedCount = count <= 0 ? 0 : count; // always display a positive number
            $("#count").html("Characters remaining: " + adjustedCount);
        });
    </script>
    
</asp:Content>
