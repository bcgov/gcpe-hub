<%@ Page Language="C#" MasterPageFile="~/News/ReleaseManagement/ReleaseManagement.master" AutoEventWireup="true" CodeBehind="New.aspx.cs" Inherits="Gcpe.Hub.News.ReleaseManagement.New" %>
<%@ MasterType TypeName="Gcpe.Hub.News.ReleaseManagement.ReleaseManagement" %>
<%@ Import Namespace="Gcpe.Hub" %>
<%@ Import Namespace="Gcpe.Hub.News" %>
<%@ Import Namespace="Gcpe.Hub.News.ReleaseManagement.Controls" %>

<%@ Register Assembly="Gcpe.Hub.Legacy.Website" Namespace="Gcpe.Hub.News.ReleaseManagement.Controls" TagPrefix="asd" %>
<%@ Register Assembly="Gcpe.News.ReleaseManagement.Controls" Namespace="Gcpe.News.ReleaseManagement.Controls" TagPrefix="extcontrols" %>

<%@ Register TagPrefix="controls" TagName="ReleaseImagePicker" Src="~/News/ReleaseManagement/Controls/ReleaseImagePicker.ascx" %>
<%@ Register Assembly="Gcpe.Hub.Legacy.Website" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContentPlaceHolder" runat="server">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scriptsContentPlaceHolder" runat="server">

    <% // These are OUTSIDE the UpdatePanel, they do not change on postback %>

    <script id="defaultVariables">

        var isNew = true;

        /* Tracks if user makes any changes so values are no longer updated automatically */
        var hasUserChangedPageLayout = false;
        var hasUserChangedPageImage = false;
        var hasUserChangedSector = false;
        var hasUserChangedRegion = false;
        var hasUserChangedOrganization = false;

        /* Default Value Lists */
        var defaultPageImages = { <%= DefaultPageImages.ToJavaScript() %> };
        var defaultPageLayouts = { <%= Model.DefaultPageLayouts.ToJavaScript() %> };
        var defaultMinistrySectors = { <%= DefaultMinistrySectors.ToJavaScript() %> };
        var pageTitleOptions = { <%= ReleaseDocumentTypes.ToJavaScript() %> };


        var bylineTxt = "";
        var orgTxt = "";

    </script>

    <script id="functions">

        /* Sets the Default Image and Default Layout based on the Page Title passed in */
        function SetPageDefaults(type) {

            var defaultImage = "";
            var defaultLayout = "";


            /* Set the Default Page Layout for the document type */
            if (!hasUserChangedPageLayout) {
                $.each(defaultPageLayouts, function (key, value) {
                    if (key.toUpperCase() == type.toUpperCase())
                        defaultLayout = value;
                });
                if (defaultLayout != "") {
                    $('#<%= rblPageLayout.ClientID%> input[value="' + defaultLayout + '"]').prop('checked', true);
                    SetPageLayoutDefaults(defaultLayout);
                    $("#<%= rblPageLayout.ClientID%>").parents('div .field-group').removeClass('warning-chkbxlst'); // just in case validation has happened already
                }
            }

            if (!hasUserChangedPageImage) {

                $.each(defaultPageImages, function (key, value) {
                    if (key.toUpperCase() == type.toUpperCase())
                        defaultImage = value;
                });

                $("#ImageChoice img").removeClass("selected-image"); // in case any selected
                if (defaultImage != "") {
                    $("#<%= pageImagePicker.ValueClientID %>").val(defaultImage);

                    var newSrc = '<%= pageImagePicker.GetReleaseImageClientUrl() %>' + "&Id=" + defaultImage;
                    $("#<%= pageImagePicker.ImageClientID %>").attr("src", newSrc);

                    $("#" + defaultImage).addClass("selected-image");
                } else {
                    $("#<%= pageImagePicker.ValueClientID %>").val("");

                    var newSrc = '<%= pageImagePicker.GetReleaseImageClientUrl() %>';
                    $("#<%= pageImagePicker.ImageClientID %>").attr("src", newSrc);

                    $("#noimage").addClass("selected-image");
                }
            }
        }

        /* Disable or Enables Organization & Byline depending on the layout  */
        function SetPageLayoutDefaults(layout) {
            if (layout == "Formal") {
                bylineTxt = PageLayout_FormalSelected($("#<%= txtOrganization.ClientID %>"), $("#<%= txtByline.ClientID %>"));
            } else {
                orgTxt = PageLayout_InformalSelected($("#<%= txtOrganization.ClientID %>"), $("#<%= txtByline.ClientID %>"));
            }
        }


        function SetOrganizationDefaults() {

            /* First update the organization text if enabled and no manual changes made yet */
            if ($("#<%= txtOrganization.ClientID %>").is(':enabled')) {
                if (!hasUserChangedOrganization) {
                    var names = GetNamesSelected($("#<%= txtOrganization.ClientID %>").val(), "<%= chklstMinistries.ClientID %>");
                    $("#<%= txtOrganization.ClientID %>").val(names);
                }
            }

            if ($("#<%= txtOrganization.ClientID %>").val().trim() != "") {
                $("#<%= txtOrganization.ClientID %>").parents('div .field-group').removeClass('warning'); // in case the validation was there, remove it as there is now text
            }
        }

    </script>

    <script>

        var dateIsValid = true;   

        function isValidDateFormat(value) {
            if (value == "") return true;
            //2011-05-16 12:50 am
            reg = /^((199\d)|([2-9]\d{3}))\-(([0][1-9])|([1][0-2]))\-(([0-2]?\d)|([3][01]))\s(([0]?\d)|([1][0-2])):[0-5][0-9] (am|pm|AM|PM)?$/
            return reg.test(value);
        }

        function DateTimePicker_Validate(sender, args) {
            if (!isValidDateFormat($(sender).val())) {
                 $('#<%= publishDateTimePicker.ClientID %>').parents('div .field-group').addClass('warning-date');
                dateIsValid = false;
            }
            else {
                $('#<%= publishDateTimePicker.ClientID %>').parents('div .field-group').removeClass('warning-date');
                dateIsValid = true;
            }
        }

        // Validate the full page before submit
        function IsPageValid() {

            var firstError = null;
            var fieldArea = null;

            if (SelectedReleaseType() == 5) { // Advisory
                // Make sure that at least one Media Distribution List is selected.
                var lbxMediaDistribution = $("#<%= mediaDistributionListBox.ClientID %>");
                fieldArea = lbxMediaDistribution.parents('div .field-group');
                if (!lbxMediaDistribution.val()) {
                    fieldArea.addClass('warning-chkbxlst');
                    firstError = firstError || fieldArea;
                } else {
                    fieldArea.removeClass('warning-chkbxlst');
                }
            }

            var activityId = $('#<%= txtCorpCalId.ClientID %>').val().trim();
            if (activityId != "") {
                var numericExpression = /^[0-9]+$/;
                if(!activityId.match(numericExpression)) {
                    fieldArea = $('#<%= txtCorpCalId.ClientID %>').parents('div .field-group');
                    fieldArea.addClass('warning-number');
                    if (firstError == null)
                        firstError = fieldArea;
                }
            }

            var txtPageTitle = $("#<%= txtPageTitle.ClientID %>");
            if ( !txtPageTitle.val().trim() ) {

                fieldArea = txtPageTitle.parents('div .field-group');
                fieldArea.addClass('warning');
                if (firstError == null)
                    firstError = fieldArea;

            } else {

                if ($(".PageTitleConfirmation").is(':visible')) {
                    fieldArea = txtPageTitle;
                    fieldArea.addClass('warning-page-title');
                    if (firstError == null)
                        firstError = fieldArea;
                }
            }

            firstError = ValidateCheckBoxList($("#<%= rblPageLayout.ClientID %>"), firstError);

            if (SelectedReleaseType() != 5) { // Not an Advisory
                // Ministry and Sector aren't required on Media Advisories.
                firstError = ValidateCheckBoxList($("#<%= chklstMinistries.ClientID %>"), firstError);
                firstError = ValidateCheckBoxList($("#<%= cklstSector.ClientID %>"), firstError);
            }


            fieldArea = $('#<%=  txtOrganization.ClientID %>').parents('div .field-group');
            if ( !$('#<%=  txtOrganization.ClientID %>').val().trim() && $('#<%=  txtOrganization.ClientID %>').is(':enabled')) {
                fieldArea.addClass('warning');
                if (firstError == null)
                    firstError = fieldArea;
            } else {
                fieldArea.removeClass('warning');
            }

            if ( !$('#<%= txtHeadline.ClientID %>').val().trim() ) {
                fieldArea = $('#<%= txtHeadline.ClientID %>').parents('div .field-group');
                fieldArea.addClass('warning');
                if (firstError == null)
                    firstError = fieldArea;
            }


            if (!dateIsValid) {
                $("#<%= publishDateTimePicker.ClientID %>").parents('div .field-group').addClass('warning-date'); /* Inline validation */
                if (firstError == null) {
                    firstError = $("#<%= publishDateTimePicker.ClientID %>");
                }
            }



            if (firstError != null) {
                $('html, body').animate({ scrollTop: firstError.offset().top - 100 }, 500);// scroll the field into view.
                firstError.find(":input").focus(); // focus on field
                return false;
            } else {
                return true;
            }

        }
    </script>


</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="formContentPlaceHolder" runat="server">

    <script id="pageReady">


        function SelectedReleaseType() {
            var selectedRadio = $("#<%= rblNewsType.ClientID %> input:checked:first");
            if (selectedRadio != null) {
                return selectedRadio.val();
            }
            return 0;
        }

        function OnReleaseTypeChange() {
            var txtPageTitle = $("#<%= txtPageTitle.ClientID %>");
            txtPageTitle.val("");
            HidePageTitleConfirmation(txtPageTitle, $(".PageTitleConfirmation"));

            var isStory = SelectedReleaseType() == 2;
            var isUpdate = SelectedReleaseType() == 4;
            var isAdvisory = SelectedReleaseType() == 5;


            // Show the MediaContacts picker but NOT for Updates & Stories.
            var showMediaDistributionLists = !(SelectedReleaseType() == 2 || SelectedReleaseType() == 4);
            $("#mediaListPicker").toggle(!(isStory || isUpdate));

            // Hide the stuff for Advisories -- see also the Save button_click
            $("#imageSection").toggle(!isAdvisory);
            $("#sectorsSection").toggle(!isAdvisory);
            $("#themesSection").toggle(!isAdvisory);
            $("#tagsSection").toggle(!isAdvisory);

        }

        function pageLoad() {


            // Performs the validation, cancels updatepanel postback if not valid
            //Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeNewPageRequest);

            $("#<%= btnSave.ClientID %>").click(function () {
                return IsPageValid();
            });


            // Document Type
            //  - setup autocomplete
            //  - handle when changed
            //  - inline validation
            var txtPageTitle = $("#<%= txtPageTitle.ClientID %>");
            txtPageTitle.autocomplete({
                minLength: 0,
                source: function (request, response) {
                    var selectedReleaseType = SelectedReleaseType();
                    var data = [];
                    for (var property in pageTitleOptions) {
                        var releaseType = pageTitleOptions[property];
                        if (selectedReleaseType == 5) {
                            // Advisories(5) should include the PageTitle options for Releases(1) & Factsheets(3) too.
                            // FYI: This same check exists in the validation code in ReleaseManagement.js page too!
                            if (releaseType == 5 || releaseType == 1 || releaseType == 3) {
                                data.push(property);
                            }
                        } else if (releaseType == selectedReleaseType) {
                            // For everything else, only show the one with the *same* release Type.
                            data.push(property);
                        }
                    }
                    var data = $.grep(data, function (value) {
                        return value.substring(0, request.term.length).toLowerCase() == request.term.toLowerCase();
                    });
                    response(data);
                },
                select: function (a, b) { // From the list
                    CheckReleaseTypeValid(txtPageTitle, b.item.value, $(".PageTitleConfirmation"), pageTitleOptions, SelectedReleaseType());
                    SetPageDefaults(b.item.value);
                    $(".PageTitleConfirmation").hide();
                },
                change: function (event, ui) { // On mouse out or blur
                    var title = txtPageTitle.val();
                    if (title.trim() == "") {
                        txtPageTitle.parents('div .field-group').addClass('warning'); /* Inline validation */
                    } else {
                        CheckReleaseTypeValid(txtPageTitle, title, $(".PageTitleConfirmation"), pageTitleOptions, SelectedReleaseType());
                        SetPageDefaults(title);
                    }
                    var multiselect = $("#<%=mediaDistributionListBox.ClientID %>").data("kendoMultiSelect");
                    var selectedItems = multiselect.value();
                   
                    // if document type is media advisory or updated media advisory, then antopopulate distribution list to be MINIS2
                    if ((title.trim() == 'Media Advisory') || (title.trim() == 'UPDATED Media Advisory'))
                    {
                        var res =  multiselect.value().slice();
                        res.push("f7313b16-4fcb-4a30-9bc9-bc5472da00e8");   
                        multiselect.dataSource.filter({});
                        multiselect.value(res) 
                        multiselect.trigger("change");
                    }
                    else
                    {
                        multiselect.value(selectedItems);
                        multiselect.trigger("change");
                    }
                }
            }).focus(function () {
                $(this).autocomplete("search", ""); /* Open autocomplete on focus and show the whole list */
            }).keydown(function (event) { /* Catches enter button click so it does not submit form and tab to next form field */
                if (event.keyCode == 13) {
                    var inputs = $(this).closest('form').find(':input');
                    inputs.eq(inputs.index(this) + 1).focus();
                    return false;
                }
            }).keyup(function (e) { /* Catches enter button click so it does not submit form and tab to next form field */
                if (e.which === 13) {
                    var inputs = $(this).closest('form').find(':input');
                    inputs.eq(inputs.index(this) + 1).focus();
                    $(".ui-menu-item").hide();
                }
            });


            /* User has confirmed the new page title */
            $(".AddNewDocumentType").button({
                icons: {
                    primary: "ui-icon-circle-check"
                }
            }).click(function () {
                HidePageTitleConfirmation(txtPageTitle, $(".PageTitleConfirmation"), true);
                return false;
            });

            $(".IgnoreNewDocumentType").button({
                icons: {
                    primary: "ui-icon-circle-check"
                }
            }).click(function () {
                HidePageTitleConfirmation(txtPageTitle, $(".PageTitleConfirmation"));
                return false;
            });



            // Locations
            //  - set up autocomplete 
            $("#<%= txtLocation.ClientID %>").autocomplete({
                minLength: 0,
                source: function (request, response) {
                    var data = $.grep([ <%= EnglishLocations.ToJavaScript() %>], function (value) {
                        return value.substring(0, request.term.length).toLowerCase() == request.term.toLowerCase();
                    });
                    response(data);
                },
                autoFocus: true
            }).keydown(function (event) { /* Catches enter button click so it does not submit form  */
                if (event.keyCode == 13) {
                    return false;
                }
            }).keyup(function (e) { /* Catches enter button click so it does not submit form  */
                if (e.which === 13) {
                    $(".ui-menu-item").hide();
                }
            });


            // Ministries 
            //  - Set organization text
            //  - Set sectors checked
            //  - Inline validation check
            $("#<%= chklstMinistries.ClientID %>").click(function () {

                SetOrganizationDefaults();

                /* Check the Sector Defaults for the Ministry */
                var cklstSector = $("#<%= cklstSector.ClientID %>");
                if (!hasUserChangedSector) {
                    var defaultSectors = [];

                    // Find out which Sectors to Default
                    cklstSector.find("input").attr('checked', false); 

                    $("[id*=<%= chklstMinistries.ClientID %>] input").each(function () {
                        var ministryId = $(this).val();
                        if (this.checked) {
                            $.each(defaultMinistrySectors, function (key, value) {
                                if (key == ministryId)
                                    defaultSectors = value;
                            });
                        }
                        // Set Values
                        for (var i in defaultSectors) {
                            cklstSector.find('input:checkbox[value=' + $.trim(defaultSectors[i]) + ']').prop('checked', true);
                        }
                    });
                }

                if (cklstSector.find("input:checked").length > 0) { /* If there are sectors selected now and by chance there was the validation settings, remove them */
                    cklstSector.parents('div .field-group').removeClass('warning-chkbxlst');
                }

                /* Inline Validation */
                if ($("#<%= chklstMinistries.ClientID %> input:checked").length == 0) {
                    if (SelectedReleaseType() != 5) { //only if it's not an advisory
                        $(this).parents('div .field-group').addClass('warning-chkbxlst');
                    }
                } else {
                    $(this).parents('div .field-group').removeClass('warning-chkbxlst');
                }


            });


            // Track any changes to the organization
            $('#<%=  txtOrganization.ClientID %>').keyup(function () {
                hasUserChangedOrganization = true;
            });


            // Page Layout 
            //  - Enable/Disable Byline and Organization
            //  - Track if user has changed value
            $("#<%= rblPageLayout.ClientID %> input").change(function () {
                hasUserChangedPageLayout = true;
                SetPageLayoutDefaults($(this).val());
                SetOrganizationDefaults();
                $("#<%=  rblPageLayout.ClientID%>").parents('div .field-group').removeClass('warning-chkbxlst');
            });


            // Add or Remove Media Contact Row
            var counter = <%# Model.Contacts.Count() %> + 1;

            $("#contacts").on('keyup', 'textarea', function () {

                /* TODO: Remove blank contacts

                // If all the elmeents are blank, remove them all except the first and focus on the first element
                var firstEl = $("#contacts textarea").first();
                var nbrBlanks = 0;
                if (firstEl.val() == "") {
                    $("#contacts textarea").each(function () {
                        if ($(this).val() == "")
                            nbrBlanks++;
                    });
                }
                if (nbrBlanks == $("#contacts textarea").length) {
                    firstEl.focus();
                    $("#contacts textarea").each(function () {
                        if ($(this).val() == "" && $(this).prop('id') != firstEl.prop('id'))
                            $(this).remove();
                    });
                    return;
                }

                // Not all are blank, but check if this one and the next one is blank, remove the next blank one
                if ($(this).val() == '') {
                    if ($(this).next().val() == '') {
                        $(this).next().remove();
                    }
                    return;
                } 

                */


                //If the last one is blank, just get out of here
                if ($("#contacts textarea").last().val() == "") {
                    return;
                }

                var newfieldId = "parameters" + counter;
                var newTxt = $(this).clone();
                newTxt.attr('id', newfieldId);
                newTxt.attr('name', newfieldId);
                newTxt.val('');

                $(this).parent().append(newTxt);
                counter++;
            });


            /* Inline Validation for required fields */
            $('#<%= txtHeadline.ClientID %>').blur(function () {
                if (!$(this).val().trim())
                    $(this).parents('div .field-group').addClass('warning');
                else
                    $(this).parents('div .field-group').removeClass('warning');
            });

            $('#<%=  txtOrganization.ClientID %>').blur(function () {
                if (!$(this).val().trim())
                    $(this).parents('div .field-group').addClass('warning');
                else
                    $(this).parents('div .field-group').removeClass('warning');
            });


            $("#<%= cklstSector.ClientID %>").click(function () {

                hasUserChangedSector = true; /* Track user manually made a change to sectors */

                if ($("#<%= cklstSector.ClientID %> input:checked").length == 0)
                    $(this).parents('div .field-group').addClass('warning-chkbxlst');
                else
                    $(this).parents('div .field-group').removeClass('warning-chkbxlst');

            });

            CKEDITOR.on('instanceReady', function (ev) {
                ev.editor.on('paste', function (e) {
                    e.data.dataValue = e.data.dataValue.replace(/<p>\&nbsp\;<\/p>/g, '');
                });
            });

             $('#<%= publishDateTimePicker.ClientID %>').kendoDateTimePicker({
                value: '',
                format: "yyyy-MM-dd hh:mm tt",
                change: function(e){                                        
                    DateTimePicker_Validate( $('#<%= publishDateTimePicker.ClientID %>'));                   
                }
             });    
            
            // ----------------------------------------------------------------------
            // MediaDistribution multiselect
            // ----------------------------------------------------------------------
            $('#<%= mediaDistributionListBox.ClientID %>').kendoMultiSelect({
                autoClose: false,
                filter: "contains"
            });
            $('#mediaListPicker').show();


            ImagePickerEvents($("#imageSection"))

        }

<%--        function InitializeNewPageRequest(sender, args) {

            var SubmitButtonID = "<% = btnSave.ClientID %>";
            var currentButtonClickedID = args.get_postBackElement().id.toLowerCase();

            if (currentButtonClickedID == SubmitButtonID.toLowerCase()) {
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                if ( !IsPageValid() ) {
                    prm.abortPostBack();
                    args.set_cancel(true);
                }
            }
        }--%>

        function NumbersOnly(evt)
        {
            var e = event || evt; 
            var charCode = e.which || e.keyCode;

            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            return true;

        }

        function OnTxtCorpCalIdChange() {
            var fieldArea = $('#<%= txtCorpCalId.ClientID %>').parents('div .field-group');
            fieldArea.removeClass('warning-number');
            PreloadCalActivityData();
        }

        function PreloadCalActivityData() {
            $.ajax({
                async: true,
                url: "<%= ResolveUrl("~/News/Services.asmx/FindCalendarActivity") %>",
                contentType: "application/json; charset=utf-8",
                processData: false,
                data: JSON.stringify({
                    'id': $("#<%=txtCorpCalId.ClientID %>").val().trim(),
                }),
                dataType: "json",
                type: 'POST',
                error: function () {
                    alert("Error occurred while trying to retrieve Corp Calendar data.");
                    return;
                },
                success: function (data) {
                    if (data == null || data.d == null) {
                        $("#activityTitle").text("No result is found.");
                        return;
                    }

                    $("#activityTitle").text(data.d.Title);
                    //if(!confirm("Do you want to reload the data?")) {
                    //    return;
                    //}
                    $("#<%=publishDateTimePicker.ClientID %>").val(data.d.FormatedStartDateTime);
                    $("#<%=txtLocation.ClientID %>").val(data.d.City);
                    $("#<%= txtOrganization.ClientID %>").val("");
                    var leadOrganization = data.d.LeadOrganization;
                    
                    var chklstMinistries = $("#<%=chklstMinistries.ClientID%> input");
                    var chklstSectors = $("#<%=cklstSector.ClientID%> input");
                    var chklstThemes = $("#<%=cklstTheme.ClientID%> input");
                    var chklstTags = $("#<%=cklstTag.ClientID%> input");

                    chklstMinistries.prop("checked", false);
                    chklstMinistries.filter(function() {
                        return $(this).val() === data.d.LeadMinistry;
                    }).prop("checked", true);
                    if(data.d.LeadMinistry != null && data.d.LeadMinistry != "") {
                        var names = GetNamesSelected($("#<%= txtOrganization.ClientID %>").val(), "<%= chklstMinistries.ClientID %>");
                        $("#<%= txtOrganization.ClientID %>").val(names);
                    }        
                    jQuery.each(data.d.Ministries, function(index, ministry) {
                        chklstMinistries.filter(function() {
                            return $(this).val() == ministry;
                        }).prop("checked", true);
                        var names = GetNamesSelected($("#<%= txtOrganization.ClientID %>").val(), "<%= chklstMinistries.ClientID %>");
                        $("#<%= txtOrganization.ClientID %>").val(names);
                    });
                    if(leadOrganization != null && leadOrganization != "")
                    {
                        var ministries = $("#<%= txtOrganization.ClientID %>").val();
                        $("#<%= txtOrganization.ClientID %>").val(ministries + "\n" + leadOrganization);
                    }

                    chklstSectors.prop("checked", false);
                    jQuery.each(data.d.Sectors, function(index, sector) {
                        chklstSectors.filter(function() {
                            return $(this).siblings('label').text() == sector;
                        }).prop("checked", true);
                    });

                    chklstThemes.prop("checked", false);
                    jQuery.each(data.d.Themes, function(index, theme) {
                        chklstThemes.filter(function() {
                            return $(this).siblings('label').text() == theme;
                        }).prop("checked", true);
                    });

                    chklstTags.prop("checked", false);
                    jQuery.each(data.d.Initiatives, function(index, tag) {
                        chklstTags.filter(function() {
                            return $(this).siblings('label').text() == tag;
                        }).prop("checked", true);
                    });

                    IsPageValid();
                }
            });
        }
    </script>

<script type="text/javascript">

    <% // If there is a server error, we want them to see it so scroll to the top %>
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
        window.scrollTo(0, 0);
    });

</script>


    <h1>New</h1>
    <div class="helper">Enter the release package information.</div>
    <br />

    <asp:Panel runat="server" ID="pnlErrors" CssClass="section-error" Visible="false">
        <h2>Sorry, but there was an error with your submission.</h2>
        <asp:Repeater runat="server" ID="rptErrors">
            <HeaderTemplate><ul></HeaderTemplate>
            <ItemTemplate><li><%# Container.DataItem %></li></ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
    </asp:Panel>

    <div class="section">

        <h2>Release and Document Details</h2>
        <div class="helper">Add the details for the first document in the release. You may add additional documents to the release after you save the release.</div>
        <br />

        <div class="field-group required">
            <div class="label">News Type</div>
            <div class="txt">
                <asp:RadioButtonList runat="server" ID="rblNewsType" onchange="OnReleaseTypeChange()">
                    <asp:ListItem runat="server" Selected="True" Text="Release" Value="1" />
                    <asp:ListItem runat="server" Text="Story" Value="2" />
                    <asp:ListItem runat="server" Text="Factsheet" Value="3" />
                    <asp:ListItem runat="server" Text="Update" Value="4" />
                    <asp:ListItem runat="server" Text="Advisory" Value="5" />
                </asp:RadioButtonList>
            </div> 
        </div>

        <!-- Media Contacts are not available for Story or Update ReleaseTypes. -->
        <div id="mediaListPicker" class="field-group" style="width:500px;">
            <div class="label">Media Distribution Lists</div>
            <asp:ListBox ID="mediaDistributionListBox" SelectionMode="Multiple" AutoPostBack="false" runat="server"></asp:ListBox>
        </div>

        <div class="field-group">
            <div class="label">Activity ID</div>
            <div class="txt"> 
                <asp:TextBox style="display:inline-block;" ID="txtCorpCalId" runat="server" onkeypress="return NumbersOnly();" onchange="OnTxtCorpCalIdChange()"></asp:TextBox>
            </div>
            <div class="txt" id="activityTitle"></div>
        </div>

        <div class="field-group required">
            <div class="label">Document Type</div>
            <div class="txt">
                <div style="display:inline-block;"><asp:TextBox ID="txtPageTitle" runat="server" MaxLength="50" Width="500px"></asp:TextBox></div>
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
                <asp:RadioButtonList ID="rblPageLayout" runat="server" DataSource="<%# PageLayouts %>" DataValueField="Key" DataTextField="Value" SelectedItem="<%# Model.PageLayout %>" />
            </div>
        </div>

        <div class="field-group required" id="imageSection">
            <div class="label">Page Image</div>
            <controls:ReleaseImagePicker ID="pageImagePicker" runat="server" Value="<%# Model.PageImageId %>" LanguageId="4105" ReleaseImageHeight="60" />
        </div>

        <div class="field-group" id="validator">
            <div class="label">Planned Publish Date</div>           
            <asp:TextBox  ID="publishDateTimePicker" runat="server" ></asp:TextBox>
        </div>

        <div class="field-group required">
            <div class="label">Ministries</div>
            <div class="CheckBoxList">
                <asp:CheckBoxList ID="chklstMinistries" CssClass="chkbxlst" runat="server" DataSource="<%# Model.Ministries %>" DataTextField="Text" DataValueField="Value" OnDataBound="chklstMinistries_DataBound" />
            </div>
        </div>

        <div class="field-group required" id="sectorsSection">
            <div class="label">Sectors</div>
            <div class="CheckBoxList">
                <asp:CheckBoxList ID="cklstSector" CssClass="chkbxlst" runat="server" DataSource="<%# Model.Sectors %>" DataTextField="Text" DataValueField="Value" OnDataBound="cklstSector_DataBound" />
            </div>
        </div>

        <div class="field-group" id="themesSection">
            <div class="label">Themes</div>
            <div class="CheckBoxList">
                <asp:CheckBoxList ID="cklstTheme" CssClass="chkbxlst" runat="server" DataSource="<%# Model.Themes %>" DataTextField="Text" DataValueField="Value" OnDataBound="cklstTheme_DataBound" />
            </div>
        </div>

        <div id="Organizations" class="field-group required">
            <div class="label">Organizations<span class="disable-note"> Update the page layout above to formal to enter organizations</span></div>
            <div class="txt"> 
                <asp:TextBox ID="txtOrganization" runat="server" MaxLength="250" Width="500px" TextMode="MultiLine" Height="90px"></asp:TextBox>
            </div>
        </div>

        <div class="field-group required">
            <div class="label">Headline</div>
            <div class="txt"> 
                <asp:TextBox ID="txtHeadline" runat="server" MaxLength="255" Width="500px"></asp:TextBox>
            </div>
        </div>

        <div class="field-group">
            <div class="label">Subheadline</div>
            <div class="txt"> 
                <asp:TextBox ID="txtSubheadline" runat="server" MaxLength="100" Width="500px"></asp:TextBox>
            </div>
        </div>

        <div id="ByLine" class="field-group">
            <div class="label">Byline<span class="disable-note"> Update the page layout above to informal to enter a byline</span></div>                    
            <div class="txt"> 
                <asp:TextBox ID="txtByline" runat="server" MaxLength="250" Width="500px" TextMode="MultiLine" Height="90px"></asp:TextBox>
            </div>
        </div>

        <div class="field-group">
            <div class="label">Location</div>
            <div class="txt">
                <asp:TextBox ID="txtLocation" runat="server" MaxLength="50"  Width="300px" ></asp:TextBox>
            </div>
        </div>

        <div class="field-group required" id="tagsSection">
            <div class="label">Tags</div>
            <div class="CheckBoxList">
                <asp:CheckBoxList ID="cklstTag" CssClass="chkbxlst" runat="server" DataSource="<%# Model.Tags %>" DataTextField="Text" DataValueField="Value" OnDataBound="cklstTag_DataBound" />
            </div>
        </div>

        <div class="field-group">
            <div class="label">Body</div>
            <CKEditor:CKEditorControl ID="contentCKEditor" ClientIDMode="Static" BasePath="~/Scripts/ckeditor" runat="server" Height="300"></CKEditor:CKEditorControl>     
            <%--CustomDictionarySourceTypeName="Gcpe.Hub.DatabaseCustomDictionarySource, Gcpe.Hub"--%>
        </div>

        <div class="field-group">
            <div class="label">Contacts</div><%--Comm. Contacts--%>
            <div class="txt" id="contacts" style="padding:0px 5px 5px 0px;">
                <asp:Repeater runat="server" DataSource="<%# Model.Contacts %>" ItemType="System.String">
                    <ItemTemplate>
                        <textarea name="parameters<%# Container.ItemIndex.ToString() %>" id="parameters<%# Container.ItemIndex.ToString() %>" class="media-textarea"><%# Item %></textarea>
                    </ItemTemplate>
                    <FooterTemplate>
                        <textarea name="parameters<%# Model.Contacts.Count().ToString() %>" id="parameters<%# Model.Contacts.Count().ToString() %>" class="media-textarea"></textarea>
                    </FooterTemplate>
                </asp:Repeater>
            </div> 
           
        </div>

        <div class="actions">
            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="primary" OnClick="btnSave_Click" />
        </div>

    </div>

</asp:Content>
