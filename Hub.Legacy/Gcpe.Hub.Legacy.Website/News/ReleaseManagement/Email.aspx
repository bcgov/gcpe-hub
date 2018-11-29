<%@ Page Title="" Language="C#" MasterPageFile="~/News/ReleaseManagement/ReleaseManagement.master" AutoEventWireup="true" CodeBehind="Email.aspx.cs" Inherits="Gcpe.Hub.News.ReleaseManagement.Email" %>
<%@ MasterType TypeName="Gcpe.Hub.News.ReleaseManagement.ReleaseManagement" %>
<%@ Import Namespace="Gcpe.Hub" %>
<%@ Import Namespace="Gcpe.Hub.News" %>


<asp:Content ID="Content1" ContentPlaceHolderID="headContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="formContentPlaceHolder" runat="server">

    <script>

        $(function () {

            $("#<%= chkPeopleWhoEdited.ClientID %>").click(function () {
                if ($(this).is(":checked")) {
                    SetCheckedBoxList("<%= chklstPeopleWhoEdited.ClientID %>", true);
                } else {                    
                    SetCheckedBoxList("<%= chklstPeopleWhoEdited.ClientID %>", false);
                }
            });

            $("#<%= chkPeopleInWritingGroup.ClientID %>").click(function () {
                if ($(this).is(":checked")) {
                    SetCheckedBoxList("<%= chklstPeopleInWritingGroup.ClientID %>", true);
                } else {
                    SetCheckedBoxList("<%= chklstPeopleInWritingGroup.ClientID %>", false);
                }
            });

            $("#<%= chkPeopleInCommContacts.ClientID %>").click(function () {
                if ($(this).is(":checked")) {
                    SetCheckedBoxList("<%= chklstPeopleInCommContacts.ClientID %>", true);
                } else {
                    SetCheckedBoxList("<%= chklstPeopleInCommContacts.ClientID %>", false);
                }
            });

            $("#<%= chkPeopleEmailedBefore.ClientID %>").click(function () {
                if ($(this).is(":checked")) {
                    SetCheckedBoxList("<%= chklstPeopleEmailedBefore.ClientID %>", true);
                } else {
                    SetCheckedBoxList("<%= chklstPeopleEmailedBefore.ClientID %>", false);
                }
            });


        });

        function SetCheckedBoxList(el, val) {
            $("#" + el + " input").prop("checked", val);
            if(val)
                $("#" + el + "").show();
            else
                $("#" + el + "").hide();
        }

    </script>

    <div id="two-column">
        <div id="left-menu">
            
        </div>
        <div id="main-content">
            <h1>Release Headline</h1>
            
            
            <div ID="DetailsEdit" class="section view">

                <h2>Recipients</h2>

                <div class="field-group">

                    <asp:Panel runat="server" ID="pnlWhoEdited" Visible="true">
                        <asp:CheckBox runat="server" ID="chkPeopleWhoEdited" Text="User's who have edited this release" CssClass="bpad" />
                        <asp:CheckBoxList runat="server" ID="chklstPeopleWhoEdited" CssClass="email-options-lst chkbxlst">
                            <asp:ListItem Value="email" Text="Trinity.Wolfe@gov.bc.ca"></asp:ListItem>
                            <asp:ListItem Value="email" Text="John.Bird@gov.bc.ca"></asp:ListItem>
                        </asp:CheckBoxList>
                    </asp:Panel>


                    <div>
                        <asp:CheckBox runat="server" ID="chkPeopleInWritingGroup" Text="User's in the writing group" CssClass="bpad" />
                        <asp:CheckBoxList runat="server" ID="chklstPeopleInWritingGroup" CssClass="email-options-lst chkbxlst">
                            <asp:ListItem Value="email" Text="Trinity Wolfe EDUC:EX"></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>


                    <div>
                        <asp:CheckBox runat="server" ID="CheckBox1" Text="Contacts associated with the release" CssClass="bpad" /> <%--Comm. Contacts--%>
                        <asp:CheckBoxList runat="server" ID="chklstPeopleInCommContacts" CssClass="email-options-lst chkbxlst">
                            <asp:ListItem Value="email" Text="Trinity Wolfe EDUC:EX"></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>                


                    <div>
                        <asp:CheckBox runat="server" ID="chkPeopleEmailedBefore" Text="User's who have been emailed this before" CssClass="bpad" />
                        <asp:CheckBoxList runat="server" ID="chklstPeopleEmailedBefore" CssClass="email-options-lst chkbxlst">
                            <asp:ListItem Value="email" Text="Trinity Wolfe EDUC:EX"></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>

                </div>

                <div class="field-group">
                    <div class="label">Other Recipients</div>
                    <div class="helper">Enter multiple recipients separated by a semi-colon.</div>
                    <div class="txt">
                        <asp:TextBox runat="server" ID="txtRecipients" TextMode="MultiLine" Width="500px" Height="50px"></asp:TextBox>
                    </div>
                </div>

                <div class="actions">
                    <asp:Button ID="btnSend" runat="server" Text="Send" CssClass="primary" OnClick="btnSend_Click" />
                    <asp:LinkButton runat="server" ID="lbtnCancelSave" Text="Cancel" CssClass="cancel" OnClick="lbtnCancelSave_Click" />
                </div>

            </div>

            </div>


            <div style="display:none;">
I wanted to have a quick list of email address (as well as a custom field obviously).
            I was thinking of the following sources, and wondering if you could present them in a meaningful way.
            So if an editor needed to email a release to all media contacts to get their final approval, 
            they could do it from the release screen in 3 clicks (e.g. Email à All Comm. Contacts à Send).
 
·         the currently logged in user
·         all users who have edited this release
·         all users in the writing group
·         all media contacts associated with the release
·         all email addresses that have been sent this email before
</div>



        </div>
</asp:Content>
