var warn_on_unload = "The activity has changes that have not been saved.";
// Tracking changes can be done all in one variable
// No need to capture every changes.
// Highlighting fields modified (but other users) is useful to HQ
var isChanged = false;
function limitChars(textid, limit, infodiv) {
    var text = $('#' + textid).val();
    if (!text) {
        $('#' + infodiv).html('Characters remaining: ' + limit);
        return;
    }
    //text = $.trim(text);
    var textlength = text.length;
    var newLines = (text.match(/\n/g) || []);
    var addition = 0;
    if (newLines != null) {
        addition = newLines.length;
    }
    textlength += addition;
    if (textlength > limit) {
        var newText = text.substr(0, limit - addition);
        var nextText = text.substr(0, limit - addition + 1);
        do {
            textlength = nextText.length;
            newLines = (nextText.match(/\n/g) || []);
            addition = 0;
            if (newLines != null) {
                addition = newLines.length;
            }
            textlength += addition;
            if (textlength <= limit)
                newText = nextText;
            nextText = text.substr(0, textlength - addition + 1);
        } while (textlength < limit);
        $('#' + infodiv).html('You cannot write more than ' + limit + ' characters!');
        $('#' + textid).val(newText);
        return false;
    }
    else {
        $('#' + infodiv).html('Characters remaining: ' + (limit - textlength));
        return true;
    }
}
function SetChanged(activityId) {
    var id = activityId || '';
    if (!isChanged)
        GetActivityStatus(id); // we only one to do this once per open activity, rather than every time a change event is fired on a control
    isChanged = true;
    $('#ChangesPending').show();
    $('#InactivityNotice').hide();
    $("#SavedSuccessfullyNotice").hide();
}
function GetChanged() {
    return isChanged;
}
var conflictMsgWasShown = false;
function GetActivityStatus(activityId) {
    $.ajax({
        type: "POST",
        url: "ActivityEditingHandler.ashx?Op=GetActivityStatus",
        data: { 'activityId': activityId },
        dataType: "text",
        success: function (msg) {
            if (msg) {
                alert(msg);
                conflictMsgWasShown = true;
                window.onbeforeunload = function () { };
                window.open('', '_self', '').close();
            }
        }
    });
}
function DoCancel(id) {
    if (conflictMsgWasShown)
        return;
    var formData = new FormData();
    formData.append("activityId", id);
    navigator.sendBeacon("ActivityEditingHandler.ashx?Op=CancelEditingActivity", formData);
}
// I can't call uncheckAll as it forces the close method where we track if things have changed
function MultiSelectReset(sender) {
    sender.multiselect("widget").find(":checkbox").each(function () {
        $(this).removeAttr('checked');
    });
}
function SetMultiSelectedValues(sender, target, hiddentarget, validator, validatorRow, isDirtyYN) {
    // the getChecked method returns an array of DOM elements.
    // map over them to create a new array of just the values.
    // you could also do this by maintaining your own array of
    // checked/unchecked values, but this is just as easy.
    var checkedValues = $.map(sender.multiselect("getChecked"), function (input) {
        return input.value;
    });
    var checkedTitles = $.map(sender.multiselect("getChecked"), function (input) {
        return input.title;
    });
    // update the target based on how many are checked
    target.html(checkedTitles.length ? checkedTitles.join(', ') : '');
    // Show / hide the target row
    if (checkedTitles.length > 0) {
        if (validator != null) {
            validator.hide();
        }
        validatorRow.show();
    }
    else {
        if (isDirtyYN == 'true' && validator != null) {
            validator.show();
        }
        validatorRow.hide();
    }
    // update the target based on how many are checked
    hiddentarget.val(checkedValues.length ? checkedValues.join(',') : '');
}
function hideIrrelevantPanels(category) {
    //var hideEvent = category == "Release Only (No Event)" || category == "Public Service Announcement";
    var hideRelease = false;
    switch (category) {
        case "Marketing / Advertising":
        case "Conference / AGM / Forum":
        case "TV / Radio":
        case "Event (3rd Party) - No Release":
        case "Awareness Day / Week / Month":
        case "IGRS use: Half-Masting":
        case "IGRS use: National Day":
        case "IGRS use: Visit":
            hideRelease = true;
            break;
    }
    /*if (hideEvent)
        $("#eventFieldset").hide();
    else
        $("#eventFieldset").show();*/
    if (hideRelease) {
        $("#releaseFieldset").hide();
        $('#NRTime').val('');
    }
    else {
        $("#releaseFieldset").show();
    }
}
//# sourceMappingURL=activityhelper.js.map