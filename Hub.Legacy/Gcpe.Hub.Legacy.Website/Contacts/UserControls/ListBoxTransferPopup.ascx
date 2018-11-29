<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserControls_ListBoxTransferPopup" CodeBehind="ListBoxTransferPopup.ascx.cs" %>

<div id="listBoxTransferPopup" class="modal fade">
    <div class="modal-dialog">
        <div class="modal-content">

            <h2 style=""><%= PopupHeader %></h2>

            <div class="left">

                <div class="field"><%= AvailableListHeader %></div>

                <p>
                    <input type="text" id="availableDistributionListsFilter" onkeyup="FilterAvailableDistributionLists(this);" />
                    <a href="javascript:void(0);" onclick="ClearAllInputFilter();">
                        <img runat="server" src="~/Contacts/images/GreyX.png" border="0" /></a>
                    <span class="common-text-placeholder" id="available-place-holder">Filter lists</span>
                </p>

                <select multiple="multiple" name="availableDistributionLists" id="availableDistributionLists">
                    <asp:Literal runat="server" ID="availableDistributionListsLit" />
                </select>

            </div>

            <div class="center">
                <p>
                    <a href="javascript:void(0);" id='addListBoxHref' onclick="AddToSelectedLists();return false;" class="add-off"></a>
                    <a href="javascript:void(0);" id='removeListBoxHref' onclick="RemoveFromSelectedLists();return false;" class="remove-off"></a>
                </p>
            </div>

            <div class="right">

                <div class="field"><%= SelectedListHeader %></div>

                <p>
                    <input type="text" id="selectedDistributionListsFilter" onkeyup="FilterSelectedDistributionLists(this);" />
                    <a href="javascript:void(0);" onclick="ClearSelectedInputFilter();">
                        <img runat="server" src="~/Contacts/images/GreyX.png" border="0" /></a>
                    <span class="common-text-placeholder" id="selected-place-holder">Filter lists</span>
                </p>

                <select multiple="multiple" name="selectedDistributionLists" id="selectedDistributionLists">
                    <asp:Literal runat="server" ID="selectedDistributionListsLit" />
                </select>

            </div>

            <div class="clear"></div>

            <div class="button-container">
                <a href="javascript:void(0);" onclick="CancelDistributionListChange();return false;" class="common-admin-link-button">Cancel</a>
                <a href="javascript:void(0);" onclick="SaveDistributionListChange();return false;" class="common-admin-link-button"><%= doneButtonText %></a>
            </div>
        </div>
    </div>

</div>


<script type="text/javascript">

    $("#listBoxTransferPopupLink").click(function () {
        $("#listBoxTransferPopup").modal('show');
        DialogBoxOnStart();
    });
    var addButtonMode = "off";
    var removeButtonMode = "off";

    function DialogBoxOnStart() {
        SelectAllElements(false);

        SetupTypeDown("available-place-holder", "availableDistributionListsFilter");
        SetupTypeDown("selected-place-holder", "selectedDistributionListsFilter");

        addButtonMode = "off";
        removeButtonMode = "off";

        SetButtonModes();

        SetListBoxEvents();
    }

    function SetListBoxEvents() {
        var addToListBox = document.getElementById('availableDistributionLists');
        var removeFromListBox = document.getElementById('selectedDistributionLists');


        addToListBox.onclick = function () {
            addButtonMode = "on";
            removeButtonMode = "off";

            SetButtonModes();
        }

        removeFromListBox.onclick = function () {
            addButtonMode = "off";
            removeButtonMode = "on";

            SetButtonModes();
        }

    }

    function SetButtonModes() {
        var addElm = document.getElementById('addListBoxHref');
        var removeElm = document.getElementById('removeListBoxHref');

        if (addButtonMode == "off") {
            addElm.className = "add-off";
        } else {
            addElm.className = "add-on";
        }

        if (removeButtonMode == "off") {
            removeElm.className = "remove-off";
        } else {
            removeElm.className = "remove-on";
        }
    }

    var availableDistributionListOptions = null;
    var selectedDistributionListOptions = null;

    var originalSelectedItems = null;
    var originalAvailableItems = null;

    function ClearAllInputFilter() {
        var textElement = document.getElementById('availableDistributionListsFilter');
        textElement.value = "";

        FilterAvailableDistributionLists(textElement);
    }

    function ClearSelectedInputFilter() {
        textElement = document.getElementById('selectedDistributionListsFilter');
        textElement.value = "";

        FilterSelectedDistributionLists(textElement);
    }

    function ClearFilters() {

        var selectedListsElement = document.getElementById('selectedDistributionLists');
        var availableListsElement = document.getElementById('availableDistributionLists');

        //available distribution lists        
        var textElement = document.getElementById('availableDistributionListsFilter');
        textElement.value = "";

        FilterAvailableDistributionLists(textElement);

        // selected distribution lists
        textElement = document.getElementById('selectedDistributionListsFilter');
        textElement.value = "";

        FilterSelectedDistributionLists(textElement);
    }

    function CancelDistributionListChange() {
        ClearFilters();

        var selectedListsElement = document.getElementById('selectedDistributionLists');
        var availableListsElement = document.getElementById('availableDistributionLists');

        // reset available distribution lists
        if (originalAvailableItems != null) {
            RemoveItemsFromListBox(availableListsElement);
            AddItemsToListBox(availableListsElement, originalAvailableItems);
        }

        // reset selected distribution lists
        if (originalSelectedItems != null) {
            RemoveItemsFromListBox(selectedListsElement);
            AddItemsToListBox(selectedListsElement, originalSelectedItems);
        }

        CloseDialogBox();
    }

    function SaveDistributionListChange() {
        ClearFilters();
        CloseDialogBox();
    }

    function CloseDialogBox() {
        // reset the variables for next opening of the popup
        availableDistributionListOptions = null;
        selectedDistributionListOptions = null;
        originalSelectedItems = null;
        originalAvailableItems = null;

        // populate the distribution list display
        UpdateDistributionListArea();

        SelectAllElements(true);

        $('#listBoxTransferPopup').modal('hide');
        var element = document.getElementById('<%= SaveButtonId %>');
        element.click();
    }

    function SetInitialLists() {
        var selectedListsElement = document.getElementById('selectedDistributionLists');
        var availableListsElement = document.getElementById('availableDistributionLists');

        // only have to check one of the arrays since both will be set in here
        if (originalSelectedItems == null) {
            originalAvailableItems = new Array();
            originalSelectedItems = new Array();

            // must read from the array first in case the user has done a filter before
            // doing any changes to the lists
            if (availableDistributionListOptions != null) {
                for (var i = 0; i < availableDistributionListOptions.length; i++) {
                    originalAvailableItems.push(availableDistributionListOptions[i]);
                }
            } else {
                for (var i = 0; i < availableListsElement.options.length; i++) {
                    var option = document.createElement("option");
                    option.value = availableListsElement.options[i].value;
                    option.innerHTML = availableListsElement.options[i].innerHTML;

                    originalAvailableItems.push(option);
                }
            }

            // must read from the array first incase the user has done a filter before
            // doing any changes to the lists
            if (selectedDistributionListOptions != null) {
                for (var i = 0; i < selectedDistributionListOptions.length; i++) {
                    originalSelectedItems.push(selectedDistributionListOptions[i]);
                }
            } else {
                for (var i = 0; i < selectedListsElement.options.length; i++) {

                    var option = document.createElement("option");
                    option.value = selectedListsElement.options[i].value;
                    option.innerHTML = selectedListsElement.options[i].innerHTML;

                    originalSelectedItems.push(option);
                }
            }
        }
    }

    function AddToSelectedLists() {

        if (addButtonMode == "off") return;

        SetInitialLists();

        var selectedListsElement = document.getElementById('selectedDistributionLists');
        var availableListsElement = document.getElementById('availableDistributionLists');

        for (var i = availableListsElement.options.length - 1; i >= 0; i--) {
            if (availableListsElement.options[i].selected) {
                var element = availableListsElement.options[i];

                // create the new listbox item
                var option = document.createElement("option");
                option.value = element.value;
                option.innerHTML = element.innerHTML;

                if (availableDistributionListOptions != null) {
                    // remove the item from the list of available options
                    // this is only set if there was a filter previously
                    for (var k = availableDistributionListOptions.length - 1; k >= 0; k--) {
                        if (availableDistributionListOptions[k].value == option.value) {
                            availableDistributionListOptions.splice(k, 1);
                            break;
                        }
                    }
                }

                if (selectedDistributionListOptions != null) {
                    // add this item to the list of selected options
                    // this is only done if there was a filter previously done
                    selectedDistributionListOptions.push(option);
                }

                // clear out the filter of the selected distribution lists
                var textElement = document.getElementById('selectedDistributionListsFilter');
                textElement.value = '';

                // if there has never been a filter done just add the item to the listbox
                if (selectedDistributionListOptions == null) {
                    //selectedListsElement.appendChild(option);
                    AddItemsToListBox(selectedListsElement, [option]);
                } else {
                    // otherwise populate the listbox with the items from the array
                    RemoveItemsFromListBox(selectedListsElement);
                    AddItemsToListBox(selectedListsElement, selectedDistributionListOptions);
                }

                availableListsElement.remove(i);
            }
        }

        addButtonMode = "off";
        SetButtonModes();
    }

    function RemoveFromSelectedLists() {

        if (removeButtonMode == "off") return;

        SetInitialLists();

        var selectedListsElement = document.getElementById('selectedDistributionLists');
        var availableListsElement = document.getElementById('availableDistributionLists');

        for (var i = selectedListsElement.options.length - 1; i >= 0; i--) {
            if (selectedListsElement.options[i].selected) {
                var element = selectedListsElement.options[i];

                // create the new listbox item
                var option = document.createElement("option");
                option.value = element.value;
                option.innerHTML = element.innerHTML;

                if (selectedDistributionListOptions != null) {
                    // remove the item from the list of selected options
                    // this is only set if there was a filter previously
                    for (var k = selectedDistributionListOptions.length - 1; k >= 0; k--) {
                        if (selectedDistributionListOptions[k].value == option.value) {
                            selectedDistributionListOptions.splice(k, 1);
                            break;
                        }
                    }
                }

                if (availableDistributionListOptions != null) {
                    // add this item to the list of selected options
                    // this is only done if there was a filter previously done
                    availableDistributionListOptions.push(option);
                }

                // clear out the filter of the selected distribution lists
                var textElement = document.getElementById('availableDistributionListsFilter');
                textElement.value = '';

                // if there has never been a filter done just add the item to the listbox
                if (availableDistributionListOptions == null) {
                    //availableListsElement.appendChild(option);
                    AddItemsToListBox(availableListsElement, [option]);
                } else {
                    // otherwise populate the listbox with the items from the array
                    RemoveItemsFromListBox(availableListsElement);
                    AddItemsToListBox(availableListsElement, availableDistributionListOptions);
                }

                selectedListsElement.remove(i);
            }
        }

        removeButtonMode = "off";
        SetButtonModes();
    }

    function UpdateDistributionListArea() {
        var selectedListsElement = document.getElementById('selectedDistributionLists');

        if ("<%= hiddenFieldFlagId %>" != "") {
            document.getElementById("<%= hiddenFieldFlagId %>").value = "true";
        }

        if ("<%= CallBackMode %>" == "Listing") {

            var containerElement = document.getElementById('<%= ContainerClientId %>');
            containerElement.innerHTML = "";

            for (var i = 0; i < selectedListsElement.options.length; i++) {
                var div = document.createElement("div");
                div.innerHTML = selectedListsElement.options[i].innerHTML;

                containerElement.appendChild(div);
            }
        }

        if ("<%= CallBackMode %>" == "Count") {

            var containerElement = document.getElementById('<%= ContainerClientId %>');
            containerElement.innerHTML = selectedListsElement.options.length;

        }
    }

    function FilterAvailableDistributionLists(element) {

        var availableListsElement = document.getElementById('availableDistributionLists');

        if (availableDistributionListOptions == null) {
            availableDistributionListOptions = new Array();

            for (var i = 0; i < availableListsElement.options.length; i++) {
                var option = document.createElement("option");
                option.innerHTML = availableListsElement.options[i].innerHTML;
                option.value = availableListsElement.options[i].value;

                availableDistributionListOptions.push(option);
            }
        }

        var searchText = element.value.trim();

        var elementsToShow = new Array();

        if (searchText == null || searchText == "") {
            elementsToShow = availableDistributionListOptions;
        } else {
            for (var i = 0; i < availableDistributionListOptions.length; i++) {

                var regex = new RegExp(".*" + searchText + ".*", "gi");

                if (regex.test(availableDistributionListOptions[i].innerHTML)) {
                    elementsToShow.push(availableDistributionListOptions[i]);
                }
            }
        }

        RemoveItemsFromListBox(availableListsElement);
        AddItemsToListBox(availableListsElement, elementsToShow);
    }

    function FilterSelectedDistributionLists(element) {
        var selectedListsElement = document.getElementById('selectedDistributionLists');

        if (selectedDistributionListOptions == null) {
            selectedDistributionListOptions = new Array();

            for (var i = 0; i < selectedListsElement.options.length; i++) {
                var option = document.createElement("option");
                option.innerHTML = selectedListsElement.options[i].innerHTML;
                option.value = selectedListsElement.options[i].value;

                selectedDistributionListOptions.push(option);
            }
        }

        var searchText = element.value.trim();

        var elementsToShow = new Array();

        if (searchText == null || searchText == "") {
            elementsToShow = selectedDistributionListOptions;
        } else {
            for (var i = 0; i < selectedDistributionListOptions.length; i++) {

                var regex = new RegExp(".*" + searchText + ".*", "gi");

                if (regex.test(selectedDistributionListOptions[i].innerHTML)) {
                    elementsToShow.push(selectedDistributionListOptions[i]);
                }
            }
        }

        RemoveItemsFromListBox(selectedListsElement);
        AddItemsToListBox(selectedListsElement, elementsToShow);
    }

    function RemoveItemsFromListBox(element) {
        for (var i = element.options.length - 1; i >= 0; i--) {
            element.remove(i);
        }
    }

    function AddItemsToListBox(element, items) {
        for (var i = 0; i < items.length; i++) {
            /*items[i].selected = false;
            element.appendChild(items[i]);       */
            /*alert(items[i]);
            try {                
                element.options.add(items[i]);                
            } catch (ex) {                
                element.options.add(items[i], element.options[0]);
            }*/

            var option = new Option(items[i].innerHTML, items[i].value, false);
            element.options[element.options.length] = option;
        }
    }

    function SelectAllElements(isChecked) {
        var selectedListsElement = document.getElementById('selectedDistributionLists');
        var availableListsElement = document.getElementById('availableDistributionLists');

        for (var i = 0; i < availableListsElement.options.length; i++) {
            availableListsElement.options[i].selected = isChecked;
        }

        for (var i = 0; i < selectedListsElement.options.length; i++) {
            selectedListsElement.options[i].selected = isChecked;
        }
    }

</script>
