$(function () {
    $(".filter").keypress(function (event) {
        if (event.keyCode == 13 || event.which == 13) {
            $("#ExecuteSearchButton").click();
            event.preventDefault();
        }
    });
});
//----------------------------------------------------------------------
// This is for the Flexigrid, it handles the row being selected
// In the UserControl, it is a parameter
var zeroWidthSpace = '\u200B';
function rowSelected(row, ctrl) {
    var selectedActivity = $(row.children("td[model='MinistryActivityId']")).text().replace(zeroWidthSpace, '');
    var updatedTextIndex = selectedActivity.indexOf("updated");
    if (updatedTextIndex >= 0) {
        selectedActivity = selectedActivity.substring(0, updatedTextIndex);
    }
    else {
        var updatedTextIndex = selectedActivity.indexOf("created");
        if (updatedTextIndex >= 0)
            selectedActivity = selectedActivity.substring(0, updatedTextIndex);
    }
    var selectedActivities = $('#SelectedTextBox').val(); // Should be a comma delimited list of activity ids
    if (selectedActivities.indexOf(selectedActivity + ',') >= 0) {
        selectedActivities = selectedActivities.replace(selectedActivity + ',', '');
    }
    else {
        selectedActivities += selectedActivity + ',';
    }
    $('#SelectedTextBox').val(selectedActivities);
    return true;
}
//----------------------------------------------------------------------
// Checks if the From & To dates are the same if This day only is selected.
function checkThisDayOnly() {
    var checkBox = $("#ThisDayOnlyCheckBox")[0];
    if (checkBox.checked) {
        if ($("#StartDateTextBox").val() == "") {
            return 'The start date cannot be empty.';
        }
    }
    return null;
}
//----------------------------------------------------------------------
// Save a filter to DB
function saveFilter(name, queryString, msgCallBack) {
    $.ajax({
        type: "POST",
        data: JSON.stringify({ name: name, queryString: queryString }),
        contentType: "application/json; charset=utf-8",
        url: "ActivityFilter.asmx/SaveNewQuery",
        dataType: "json",
        async: true,
        success: function (result) {
            var newFilterId = result.d;
            var listItem = '<li>' + '<span id="' + newFilterId + '">';
            listItem += '<a href=\"javascript:runMyQuery(\'' + queryString + '\')\">' + name + '</a></span></li>';
            $("#MyQueryList").append(listItem);
            msgCallBack();
        },
        error: function (request, statusText, error) {
            msgCallBack(error);
        }
    });
}
function UpdateMyQueriesList(updatedData) {
    $("#MyQueryList").empty();
    for (var i in updatedData) {
        var aFilter = updatedData[i];
        var listItem = '<li><span id="' + aFilter.Id + '">'
            + '<a href=\"javascript:runMyQuery(\'' + aFilter.QueryString + '\')\">' + aFilter.Name + '</a></span></li>';
        $("#MyQueryList").append(listItem);
    }
}
function clickDelete(a, btn) {
    var item = btn.parentNode.parentNode;
    item.parentNode.removeChild(item);
    deleteOrderClick(a);
    event.preventDefault();
    return false;
}
function deleteOrderClick(itemsToDelete) {
    if ($('#toBeDeletedIds').val().length > 1) {
        var delIds = $('#toBeDeletedIds').val() + "|" + itemsToDelete;
        $('#toBeDeletedIds').val(delIds);
    }
    else {
        $('#toBeDeletedIds').val(itemsToDelete);
    }
}
function saveOrderClick(msgCallBack) {
    //------
    // Get the order of the items
    // ----- Retrieve the li items inside our sortable list
    var items = $("#sortables li");
    var idWithName = [];
    var index = 0;
    // ----- Iterate through each li, extracting the ID embedded as an attribute and text
    // The order they should be displayed is the index value
    // ID|Text,ID|Text,ID|Text
    items.each(function (intIndex) {
        idWithName.push($(this).attr("exampleItemID") + '|' + $(this).find('input:text:first').val());
    });
    //---------------------------------------------------------------------------
    // Save it to the database
    var itemsToDelete = $('#toBeDeletedIds').val();
    var json_IdInOrderWithNames = JSON.stringify(idWithName);
    var json_DeleteIds = JSON.stringify(itemsToDelete);
    $.ajax({
        type: "POST",
        data: JSON.stringify({ names: json_IdInOrderWithNames, todelete: json_DeleteIds }),
        contentType: "application/json; charset=utf-8",
        url: "ActivityFilter.asmx/SaveMyQueries",
        dataType: "json",
        async: true,
        success: function (result) {
            UpdateMyQueriesList(result.d);
            msgCallBack();
        },
        error: function (request, error) {
            msgCallBack(error);
        }
    });
    $('#toBeDeletedIds').val("");
}
//# sourceMappingURL=activityList.js.map