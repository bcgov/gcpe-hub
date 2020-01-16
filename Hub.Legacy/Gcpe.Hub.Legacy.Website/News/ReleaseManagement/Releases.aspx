<%@ Page Title="Releases" Language="C#" MasterPageFile="~/News/ReleaseManagement/ReleaseManagement.master" AutoEventWireup="true" CodeBehind="Releases.aspx.cs" Inherits="Gcpe.Hub.News.ReleaseManagement.Releases" %>

<%@ MasterType TypeName="Gcpe.Hub.News.ReleaseManagement.ReleaseManagement" %>
<%@ Import Namespace="Gcpe.Hub.News.ReleaseManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="formContentPlaceHolder" runat="server">

    <div>
        <h1 style="display: inline">
            <asp:Literal ID="ltrPageTitle" runat="server" Text="<%# Model.ResultSetName %>"></asp:Literal>
        </h1>
        <% if (this.Folder == "Forecast")
            { %>
        <a style="text-decoration:none" href='<%= GetWebCalUrl() %>'>Connect to Outlook</a>
        <% } %>
        <br />&nbsp;
    </div>
    
    <asp:Panel runat="server" Visible='<%# this.Folder != "Forecast" %>'>
    <div id="typeFilterLink" style='margin-left: -2px; <%# Folder == "Forecast" ? "display:none;": "display:table;"%>'>
        <div class="release-types" style="width:80px;">
            <a href='<%= this.ResolveUrl("~/News/ReleaseManagement/" + Folder) %>' class='<%# Type == "All" || Type =="" ? "nav-menu-item selected-page" : "nav-menu-item" %>'>All</a>
        </div>
        <div style="color:dimgrey">
            |
        </div>
        <div class="release-types" style="width:100px;">
            <a href='<%= this.ResolveUrl("~/News/ReleaseManagement/" + Folder) %>/Release' class='<%# Type == "Release" ? "nav-menu-item selected-page" : "nav-menu-item" %>'>Releases</a>
        </div>
        <div class="release-types" style="width:100px;">    
            <a href='<%= this.ResolveUrl("~/News/ReleaseManagement/" + Folder) %>/Story' class='<%# Type == "Story" ? "nav-menu-item selected-page" : "nav-menu-item" %>'>Stories</a>
        </div>
        <div class="release-types" style="width:100px;">
            <a href='<%= this.ResolveUrl("~/News/ReleaseManagement/" + Folder) %>/Factsheet' class='<%# Type == "Factsheet" ? "nav-menu-item selected-page" : "nav-menu-item" %>'>Factsheets</a>
        </div>
         <div class="release-types" style="width:100px;">
            <a href='<%= this.ResolveUrl("~/News/ReleaseManagement/" + Folder) %>/Update' class='<%# Type == "Update" ? "nav-menu-item selected-page" : "nav-menu-item" %>'>Updates</a>
        </div>
        <div class="release-types" style="width:100px;">
            <a href='<%= this.ResolveUrl("~/News/ReleaseManagement/" + Folder) %>/Advisory' class='<%# Type == "Advisory" ? "nav-menu-item selected-page" : "nav-menu-item" %>'>Advisories</a>
        </div>
    </div>
    <%--<p runat="server"><%# Model.CountResults() + (Model.CountResults() == 1 ? "Release" : "Releases") %></p>--%>
    
    <asp:ObjectDataSource ID="listViewDataSource" EnablePaging="true" runat="server" TypeName="Gcpe.Hub.News.ReleaseManagement.ReleasesDataSource" SelectMethod="GetNewsReleases" SelectCountMethod="GetNewsReleasesCount" />

    <asp:ListView ID="listView" runat="server" DataSourceID="listViewDataSource" ItemType="ReleasesModel.Result" Enabled='<%# this.Folder != "Forecast" %>' Visible='<%# this.Folder != "Forecast" %>'>
       
        <LayoutTemplate>
            <asp:Panel runat="server" id="itemPlaceholder" />
            <br />
            <div style="text-align:center">
            <asp:DataPager ID="dataPager" runat="server" OnInit="DataPager_Init">
                <Fields>
                    <asp:NumericPagerField ButtonCount="25" PreviousPageText=" < " NextPageText=" > " NumericButtonCssClass="numeric_button" CurrentPageLabelCssClass="current_page" />
                </Fields>
               <%-- <Fields>
                  <asp:TemplatePagerField>
                    <PagerTemplate>
                    <b>
                    Page
                    <asp:Label runat="server" ID="CurrentPageLabel" 
                      Text="<%# Container.TotalRowCount>0 ? (Container.StartRowIndex / Container.PageSize) + 1 : 0 %>" />
                    of
                    <asp:Label runat="server" ID="TotalPagesLabel" 
                      Text="<%# Math.Ceiling ((double)Container.TotalRowCount / Container.PageSize) %>" />
                    </b>
                    <br /><br />
                    </PagerTemplate>
                  </asp:TemplatePagerField>

                  <asp:NextPreviousPagerField ShowFirstPageButton="true" ShowNextPageButton="false" ShowPreviousPageButton="false" />
                  <asp:NumericPagerField PreviousPageText="&lt;&lt;" NextPageText="&gt;&gt;" ButtonCount="10" />
                  <asp:NextPreviousPagerField ShowLastPageButton="true" ShowNextPageButton="false" ShowPreviousPageButton="false" />
                </Fields>--%>
            </asp:DataPager>
                </div>
        </LayoutTemplate>
       
        <ItemTemplate>
            <table class="release-list-item-row">
                <tr>
                    <%--<td title="<%# Item.ReleaseType %>" style="width:12px; background-color:<%# Item.ReleaseType == "Story" ? "rgb(89, 57, 58)" : (Item.ReleaseType == "Release" ? "darkslategray" : "darkblue") %>;">--%>
                    <td title="<%# Item.ReleaseType %>" style="border-color:white; width:12px; background-color:<%# ReleaseModel.ReleaseColor(Item.ReleaseType) %>;">
                        &nbsp;
                    </td>
                    <td>
                    
                        <asp:HyperLink runat="server" NavigateUrl="<%# ReleaseModel.ReleaseHubUrl(Item.IsPublished, Item.IsCommitted, Item.Reference, (Guid)Item.Id) %>" CssClass="release-list-item"> 
                     
                            <span style="display:inline-block;width:24%; padding-right:1%; padding-left:2px; vertical-align:top;">
                                <span style="display:block;font-weight:bold;padding-bottom:3px"><%# Item.FirstOrganization %></span>
                                <span style="padding-bottom:4px;display:block;">
                                    <span style="font-size:0.95em; text-transform:uppercase"><%# ReleaseModel.ReleaseDocumentType(Item.ReleaseType, Item.PageTitle) %></span>
                                    <asp:Literal runat="server" Text='<%# Item.ReleaseType == "Story" && Item.PageTitle == "Release" ? "" : Item.PageTitle %>' Mode="Encode" />
                                </span>
                                <span style="display:block;"><%# PublishStatusDate(Item) %></span> 
                            </span>
                            <span style="display:inline-block;width:72%;line-height:1.2em;vertical-align:top;">
                                <span class="title"><asp:Literal runat="server" Text="<%# Item.Headline %>" Mode="Encode" /></span>
                                <span><%# Item.Summary %></span>
                            </span>

                        </asp:HyperLink>
                
                    </td>
                   
                    <% if (this.Folder == "Drafts")
                    { %>
                    <td title="<%# Item.Reference == string.Empty ? "" : "Approved" %>" style="border-color:white; width:12px; background-color:<%# Item.Reference == string.Empty ? "" : "green" %>;">
                        &nbsp;
                    </td>
                    <% } %>
                    
                </tr>
            </table>
            
        </ItemTemplate>
    </asp:ListView>
    </asp:Panel>
    <asp:Panel runat="server" Visible='<%# this.Folder == "Forecast" %>'>
     <asp:ObjectDataSource ID="CalendarlistViewDataSource" EnablePaging="true" runat="server" TypeName="Gcpe.Hub.News.ReleaseManagement.ReleasesDataSource" SelectMethod="GetActiveActivities" SelectCountMethod="GetActivityCount"/>

    <asp:ListView ID="ActivityListView" runat="server" DataSourceID="CalendarlistViewDataSource" ItemType="ReleasesModel.Result" Enabled='<%# this.Folder == "Forecast" %>' Visible='<%# this.Folder == "Forecast" %>'>
       
        <LayoutTemplate>
            <asp:Panel runat="server" id="itemPlaceholder" />
            <br />
            <div style="text-align:center">
            <asp:DataPager ID="dataPager" runat="server">
                <Fields>
                    <asp:NumericPagerField ButtonCount="25" PreviousPageText=" < " NextPageText=" > " NumericButtonCssClass="numeric_button" CurrentPageLabelCssClass="current_page" />
                </Fields>
               
            </asp:DataPager>
                </div>
        </LayoutTemplate>
       
        <ItemTemplate>
 
           <table class="release-list-item-row">
                <tr>

                    <td title="<%# Item.IsCalActivity ? "Calendar Activity" : Item.ReleaseType %>" style='border-color:white; width:12px; background-color:<%# Item.IsCalActivity ? "rgb(72, 113, 72)" : ReleaseModel.ReleaseColor(Item.ReleaseType) %>;'>
                        &nbsp;
                    </td>
                    <td>
                    
                        <asp:HyperLink runat="server" NavigateUrl="<%# Item.IsCalActivity ?  GetCalActivityUrl(Item.ActivityId) : ReleaseModel.ReleaseHubUrl(Item.IsPublished, Item.IsCommitted, Item.Reference, (Guid)Item.Id) %>" CssClass="release-list-item" Target='<%# Item.IsCalActivity ? "_blank" : "" %>'> 
                     
                            <span style="display:inline-block;width:24%; padding-right:1%; padding-left:2px; vertical-align:top;">
                                <span style="display:block;font-weight:bold;padding-bottom:3px"><%# Item.FirstOrganization %></span>
                                <span style="padding-bottom:4px;display:block;">
                                    <span style="font-size:0.95em; text-transform:uppercase"><%# Item.IsCalActivity ? Item.EventType : ReleaseModel.ReleaseDocumentType(Item.ReleaseType, Item.PageTitle) %></span>
                                    <asp:Literal runat="server" Text='<%# Item.IsCalActivity || (Item.ReleaseType == "Story" && Item.PageTitle == "Release") ? "" : Item.PageTitle %>' Mode="Encode" />
                                </span>
                                <span style="display:block;"><%# PublishStatusDate(Item) %></span> 
                            </span>
                            <span style="display:inline-block;width:72%;line-height:1.2em;vertical-align:top;">
                                <span class="title"><asp:Literal runat="server" Text='<%# (Item.IsCalActivity && !string.IsNullOrWhiteSpace(Item.Location) ? Item.Location + " - " : "") + (Item.IsConfidential == true ? "Not For Look Ahead" : Item.Headline) %>' Mode="Encode" /></span>
                                <span><%# Item.Summary %></span>
                            </span>

                        </asp:HyperLink>
                
                    </td>
                    <td title="<%# Item.Reference == string.Empty ? "" : "Approved" %>" style="border-color:white; width:12px; background-color:<%# Item.Reference == string.Empty ? "" : "green" %>;">
                        &nbsp;
                    </td>
                </tr>
            </table>
        </ItemTemplate>
    </asp:ListView>
    </asp:Panel>
</asp:Content>
