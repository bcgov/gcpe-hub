<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReleaseNavigationBar.ascx.cs" Inherits="Gcpe.Hub.News.ReleaseManagement.Controls.ReleaseNavigationBar" %>

<%@ Import Namespace="Gcpe.Hub.Data.Entity" %>

<div id="nav-menu">
    <asp:Panel runat="server" CssClass="release-actions" Visible="<%# Model.CanEditDocuments %>">
        <asp:LinkButton runat="server" id="BtnScheduleEmail" CssClass="email" OnClick="BtnScheduleEmail_Click">Send Email</asp:LinkButton>
        <asp:LinkButton runat="server" ID="btnPrint" OnClick="btnPrint_Click" CssClass="print">View PDF</asp:LinkButton>
        <asp:HyperLink runat="server" ID="btnViewRelease" NavigateUrl='<%# Model.ReleaseUri.ToString()%>' CssClass="view hideForAdvisories" Target="_blank" Visible="<%# (Model.IsPublished || Model.IsCommitted) && !Model.IsScheduled %>">View Web</asp:HyperLink>
    </asp:Panel>


    <%--min-height:400px;--%>

    <div style="background-color:#f9f9f9; padding: 3px 2px; border-top:0px solid #eeeeee;border-right:1px solid #eeeeee;border-bottom:0px solid #eeeeee;border-left: 0px solid #eeeeee;width:100%;">


    <%--background-color:#efefef;--%>
    <%--<div style="font-size: 1em; color: #bebebe; ">--%>

    <div class="release-documents" style="padding:0px 2px 0px 0px;">
        <a href="#Top" draggable='false' class="nav-menu-item">Publish</a>
    </div>


    <div class="release-documents" style="margin-bottom: 5px;">
        <table>
            <tr class='<%# !Model.Documents.Any(item => item.Selected) ? "selected-page release-link" : "release-link" %>'">
                <td>
                    <div class="release-documents-reference">
                        <% if (string.IsNullOrEmpty(Model.Reference)) { %>

                            Draft

                        <% } else if (Model.ReleaseTypeId == ReleaseType.Release) { %>
                            <a href='<%# Model.PermanentUri.ToString() %>' target="_blank"><%# Model.Reference %></a>

                            <% if (Model.Reference != Model.Key) { %>
                                <div style="margin-top: 8px; font-size: 1em"><%# Model.Key %></div>
                            <% } %>
                        <% } else if (Model.ReleaseTypeId == ReleaseType.Advisory) { %>
                                <div style="margin-top: 8px; font-size: 1em"><%# Model.Reference %></div>
                        <% } else { %>

                            <a href='<%# Gcpe.Hub.Properties.Settings.Default.NewsHostUri  + (Model.Reference == "" ? "" : Model.Reference.Substring("NEWS-".Length)) %>' target="_blank"><%# Model.Reference %></a>

                        <% } %>

                    </div>
                    <asp:Panel runat="server" Visible="<%# Model.IsCommitted || Model.IsPublished || Model.LeadOrganization != null %>">
                        <span style="display:block;font-size: 0.7em; padding-top: 5px;">
                            <span class="status" style="color:#5b5b5b;"><%# Model.ReleaseStatus %></span>
                        </span>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>

    <div class="release-documents" style="padding:0px 2px 0px 0px;">
        <a href="#Categories" draggable='false' class="nav-menu-item">Categories</a>
    </div>

    <div class="release-documents hideForAdvisories" style="padding:0px 2px 0px 0px;">
        <a href="#Assets" draggable='false' class="nav-menu-item">Assets</a>
    </div>

    <div class="release-documents hideForAdvisories" style="padding:0px 2px 0px 0px;">
        <a href="#Translations" draggable='false' class="nav-menu-item">Translations</a>
    </div>

    <div class="release-documents" style="padding:0px 2px 0px 0px;">
        <a href="#Meta" draggable='false' class="nav-menu-item">Meta</a>
    </div>

    <%--background-color:#f3f3f3;--%>
    <div style="font-size: 1em; color: #b7b7b7; padding: 3px 1px;margin-top:10px;">Documents</div>

    <div class="release-documents" style="padding:0px 2px 10px 0px;">

        <asp:Repeater runat="server" DataSource="<%# Model.DocumentIds %>" ItemType="Guid">
            <ItemTemplate>
                <asp:Panel runat="server" ID="DocumentPanel" style="background-color: #fbfbfb; padding: 2px; border-top: 1px solid #dddddd" draggable="true" ondragstart="OnDragStart(event)" ondragend="OnDragEnd(event)" ondragover="OnDragOver(event)" ondrop="OnDocumentDrop(event)">
                <asp:Repeater runat="server" DataSource="<%# DocumentLanguages(Item) %>" OnItemDataBound="rptDocumentLanguage_DataBound">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" draggable='false' ID="DocumentLink" CssClass="nav-menu-item"></asp:LinkButton>
                    </ItemTemplate>
                </asp:Repeater>
                </asp:Panel>
            </ItemTemplate>
        </asp:Repeater>


        <asp:Panel runat="server" Visible="<%# !Model.IsNew && Model.CanEditDocuments %>">
            <div class="section-action-options" style="width:100%" >
                <asp:LinkButton runat="server" NavigateUrl="#" ID="AddDocumentSwitch" draggable='false' ondragover="OnDragOver(event)" ondrop="OnDocumentDrop(event)"  CssClass="switch" ClientIDMode="Static" OnClick="lbtnAdd_Click">Add...&nbsp;&nbsp;</asp:LinkButton>
            </div>
        </asp:Panel>


    </div>

<asp:Panel runat="server" Visible="<%# Model.Attachments.Any() %>" class="release-documents">
    <div style="font-size: 1em; ">
        <div style="display:inline;color: #b7b7b7;">Attachments</div>
    </div>
    <asp:Repeater runat="server" ItemType="ListItem" DataSource="<%# Model.Attachments %>">
        <HeaderTemplate>
        <table>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <asp:HyperLink runat="server" NavigateUrl="<%# Item.Value %>">
                        <b><asp:Literal runat="server" Mode="Encode" Text="<%# Item.Text %>" /></b>
                    </asp:HyperLink>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
        </table>
        </FooterTemplate>
    </asp:Repeater>

</asp:Panel>

    <div class="release-documents" style="padding:0px 2px 0px 0px;">
        <a href="#History" draggable='false' class="nav-menu-item">History</a>
    </div>

</div>
<script type="text/javascript">
function OnDocumentDrop(ev) {
    function DocumentIndex(s) {
        var posSuffix = s.lastIndexOf('#Documents-');
        if (posSuffix == -1)
            return draggedSource.siblings().length + 1; // happens when the target is the Add link
        return parseInt(s.substring(posSuffix + '#Documents-'.length));
    }
    ev.preventDefault();
    if (!draggedSource || !ev.target.href) return;
    var sDoc = draggedSource.find('.nav-menu-item')[0];
    var sIndex = DocumentIndex(sDoc.href);
    var tDoc = ev.target.parentNode.children[0]; // make sure we get the English document
    var tIndex = DocumentIndex(tDoc.href);

    if (tIndex > sIndex)
        tIndex--; // make moving down also insertbefore target

    if (sIndex != tIndex && confirm("Do you want to change the document order ?")) {
        __doPostBack(sDoc.id, sIndex + '-' + tIndex);
    }
}
</script>

</div>