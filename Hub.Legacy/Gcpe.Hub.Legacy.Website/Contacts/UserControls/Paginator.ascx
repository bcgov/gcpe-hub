<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserControls_Paginator" CodeBehind="Paginator.ascx.cs" %>

<asp:Literal ID="PaginatorLiteral" runat="server" />

<asp:Panel runat="server" ID="pageItemCountPanel" CssClass="common-page-manageitems-count">
    <asp:Literal runat="server" ID="itemCountLit" /> <%= EntityType %>
</asp:Panel>

<asp:Panel ID="actionsPanel" runat="server">

    <div class="common-paginator-container">

        <asp:Panel ID="bulkActionsPanel" CssClass="common-paginator-left" runat="server" Visible="false">
            <asp:DropDownList runat="server" ID="bulkActionsDropDown" CssClass="paginatorDropDown ignore-unload" onchange="changePaginatorDropDownValues(this);" />
            <asp:Button runat="server" OnClick="BulkActionsClick" Text="Apply" ID="bulkActionsBtn" CssClass="gradient" />
        </asp:Panel>

        <asp:Panel ID="pagePanel" runat="server">
            <div class="common-paginator-right">

                <div class="common-paginatory-perpage">
                    <span>Results per page:</span>
                    <asp:DropDownList runat="server" ID="perPageDD" CssClass="ignore-unload" />
                </div>
                <div class="common-paginator-row-numbers">
                    <asp:Literal runat="server" ID="rowNumbersLit" />
                </div>
                <div class="common-paginator-pagelinks">
                    <asp:Literal runat="server" ID="pageLinksLit" />
                </div>

            </div>
        </asp:Panel>

    </div>

</asp:Panel>