<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Document.ascx.cs" Inherits="Gcpe.Hub.News.ReleaseManagement.Controls.Document" %>

<%@ Register Assembly="Gcpe.News.ReleaseManagement.Controls" Namespace="Gcpe.News.ReleaseManagement.Controls" TagPrefix="extcontrols" %>
<%@ Import Namespace="Gcpe.Hub" %>
<%@ Import Namespace="Gcpe.Hub.News" %>
<%@ Register TagPrefix="controls" TagName="ReleaseImagePicker" Src="~/News/ReleaseManagement/Controls/ReleaseImagePicker.ascx" %>
<%@ Register Assembly="Gcpe.Hub.Legacy.Website" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<div id='<%# "Documents-" + DocumentPath.Replace("/", "-") %>'>

    <script id="defaultVariables">
        /* Default Value Lists */
        var defaultPageLayouts = { <%= Model.DefaultPageLayouts.ToJavaScript() %> };

        var bylineTxt = "";
        var orgTxt = "";

        /* Sets the Default Layout based on the Page Title passed in */
        function SetPageDefaults(type, rblPageLayout, txtOrganization) {

            var defaultLayout = "";

            $.each(defaultPageLayouts, function (key, value) {
                if (key.toUpperCase() == type.toUpperCase())
                    defaultLayout = value;
            });
            if (defaultLayout != "") {
                rblPageLayout.filter('input[value="' + defaultLayout + '"]').prop('checked', true);
                rblPageLayout.parents('div .field-group').removeClass('warning-chkbxlst');
                SetPageLayoutDefaults(defaultLayout, txtOrganization);
            }
        }


    </script>

    <div>

        <asp:HiddenField ID="documentPath" runat="server" Value= "<%# DocumentPath %>"/>

        <asp:Panel runat="server" ID="pnlErrors" CssClass="section-error" Visible="false">
            <h2>Sorry, but there was an error with your submission.</h2>
            <asp:Repeater runat="server" ID="rptErrors">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li><%# Container.DataItem %></li>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
        </asp:Panel>


        <asp:Panel runat="server" ID="DocumentPreview" class="section view">

            <h2 class="section-title"><%# PageTitle %></h2>
            
                <asp:Panel runat="server" CssClass="section-action-options" Visible="<%# Model.CanEdit && Model.CanEditDocuments %>">
                    <asp:HyperLink ID="DocumentEditSwitch" runat="server" NavigateUrl="#" CssClass="switch editButton" Style='<%# DocumentIsNew ? "visibility: hidden;" : "" %>' data-defaultPageTitles="<%# Model.DefaultPageTitles(LanguageId).ToJavaScript() %>" onclick="return onDocumentEdit(this)">Edit</asp:HyperLink>
                    <asp:Repeater ID="rptrAddLanguage" runat="server" DataSource="<%# AddLanguages %>" ItemType="System.Int32">
                            <ItemTemplate>
                            <asp:LinkButton ID="btnAddLanguage" runat="server" NavigateUrl="#"  CssClass="switch" OnClick="btnAddLanguage_Click" >Add <%# Model.FindLanguageName(Item) %></asp:LinkButton>
                            </ItemTemplate>
                        </asp:Repeater>
                    <asp:LinkButton runat="server" NavigateUrl="#" CssClass="switch" Visible="<%# CanDeleteDocument %>" OnClick="lbtnDelete_Click" OnClientClick='<%# "return confirm(" + DeleteMessage() + ")"%>'><%# LanguageId == 4105 ? "Remove Document" : "Remove Translation" %></asp:LinkButton>
                </asp:Panel>

                <div class="container">

                <div style="width: 100%">
                <asp:Image runat="server" Visible="<%# PageImageId.HasValue %>" ImageUrl="<%# GetPageImageUrl() %>" />
                </div>

                <div style="width: 100%; text-align: center; text-transform: uppercase; font-size: 1.2em; padding-bottom: 20px; text-transform: uppercase;">
                <%# PreviewPageTitleHtml %>
                </div>
                <asp:Panel runat="server" Visible="<%# !string.IsNullOrEmpty(PreviewTopLeftHeaderHtml) || !string.IsNullOrEmpty(PreviewTopRightHeaderHtml) %>">
                    <table style="width: 100%;">
                        <tr>
                            <td>
                            <%# PreviewTopLeftHeaderHtml %>
                            </td>
                            <td style="vertical-align: top; text-align: right;">
                            <%# PreviewTopRightHeaderHtml %>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                </asp:Panel>
                <asp:Panel runat="server" CssClass="headline" Visible="<%# !string.IsNullOrEmpty(PreviewMiddleHeaderHtml) %>">
                <%# PreviewMiddleHeaderHtml %>
                </asp:Panel>
                <asp:Panel runat="server" Visible="<%# !string.IsNullOrEmpty(PreviewBottomHeaderHtml) %>">
                    <table style="width: 100%;">
                        <tr>
                            <td>
                            <%# PreviewBottomHeaderHtml %>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                </asp:Panel>
                    <div style="padding-top: 8px; padding-bottom: 12px;">
                    <%# PreviewBodyHtml %>
                </div>

            <asp:Panel runat="server" ID="pnlPreviewContacts" Visible="<%# Contacts.Any() %>">
                    <div>
                            <table style="width: 100%;">
                            <tr>
                                <%--TODO: Make Comm. Contacts and Headline a little darker--%>
                                <td style="vertical-align: top; width: 15%; color: #333;">
                                    <b><%: LanguageId==3084 ? "Renseignements additionnels:" : (Contacts.Count() > 1 ? "Contacts:" : "Contact:") %></b><%--Comm. Contacts--%>
                                </td>
                                <td style="width: 85%;">
                                <asp:Repeater ID="Repeater1" runat="server" DataSource="<%# PreviewContactsHtml %>" ItemType="System.String">
                                        <ItemTemplate>
                                                <div style="display: inline-block; width: 48%; margin-right: 1%; vertical-align: top; margin-bottom: 13px;"><%# Item.Replace("\r\n", "<br />") %></div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>



                </div>
            </asp:Panel>



            <asp:Panel runat="server" ID="DetailsEdit" class="section edit" Visible="<%# Model.CanEdit %>">
                <h2>Details</h2>

                <div class="field-group">
                    <div class="label">Language</div>
                    <div class="txt">
                    <asp:Literal ID="ltrLanguageName" runat="server" Mode="Encode" Text="<%# LanguageName %>" /></div>
                </div>

                <div class="field-group required">
                    <div class="label">Document Type</div>
                    <div class="txt">
                        <div style="display: inline-block;">
                        <asp:TextBox ID="txtPageTitle" runat="server" MaxLength="50" Width="500px" Text="<%# PageTitle %>"></asp:TextBox></div>
                        <asp:HiddenField runat="server" ID="hidReleaseType" ClientIDMode="Static" Value="<%# (int)Model.ReleaseTypeId %>" />
                        <asp:HiddenField runat="server" ID="hidAddReleaseType" ClientIDMode="Static" />
                        <div class="PageTitleConfirmation" style="display: none;">
                            <div style="display: inline-block;"><a href="#" class="IgnoreNewDocumentType" style="font-size: 0.9em;">Ignore</a></div>
                            <div style="display: inline-block;"><a href="#" class="AddNewDocumentType" style="font-size: 0.9em;">Add New Document Type</a></div>
                        </div>
                    </div>
                    
                </div>

                <div class="field-group required">
                    <div class="label">Page Layout</div>
                    <div class="txt">
                    <asp:RadioButtonList ID="rblPageLayout" runat="server" DataSource="<%# PageLayouts %>" DataValueField="Key" DataTextField="Value" SelectedValue="<%# PageLayout %>" Visible="<%# !HasTranslation %>" />
                    <asp:Literal ID="ltrPageLayout" runat="server" Mode="Encode" Text="<%# PageLayout %>" Visible="<%# HasTranslation %>" />
                    </div>
                </div>

                <% if (Model.ReleaseTypeName != "Advisory") { %>
                    <div class="field-group required">
                        <div class="label">Page Image</div>
                        <div class="txt">
                        <controls:ReleaseImagePicker ID="pageImagePicker" runat="server" Value="<%# PageImageId %>" LanguageId="<%# LanguageId %>" ReleaseImageHeight="60" />
                        </div>
                    </div>
                <% } %>

                <div class="field-group required">
                    <div class="label">Headline</div>
                    <div class="txt"> 
                    <asp:TextBox ID="txtHeadline" runat="server" MaxLength="255" Width="800px" Text="<%#Headline %>"></asp:TextBox>
                    </div>
                </div>

                <div class="field-group">
                    <div class="label">Subheadline</div>
                    <div class="txt"> 
                    <asp:TextBox ID="txtSubheadline" runat="server" MaxLength="100" Width="500px" Text="<%#Subheadline %>"></asp:TextBox>
                    </div>
                </div>

                <asp:Panel runat="server" Visible="<%# !HasTranslation || PageLayout == FormalPageLayout %>">
                    <div id="Organizations" class="field-group required">
                        <div class="label">Organizations<span class="disable-note"> Update the page layout above to formal to enter organizations</span></div>
                        <div class="txt"> 
                            <asp:TextBox ID="txtOrganization" runat="server" MaxLength="250" Width="500px" TextMode="MultiLine" Text="<%# Organizations %>" Height="90px"></asp:TextBox>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel runat="server" Visible="<%# !HasTranslation || PageLayout == InformalPageLayout %>">
                    <div id="ByLine" class="field-group">
                        <div class="label">Byline<span class="disable-note"> Update the page layout above to informal to enter a byline</span></div> 
                        <div class="txt"> 
                            <asp:TextBox ID="txtByline" runat="server" MaxLength="250" Width="500px" TextMode="MultiLine" Text="<%#Byline %>" Height="90px"></asp:TextBox>
                        </div>
                    </div>
                </asp:Panel>

     <%--       <div class="field-group">
                    <div class="label">Body</div>
                    <extcontrols:ContentEditor runat="server" ID="contentEditor" LanguageId="<%# Model.LanguageId %>" />
                </div>--%>

                <div class="field-group required">
                    <div class="label">Body</div>
                    <CKEditor:CKEditorControl ID="contentCKEditor" BasePath="~/Scripts/ckeditor" runat="server" Height="300"></CKEditor:CKEditorControl>
                </div>


                <div class="field-group" style="margin-top:40px;">
                    <div class="label">Contacts</div><%--Comm. Contacts--%>
                    <div id="contacts" class="comm-contacts" style="padding: 0 5px 5px 0;">
                        <asp:Repeater ID="rptMediaContactEdit" runat="server" DataSource="<%# Contacts %>" ItemType="System.String">
                            <ItemTemplate>
                                <textarea name="<%# "parameters" + DocumentPath + ":" + Container.ItemIndex.ToString() %>" class="media-textarea"><%# Item %></textarea>
                            </ItemTemplate>
                            <FooterTemplate>
                                <textarea name="<%# "parameters" + DocumentPath + ":" + rptMediaContactEdit.Items.Count.ToString() %>" class="media-textarea"></textarea>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <br />
                <br />

                <div class="actions">
                    <asp:Button ID="btnSaveDocument" runat="server" Text="Save" CssClass="primary" OnClick="btnSaveDocument_Click" OnClientClick="if (!IsDocumentValid(this))return false;" />
                    <asp:LinkButton ID="lbtnCancelSave" runat="server" Text="Cancel" CssClass="cancel" OnClick="lbtnCancelSave_Click" OnClientClick ="onCancel(this)"/>
                </div>
            </asp:Panel>
    </div>

</div>
