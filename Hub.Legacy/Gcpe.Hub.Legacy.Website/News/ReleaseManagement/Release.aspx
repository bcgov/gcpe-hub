<%@ Page Language="C#" MasterPageFile="~/News/ReleaseManagement/ReleaseManagement.master" AutoEventWireup="true" CodeBehind="Release.aspx.cs" Inherits="Gcpe.Hub.News.ReleaseManagement.Release" ValidateRequest="false" Async="true" %>

<%@ MasterType TypeName="Gcpe.Hub.News.ReleaseManagement.ReleaseManagement" %>
<%@ Import Namespace="Gcpe.Hub" %>
<%@ Import Namespace="Gcpe.Hub.Data.Entity" %>
<%@ Import Namespace="Gcpe.Hub.News" %>
<%@ Register Assembly="Gcpe.News.ReleaseManagement.Controls" Namespace="Gcpe.News.ReleaseManagement.Controls" TagPrefix="controls" %>


<%@ Register TagPrefix="controls" TagName="ReleaseNavigationBar" Src="~/News/ReleaseManagement/Controls/ReleaseNavigationBar.ascx" %>
<%@ Register TagPrefix="controls" TagName="Document" Src="~/News/ReleaseManagement/Controls/Document.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="formContentPlaceHolder" runat="server">
    <script>
        var isPublishDateRequired = false;

        function pageLoad() {
            var translationsAnchor = $("#TranslationsAnchorPanel");
            if (translationsAnchor.length) {
                $('html, body').animate({
                    scrollTop: $('#Assets').offset().top + $('#Assets').outerHeight(true)
                }, 1000);
            }

            $("#FileInput1").change(function () {
                if (!$("#chkHasTranslations").is(":checked")) {
                    $("#formContentPlaceHolder_formContentPlaceHolder_chkHasTranslations").prop("checked", true);
                }
            });

            function getPhotoId(url) {
                var photoId = "";

                if (url.includes('www.flickr.com') || url.includes('flic.kr')) { // handle urls for private images on Flickr
                    var segments = url.split('/');
                    photoId = segments[5];
                } else if (url.includes('staticflickr.com')) { // handle urls for public images
                    var segments = url.split('/');
                    photoId = segments[4].split('_')[0];
                }

                return photoId;
            }

            var lockFlickrAsset = function () {
                $.ajax({
                    type: "POST",
                    url: "../FlickrHandler.ashx",
                    data: { photoId: getPhotoId('<%= Model.Asset %>') },
                    dataType: "json"
                });
            };

            var isPublished = '<%= Model.IsPublished.ToString().ToLower() %>';

            if (!isPublished) {
                lockFlickrAsset();
            }

            // Locations
            //  - set up autocomplete 
            $("#<%= txtLocation.ClientID %>").autocomplete({
            minLength: 0,
            source: function (request, response) {
                var data = $.grep([ <%= EnglishLocations.ToJavaScript() %>], function (value) {
                        return value.substring(0, request.term.length).toLowerCase() == request.term.toLowerCase();
                    });
                    response(data);
                }
        }).keydown(function (event) { /* Catches enter button click so it does not submit form  */
            if (event.keyCode == 13) {
                return false;
            }
        }).keyup(function (e) { /* Catches enter button click so it does not submit form  */
            if (e.which === 13) {
                $(".ui-menu-item").hide();
            }
        });

        $('#<%= rblMinistry.ClientID %>').click(function () {
                $('#<%= rblMinistry.ClientID %>').removeClass("warning-rdlst");
            });

            $("#<%= btnApprove.ClientID%>").click(function () {
                return IsApproveFormValid();
            });

            $("#<%= btnSaveCategories.ClientID %>").click(function () {
                return IsCategoriesFormValid();
            });

            $("#<%= btnSaveAssets.ClientID %>").click(function () {
                var assetUrl = $("#<%= txtAsset.ClientID %>").val();

                if (assetUrl.includes('www.flickr.com') || assetUrl.includes('flic.kr') || assetUrl.includes('staticflickr.com')) {
                    var sp = new StatusPage.page({ page : '9htz5wc2q8lk' });
                    sp.incidents({
                        filter: 'unresolved',
                        success: function (data) {
                            if (data.incidents.length > 0) {
                                alert('Please note that Flickr has been experiencing technical issues within the past few days.' +
                                    ' As a result, you may experience degraded performance and/or upload failures when using a Flickr post as the super asset.');
                            }
                        }
                    });
                }
                
                return IsAssetsFormValid();
            });

            $("#<%= btnSaveTranslations.ClientID %>").click(function () {
                return IsAssetsFormValid();
            });

            $("#<%= btnSaveMeta.ClientID %>").click(function () {
                return IsMetaFormValid();
            });


            $("#<%= btnSavePlannedPublishDate.ClientID %>").click(function () {
                var valid = IsPlannedPublishDateFormValid();
                var medlist = IsMediaDistributionListValid();
                return IsCorpCalIdValid() && valid && medlist;
            });

            $("#<%= btnDelete.ClientID %>").click(function () {
                return confirm("Are you sure you want to delete this release and all associated documents?");
            });

            $("#<%= EditSwitch.ClientID %>").click(function () {
                HideAllSettings();
                $("#ReleaseSettingsEdit").removeClass('disable-section');
                $("#ReleaseSettingsEdit").show();
                $("#PublishSettingsView").hide();
                return false;
            });

            $("#CancelSavePlannedPublishDate").click(function () {
                $("#ReleaseSettingsEdit").hide();
                $("#PublishSettingsView").show();
                ShowAllSettings();
                return false;
            });

            $("#<%= PublishSwitch.ClientID %>").click(function () {
                HideAllSettings();
                $("#ReleaseSettingsPublish").removeClass('disable-section');
                $("#ReleaseSettingsPublish").show();
                $("#PublishSettingsView").hide();
                return false;
            });

            $("#CancelPublish").click(function () {
                $("#ReleaseSettingsPublish").hide();
                $("#PublishSettingsView").show();
                ShowAllSettings();
                return false;
            });

            $("#<%= ApproveSwitch.ClientID %>").click(function () {
                HideAllSettings();
                $("#ReleaseSettingsApprove").removeClass('disable-section');
                $("#ReleaseSettingsApprove").show();
                $("#PublishSettingsView").hide();
                return false;
            });

            $("#CancelApprove").click(function () {
                $("#ReleaseSettingsApprove").hide();
                $("#PublishSettingsView").show();
                ShowAllSettings();
                return true; // so it posts back
            });


            $("#<%= lbtnUnpublishSwitch.ClientID %>").click(function () {
                return confirm("Are you sure you want to unpublish this release?");
            });

            $("#<%= ReleaseCategoriesEdit.ClientID %>").click(function () {
                HideAllSettings();
                $("#<%= CategoriesEdit.ClientID %>").removeClass('disable-section');
                $("#CategoriesView").hide();
                $("#<%= CategoriesEdit.ClientID %>").show();
                return false;
            });

            $("#<%= lbtnCancelCategories.ClientID %>").click(function () {
                ShowAllSettings();
                $("#<%= CategoriesEdit.ClientID %>").hide();
                $("#CategoriesView").show();
                return true;
            });

            $("#<%= AssetsEditSwitch.ClientID %>").click(function () {
                HideAllSettings();
                $("#AssetsEdit").removeClass('disable-section');
                $("#AssetsView").hide();
                $("#AssetsEdit").show();
                return false;
            });

            $("#<%= TranslationsEditSwitch.ClientID %>").click(function () {
                HideAllSettings();
                $("#TranslationsEdit").removeClass('disable-section');
                $("#TranslationsView").hide();
                $("#TranslationsEdit").show();
                return false;
            });

            $("#<%= lbtnCancelAssets.ClientID %>").click(function () {
                ShowAllSettings();
                $("#AssetsEdit").hide();
                $("#AssetsView").show();
                return true;
            });

            $("#<%= lbtnCancelTranslations.ClientID %>").click(function () {
                ShowAllSettings();
                $("#TranslationsEdit").hide();
                $("#TranslationsView").show();
                return true;
            });

            $("#<%= MetaEditSwitch.ClientID %>").click(function () {
                HideAllSettings();
                $("#<%= MetaEdit.ClientID %>").removeClass('disable-section');
                $("#MetaView").hide();
                $("#<%= MetaEdit.ClientID %>").show();
                return false;
            });

            $("#<%= lbtnCancelMeta.ClientID %>").click(function () {
                ShowAllSettings();
                $("#<%= MetaEdit.ClientID %>").hide();
                $("#MetaView").show();
                return true;
            });

            MapScrollItems();

            CKEDITOR.on('instanceReady', function (ev) {
                ev.editor.on('paste', function (e) {
                    e.data.dataValue = e.data.dataValue.replace(/<p>\&nbsp\;<\/p>/g, '');
                });
            });

            $('#<%= plannedPublishDateTimePicker.ClientID %>').kendoDateTimePicker({
                format: "yyyy-MM-dd hh:mm tt",
                change: function (e) {
                    PublishDateTimePicker_Validate($('#<%= plannedPublishDateTimePicker.ClientID %>'));
                }
            });

            $('#<%= releaseDateTimePicker.ClientID %>').kendoDateTimePicker({
                format: "yyyy-MM-dd hh:mm tt",
                change: function (e) {
                    ReleaseDateTimePicker_Validate($('#<%= releaseDateTimePicker.ClientID %>'));
                }
            });

            // ----------------------------------------------------------------------
            // MediaDistribution multiselect
            // ----------------------------------------------------------------------
            $('#<%= mediaDistributionListBox.ClientID %>').kendoMultiSelect({
                autoClose: false,
                filter: "contains",
                enable: <%= (mediaDistributionListBox.Enabled ? "true" : "false") %>,
                change: function () {
                    // Update the checkbox to reflect current selection.
                    var sendingToMediaDistribution = (this.value() && this.value().length > 0);
                    $('#<%= chkMediaContacts.ClientID %>').prop('checked', sendingToMediaDistribution);
                }
            });

            $('#mediaListPicker').show();


            // Hide a bunch of stuff when ReleaseType is Advisory.
            <% if (Model.ReleaseTypeId == ReleaseType.Advisory)
        { %>
            $('.hideForAdvisories').hide();
            <% } %>

        }
    </script>
    <script>

        function ShowAllSettings() {
            $(".section").removeClass('disable-section');
            $("#SettingsStatus").removeClass('disable-section');
            $("#SettingsTitle").removeClass('disable-section');
            $("#SettingsApproval").removeClass('disable-section');
        }

        function HideAllSettings() {

            $(".section").addClass('disable-section');
            $("#SettingsStatus").addClass('disable-section');
            $("#SettingsTitle").addClass('disable-section');
            $("#SettingsApproval").addClass('disable-section');

        }

        function OnCheckClick(checkBox) {
            $(checkBox).parent().parent().find('a').css('visibility', checkBox.checked ? '' : 'hidden')
        }

        function isValidDateFormat(value) {
            //2011-05-16 12:50 am
            reg = /^((199\d)|([2-9]\d{3}))\-(([0][1-9])|([1][0-2]))\-(([0-2]?\d)|([3][01]))\s(([0]?\d)|([1][0-2])):[0-5][0-9] (am|pm|AM|PM)?$/
            return reg.test(value);
        }

        /* For planned publish date */
        var plannedPublishDateIsValid = true;

        function PublishDateTimePicker_Validate(sender, args) {

            var root = $(sender).parent().closest('div .field-group');
            var value = $(sender).val().trim();

            root.removeClass('warning-date');
            root.removeClass('warning');

            if (isPublishDateRequired && value == "") {
                root.addClass('warning');
                plannedPublishDateIsValid = false;
            }
            else if (value != "" && !isValidDateFormat(value)) {
                root.addClass('warning-date');
                plannedPublishDateIsValid = false;
            }
            else {
                plannedPublishDateIsValid = true;
            }
        }


        function ReleaseDateTimePicker_Validate(sender, args) {

            var root = $(sender).parent().closest('div .field-group');
            var value = $(sender).val().trim();
            root.removeClass('warning-date');
            if (value != "" && !isValidDateFormat(value)) {
                root.addClass('warning-date');
            }
        }

    </script>

    <script>

        function IsCorpCalIdValid() {
            var activityId = $('#<%= txtCorpCalId.ClientID %>').val().trim();
            if (activityId != "") {
                var numericExpression = /^[0-9]+$/;
                if (!activityId.match(numericExpression)) {
                    $("#<%= txtCorpCalId.ClientID %>").parents('div .field-group').addClass('warning-number');
                    $('#<%= txtCorpCalId.ClientID %>').focus();
                    return false;
                }
            }
            $("#<%= txtCorpCalId.ClientID %>").parents('div .field-group').removeClass('warning-number');
            return true;
        }

        function IsMediaDistributionListValid() {
            var selectedReleaseType = "<%= Model.ReleaseTypeName %>";
            var lbxMediaDistribution = $("#<%= mediaDistributionListBox.ClientID %>");
            <% if (Model.ReleaseTypeId == ReleaseType.Advisory)
        { %>
            // Make sure that at least one Media Distribution List is selected.
            if (!lbxMediaDistribution.val()) {
                lbxMediaDistribution.parents('div .field-group').addClass('warning-chkbxlst');
                lbxMediaDistribution.focus();
                return false;
            }
            <% } %>
            lbxMediaDistribution.parents('div .field-group').removeClass('warning-number');
            return true;
        }

        function IsPlannedPublishDateFormValid() {
            if (!plannedPublishDateIsValid) {
                $("#<%= plannedPublishDateTimePicker.ClientID %>").parents('div .field-group').addClass('warning-date');
                $("#<%= plannedPublishDateTimePicker.ClientID %>").focus();// focus to textbox element
                return false;
            }
            $("#<%= plannedPublishDateTimePicker.ClientID %>").parents('div .field-group').removeClass('warning-date');
            return true;
        }

        function IsApproveFormValid() {

            if ($("#<%= rblMinistry.ClientID %>").length > 0) { // element is found
                if ($("#<%= rblMinistry.ClientID %> :radio:checked").length == 0) {
                    $('#<%= rblMinistry.ClientID %>').parents('div .field-group').addClass('warning-rdlst');
                    $('#<%= rblMinistry.ClientID %>').focus();
                    return false;
                }
            }
            return true;
        }

        function IsCategoriesFormValid() {
            var fieldArea = null;
            var firstError = null;


            <% if (Model.ReleaseTypeId != ReleaseType.Advisory)
        { %>
            firstError = ValidateCheckBoxList($("#tblMinistry"), firstError);
            firstError = ValidateCheckBoxList($("#tblSector"), firstError);
            <% } %>

            if (firstError != null) {
                $('html, body').animate({ scrollTop: firstError.offset().top - 75 }, 500);// scroll to the position
                firstError.find(":input").focus(); // focus on field
                return false;
            } else {
                return true;
            }

        }

        function IsAssetsFormValid() {

            return true;

            <%--var firstError = null;
            var fieldArea = null;

            if ($("#<%= txtAsset.ClientID %>").val() == '') {
                $("#<%= txtAsset.ClientID %>").parents('div .field-group').addClass('warning');
                if (firstError == null)
                    firstError = $("#<%= txtAsset.ClientID %>");
            }

            if (firstError != null) {
                $('html, body').animate({ scrollTop: firstError.offset().top - 75 }, 500);// scroll to the position
                firstError.find(":input").focus(); // focus on field
                return false;
            } else {
                return true;
            }--%>

        }

        function IsMetaFormValid() {
            return true;
        }
        <% // If there is a server error, we want them to see it so scroll to the top %>
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(onPageReloaded);


        function OnHasMediaAssetsClick(checkBox) {

            if (checkBox.checked) {
                $("#AssetUploadBlock").show();
            } else {
                $("#AssetUploadBlock").hide();
                $("#FileInput").replaceWith($("#FileInput").clone(true));
            }
        }

        function OnHasTranslationsClick(checkBox) {

            if (checkBox.checked) {
                $("#TranslationUploadBlock").show();
            } else {
                $("#TranslationUploadBlock").hide();
                $("#FileInput1").replaceWith($("#FileInput1").clone(true));
            }
        }


        function OnAssetDeleteSwitch(link) {
            deleted = (link.text != "Cancel");

            file = $('td:first span', $(link).parent().parent());

            if (deleted) {
                file.css("text-decoration", "line-through");
                link.text = "Cancel";

            } else {
                file.css("text-decoration", "");
                link.text = "Delete";
            }

            $(link).parent().find("input[type=hidden]").val(deleted);

        }

        function OnTranslationDeleteSwitch(link) {
            deleted = (link.text != "Cancel");

            file = $('td:first span', $(link).parent().parent());

            if (deleted) {
                file.css("text-decoration", "line-through");
                link.text = "Cancel";

            } else {
                file.css("text-decoration", "");
                link.text = "Delete";
            }

            $(link).parent().find("input[type=hidden]").val(deleted);

        }

        function SaveAssets() {
            if ("<%# AssetUploadBlock.Visible%>".toLowerCase() == "true"
                && document.getElementById("FileInput").files != null
                && document.getElementById("FileInput").files.length > 0) {
                startUpload('<%= ResolveUrl("~/News/MediaAssetManagement.asmx") %>', 'FileInput', 4 * 1024 * 1024, 'uploadProgress', 'uploadStatusMessage', '', '', 'uploadkey', 'uploadPath', 'hdnAssetButton');
            } else {
                $("#hdnAssetButton").click();
            }
        }

        function SaveTranslations() {
            if ("<%# TranslationUploadBlock.Visible%>".toLowerCase() == "true"
                && document.getElementById("FileInput1").files != null
                && document.getElementById("FileInput1").files.length > 0) {
                startUpload('<%= ResolveUrl("~/News/TranslationManagement.asmx") %>', 'FileInput1', 4 * 1024 * 1024, 'uploadProgress1', 'uploadStatusMessage1', '', '', 'uploadkey1', 'uploadPath1', 'hdnTranslationButton');
            } else {
                $("#hdnTranslationButton").click();
            }
        }
    </script>

    <div id="two-column">
        <div id="left-menu">
            <controls:ReleaseNavigationBar ID="ctrlNavBar" runat="server" Model="<%# Model %>" />
        </div>
        <div id="main-content">
            <div id="Top">
                <h1>
                    <asp:Literal ID="ltrReleaseName" runat="server" Text="<%# Model.FirstHeadline %>" Mode="Encode" />
                </h1>
            </div>
            <p>
                <asp:Literal ID="ltrRssSummay" runat="server" Mode="Encode" Text="<%# Model.LocationSummary %>" />
            </p>

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

            <div id="ReleaseSettingsApprove" class="section edit">
                <h2>Approve <%= Model.ReleaseTypeName%></h2>

                <div class="helper" style="padding-bottom: 20px; font-size: 1.2em;">
                    <% if (Model.ReleaseTypeId == ReleaseType.Release)
                        { %>
                    Once approved, a permanent reference number will be generated and the release may be published.
                    <% }
                        else if (Model.ReleaseTypeId == ReleaseType.Advisory)
                        { %>
                    Once approved, the release may be published.
                    <% }
                        else
                        { %>
                    Once approved, a lead ministry will be permanently assigned and the <%# Model.ReleaseTypeName.ToLower() %> may be published.
                    <% } %>
                </div>

                <asp:Panel runat="server" ID="pnlSelectMinistry" Visible="<%# Model.Ministries.Where(item => item.Selected).Count() > 1 %>">
                    <div class="field-group required">
                        <div class="label">Which of the ministries is the lead ministry?</div>
                        <div class="txt">
                            <asp:RadioButtonList ID="rblMinistry" runat="server" DataSource="<%# Model.Ministries.Where(item => item.Selected) %>" ItemType="ListItem<Guid>" DataTextField="Text" DataValueField="Value" OnDataBound="rblMinistry_DataBound" />
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlShowMinistry" Visible="<%# Model.Ministries.Where(item => item.Selected).Count() == 1 %>">
                    <div class="field-group">
                        <div class="label">Lead Ministry</div>
                        <div class="txt">
                            <%# Model.Ministries.Where(item => item.Selected).Select(item => item.Text).FirstOrDefault() %>
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlNoMinistry" Visible="<%# Model.Ministries.Where(item => item.Selected).Count() == 0 %>">
                    <div class="field-group">
                        <div class="label">Lead Organization</div>
                        <div class="txt">
                            <%# Model.NewsReleaseDocuments.Select(item => item.Languages).FirstOrDefault().Select(x => String.IsNullOrEmpty(x.Organizations) ? String.IsNullOrEmpty(x.Byline) ? "Third Party" : x.Byline : x.Organizations).FirstOrDefault() %>
                        </div>
                    </div>
                </asp:Panel>

                <div class="inform-actions">
                    <asp:Button ID="btnApprove" runat="server" Text="Ok" CssClass="primary" OnClick="btnApprove_Click" />
                    <asp:LinkButton runat="server" ID="CancelApprove" ClientIDMode="Static" CssClass="cancel" Text="Cancel" OnClick="CancelApprove_Click" />
                </div>
            </div>

            <div id="PublishSettingsView" class="section view">
                <h2 class="section-title">Publish</h2>

                <div class="section-action-options">
                    <asp:HyperLink runat="server" ID="EditSwitch" NavigateUrl="#" CssClass="switch" Visible="<%# Model.CanEdit && (!Model.IsCommitted && !Model.IsPublished) %>">Edit</asp:HyperLink>
                </div>

                <div id="SettingsStatus" style="margin-bottom: 0; padding-bottom: 0;">

                    <div class="view-group">
                        <div class="lbl">News Type</div>
                        <div class="txt">
                            <%# Model.ReleaseTypeName %>
                        </div>
                    </div>

                    <div class="view-group">
                        <div class="lbl">Activity ID</div>
                        <div class="txt">
                            <%# Model.ActivityId.HasValue ? "<a href=\"" + ResolveUrl("~/Calendar/Activity.aspx?ActivityId=" + Model.ActivityId.Value.ToString()) + "\">" + Model.ActivityId.Value.ToString() + "</a>" : "None" %>
                        </div>
                    </div>

                    <%--<div class="status" style="font-size:1em;"><%# ReleaseStatusHtml %></div>--%>

                    <div class="view-group">
                        <div class="lbl"><%: Model.IsCommitted ? "" : "Planned " %>Publish Date</div>
                        <div class="txt">
                            <%: PublishDateHtml %>
                        </div>
                    </div>

                    <div class="view-group">
                        <div class="lbl">Publish to</div>
                        <ul class="txt" style="margin-top: 0px; list-style: none; padding-left: 0;">
                            <% if (Model.PublishNewsArchives)
                                { %>
                            <li>BC Gov News</li>
                            <% } %>
                            <% if (Model.PublishNewsOnDemand)
                                { %>
                            <li>News On Demand <%= Model.NodSubscriberCount %></li>
                            <% } %>
                            <% if (Model.PublishMediaContacts)
                                { %>
                            <li>Media Distribution Lists <%= Model.MediaSubscriberCount %>
                                <asp:Literal runat="server" ID="literalSelectedLists" Text="<%# MediaDistributionListDisplay() %>" />
                            </li>
                            <% } %>
                        </ul>

                    </div>
                </div>

                <asp:Panel runat="server" Visible="<%# Model.CanEdit %>" Style="min-height: 36px">
                    <asp:HyperLink runat="server" CssClass="doaction" NavigateUrl="#" ID="ApproveSwitch" ClientIDMode="Static" Visible="<%# !Model.IsApproved %>">Approve</asp:HyperLink>
                    <asp:HyperLink runat="server" ID="PublishSwitch" NavigateUrl="#" CssClass="doaction" Visible="<%# (Model.IsApproved && !Model.IsCommitted ) %>">Publish</asp:HyperLink>
                    <%--<asp:LinkButton runat="server" ID="lbtnStopPublishSwitch" NavigateUrl="#" CssClass="doaction" Visible="<%# (  Model.IsApproved &&  Model.IsCommitted && !Model.IsPublished ) %>" OnClick="lbtnStopPublishSwitch_Click">Stop Publishing</asp:LinkButton>--%>
                    <% if (Model.ReleaseTypeId == ReleaseType.Advisory && Model.IsCommitted && !Model.IsScheduled)
                        { %>
                            Advisory has been published, there is no Unpublish for this Release Type.
                    <% }
                        else
                        { %>
                    <asp:LinkButton runat="server" ID="lbtnUnpublishSwitch" NavigateUrl="#" CssClass="doaction" OnClick="lbtnUnpublishSwitch_Click" Visible="<%# (Model.IsApproved && Model.IsCommitted) %>">Unpublish</asp:LinkButton>
                    <% } %>
                </asp:Panel>

            </div>

            <div id="ReleaseSettingsEdit" class="section edit">
                <h2 style="padding-bottom: 0;">Publish</h2>

                <div class="view-group">
                    <div class="lbl">News Type</div>
                    <div class="txt">
                        <%# Model.ReleaseTypeName %>
                    </div>
                </div>

                <div class="field-group">
                    <div class="label">Activity ID</div>
                    <div class="txt">
                        <div>
                            <asp:TextBox runat="server" ID="txtCorpCalId" Text="<%# Model.ActivityId %>" />
                        </div>
                    </div>
                </div>
                <br />
                <div class="helper">Enter the date you plan to publish the release.</div>
                <br />
                <div class="field-group">
                    <div class="label">Planned Publish Date</div>
                    <div class="txt">
                        <asp:TextBox ID="plannedPublishDateTimePicker" runat="server"></asp:TextBox>
                    </div>
                </div>

                <div class="field-group">
                    <div class="label">Publish to</div>
                    <div class="txt">
                        <div>
                            <asp:CheckBox runat="server" ID="chkNewsArchives" Enabled="false" Text="BC Gov News" Checked="<%# Model.PublishNewsArchives %>" />
                        </div>
                        <div>
                            <asp:CheckBox runat="server" ID="chkNewsOnDemand" Enabled="<%# Model.AllowPublishToNewsOnDemand %>" Checked="<%# Model.PublishNewsOnDemand %>" Text='<%# "News On Demand "+ Model.NodSubscriberCount %>' />
                        </div>
                        <div>
                            <asp:CheckBox runat="server" ID="chkMediaContacts" Enabled="false" Text='<%# "Media Distribution Lists " + Model.MediaSubscriberCount %>' Checked="<%# Model.PublishMediaContacts %>" />
                        </div>
                    </div>
                </div>

                <% if (Model.ReleaseTypeId == ReleaseType.Story || Model.ReleaseTypeId == ReleaseType.Update)
                    {  %>
                <!-- Media Contacts are not available for Story or Update ReleaseTypes. -->
                <% }
                    else
                    { %>
                <div id="mediaListPicker" class="field-group" style="width: 500px;">
                    <div class="label">Media Distribution Lists</div>
                    <asp:ListBox ID="mediaDistributionListBox" SelectionMode="Multiple" AutoPostBack="false" runat="server"></asp:ListBox>
                </div>
                <% } %>

                <div class="inform-actions">
                    <asp:Button ID="btnSavePlannedPublishDate" runat="server" Text="Save" CssClass="primary" OnClick="btnSavePlannedPublishDate_Click" />
                    <asp:LinkButton runat="server" CssClass="cancel" Text="Cancel" ID="CancelSavePlannedPublishDate" ClientIDMode="Static" OnClick="CancelSavePlannedPublishDate_Click" />
                </div>
            </div>

            <div id="ReleaseSettingsPublish" class="section edit">
                <h2>Publish</h2>
                <div class="helper"></div>

                <div style="padding: 10px 0;">
                    This release will be published <%: !Model.PublishDateTime.HasValue || Model.PublishDateTime.Value < UtcPage ? "now" : string.Format("as planned on " + FormatDateTime(Model.PublishDateTime.Value)) %>.
                </div>
                <div>
                    It will be published to:
                    <ul class="txt" style="margin-top: 0px; list-style: none; padding-left: 0;">
                        <% if (Model.PublishNewsArchives)
                            { %>
                        <li>BC Gov News</li>
                        <% } %>
                        <% if (Model.PublishNewsOnDemand)
                            { %>
                        <li>News On Demand <%= Model.NodSubscriberCount %></li>
                        <% } %>
                        <% if (Model.PublishMediaContacts)
                            { %>
                        <li>Media Distribution Lists <%= Model.MediaSubscriberCount %>
                            <asp:Literal runat="server" ID="literal2" Text="<%# MediaDistributionListDisplay() %>" />
                        </li>
                        <% } %>
                    </ul>
                </div>

                <br />

                <div class="inform-actions">
                    <asp:Button ID="btnPublish" runat="server" Text="Publish" CssClass="primary" OnClick="btnPublish_Click" />
                    <a href="#" id="CancelPublish" class="cancel">Cancel</a>
                </div>

            </div>

            <div id="Categories">
                <div id="CategoriesView" class="section view">
                    <h2 class="section-title">Categories</h2>

                    <div class="section-action-options">
                        <asp:HyperLink runat="server" NavigateUrl="#" ID="ReleaseCategoriesEdit" CssClass="switch" ClientIDMode="Static" Visible="<%# Model.CanEdit %>">Edit</asp:HyperLink>
                    </div>

                    <% if (!string.IsNullOrEmpty(lblViewHomeTopOrFeature.Text) && Model.IsPublished && Model.IsCommitted)
                        { %>
                    <div class="view-group hideForAdvisories">
                        <asp:Literal runat="server" Text="Home" />
                        <asp:Label ID="lblViewHomeTopOrFeature" runat="server" CssClass="TopOrFeature" Text='<%# HomeTopOrFeatureLabel %>' />
                    </div>
                    <% } %>


                    <div class="view-group">
                        <div class="lbl">Ministries</div>
                        <table class="txt">
                            <% if (Model.Ministries.Any(item => item.Selected))
                                { %>
                            <asp:Repeater runat="server" DataSource="<%# Model.Ministries.OrderBy(m=>Model.MinistryId.HasValue ? (Model.MinistryId.Value == m.Value ? 0 : 1) : 0).Where(item => item.Selected) %>" ItemType="CategoryItem<Guid>">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:Label runat="server" Text="<%# Item.Text %>" Font-Bold="<%# Model.MinistryId.HasValue && Model.MinistryId.Value == Item.Value %>" /></td>
                                        <td>
                                            <asp:Label runat="server" CssClass="TopOrFeature" Text='<%# TopOrFeatureLabel(Item) %>' /></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <%
                                }
                                else
                                {
                            %>
                            <tr>
                                <td>None</td>
                            </tr>
                            <%
                                }
                            %>
                        </table>
                    </div>

                    <% if (Model.ReleaseTypeId != ReleaseType.Advisory)
                        { %>
                    <div class="view-group">
                        <div class="lbl">Sectors</div>
                        <table class="txt">
                            <% if (Model.Sectors.Any(item => item.Selected))
                                { %>
                            <asp:Repeater DataSource="<%# Model.Sectors.Where(item => item.Selected) %>" ItemType="CategoryItem<Guid>" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:Literal runat="server" Text="<%# Item.Text %>" Mode="Encode" /></td>
                                        <td>
                                            <asp:Label runat="server" CssClass="TopOrFeature" Text='<%# TopOrFeatureLabel(Item) %>' /></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <%
                                }
                                else
                                {
                            %>
                            <tr>
                                <td>None</td>
                            </tr>
                            <%
                                }
                            %>
                        </table>
                    </div>
                    <% } %>

                    <% if (Model.ReleaseTypeId != ReleaseType.Advisory)
                        { %>
                    <div class="view-group">
                        <div class="lbl">Themes</div>
                        <table class="txt">
                            <% if (Model.Themes.Any(item => item.Selected))
                                {
                            %>
                            <asp:Repeater DataSource="<%# Model.Themes.Where(item => item.Selected) %>" ItemType="CategoryItem<Guid>" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:Literal runat="server" Text="<%# Item.Text %>" Mode="Encode" /></td>
                                        <td>
                                            <asp:Label runat="server" CssClass="TopOrFeature" Text='<%# TopOrFeatureLabel(Item) %>' /></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <%
                                }
                                else
                                {
                            %>
                            <tr>
                                <td>None</td>
                            </tr>
                            <%
                                }
                            %>
                        </table>
                    </div>
                    <% } %>

                    <% if (Model.ReleaseTypeId != ReleaseType.Advisory)
                        { %>
                    <div class="view-group">
                        <div class="lbl">Tags</div>
                        <div class="txt">
                            <% if (Model.Tags.Any(item => item.Selected))
                                {
                            %>
                            <asp:Repeater DataSource="<%# Model.Tags.Where(item => item.Selected) %>" ItemType="ListItem<Guid>" runat="server">
                                <ItemTemplate>
                                    <div>
                                        <asp:Literal runat="server" Text="<%# Item.Text %>" Mode="Encode" />
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <%
                                }
                                else
                                {
                            %><div>None</div>
                            <%
                                }
                            %>
                        </div>
                    </div>
                    <% } %>
                </div>

                <asp:Panel runat="server" ID="CategoriesEdit" ClientIDMode="Static" CssClass="section edit" Visible="<%# Model.CanEdit %>">
                    <h2>Categories</h2>

                    <div class="field-group required hideForAdvisories ToggleTopOrFeature">
                        <div>
                            <asp:Literal runat="server" Text="Home" />
                            <asp:Label ID="lblHomeTopOrFeature" runat="server" CssClass="TopOrFeature" Text='<%# HomeTopOrFeatureLabel %>' />
                            <asp:HyperLink ID="homeTopSwitch" runat="server" NavigateUrl="#" CssClass="switch" onclick="return OnTopOrFeatureSwitch(this);">Set as Top</asp:HyperLink>
                            <asp:HiddenField ID="valHomeTopOrFeature" runat="server" Value="" />
                            <asp:HyperLink ID="homeFeatureSwitch" runat="server" NavigateUrl="#" CssClass="switch" onclick="return OnTopOrFeatureSwitch(this);">Set as Feature</asp:HyperLink>
                        </div>
                    </div>

                    <div class="field-group required">
                        <div class="label">Ministries</div>
                        <table id="tblMinistry" class="txt">
                            <asp:Repeater ID="rptMinistry" DataSource="<%# Model.Ministries.OrderBy(m=>Model.MinistryId.HasValue ? (Model.MinistryId.Value == m.Value ? 0 : 1) : 0) %>" runat="server" OnItemDataBound="rptCategory_DataBound">
                                <ItemTemplate>
                                    <tr class="ToggleTopOrFeature">
                                        <td>
                                            <asp:CheckBox ID="chkCategory" runat="server" onclick="OnCheckClick(this);" /></td>
                                        <td style="text-align: center">
                                            <div class="hideForAdvisories">
                                                <asp:Label ID="lblTopOrFeature" runat="server" CssClass="TopOrFeature" />
                                                <asp:HyperLink ID="topSwitch" runat="server" NavigateUrl="#" CssClass="switch" onclick="return OnTopOrFeatureSwitch(this);">Set as Top</asp:HyperLink>
                                                <asp:HiddenField ID="valTopOrFeature" runat="server" Value="" />
                                            </div>
                                        </td>
                                        <td>
                                            <div class="hideForAdvisories">
                                                <asp:HyperLink ID="featureSwitch" runat="server" NavigateUrl="#" CssClass="switch" onclick="return OnTopOrFeatureSwitch(this);">Set as Feature</asp:HyperLink>
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                    </div>

                    <div class="field-group required hideForAdvisories">
                        <div class="label">Sectors</div>
                        <table id="tblSector" class="txt">
                            <asp:Repeater ID="rptSector" DataSource="<%# Model.Sectors %>" runat="server" OnItemDataBound="rptCategory_DataBound">
                                <ItemTemplate>
                                    <tr class="ToggleTopOrFeature">
                                        <td>
                                            <asp:CheckBox ID="chkCategory" runat="server" onclick="OnCheckClick(this);" /></td>
                                        <td style="text-align: center">
                                            <asp:Label ID="lblTopOrFeature" runat="server" CssClass="TopOrFeature" />
                                            <asp:HyperLink ID="topSwitch" runat="server" NavigateUrl="#" CssClass="switch" onclick="return OnTopOrFeatureSwitch(this);">Set as Top</asp:HyperLink>
                                            <asp:HiddenField ID="valTopOrFeature" runat="server" Value="" />
                                        </td>
                                        <td>
                                            <asp:HyperLink ID="featureSwitch" runat="server" NavigateUrl="#" CssClass="switch" onclick="return OnTopOrFeatureSwitch(this);">Set as Feature</asp:HyperLink></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                    </div>

                    <div class="field-group required hideForAdvisories">
                        <div class="label">Themes</div>
                        <table class="txt">
                            <asp:Repeater ID="rptTheme" DataSource="<%# Model.Themes %>" runat="server" OnItemDataBound="rptCategory_DataBound">
                                <ItemTemplate>
                                    <tr class="ToggleTopOrFeature">
                                        <td>
                                            <asp:CheckBox ID="chkCategory" runat="server" onclick="OnCheckClick(this);" /></td>
                                        <td style="text-align: center">
                                            <asp:Label ID="lblTopOrFeature" runat="server" CssClass="TopOrFeature" />
                                            <asp:HyperLink ID="topSwitch" runat="server" NavigateUrl="#" CssClass="switch" onclick="return OnTopOrFeatureSwitch(this);">Set as Top</asp:HyperLink>
                                            <asp:HiddenField ID="valTopOrFeature" runat="server" Value="" />
                                        </td>
                                        <td>
                                            <asp:HyperLink ID="featureSwitch" runat="server" NavigateUrl="#" CssClass="switch" onclick="return OnTopOrFeatureSwitch(this);">Set as Feature</asp:HyperLink></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                    </div>

                    <div class="field-group required hideForAdvisories">
                        <div class="label">Tags</div>
                        <div class="txt">
                            <asp:CheckBoxList ID="cklstTag" runat="server" DataSource="<%# Model.Tags %>" DataTextField="Text" DataValueField="Value" OnDataBound="cklstTag_DataBound" />
                        </div>
                    </div>

                    <div class="inform-actions">
                        <asp:Button ID="btnSaveCategories" runat="server" Text="Save" CssClass="primary" OnClick="btnSaveCategories_Click" />
                        <asp:LinkButton runat="server" ID="lbtnCancelCategories" CssClass="cancel" Text="Cancel" OnClick="lbtnCancelCategories_Click"></asp:LinkButton>
                    </div>
                </asp:Panel>
            </div>

            <div id="Assets" class="hideForAdvisories">
                <div id="AssetsView" class="section view">
                    <h2 class="section-title">Assets</h2>
                    <div class="section-action-options">
                        <asp:HyperLink runat="server" NavigateUrl="#" ID="AssetsEditSwitch" CssClass="switch" ClientIDMode="Static" Visible="<%# Model.CanEdit %>">Edit</asp:HyperLink>
                    </div>

                    <%--<h3 class="section-title">SuperAsset</h3>--%>

                    <div class="view-group">
                        <div class="lbl">SuperAsset</div>
                        <div class="txt" style="display: table;">
                            <%# (Model.SuperAssetInHtml()) %>
                            <div style="display: table-row;">
                                <asp:Literal runat="server" Text='<%# (Model.Asset == null ? "None" : "<a href=\"" + Model.Asset +"\" target=\"_blank\" style=\"display:none;\">" + Model.Asset + "</a>") %>' />
                            </div>
                        </div>
                    </div>

                    <div class="view-group">
                        <div class="lbl">Has Media Assets</div>
                        <div class="txt">
                            <asp:Literal runat="server" Text='<%# (Model.HasMediaAssets ? "Yes" : "No") %>' />
                        </div>
                        <table id="MediaAssetList1">
                            <% if (Model.MediaAssets.Any())
                                {
                            %>
                            <asp:Repeater DataSource="<%# Model.MediaAssets %>" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:HyperLink runat="server" NavigateUrl='<%# Gcpe.Hub.Properties.Settings.Default.MediaAssetsUri + Model.ReleasePathName  + "/" + Model.Key.ToLower() + "/" + Container.DataItem.ToString().ToLower() %>' Text="<%# Container.DataItem.ToString() %>" /></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>

                            <% }
                                else
                                {
                            %>
                            <tr>
                                <td>No media files are uploaded.</td>
                            </tr>
                            <% } %>
                        </table>
                    </div>
                </div>

                <div id="AssetsEdit" class="section edit">
                    <h2>Assets</h2>

                    <%--<h3>SuperAsset</h3>--%>

                    <div class="field-group">
                        <div class="label">SuperAsset</div>
                        <div class="txt">
                            <div>
                                <asp:TextBox runat="server" ID="txtAsset" Width="400px" Text="<%# Model.Asset %>" />
                            </div>
                        </div>
                    </div>

                    <%--<h3>Media Assets</h3>--%>

                    <div class="field-group">
                        <div class="txt">
                            <asp:CheckBox runat="server" ID="chkHasMediaAssets" Checked="<%# Model.HasMediaAssets %>" Text="Has Media Assets" onclick="OnHasMediaAssetsClick(this);" />
                        </div>
                    </div>
                    <div class="field-group">
                        <table class="txt" id="MediaAssetList2">
                            <% if (Model.MediaAssets.Any())
                                {
                            %>
                            <asp:Repeater ID="rptAssetList" DataSource="<%# Model.MediaAssets %>" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:Label ID="file" runat="server" Text="<%# Container.DataItem.ToString() %>" /></td>
                                        <td style="text-align: center; padding: 3px;">
                                            <asp:HyperLink ID="btnDeleteAsset" runat="server" CssClass="switch" NavigateUrl="#" onclick='OnAssetDeleteSwitch(this); return false;' Text="Delete"></asp:HyperLink>
                                            <asp:HiddenField ID="valDeleted" Value="" runat="server" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <% }
                                else
                                {
                            %>
                            <tr>
                                <td colspan="2">No media files are uploaded.</td>
                            </tr>
                            <% } %>
                        </table>
                    </div>

                    <asp:Panel ID="AssetUploadBlock" CssClass="field-group" ClientIDMode="Static" runat="server" Visible="<%# Model.IsPublished || (Model.IsCommitted && Model.PublishDateTime < DateTime.UtcNow) %>">
                        <h3>Upload File</h3>
                        <table>
                            <tr>
                                <td>
                                    <input type="file" name="FileInput" id="FileInput" class="fileInput" multiple="multiple" size="60" />
                                </td>
                                <td style="padding-left: 10px">
                                    <input type="hidden" value="<%# Model.Key %>" id="uploadkey" name="uploadkey" />
                                    <input type="hidden" value="<%# Model.ReleasePathName %>" id="uploadPath" name="uploadPath" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <progress id="uploadProgress" value="0" max="100" style="width: 100%; height: 10px" hidden="hidden" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="left">
                                    <span id="uploadStatusMessage" class="message"></span>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>

                    <div class="inform-actions">
                        <asp:Button ID="btnSaveAssets" runat="server" Text="Save" CssClass="primary" OnClientClick="SaveAssets(); return false;" />
                        <asp:Button ID="hdnAssetButton" runat="server" Text="Save" CssClass="primary" OnClick="btnSaveAssets_Click" Style="visibility: hidden; display: none;" ClientIDMode="Static" />
                        <asp:LinkButton runat="server" ID="lbtnCancelAssets" CssClass="cancel" Text="Cancel" OnClick="lbtnCancelAssets_Click"></asp:LinkButton>
                    </div>
                </div>
            </div>
            <div id="Translations" class="hideForAdvisories">
                <div id="TranslationsView" class="section view">
                    <h2 class="section-title">Translations</h2>
                    <div class="section-action-options">
                        <asp:HyperLink runat="server" NavigateUrl="#" ID="TranslationsEditSwitch" CssClass="switch" ClientIDMode="Static" Visible="<%# Model.CanEdit %>">Edit</asp:HyperLink>
                    </div>  
                    <div class="view-group">
                        <div class="lbl">Has Translations</div>
                        <div class="txt">
                            <asp:Literal runat="server" Text='<%# (Model.HasTranslations ? "Yes" : "No") %>' />
                        </div>
                        <div class="lbl">Required Translations</div>
                        <div class="txt">
                            <asp:Literal runat="server" Text='<%# (!string.IsNullOrWhiteSpace(Model.RequiredTranslations()) ? Model.RequiredTranslations() : "") %>' />
                        </div>
                         <asp:Panel ID="TranslationsAnchorPanel" CssClass="field-group" ClientIDMode="Static" runat="server" Visible="<%# Model.DisplayTranslationsAnchorPanel %>" />
                        <table id="TranslationList1">
                            <% if (Model.Translations.Any())
                                {
                            %>
                            <asp:Repeater DataSource="<%# Model.Translations %>" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:HyperLink runat="server" NavigateUrl='<%# Gcpe.Hub.Properties.Settings.Default.ContentDeliveryUri + Model.ReleasePathName  + "/" + Model.Key + "/" + Container.DataItem.ToString() %>' Text="<%# Container.DataItem.ToString() %>" Target="_blank" /></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>

                            <% }
                                else
                                {
                            %>
                            <tr>
                                <td>No translations are uploaded.</td>
                            </tr>
                            <% } %>
                        </table>
                    </div>
                </div>

                <div id="TranslationsEdit" class="section edit">
                    <h2>Translations</h2>
                    <div class="field-group">
                        <div class="txt">
                            <asp:CheckBox runat="server" ID="chkHasTranslations" Checked="<%# Model.HasTranslations %>" Text="Has Translations" onclick="OnHasTranslationsClick(this);" />
                        </div>
                    </div>
                    <div class="lbl">Required Translations</div>
                        <div class="txt" style="color: black;">
                            <asp:Literal runat="server" Text='<%# (!string.IsNullOrWhiteSpace(Model.RequiredTranslations()) ? Model.RequiredTranslations() : "") %>' />
                        </div>
                    <div class="field-group" style="margin-top: 8px;">
                        <table class="txt" id="TranslationList2">
                            <% if (Model.Translations.Any())
                                {
                            %>
                            <asp:Repeater ID="rptTranslationList" DataSource="<%# Model.Translations %>" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:Label ID="file" runat="server" Text="<%# Container.DataItem.ToString() %>" /></td>
                                        <td style="text-align: center; padding: 3px;">
                                            <asp:HyperLink ID="btnDeleteTranslation" runat="server" CssClass="switch" NavigateUrl="#" onclick='OnTranslationDeleteSwitch(this); return false;' Text="Delete"></asp:HyperLink>
                                            <asp:HiddenField ID="valDeleted" Value="" runat="server" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <% }
                                else
                                {
                            %>
                            <tr>
                                <td colspan="2">No translations are uploaded.</td>
                            </tr>
                            <% } %>
                        </table>
                    </div>

                    <asp:Panel ID="TranslationUploadBlock" CssClass="field-group" ClientIDMode="Static" runat="server" Visible="<%# Model.IsPublished || (Model.IsCommitted && !Model.IsPublished) || Model.IsApproved %>">
                        <h3>Upload File</h3>
                        <table>
                            <tr>
                                <td>
                                    <input type="file" name="FileInput1" id="FileInput1" class="fileInput" multiple="multiple" size="60" />
                                </td>
                                <td style="padding-left: 10px">
                                    <input type="hidden" value="<%# Model.Key %>" id="uploadkey1" name="uploadkey1" />
                                    <input type="hidden" value="<%# Model.ReleasePathName %>" id="uploadPath1" name="uploadPath1" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <progress id="uploadProgress1" value="0" max="100" style="width: 100%; height: 10px" hidden="hidden" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="left">
                                    <span id="uploadStatusMessage1" class="message"></span>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>

                    <div class="inform-actions">
                        <asp:Button ID="btnSaveTranslations" runat="server" Text="Save" CssClass="primary" OnClientClick="SaveTranslations(); return false;" />
                        <asp:Button ID="hdnTranslationButton" runat="server" Text="Save" CssClass="primary" OnClick="btnSaveTranslations_Click" Style="visibility: hidden; display: none;" ClientIDMode="Static" />
                        <asp:LinkButton runat="server" ID="lbtnCancelTranslations" CssClass="cancel" Text="Cancel" OnClick="lbtnCancelTranslations_Click"></asp:LinkButton>
                    </div>
                </div>
            </div>


            <div id="Meta">
                <div id="MetaView" class="section view">
                    <h2 class="section-title">Meta</h2>

                    <div class="section-action-options">
                        <asp:HyperLink runat="server" NavigateUrl="#" ID="MetaEditSwitch" CssClass="switch" ClientIDMode="Static" Visible="<%# Model.CanEdit %>">Edit</asp:HyperLink>
                    </div>

                    <div class="view-group hideForAdvisories">
                        <div class="lbl"><%# Model.ReleaseTypeName +" URL" %></div>
                        <div class="txt">
                            <% if (Model.IsPublished || Model.IsCommitted)
                                { %>
                            <asp:HyperLink runat="server" NavigateUrl="<%# Model.ReleaseUri.ToString()%>" ClientIDMode="Static" Target="_blank"><%# Model.ReleaseUri.ToString() %></asp:HyperLink>
                            <% }
                                else
                                { %>
                            <% if (Model.ReleaseTypeId == ReleaseType.Release && Model.Reference == "")
                                { %>
                                Not Approved
                            <% }
                                else
                                { %>
                            <%# Model.ReleaseUri.ToString() %>
                            <% } %>
                            <% } %>
                            <%--<asp:HyperLink runat="server" NavigateUrl="<%# Model.ReleaseUri.ToString()%>" ClientIDMode="Static"><%# Model.Key %></asp:HyperLink>--%>
                        </div>
                    </div>

                    <div class="view-group hideForAdvisories">
                        <div class="lbl">Redirect URL</div>
                        <div class="txt">
                            <asp:HyperLink runat="server" NavigateUrl="<%#Model.RedirectUrl == null ? null:Model.RedirectUrl.ToString() %>" ClientIDMode="Static" Target="_blank"><%#Model.RedirectUrl == null ? "<div>None</div>" : Model.RedirectUrl.ToString() %></asp:HyperLink>
                        </div>
                    </div>

                    <div class="view-group hideForAdvisories">
                        <div class="lbl">Release Date</div>
                        <div class="txt">
                            <%: Model.ReleaseDate.HasValue ? FormatDateTime(Model.ReleaseDate.Value) : "Not Published" %>
                        </div>
                    </div>

                    <div class="view-group">
                        <div class="lbl">Location</div>
                        <div class="txt">
                            <asp:Literal ID="ltrLocation" runat="server" Mode="Encode" Text='<%# Model.Location == "" ? "None" : Model.Location %>' />
                        </div>
                    </div>

                    <div class="view-group hideForAdvisories">
                        <div class="lbl">Summary</div>
                        <div class="txt">
                            <asp:Literal ID="Literal1" runat="server" Mode="Encode" Text='<%# Model.Summary == "" ? "None" : Model.Summary %>' />
                        </div>
                    </div>

                    <div class="view-group hideForAdvisories">
                        <div class="lbl">Social Media Headline</div>
                        <div class="txt">
                            <asp:Literal ID="Literal4" runat="server" Mode="Encode" Text='<%# Model.SocialMediaHeadline == "" || Model.SocialMediaHeadline == null ? "None" : Model.SocialMediaHeadline %>' />
                        </div>
                    </div>

                    <div class="view-group hideForAdvisories">
                        <div class="lbl">Social Media Summary</div>
                        <div class="txt">
                            <asp:Literal ID="Literal3" runat="server" Mode="Encode" Text='<%# Model.SocialMediaSummary == "" || Model.SocialMediaSummary == null ? "None" : Model.SocialMediaSummary %>' />
                        </div>
                    </div>

                    <div class="view-group hideForAdvisories">
                        <div class="lbl">Keywords</div>
                        <div class="txt">
                            <%# Model.Keywords == "" ? "None" : Model.Keywords %>
                        </div>
                    </div>
                </div>

                <asp:Panel runat="server" ID="MetaEdit" ClientIDMode="Static" CssClass="section edit" Visible="<%# Model.CanEdit %>">
                    <h2>Meta</h2>

                    <div class="field-group required hideForAdvisories">
                        <div class="label"><%# Model.ReleaseTypeName +" URL" %></div>
                        <div class="txt">
                            <asp:TextBox ID="txtKey" runat="server" Width="98%" Visible='<%# Model.IsKeyEditable %>' Text='<%# Model.Key %>' MaxLength="300"></asp:TextBox>
                            <asp:HyperLink runat="server" NavigateUrl="<%# Model.ReleaseUri.ToString()%>" Visible='<%# !txtKey.Visible %>'><%# Model.Key %></asp:HyperLink>
                        </div>
                    </div>

                    <div class="field-group required hideForAdvisories">
                        <div class="label">Redirect URL</div>
                        <div class="txt">
                            <asp:TextBox ID="txtRedirect" runat="server" Width="98%" Text='<%# Model.RedirectUrl %>' MaxLength="300"></asp:TextBox>
                            <asp:HyperLink runat="server" NavigateUrl="<%# Model.RedirectUrl%>" Visible='<%# !txtRedirect.Visible %>'><%# Model.RedirectUrl %></asp:HyperLink>
                        </div>
                    </div>

                    <div class="field-group hideForAdvisories">
                        <div class="label">Release Date</div>
                        <div class="txt">
                            <asp:TextBox ID="releaseDateTimePicker" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="field-group">
                        <div class="label">Location</div>
                        <div class="txt">
                            <asp:TextBox ID="txtLocation" runat="server" Width="400px" Text='<%# Model.Location %>' MaxLength="50"></asp:TextBox>
                        </div>
                    </div>

                    <div class="field-group required hideForAdvisories">
                        <div class="label">Summary</div>
                        <div class="txt">
                            <asp:TextBox ID="txtSummary" runat="server" Width="98%" Height="80px" Text='<%# Model.Summary %>' TextMode="MultiLine"></asp:TextBox>
                        </div>
                    </div>

                    <div class="field-group required hideForAdvisories">
                        <div class="label">Social Media Headline</div>
                        <div class="txt">
                            <asp:TextBox ID="txtSocialMediaHeadline" runat="server" Width="98%" Height="80px" Text='<%# Model.SocialMediaHeadline %>' TextMode="MultiLine"></asp:TextBox>
                        </div>
                    </div>

                    <div class="field-group required hideForAdvisories">
                        <div class="label">Social Media Summary</div>
                        <div class="txt">
                            <asp:TextBox ID="txtSocialMediaSummary" runat="server" Width="98%" Height="80px" Text='<%# Model.SocialMediaSummary %>' TextMode="MultiLine"></asp:TextBox>
                        </div>
                    </div>

                    <div class="field-group hideForAdvisories">
                        <div class="label">Keywords</div>
                        <div class="txt">
                            <div>
                                <asp:TextBox runat="server" ID="txtKeywords" Text="<%# Model.Keywords %>" Width="98%" />
                            </div>
                        </div>
                    </div>

                    <div class="inform-actions">
                        <asp:Button ID="btnSaveMeta" runat="server" Text="Save" CssClass="primary" OnClick="btnSaveMeta_Click" />
                        <asp:LinkButton runat="server" ID="lbtnCancelMeta" CssClass="cancel" Text="Cancel" OnClick="lbtnCancelMeta_Click"></asp:LinkButton>
                    </div>
                </asp:Panel>
            </div>

            <asp:Repeater ID="rptDocument" DataSource="<%# Model.Documents %>" runat="server" ItemType="ListItem">
                <ItemTemplate>
                    <controls:Document ID="ctrlDocument" Model="<%# Model %>" DocumentPath="<%# Item.Value %>" runat="server" />
                </ItemTemplate>
            </asp:Repeater>

            <div id="History">
                <asp:Panel CssClass="section view" runat="server" ID="pnlHistory">
                    <h2 class="section-title">History</h2>
                    <div class="section-action-options">
                        <asp:LinkButton runat="server" ID="showAllHistory" ClientIDMode="Static" CssClass="switch" Visible="<%# Model.HiddenHistoryCount > 0 %>" OnClick="showAllHistory_Click" OnClientClick="showAllHistory.style.display='none'">Show All</asp:LinkButton>
                    </div>

                    <%--TODO: Add "Show More" link to show all items in history.--%>
                    <%--i.e. Model.History.Where(h=>!h.IsVerbose) --%>
                    <asp:Repeater runat="server" DataSource="<%# Model.History %>" ItemType="Gcpe.Hub.News.ReleaseManagement.ReleaseModel+HistoryHtml">
                        <HeaderTemplate>
                            <table>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td style="padding-right: 8px; padding-bottom: 5px;">
                                    <asp:Literal runat="server" Text='<%# FormatDateTime(Item.DateTime) %>' Mode="Encode" /></td>
                                <td style="padding-right: 8px; padding-bottom: 5px;">
                                    <asp:Literal runat="server" Text="<%# Item.UserHtml %>" Mode="PassThrough" /></td>
                                <td style="padding-bottom: 5px;">
                                    <asp:Literal runat="server" Text="<%# Item.DescriptionHtml %>" Mode="PassThrough" /></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </asp:Panel>
            </div>

            <asp:Panel runat="server" CssClass="section view" ID="pnlDelete" Visible="<%# Model.CanDelete %>">
                <h2><%# Model.Reference == string.Empty ? "Delete" : "Deactivate" %></h2>
                <p><%# Model.Reference == string.Empty ? "The release and all of its documents will be permanently deleted." : "The release will be deactivated, preventing it from being published or included in search results." %></p>
                <br />
                <asp:Button ID="btnDelete" runat="server" Text='<%# Model.Reference == string.Empty ? "Delete Release" : "Deactivate " + Model.ReleaseTypeName %>' CssClass="secondary" OnClick="btnDelete_Click" />
            </asp:Panel>

        </div>
    </div>

</asp:Content>
