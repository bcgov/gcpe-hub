var isNavOpened = false;
function ToggleMobileNavigation() {

    var mobileNavigationElement = document.getElementById('mobileNavigationContainer');
    var pageContainerElement = document.getElementById('pageContainer');

    if (isNavOpened === false) {
        mobileNavigationElement.className = "mobile-navigation-container mobile-navigation-container-open";
        pageContainerElement.className = "mobile-page-container-open";
        isNavOpened = true;
    } else {
        mobileNavigationElement.className = "mobile-navigation-container mobile-navigation-container-closed";
        pageContainerElement.className = "";
        isNavOpened = false;
    }

}

function AddSpacesCamelCase(str) {
    var result = str.replace(/([A-Z])/g, " $1");
    return result.trim();
}





function RemoveSpaces(str) {

    var result = str.replace(/[\s]/g, "");


    return result;



}

function doPrint() {
    var loc = window.location.toString();
    if (loc.indexOf("?") >= 0) loc += "&print=true";
    else loc += "?print=true";

    var printWin = window.open(loc, "_blank", "");

    //var ifrm = document.getElementById("printFrame");
    //if (ifrm !== null) {
    //}
}

function maxFieldLength(elem, len) {
    if (elem.value.length > 250) elem.value = elem.value.substring(0, len);
}

function toggleMobileNavItem(element, containerIdStr) {
    var container = document.getElementById(containerIdStr);

    if (container.style.display === 'none') {
        container.style.display = '';
        element.getElementsByTagName("div")[0].className = '';
    } else {
        container.style.display = 'none';
        element.getElementsByTagName("div")[0].className = 'down';
    }

}

function getRadioValue(radioName) {
    var rad = document.forms[0][radioName];
    for (var x in rad) {
        if (rad[x].checked) return rad[x].value;
    }
    return null;
}
function getSelectIndex(sel, val) {
    var index = -1;
    for (var i = 0; index < 0 && i < sel.options.length; i++) {
        if (sel.options[i].value === val) index = i;
    }
    return index;
}

function toggleSelectAll(elem, containerId) {
    var con = document.getElementById(containerId);
    var cbs = con.getElementsByTagName("input");
    for (var i = 0; i < cbs.length; i++) {
        if (cbs[i].type === "checkbox") {
            cbs[i].checked = elem.checked;
        }
    }
    var selectAll = elem.checked;
    // get the parent table to make both the top and 
    // bottom select all match
    var parent = con.parentNode;
    var elements = parent.getElementsByTagName("th");
    for (var i = 0; i < elements.length; i++) {
        if (elements[i].className === "checkbox") {
            var cb = elements[i].getElementsByTagName("input");
            if (cb.length > 0) {
                cb[0].checked = selectAll;
            }
        }
    }
}

function ConfirmBulkActions(dropdown) {
    var element = document.getElementById(dropdown);
    var action = element.options[element.selectedIndex].value;
    if (action.toLowerCase() === "delete") return confirm(deleteButtonText);
    return confirm("Are you sure you want to perform the following action on the list?\n" + element.options[element.selectedIndex].innerHTML);
}

String.prototype.trim = function () {
    return this.replace(/^\s+|\s+$/, '');
};

function CheckEmailValidity(email) {
    return /^[\w\-\.]+\@[\w\-\.]+\.[\w]+$/.test(email);
}

function GetHttpRequestObj() {
    if (window.XMLHttpRequest) return new XMLHttpRequest();
    return new ActiveXObject("Microsoft.XMLHTTP");
}

function SetupTypeDown(placeholderElementId, textboxElementId) {
    var placeholder = document.getElementById(placeholderElementId);
    var textbox = document.getElementById(textboxElementId);

    placeholder.style.display = "";

    textbox.onclick = function () {
        if (placeholder.style.display !== "none") {
            placeholder.style.display = "none";
            textbox.focus();
        }
    }

    placeholder.onclick = function () {
        this.style.display = "none";
        textbox.focus();
    }

    textbox.onblur = function () {
        if (this.value.trim() === "") {
            placeholder.style.display = "";
        }
    }

}

var doUnloadCheck = false;
function onBeforeUnload() {
    if (doUnloadCheck) {
        return unloadText;
    }
}
window.onbeforeunload = onBeforeUnload;
window.onunload = onBeforeUnload;

function setUnloadCheckTrue() {
    doUnloadCheck = true;
}
function setUnloadCheckFalse() {
    doUnloadCheck = false;
}

function disableEnterKey(element, event) {
    var event = (event) ? event : ((event) ? event : null);
    if (event === null) event = window.event;
    if (event.keyCode === 13) return false;
}

function setupInputsDirtyBit() {
    var inputs = document.getElementsByTagName("input");
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].className === null || inputs[i].className.indexOf("ignore-unload") < 0) {

            if (inputs[i].type === "checkbox" && inputs[i].name !== null && inputs[i].name === "categoryAction") continue;
            if (inputs[i].type === "checkbox" && inputs[i].name !== null && inputs[i].name === "selectAll") continue;

            if (inputs[i].type === "text" || inputs[i].type === "checkbox" || inputs[i].type === "radio") {
                if (inputs[i].addEventListener) inputs[i].addEventListener("change", setUnloadCheckTrue, false);
                else inputs[i].attachEvent("onchange", setUnloadCheckTrue);

                inputs[i].onkeypress = function (e) { return disableEnterKey(this, e); };
            }

            if ((inputs[i].type === "button" || inputs[i].type === "submit")
                && inputs[i].className !== null && inputs[i].className.indexOf("common-admin-button") >= 0) {
                if (inputs[i].addEventListener) inputs[i].addEventListener("click", setUnloadCheckFalse, false);
                else inputs[i].attachEvent("onclick", setUnloadCheckFalse);
            }
        }
    }
    inputs = document.getElementsByTagName("textarea");
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].className === null || inputs[i].className.indexOf("ignore-unload") < 0) {
            if (inputs[i].addEventListener) inputs[i].addEventListener("change", setUnloadCheckTrue, false);
            else inputs[i].attachEvent("onchange", setUnloadCheckTrue);
        }
    }
    inputs = document.getElementsByTagName("select");
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].className === null || inputs[i].className.indexOf("ignore-unload") < 0) {
            if (inputs[i].addEventListener) inputs[i].addEventListener("change", setUnloadCheckTrue, false);
            else inputs[i].attachEvent("onchange", setUnloadCheckTrue);

            inputs[i].onkeypress = function (e) { return disableEnterKey(this, e); };
        }
    }
}

$(document).ready(function () {
    setupInputsDirtyBit();
});

function GetHttpRequestObj() {
    if (window.XMLHttpRequest) return new XMLHttpRequest();
    return new ActiveXObject("Microsoft.XMLHTTP");
}

var typedown_cnt = 0;
var typedown_focus = new Array();
var typedown_timers = new Array();
var typedown_containers = new Array();
var typedown_queries = new Array();
var typedown_urls = new Array();
var typedown_selected_item = new Array();
var typedown_button = new Array();
var typedown_off = new Array();
function SetupTypedown(elem, url, btn) {
    if (elem.id === null) elem.id = "typedown_elem_" + (typedown_cnt++);

    elem.onkeydown = function (event) {
        return arrowKeyTypedown(this.id, event);
    }

    typedown_button[elem.id] = btn;
    typedown_off[elem.id] = false;
    typedown_urls[elem.id] = url;

    //reparent in relative position div
    var rdiv = document.createElement("div");
    rdiv.style.position = "relative";
    rdiv.style.zIndex = 1002;
    elem.parentNode.insertBefore(rdiv, elem);
    elem.parentNode.removeChild(elem);
    rdiv.appendChild(elem);

    var tdiv = document.createElement("div");
    tdiv.className = "typedown-results";
    tdiv.style.display = "none";
    rdiv.appendChild(tdiv);
    typedown_containers[elem.id] = tdiv;

    elem.onfocus = function (event) {
        setTypedownFocus(true, this.id, event);
    }
    elem.onblur = function (event) {
        setTypedownFocus(false, this.id, event);
    }
}

function arrowKeyTypedown(id, event) {
    typedown_off[id] = false;
    if (event === null) event = window.event;
    var idx = typedown_selected_item[id];
    var con = typedown_containers[id];
    var divs = con.getElementsByTagName("div");
    if (event.keyCode === 38) {
        //up
        if (idx === undefined) {
            //get first item
            idx = 0;
        } else {
            //unset selected class
            divs[idx].className = divs[idx].className.replace(" selected", "");
            //get next item up
            idx = idx - 1;
            while (idx < 0) idx += divs.length;
        }
        divs[idx].className = divs[idx].className + " selected";
        typedown_selected_item[id] = idx;
    } else if (event.keyCode === 40) {
        //down
        if (idx === undefined) {
            //get first item
            idx = 0;
        } else {
            //unset selected class
            divs[idx].className = divs[idx].className.replace(" selected", "");
            //get next item up
            idx = idx + 1;
            while (idx >= divs.length) idx -= divs.length;
        }
        divs[idx].className = divs[idx].className + " selected";
        typedown_selected_item[id] = idx;
    } else if (event.keyCode === 37) {
        if (idx !== undefined) {
            setTypedownContent(divs[idx], id);
        }
    } else if (event.keyCode === 39) {
        if (idx !== undefined) {
            setTypedownContent(divs[idx], id);
        }
    } else if (event.keyCode === 13) {
        if (idx !== undefined) {
            typedown_off[id] = true;

            //if (!setTypedownContent(divs[idx], id)) {
            //    if(typedown_button[id]!==null) typedown_button[id].click();
            //}

            gotoTypedownUrl(divs[idx], id);

            return false;
        } else {
            if (typedown_button[id] !== null) typedown_button[id].click();
            return false;
        }
    } else if (event.keyCode === 27) {
        if (idx !== undefined) {
            divs[idx].className = divs[idx].className.replace(" selected", "");
        }

        typedown_selected_item[id] = null;
        hideTypedown(id);
        typedown_off[id] = true;
    }
    return true;
}

function setTypedownFocus(tf, id, event) {
    if (event === null) event = window.event;
    typedown_focus[id] = tf;
    window.clearInterval(typedown_timers[id]);
    if (tf) {
        {
            typedown_timers[id] = window.setInterval("getTypedownResults('" + id + "')", 500);
        }
    } else {
        typedown_timers[id] = window.setTimeout("hideTypedown('" + id + "')", 1000);
    }
}
function hideTypedown(id) {
    typedown_containers[id].style.display = "none";
}
function getTypedownResults(id) {
    var q = document.getElementById(id).value;
    if (typedown_off[id] || /^[\s]*$/.test(q)) {
        typedown_containers[id].style.display = "none";
    } else if (typedown_queries[id] === q) {
        typedown_containers[id].style.display = "";
    } else {
        typedown_queries[id] = q;
        $.ajax({
            url: typedown_urls[id] + q,
            context: document.body
        }).done(function (response, textStatus, jqXHR) {
            //var repl = "setTypedownContent(this, '" + id + "');";
            var repl = "gotoTypedownUrl(this, '" + id + "');";
            typedown_containers[id].innerHTML = response.replace(/\%clk\%/g, repl);
            typedown_containers[id].style.display = "";
            typedown_selected_item[id] = null;
        });
    }
}

function gotoTypedownUrl(elem, id) {
    var q = document.getElementById(id);
    var spans = elem.getElementsByTagName("span");
    var url = null;
    for (var x in spans) {
        if (spans[x].className === "result-url") {
            url = spans[x].innerHTML;
        }
        if (spans[x].className === "result-original") {
            q.value = spans[x].innerHTML;
        }
    }
    typedown_containers[id].style.display = "none";
    if (url !== null) window.location = url;
    return false;
}

function setTypedownContent(elem, id) {
    var q = document.getElementById(id);
    var spans = elem.getElementsByTagName("span");
    var returnval = true;
    for (var x in spans) {
        if (spans[x].className === "result-original") {
            if (spans[x].innerHTML === q.value) returnval = false;
            q.value = spans[x].innerHTML;
            break;
        }
    }
    typedown_containers[id].style.display = "none";
    return returnval;
}

function changePaginatorDropDownValues(element) {
    var elements = document.getElementsByTagName("select");
    for (var i = 0; i < elements.length; i++) {

        if (elements[i].className.indexOf("paginatorDropDown") > -1 && elements[i].id !== element.id) {
            elements[i].selectedIndex = element.selectedIndex;
        }

    }

}

function addDropDownFixToElements() {
    var elements = document.getElementsByTagName("select");
    for (var i = 0; i < elements.length; i++) {
        toggleDropDownWidthFix(elements[i]);
    }
}

function toggleDropDownWidthFix(element) {
    /*if ($.browser.msie && $.browser.version < 9) {

        var width = $(element).width();
        $(element).css("min-width", width);

        $(element)
            .bind('focus mouseover', function () {
                $(this).addClass('expand').removeClass('clicked');

                if ($(this).hasClass("add-absolute")) $(this).addClass("absolute");
            })
            .bind('click', function () { $(this).toggleClass('clicked'); })
            .bind('mouseout', function () {
                if (!$(this).hasClass('clicked')) {
                    $(this).removeClass('expand');
                    $(this).removeClass('absolute');
                }
            })
            .bind('blur', function () {
                $(this).removeClass('expand clicked');
                $(this).removeClass('absolute');
            });

    }*/
    addTitleAttributeToElement(element);
}

function addTitleAttributeToElement(element) {
    for (var i = 0; i < element.options.length; i++) {
        element.options[i].setAttribute("title", element.options[i].text);
        element.options[i].setAttribute("alt", element.options[i].text);
    }
}


var cancelAllChangesButtonText = "Confirmation Required!\nAre you sure you want to cancel all your pending changes?\nThis will cancel all your recent edits and cannot be undone!";
var cancelButtonText = "Confirmation Required!\nAre you sure you wish to cancel your changes?";
var deleteButtonText = "Confirmation Required!\nAre you sure you wish to delete this record? This operation cannot be undone!";
var saveErrorText = "Error!\nThere was an error processing your request.\n\
The following fields are required and were not supplied or contained invalid data.\n\
###errors###\
Please correct/complete the listed fields and click \"Save\"";
var unloadText = "You have unsaved changes.";
var tabConfirmText = "This will save your current changes; continue?";

var reportTitleUsedText = "Error!\nThe report title you entered is already in use. Please enter a different report title.";
var errorOccurredText = "Error!\nAn error occurred. Please try again.";
var reportSavedSuccess = "Success!\nYour report '###reportName###' has been successfully saved!";
var tooManyEmailError = "Alert!\nYou have requested to email ###emailcount### email addresses, but the maximum is ###maxemails###";
var sentShareEmailText = "Success!\nAn email containing the results of this search has been sent to ###email###.";
var noEmailFoundText = "Alert!\nThis ###type### does not contain an email";
var switchToOutletText = "Alert!\nThis will make all of its outlets orphans.";
var reportTitleTextEmpty = "Error!\nEnter a new report name";
var emailButtonText = "You are about to e-mail all the media contacts included in this report. Do you wish to continue?";

var shareHoverText = "E-mail search results to me";
var emailHoverText = "E-mail all of the media contacts listed below";
var printHoverText = "Print this report";
var exportHoverText = "Export this info to Microsoft Excel";