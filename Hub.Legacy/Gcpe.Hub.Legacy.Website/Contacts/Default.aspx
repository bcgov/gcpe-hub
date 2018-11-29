<%@ Page Language="C#" AutoEventWireup="true" Inherits="_Default" MasterPageFile="~/Contacts/MasterPage/MediaRelationsResponsive.master" CodeBehind="Default.aspx.cs" %>

<%@ Register TagPrefix="mr" TagName="AdvancedSearchControl" Src="~/Contacts/UserControls/AdvancedSearchControl.ascx" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <mrcl:PermissionContainer ID="PermissionContainer1" runat="server" Section="MediaRelationsSearch" AccessLevel="Read">
        <asp:Panel runat="server">
            <h1>Home</h1>

            <div class="desktop">
                <mr:AdvancedSearchControl runat="server" ID="advancedSearchControlDesktop" />

                <div id="ContentPlaceHolder_ContentPlaceHolder_PermissionContainer1_searchCriteria" class="search-search-criteria">Enter search criteria and click the search button for results</div>

            </div>


            <div class="phone home-container">

                <div class="search-label">Search by Name</div>

                <div class="search-button">

                    <div class="box-container">

                        <div class="left">
                            <asp:TextBox AutoComplete="off" runat="server" ID="searchFieldTb" CssClass="ignore-unload" />
                            <a href="javascript:void(0);" onclick="DeleteSearchText();return false;"><img runat="server" src="~/Contacts/images/BigX@2x.png" border="0" /></a>
                        </div>

                        <div class="search">
                            <asp:LinkButton runat="server" ID="searchButton" OnClick="SearchButtonClick" OnClientClick="ShowSearchLoadingModule()" Text="" CssClass="home-search" />
                        </div>

                        <div class="clear"></div>

                    </div>

                    <a href="AdvancedSearch.aspx" class="right-chevron">Advanced Search</a>

                </div>

                <div class="reports">

                    <div class="home-reports-header">
                        <a href="javscript:void(0);" onclick="toggleMobileNavItem(this, 'myReportsHomeContainer');return false;">MY REPORTS</a>
                    </div>

                    <div class="home-reports-subcontainer" id="myReportsHomeContainer" runat="server">
                        <asp:Literal runat="server" ID="myReportsLit" />
                    </div>

                    <div class="home-reports-header">
                        <a href="javascript:void(0);" onclick="toggleMobileNavItem(this, 'reportsHomeContainer');return false;">REPORTS</a>
                    </div>

                    <div class="home-reports-subcontainer" id="reportsHomeContainer" runat="server">
                        <asp:Literal runat="server" ID="reportsLit" />
                    </div>

                </div>


                <script type="text/javascript">

                    SetupTypedown(document.getElementById("<%=searchFieldTb.ClientID%>"), "<%=Page.ResolveUrl("~/Contacts/ajax/Typedown.ashx?q=")%>", document.getElementById("<%=searchButton.ClientID%>"));

                    function DeleteSearchText() {
                        document.getElementById('<%= searchFieldTb.ClientID %>').value = '';
                    }


                    function DeleteReport(reportGuid) {

                        var confirmed = confirm(deleteButtonText);

                        if (confirmed) {
                            window.location = "Default.aspx?delete=" + reportGuid;
                        }

                        return false;
                    }

                </script>
            </div>
        </asp:Panel>
    </mrcl:PermissionContainer>
</asp:Content>
