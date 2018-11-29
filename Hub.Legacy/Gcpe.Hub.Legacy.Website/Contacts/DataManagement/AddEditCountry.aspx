<%@ Page Language="C#" AutoEventWireup="true" Inherits="AddEditCountry" MasterPageFile="~/Contacts/MasterPage/MediaRelations.master" CodeBehind="AddEditCountry.aspx.cs" %>

<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <mrcl:PermissionContainer ID="PermissionContainer1" runat="server" Section="MediaRelationsDataLists" AccessLevel="Create">
        <asp:Panel runat="server" ID="errorPanel" Visible="false">
            <asp:Label CssClass="error" runat="server" ID="pageErrorField" />
        </asp:Panel>

        <asp:Panel runat="server" ID="displayPanel">

            <div class="error" id="formError" runat="server"></div>

            <div class="common-page-box-border">

                <div class="common-admin-control-bar" runat="server" id="editOnlyDisplayPanel" visible="false">
                    <span class="field labelbold">
                        <asp:Literal runat="server" ID="currentName" /></span>

                    <div class="right">
                        <asp:LinkButton Visible="false" runat="server" ID="deleteButton" Text="Delete" OnClick="DeleteClick" OnClientClick="return confirm(deleteButtonText);" />
                    </div>
                </div>

                <div class="common-page-box-inner-small manage-data-area">
                    <div class="field labeltop">Name: <span class="required">*</span></div>
                    <asp:TextBox runat="server" ID="countryNameTb" MaxLength="250" />
                    <div class="error" runat="server" id="nameError"></div>
                </div>

                <div class="common-page-box-inner-small manage-data-area">
                    <div class="field labeltop">Abbreviation: <span class="required">*</span></div>
                    <asp:TextBox runat="server" ID="countryAbbrevTb" MaxLength="250" />
                    <div class="error" runat="server" id="Div1"></div>
                </div>
                <div class="common-page-box-inner-small manage-data-area">
                    <div class="field labeltop">Sort Order: <span class="required">*</span></div>
                    <asp:TextBox runat="server" ID="sortOrderTextBox" Width="60" TextMode="Number" />
                    <div class="error" runat="server" id="sortOrderError"></div>
                </div>

            </div>

            <asp:Button runat="server" OnClick="CancelClick" OnClientClick="return confirm(cancelButtonText);" Text="Cancel" CssClass="common-admin-button" />
            <asp:Button ID="Button1" runat="server" OnClick="SaveClick" Text="Save" OnClientClick="return ConfirmValidPageSubmission();" CssClass="common-admin-button" />

            <script type="text/javascript">

                function ConfirmValidPageSubmission() {

                    var element = document.getElementById('<%= countryNameTb.ClientID %>');
                    var errorElm = document.getElementById('<%= nameError.ClientID %>');

                    errorElm.innerHTML = "";

                    if (element.value.trim() == "") {
                        //alert("City name cannot be empty");
                        alert(saveErrorText.replace("###errors###", "Country Name\n"));
                        errorElm.innerHTML = "Country name cannot be empty";
                        return false;
                    }
                    return true;
                }

            </script>
        </asp:Panel>
    </mrcl:PermissionContainer>
</asp:Content>
