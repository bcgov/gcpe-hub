<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserControls_AdvancedSearchControl" CodeBehind="AdvancedSearchControl.ascx.cs" %>

<div class="desktop-advanced-search-container" id="advancedSearchContainer">

    <div class="criteria-area gradient">

        <div class="search-label">Search by Name</div>
        <div style="position: relative; z-index: 1003">
            <a class="desktop-advanced-search-clear-text" onclick="clearSearchText();return false;" href=""><img runat="server" src="~/Contacts/images/BigX@2x.png" /></a>
        </div>


        <asp:TextBox AutoComplete="off" runat="server" type="text" id="searchByNameTb" ClientIDMode="Static" class="name-text-box ignore-unload"/>

        <div class="search-label">
            <div class='left'>Add Search Criteria</div>
            <div class="right" id="textInputHeader" style="display: none"></div>

            <div class="clear"></div>
        </div>

        <asp:DropDownList runat="server" ID="searchTypeListBox" ClientIDMode="Static" CssClass="left ignore-unload" OnChange="ChangeValuesForSelectedType(this);" />

        <span>
            <select id="searchValuesListBox" class="ignore-unload add-absolute">
                <option id="valueSelectOption" value="">Select Value</option>
            </select>
        </span>

        <input type="text" id="searchValueTextBox" class="search-value-tb ignore-unload" runat="server" style="display: none" />

        <div class="advanced-search-add-button-container">
            <input id="advanced-search-add-button" type="button" onclick="AddCriteria(); return false;" value="Add" class="gradient" />
        </div>
        <div class="clear"></div>


    </div>

    <div class="criteria-selected-area">
        <div class="selected-items-label search-label"><span id="criteriaCount">0</span> Criteria Added</div>
        <div class="criteria-selected-items" id="criteriaSelectedItems" runat="server">
        </div>

    </div>

    <div class="search-submit gradient">
        <div class="rb-container" id="rbContainer">
            <asp:RadioButton runat="server" id="matchAllRb" GroupName="matchType" ClientIDMode="Static" Checked="true"/> Match All
            <asp:RadioButton runat="server" id="matchAnyRb" GroupName="matchType" ClientIDMode="Static"/> Match Any
        </div>

        <script type="text/javascript">
            var rbContainer = document.getElementById('rbContainer');
            var elements = rbContainer.getElementsByTagName("input");
            for (var i = 0; i < elements.length; i++) {
                if (i == 0) elements[i].className = "first ";
                elements[i].className += "ignore-unload";
            }
        </script>

        <div class="button-container bottom-save-bar">
            <asp:LinkButton ID="cancelButton" runat="server" Text="CANCEL" CssClass="phone common-admin-link-button common-admin-button2-left" OnClick="CancelButtonClick" OnClientClick="history.back();return false;" Visible="false" />
            <asp:Button ID="searchButton" runat="server" Text="SEARCH" CssClass="search-button common-admin-button2" OnClick="SubmitSearchClick" OnClientClick="ShowSearchLoadingModule();" />
        </div>

        <div class="save-button-container">
            <a href='#' id="saveReportButtonLink" class="save-report-button" data-toggle="modal" data-target="#saveReportDialogBox" runat="server">SAVE AS REPORT</a>
        </div>
    </div>
    <div class="clear"></div>
</div>

<script type="text/javascript">

    SetupTypedown(document.getElementById("searchByNameTb"), "<%=Page.ResolveUrl("~/Contacts/ajax/Typedown.ashx?q=")%>", document.getElementById("<%=searchButton.ClientID%>"));

    function LogEmail() {
        var requestUrl = "/Contacts/ajax/LogEmailSearch.ashx?q=" + escape('<%= SearchCriteriaQueryUrl %>');

        var request = GetHttpRequestObj();
        request.open("GET", requestUrl, false);
        request.send();
    }

    function LogPrint() {
        var requestUrl = "/Contacts/ajax/LogPrintSearch.ashx?q=" + escape('<%= SearchCriteriaQueryUrl %>');

        var request = GetHttpRequestObj();
        request.open("GET", requestUrl, false);
        request.send();
    }

</script>

<asp:HiddenField ID="advancedSearchDesktopHidden" ClientIDMode="Static" runat="server" />

<asp:Literal runat="server" id="jsArrayLit"/>

<script type="text/javascript">

    // variables generated in code behind
    // listOfItems = list of the selectable items for the criteria
    // tbItems = list of values that are textbox
    // selectedItems = list of selected values    

    function clearSearchText() {
        document.getElementById("searchByNameTb").value = "";
        HideSaveButton();
        return false;
    }

    function RemoveAllDropDownElements(element) {
        for (var i = element.options.length - 1; i >= 0; i--) {
            element.remove(i);
        }
    }

    function AddElementsToDropDown(element, itemsToAdd) {

        var selectedCriteriaTypeElement = document.getElementById('<%= searchTypeListBox.ClientID %>');
        var selectedCriteriaType = selectedCriteriaTypeElement.options[selectedCriteriaTypeElement.selectedIndex].value;

        for (var i = 0; i < itemsToAdd.length; i++) {
            /*var option = document.createElement("option");
            option.innerHTML = itemsToAdd[i][0];
            option.value = itemsToAdd[i][1];*/
            var option = new Option(itemsToAdd[i][0], itemsToAdd[i][1]);


            var found = false;

            for (var k = 0; k < selectedItems.length; k++) {
                if (selectedItems[k][1] === selectedCriteriaType) {
                    if (selectedItems[k][3] === itemsToAdd[i][1]) {
                        found = true;
                        break;
                    }
                }
            }

            if (!found) {
                try {
                    element.add(option, element.options[null]);
                } catch (e) {
                    element.add(option);
                }
            }
        }
        if (element !== null) addTitleAttributeToElement(element);
    }

    function GetIsTypeTb(criteriaType) {
        for (var i = 0; i < tbItems.length; i++) {
            if (tbItems[i] == criteriaType) return true;
        }
        return false;
    }

    function ToggleTb(showTextBox) {
        var selectedCriteriaDD = document.getElementById('searchTypeListBox');

        var dropDown = document.getElementById('searchValuesListBox');
        var textBox = document.getElementById('searchValueTextBox');
        var header = document.getElementById('textInputHeader');


        if (!showTextBox) {
            textBox.style.display = 'none';
            header.style.display = 'none';
            dropDown.style.display = '';
        } else {
            textBox.style.display = '';

            if ('placeholder' in textBox) {
                textBox.placeholder = "Enter " + selectedCriteriaDD.options[selectedCriteriaDD.selectedIndex].innerHTML;
            } else {
                header.innerHTML = "Enter " + selectedCriteriaDD.options[selectedCriteriaDD.selectedIndex].innerHTML;
                header.style.display = '';
            }

            dropDown.style.display = 'none';
        }
    }

    function ChangeValuesForSelectedType(element) {
        var selectedValue = element.options[element.selectedIndex].value;
        var typeName = element.options[element.selectedIndex].text;

        var valuesDropdown = document.getElementById('searchValuesListBox');
        var addButton = document.getElementById("advanced-search-add-button");

        RemoveAllDropDownElements(valuesDropdown);

        if (selectedValue.trim() === "") {
            // reset the drop down
            AddElementsToDropDown(valuesDropdown, [["Select ", ""]]);
            valuesDropdown.disabled = true;
            addButton.disabled = true;
            valuesDropdown.className += " disabled";
            addButton.className += " disabled";
            ToggleTb(false);
        } else {
            valuesDropdown.disabled = false;
            addButton.disabled = false;
            addButton.className = addButton.className.replace(" disabled", "");
            valuesDropdown.className = valuesDropdown.className.replace(" disabled", "");
            var isTb = GetIsTypeTb(selectedValue);
            if (!isTb) {
                ToggleTb(false);
                // get the list of items to populate the drop down from the
                // javascript array of items
                listOfOptions = listOfItems[selectedValue];
                AddElementsToDropDown(valuesDropdown, listOfOptions);
            } else {
                ToggleTb(true);
            }
        }
    }

    function AddCriteria() {
        HideSaveButton();
        var typesDropDown = document.getElementById('searchTypeListBox');
        var valuesDropdown = document.getElementById('searchValuesListBox');

        var selectedType = typesDropDown.options[typesDropDown.selectedIndex].value;
        var selectedTypeName = typesDropDown.options[typesDropDown.selectedIndex].innerHTML;
        if (selectedType.trim() !== "") {
            var isTb = GetIsTypeTb(selectedType);
            if (!isTb) {
                selectedValue = valuesDropdown.options[valuesDropdown.selectedIndex].value;
                selectedValueName = valuesDropdown.options[valuesDropdown.selectedIndex].innerHTML;
                if (selectedValue.trim() != "") {
                    selectedItems.push([selectedTypeName, selectedType, selectedValueName, selectedValue]);

                    // remove the selected value from the list of available values
                    valuesDropdown.remove(valuesDropdown.selectedIndex);
                }
            } else {
                var textBoxElement = document.getElementById('searchValueTextBox');
                var value = textBoxElement.value.trim();
                if (value !== - "") {
                    selectedItems.push([selectedTypeName, selectedType, value, value]);
                    textBoxElement.value = "";
                }
            }
        }

        GenerateItemsArea();
    }

    function GenerateItemsArea() {

        var hiddenElement = document.getElementById('advancedSearchDesktopHidden');
        var criteriaElement = document.getElementById('criteriaSelectedItems');
        criteriaElement.innerHTML = "";

        var itemStr = "";

        var valueStr = "";

        for (var i = 0; i < selectedItems.length; i++) {
            itemStr += "<div class='gradient item'>\n";

            itemStr += "<div class='text'>" + selectedItems[i][0] + ": " + selectedItems[i][2] + "</div>\n";
            itemStr += "<div class='delete'><a href='javascript:void(0);' onclick='RemoveItem(" + i + ");return false;'><img src='<%= ResolveUrl("~/Contacts/") %>images/BigX@2x.png'/></a></div>\n";

            itemStr += "</div>\n";


            valueStr += selectedItems[i][1] + "=" + selectedItems[i][3] + "|";

        }

        criteriaElement.innerHTML = itemStr;
        hiddenElement.value = valueStr;
        document.getElementById("criteriaCount").firstChild.data = selectedItems.length;
    }

    function RemoveItem(index) {
        HideSaveButton();

        var item = selectedItems[index];
        selectedItems.splice(index, 1);

        var typesDropDown = document.getElementById('searchTypeListBox');
        var valuesDropdown = document.getElementById('searchValuesListBox');

        if (typesDropDown.options[typesDropDown.selectedIndex].value.trim() !== "") {

            var isTb = GetIsTypeTb(typesDropDown.options[typesDropDown.selectedIndex].value);

            if (!isTb) {
                if (typesDropDown.options[typesDropDown.selectedIndex].value == item[1]) {
                    RemoveAllDropDownElements(valuesDropdown);
                    AddElementsToDropDown(valuesDropdown, listOfItems[typesDropDown.options[typesDropDown.selectedIndex].value]);
                }
            }

        }


        GenerateItemsArea();
    }

    $(document).ready(function () {
        GenerateItemsArea();

        var textboxElement = document.getElementById('<%= searchByNameTb.ClientID %>');
        var oldFunc = textboxElement.onchange;
        textboxElement.onchange = function () {
            if (oldFunc !== null) {
                oldFunc();
            }

            HideSaveButton();
        };

        var radioElementA = document.getElementById('<%= matchAllRb.ClientID %>');
        var oldFunc2 = radioElementA.onchange;
        radioElementA.onchange = function () {
            if (oldFunc2 !== null) oldFunc2();
            HideSaveButton();
        };

        var radioElementB = document.getElementById('<%= matchAnyRb.ClientID %>');
        var oldFunc3 = radioElementB.onchange;
        radioElementB.onchange = function () {
            if (oldFunc3 !== null) oldFunc2();
            HideSaveButton();
        };

        ChangeValuesForSelectedType(document.getElementById("<%=searchTypeListBox.ClientID%>"));

        var searchCount = document.getElementById('searchCount').innerText;
        if (searchCount !== "") {
            var filters = {};
            for (var i = 0; i < selectedItems.length; i++) {
                var selectedItem = selectedItems[i];
                filters[selectedItem[0]] = selectedItem[2];
            }
            //alert(JSON.stringify(filters));
            window.snowplow('trackSiteSearch',
                textboxElement.value.split(' '),
                filters,
                searchCount);
        }
    });

    function HideSaveButton() {
        var element = document.getElementById('<%= saveReportButtonLink.ClientID %>');
        if (element !== null) {
            element.style.display = 'none';
        }
    }




</script>

