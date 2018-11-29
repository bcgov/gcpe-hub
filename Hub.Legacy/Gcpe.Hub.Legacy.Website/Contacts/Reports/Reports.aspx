<%@ Page Language="C#" AutoEventWireup="true" Inherits="Reports_Reports" MasterPageFile="~/Contacts/MasterPage/MediaRelations.master" CodeBehind="Reports.aspx.cs" %>

<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>
<%@ Register TagPrefix="mr" TagName="Paginator" Src="~/Contacts/UserControls/Paginator.ascx" %>
<%@ Register TagPrefix="mr" TagName="SortColumnHeader" Src="~/Contacts/UserControls/SortColumnHeader.ascx" %>


<asp:Content ContentPlaceHolderID="ContentPlaceHolder" runat="server">

    <mrcl:PermissionContainer ID="PermissionContainer1" runat="server" Section="MediaRelationsCommonReports" AccessLevel="Read">
        <Content>
            <h1>Reports</h1>

            <mr:Paginator ID="topPaginator" runat="server" Mode="Bottom" />

            <table class="common-admin-table">

                <thead>
                    <tr class="top">
                        <th runat="server" id="cbTopTh">
                            <input type="checkbox" name="selectAll" onclick="toggleSelectAll(this, 'tableContentTB');"></nobr></th>
                        <th>Name</th>
                        <th>Created By</th>
                        <th>Created</th>
                        <th>Function</th>
                        <th runat="server" id="orderByTop">Order</th>
                    </tr>
                </thead>

                <tbody id="tableContentTB">
                    <asp:Literal runat="server" ID="tableContentLit" />
                </tbody>

                <% if (topPaginator.PerPage > 10 && topPaginator.Count > 10) { %>

                <tfoot>
                    <tr class="bottom">
                        <th runat="server" id="cbBottomTh">
                            <input type="checkbox" name="selectAll" onclick="toggleSelectAll(this, 'tableContentTB');"></nobr></th>
                        <th>Name</th>
                        <th>Created By</th>
                        <th>Created</th>
                        <th>Function</th>
                        <th runat="server" id="orderByBottom">Order</th>
                    </tr>
                </tfoot>

                <% } %>
            </table>


            <mr:Paginator ID="bottomPaginator" runat="server" Mode="Bottom" />
        </Content>
    </mrcl:PermissionContainer>

</asp:Content>
