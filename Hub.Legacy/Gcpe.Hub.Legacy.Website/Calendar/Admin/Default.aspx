<%@ Page Language="C#" MasterPageFile="~/Calendar/Admin/Site.master" CodeBehind="Default.aspx.cs" Inherits="CorporateCalendarAdmin._Default" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server" />
        <a href="UserList.aspx" style="
	color: #666;
	font: .7em Tahoma, Arial, Sans-Serif;">Manage Users</a><br />
    <a href="Transfer.aspx" style="
	color: #666;
	font: .7em Tahoma, Arial, Sans-Serif;">Transfer Activities</a>
  <%--  
    <h2 class="DDSubHeader">Corporate Calendar System User</h2>
    <a id="SystemUserAnchor" class="DD" runat="server" href="DynamicData/CustomPages/SystemUser.aspx">System User</a>
   <br /><br />--%>
    <h2 class="DDSubHeader">Corporate Calendar Lookup Tables</h2>

  <%--  <br /><br />--%>

    
  
    <asp:GridView ID="Menu1" runat="server" AutoGenerateColumns="false"
        CssClass="DDGridView" RowStyle-CssClass="td" HeaderStyle-CssClass="th" CellPadding="6">
        <Columns>
            <asp:TemplateField HeaderText="Table Name" SortExpression="TableName">
                <ItemTemplate>
                    <%--<asp:DynamicHyperLink ID="HyperLink1" runat="server"><%# Eval("DisplayName") %></asp:DynamicHyperLink>--%>
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# (ResolveUrl("~/Calendar/Admin/") + Eval("DataContextPropertyName") + "/ListDetails.aspx") %>'><%# Eval("DisplayName") %></asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>


