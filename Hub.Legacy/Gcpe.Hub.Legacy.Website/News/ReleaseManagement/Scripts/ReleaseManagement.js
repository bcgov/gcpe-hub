var orgDisabledTxt = "Not applicable to a informal page layout";
var bylineDisabledTxt = "Not applicable to a formal page layout";


/* Disable Element passed in (used by Byline and Organization) */
function DisableElement(el) {
    var myFieldGroup = $(this).parent().closest('div .field-group');
    myFieldGroup.addClass('disable-field');
    myFieldGroup.find('.disable-note').show();
    el.prop('disabled', true);
}

/* Enable Element passed in (used by Byline and Organization) */
function EnableElement(el) {
    var myFieldGroup = $(this).parent().closest('div .field-group');
    myFieldGroup.removeClass('disable-field');
    myFieldGroup.find('.disable-note').hide();
    el.prop('disabled', false);
}


function EnableOrganization(el, currentOrgTxt) {
    EnableElement(el)
    if (el.val() == orgDisabledTxt)
        el.val(currentOrgTxt);
}

// reset value to blank OR the last value
function EnableByline(el, currentBylineText) {
    EnableElement(el)
    if (el.val() == bylineDisabledTxt)
        el.val(currentBylineText);
}




/* Gets the text based on what items are checked */
function GetNamesSelected(currentList, checkBoxList) {

    var currentOrgs = currentList;

    $("[id*=" + checkBoxList + "] input").each(function () {

        var org = $('label[for=' + this.id + ']').html();

        if ($(this).is(":checked")) {
            if (currentOrgs.indexOf(org) == -1) {
                if (currentOrgs.length == 0)
                    currentOrgs = org;
                else
                    currentOrgs = currentOrgs + "\n" + org;
            }
        } else {

            if (currentOrgs.indexOf("\n" + org) > -1)
                currentOrgs = currentOrgs.replace("\n" + org, "");

            if (currentOrgs.indexOf(org) > -1)
                currentOrgs = currentOrgs.replace(org, "");
        }
    });

    if (currentOrgs.indexOf("\n") == 0) // Remove a newline at the start    
        currentOrgs = currentOrgs.substring(1, currentOrgs.length);

    return currentOrgs;

}



function SelectAllCheckBoxList(changeLabel, checkBoxList) {

    if (changeLabel.html().trim() == "Select All") {
        changeLabel.parent().closest('div .field-group').removeClass('warning-chkbxlst');
        $("[id*=" + checkBoxList + "] input").prop('checked', true);
        changeLabel.html("Deselect All");
    } else {
        $("[id*=" + checkBoxList + "] input").prop('checked', false);
        changeLabel.html("Select All");
    }

}


function CheckReleaseTypeValid(txtPageTitle, documentType, confirmationDiv, pageTitleOptions, selectedReleaseType) {

    // Reset warning labels
    var myFieldGroup = txtPageTitle.parent().closest('div .field-group');
    myFieldGroup.removeClass('warning');
    myFieldGroup.removeClass('warning-page-title');
    // Reset the Ignore & Add buttons.
    confirmationDiv.css('display', 'none');

    var releaseType = PageTitleReleaseType(documentType, pageTitleOptions);

    var typeIsValid = false;

    if (releaseType != null) {
        if (selectedReleaseType == 5) {
            // Advisories(5) should include the PageTitle options for Releases(1) & Factsheets(3) too.
            // FYI: This same check exists in the code in New.aspx page too!
            typeIsValid = releaseType == 5 || releaseType == 1 || releaseType == 3;
        } else {
            typeIsValid = releaseType == selectedReleaseType;
        }
    }

    // If page title is new show confirmation
    if (!typeIsValid) {
        confirmationDiv.css('display', 'inline-block'); // these are the Ignore & Add buttons.
        myFieldGroup.addClass('warning-page-title');
        txtPageTitle.parent().closest('.txt').find(".AddNewDocumentType").toggle(releaseType == null);
    }
}

/* Checks if the page title entered is an existing one or custom new one entered */
function PageTitleReleaseType(title, allOptions) {
    for (var i in allOptions) {
        if ($.trim(i).toUpperCase() == title.toUpperCase())
            return allOptions[i];
    }
    return null;
}

function HidePageTitleConfirmation(txtPageTitle, pageTitleConfirmation, addReleaseType) {
    var myFieldGroup = txtPageTitle.parent().closest('div .field-group');
    if (addReleaseType)
        myFieldGroup.find("[id*='hidAddReleaseType']").val(true);
    pageTitleConfirmation.hide();
    myFieldGroup.removeClass('warning-page-title');
}



function PageLayout_FormalSelected(orgEl, bylineEl) {

    var previousText = "";

    if (bylineEl.val() != bylineDisabledTxt)
        previousText = bylineEl.val(); // store value for later before we disable it

    EnableOrganization(orgEl, orgTxt);
    DisableElement(bylineEl);
    bylineEl.val(bylineDisabledTxt);

    orgEl.parent().closest('div .field-group').addClass('required');

    return previousText;
}



function PageLayout_InformalSelected(orgEl, bylineEl) {

    var previousText = "";

    if (orgEl.val() != orgDisabledTxt)
        previousText = orgEl.val(); // store value for later before we disable it


    EnableByline(bylineEl, bylineTxt);
    DisableElement(orgEl);
    orgEl.val(orgDisabledTxt);

    var orgFieldGroup = orgEl.parent().closest('div .field-group');
    orgFieldGroup.removeClass('required');
    orgFieldGroup.removeClass('warning'); // just in case validation has happened already

    return previousText;

}

/* Disable or Enables Organization & Byline depending on the layout */
function SetPageLayoutDefaults(layout, txtOrganization) {
    var txtByline = txtOrganization.parent().closest('.section.edit').find("[id*='txtByline']");
    if (layout == "Formal")
        bylineTxt = PageLayout_FormalSelected(txtOrganization, txtByline);
    else
        orgTxt = PageLayout_InformalSelected(txtOrganization, txtByline);
}
function ValidateCheckBoxList(box, firstError) {
    var length = box.find("input:checked").length;
    if (length == 0) {
        box.parent().closest('div .field-group').addClass('warning-chkbxlst');
        if (firstError == null)
            return box;
    } else {
        box.parent().closest('div .field-group').removeClass('warning-chkbxlst');
        if (firstError == box)
            return null;
        else
            return firstError;

    }
    return firstError;
}

function IsDocumentValid(saveBtn) {

    var firstError = null;
    var fieldArea = null;
    var detailsEdit = $(saveBtn).parent().parent();
    var txtPageTitle = detailsEdit.find("[id*='txtPageTitle']");
    var pageTitleConfirmation = detailsEdit.find(".PageTitleConfirmation");

    if (txtPageTitle.length != 0) {
        if (!txtPageTitle.val().trim()) {

            fieldArea = txtPageTitle.parent().closest('div .field-group');
            fieldArea.addClass('warning');
            if (firstError == null) {
                firstError = fieldArea;
            }
        } else if (pageTitleConfirmation.is(':visible')) {
            fieldArea = txtPageTitle;
            fieldArea.addClass('warning-page-title');
            if (firstError == null) {
                firstError = fieldArea;
            }
        }
    }

    var rblPageLayout = detailsEdit.find("[id*='rblPageLayout']");
    if (rblPageLayout.length != 0) {
        firstError = ValidateCheckBoxList(rblPageLayout, firstError);
    }


    var txtHeadline = detailsEdit.find("[id*='txtHeadline']");
    if (!txtHeadline.val().trim()) {
        fieldArea = txtHeadline.parent().closest('div .field-group');
        fieldArea.addClass('warning');
        if (firstError == null)
            firstError = fieldArea;
    }

    var txtBody = detailsEdit.find("[id*='contentCKEditor']:first");
    var contentCKEditor = CKEDITOR.instances[txtBody.attr('id')];
    fieldArea = txtBody.parent().closest('div .field-group');
    if (!contentCKEditor || !contentCKEditor.getData().trim()) {
        fieldArea.addClass('warning');
        if (firstError == null)
            firstError = fieldArea;
    } else {
        fieldArea.removeClass('warning');
    }

    //TODO: TRINITY: Is this the correct way to check that this control is not visible on server-side?
    var txtOrganization = detailsEdit.find("[id*='txtOrganization']");
    if (txtOrganization.length > 0) {
        if (!txtOrganization.val().trim()) {
            if (txtOrganization.is(':enabled')) {
                fieldArea = txtOrganization.parent().closest('div .field-group');
                fieldArea.addClass('warning');
                if (firstError == null)
                    firstError = fieldArea;
            }
        }
    }

    if (firstError != null) {
        $('html, body').animate({ scrollTop: firstError.offset().top - 25 }, 500);// scroll to the position
        firstError.find(":input").focus(); // focus on field
        return false;
    } else {
        return true;
    }
}


function onPageReloaded(sender, args) {
    FB.XFBML.parse();
    var hiddenEditControlButton = $(".editButton[style='visibility: hidden;']");
    if (hiddenEditControlButton.length)
        onDocumentEdit(hiddenEditControlButton.get());
    //window.scrollTo(0,0);
}

function onCancel(cancelBtn) {
    var detailsEdit = $(cancelBtn).parent().closest(".section.edit");
    detailsEdit.hide();
    detailsEdit.siblings(".section.view").show(); // DocumentPreview
    return false;
}

function onDocumentEdit(editSwitch) {
    // Performs the validation, cancels updatepanel postback if not valid
    //Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);

    HideAllSettings();
    var documentPreview = $(editSwitch).parent().closest(".section.view");
    documentPreview.hide();
    var detailsEdit = documentPreview.siblings(".section.edit");
    var pageTitleOptions = eval('({' + $(editSwitch).attr("data-defaultPageTitles") + '})');
    detailsEdit.show(); // DetailsEdit
    detailsEdit.removeClass('disable-section');


    // Document Type
    //  - setup autocomplete
    //  - handle when changed
    //  - inline validation

    /* Tracks if user makes any changes so values are no longer updated automatically */
    //var hasUserChangedPageLayout = <%= (!Model.IsNew).ToString().ToLower() %>;

    var rblPageLayout = detailsEdit.find("[id*='rblPageLayout']");
    var txtOrganization = detailsEdit.find("[id*='txtOrganization']");
    var pageTitleConfirmation = detailsEdit.find(".PageTitleConfirmation");
    var txtPageTitle = detailsEdit.find("[id*='txtPageTitle']");
    var selectedReleaseType = $("#hidReleaseType").val();
    txtPageTitle.autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            for (var property in pageTitleOptions) {
                var releaseType = pageTitleOptions[property];
                if (releaseType == selectedReleaseType) {
                    data.push(property);
                }
            }
            var data = $.grep(data, function (value) {
                return value.substring(0, request.term.length).toLowerCase() == request.term.toLowerCase();
            });
            response(data);
        },
        select: function (a, b) { // From the list
            CheckReleaseTypeValid(txtPageTitle, b.item.value, pageTitleConfirmation, pageTitleOptions, selectedReleaseType);
            //if (!hasUserChangedPageLayout)
            SetPageDefaults(b.item.value, rblPageLayout, txtOrganization);
            pageTitleConfirmation.hide();
        },
        change: function (event, ui) { // On mouse out or blur
            var title = txtPageTitle.val();
            if (title.trim() == "") {
                txtPageTitle.parent().closest('div .field-group').addClass('warning'); /* Inline validation */
            } else {
                CheckReleaseTypeValid(txtPageTitle, title, pageTitleConfirmation, pageTitleOptions, selectedReleaseType);
                //if (!hasUserChangedPageLayout)
                SetPageDefaults(title, rblPageLayout, txtOrganization);
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
    detailsEdit.find(".IgnoreNewDocumentType").button({
        icons: {
            primary: "ui-icon-circle-check"
        }
    }).click(function () {
        HidePageTitleConfirmation(txtPageTitle, pageTitleConfirmation);
        return false;
    });

    detailsEdit.find(".AddNewDocumentType").button({
        icons: {
            primary: "ui-icon-circle-check"
        }
    }).click(function () {
        HidePageTitleConfirmation(txtPageTitle, pageTitleConfirmation, true);
        return false;
    });



    // Page Layout
    //  - Enable/Disable Byline and Organization
    //  - Track if user has changed value
    rblPageLayout.filter("input").change(function () {
        hasUserChangedPageLayout = true;
        SetPageLayoutDefaults($(this).val(), txtOrganization);
        rblPageLayout.parent().closest('div .field-group').removeClass('warning-chkbxlst');
    });


    // Add or Remove Comm. Contact Row
    var contacts = detailsEdit.find(".comm-contacts");
    contacts.on('keyup', 'textarea', function () {
        /* TODO: Remove blank contacts
        // If all the elements are blank, remove them all except the first and focus on the first element
        var firstEl = contacts.children("textarea").first();
        var nbrBlanks = 0;
        if (firstEl.val() == "") {
            $("#contacts textarea").each(function () {
                if ($(this).val() == "")
                    nbrBlanks++;
            });
        }
        if (nbrBlanks == contacts.children("textarea").length) {
            firstEl.focus();
            contacts.children("textarea").each(function () {
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
        } */

        var textareas = contacts.children("textarea");
        //If the last one is blank, just get out of here
        if (textareas.last().val() == "") {
            return;
        }

        var newTxt = $(this).clone();
        var name = newTxt.attr('name');
        newTxt.attr('name', name.substring(0, name.indexOf(':') + 1) + textareas.length);
        newTxt.val('');

        $(this).parent().append(newTxt);
    });




    /* Inline Validation for required fields */
    var txtHeadline = detailsEdit.find("[id*='txtHeadline']");
    txtHeadline.blur(function () {
        var headlineFieldGroup = $(this).parent().closest('div .field-group');
        if (!$(this).val().trim())
            headlineFieldGroup.addClass('warning');
        else
            headlineFieldGroup.removeClass('warning');
    });

    txtOrganization.blur(function () {
        var orgFieldGroup = $(this).parent().closest('div .field-group');
        if (!$(this).val().trim())
            orgFieldGroup.addClass('warning');
        else
            orgFieldGroup.removeClass('warning');
    });


    // Set defaults on page load
    if (rblPageLayout.length) {
        // CanChangeLayout
        var selectedPageLayout = rblPageLayout.filter("input:checked").val();
        SetPageLayoutDefaults(selectedPageLayout, txtOrganization);
    }

    ImagePickerEvents(detailsEdit);

    return false;
}

function ImagePickerEvents(detailsEdit) {

    var divImageChoice = detailsEdit.find("div.ImageChoice");
    var divImageSelected = detailsEdit.find("div.ImageSelected");
    var changeImage = divImageSelected.find(".ChangeImage");

    /* For Page image you can click on the 'change' link to show the list of images you can select from */
    changeImage.on("click", function () {
        divImageSelected.hide();
        divImageChoice.show();
        return false;
    });

    var imgSelected = divImageSelected.find("img");
    /* For page image you can click on the selected image to show the list of images you can select from */
    imgSelected.on("click", function () {
        divImageSelected.hide();
        divImageChoice.show();
        return false;
    });

    var imgChoice = divImageChoice.find("img");
    /* When an image is selected, close the choices and update the selected image */
    imgChoice.on("click", function () {
        hasUserChangedPageImage = true; // first lets make sure we track they made a manual change

        imgChoice.removeClass("selected-image"); /* Removes the selected indicator on all the images */
        $(this).addClass("selected-image"); /* Sets the current one clicked on to selected */

        var id = $(this).attr('id'); /* The guid is stored as the id of the field */

        var hdPageImage = detailsEdit.find("[id*='hdPageImage']");
        hdPageImage.val(id);  /* Set the hidden variable for save on the server */
        if (id != "") {
            id = "&Id=" + id;
        }
        imgSelected.prop("src", imgChoice.attr('src') + id);

        divImageSelected.show();
        divImageChoice.hide();

        return false;
    });
}


function ScrollToId(controlId) {
    $('html, body').scrollTop($(controlId).offset().top - $('#Top').offset().top);

    return false;
}

function IntersectAmount(top1, bottom1, top2, bottom2) {
    // returns the height in pixels of the intersection between 2 rectangles
    var d1 = bottom1 - top2;
    if (d1 < 0) return 0; // does not intersect
    var d2 = bottom2 - top1;
    return d2 > 0 ? Math.min(d1, d2) : 0;
}

function MapScrollItems() {
    // based on Minimalistic ScrollSpy from http://jsfiddle.net/mekwall/up4nu/
    // Cache selectors
    var lastId,
        topMenu = $("#nav-menu"),
        headerHeight = $('#header').height(),
        // All nav items
        menuItems = topMenu.find(".nav-menu-item"),
        // divs corresponding to menu items
        scrollItems = menuItems.map(function () {
            var divItem = $($(this).attr("href"));
            return divItem;
        });


    // Bind click handler to menu items
    menuItems.click(function (e) {
        var href = $(this).attr("href");
        ScrollToId(href);
        e.preventDefault();
    });

    function onScroll() {
        // Get container scroll position
        var scrollTop = $(this).scrollTop();
        var itemId = "";
        var intersectMax = 0;

        $.each(scrollItems, function (index, scrollItem) {
            var itemTop = $(scrollItem).offset().top - scrollTop;
            var itemBottom = itemTop + $(scrollItem).height();
            itemTop -= headerHeight;

            if (itemTop > 0 && itemBottom < $(window).height()) {
                // First fully visible section. Grab it
                itemId = scrollItem[0].id;
                return false;
            }
            var intersectAmount = IntersectAmount(itemTop, itemBottom, 0, $(window).height());
            if (intersectAmount > intersectMax) {
                intersectMax = intersectAmount;
                itemId = scrollItem[0].id;
            }
            return true; // keep iterating to see if we can find something better
        });

        if (lastId !== itemId) {
            lastId = itemId;
            // Set/remove active class
            menuItems.removeClass("selected-page");
            var newMenuItem = menuItems.filter("[href='#" + itemId + "']");
            newMenuItem.addClass("selected-page");
        }
    }

    // Bind to scroll
    $(window).scroll(onScroll);
    $(window).resize(onScroll);
    // Simulate scroll to set the selected item (IE does this for you but not Chrome nor Firefox)
    onScroll();
}

function OnTopOrFeatureSwitch(anchor) {
    try {
        var root = $(anchor).parent().closest(".ToggleTopOrFeature");
        var lblTopOrFeature = root.find(".TopOrFeature");
        var switches = root.find('.switch');

        if (switches[0].text != "Cancel") {
            // setting or swapping
            lblTopOrFeature.text(anchor.text.substring(anchor.text.lastIndexOf(' ') + 1));
            switches[0].text = "Cancel";
            $(switches[1]).css('visibility', 'hidden');
        } else {
            // Restore text of first switch
            if (switches[1].text.startsWith('Swap')) {
                lblTopOrFeature.text(switches[1].text.endsWith('Top') ? 'Feature' : 'Top');
                switches[0].text = switches[1].text;
            } else {
                lblTopOrFeature.text("");
                switches[0].text = "Set as Top";
                $(switches[1]).css('visibility', '');
            }
        }


        // store the text of the lblTopOrFeature in the hidden field so it is sent server side
        root.find("input[type=hidden]").val(lblTopOrFeature.text());
        // the user has to cancel first before the category can be unchecked (N/A for Home)
        root.find('input[type=checkbox]').prop('disabled', lblTopOrFeature.text());
    }
    catch (ex)
    {
    }
   
    return false;
}

var draggedSource = null; // can't use ev.dataTransfer.getData because it is not accessible from OnDragOver

function OnDragStart(ev) {
    draggedSource = $(ev.target).closest("[ondrop]");
    ev.dataTransfer.setData('text', 'Make HMTL5 D&D work in Firefox');
}

function OnDragEnd(ev) {
    draggedSource = null;
}

function OnDragOver(ev) {
    if (!draggedSource) return;
    ev.preventDefault()
}
