<%@ Page Language="C#" AutoEventWireup="true" Inherits="Reports_MyReports" MasterPageFile="~/Contacts/MasterPage/MediaRelations.master" CodeBehind="MyReports.aspx.cs" %>

<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>
<%@ Register TagPrefix="mr" TagName="Paginator" Src="~/Contacts/UserControls/Paginator.ascx" %>
<%@ Register TagPrefix="mr" TagName="SortColumnHeader" Src="~/Contacts/UserControls/SortColumnHeader.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

    <mrcl:PermissionContainer ID="PermissionContainer1" runat="server" Section="MediaRelationsUserReports" AccessLevel="Read">
        <Content>
            <h1>My Reports</h1>

            <mr:Paginator ID="topPaginator" runat="server" BulkActions="true" Mode="Bottom" />

            <table class="common-admin-table">

                <thead>
                    <tr class="top">
                        <th>
                            <input type="checkbox" name="selectAll" onclick="toggleSelectAll(this, 'tableContentTB');"></nobr></th>
                        <th>Name</th>
                        <th>Created</th>
                        <th>Function</th>
                        <th>Order</th>
                    </tr>
                </thead>

                <tbody id="tableContentTB">
                    <asp:Literal runat="server" ID="tableContentLit" />
                </tbody>

                <% if (topPaginator.PerPage > 10 && topPaginator.Count > 10)
                    { %>

                <tfoot>
                    <tr class="bottom">
                        <th>
                            <input type="checkbox" name="selectAll" onclick="toggleSelectAll(this, 'tableContentTB');"></nobr></th>
                        <th>Name</th>
                        <th>Created</th>
                        <th>Function</th>
                        <th>Order</th>
                    </tr>
                </tfoot>

                <% } %>
            </table>


            <mr:Paginator ID="bottomPaginator" runat="server" BulkActions="true" Mode="Bottom" />
        </Content>
    </mrcl:PermissionContainer>

</asp:Content>
