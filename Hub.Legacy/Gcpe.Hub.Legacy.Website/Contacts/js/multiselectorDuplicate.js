var multiselectorDuplicate_items = new Array();
var multiselectorDuplicate_subitems = new Array();
var multiselectorDuplicate_selectedCount = new Array();
var multiselectorDuplicate_allowDuplicates = new Array();
var multiselectorDuplicate_subitemCount = 0;
var multiselectorDuplicate_isShort = new Array();
var multiselectorDuplicate_initMode = new Array();
var multiselectorDuplicate_doText = new Array();
var multiselectorDuplicate_doSelect = new Array();
var multiselectorDuplicate_doExt = new Array();

function multiselectorDuplicate_setButtonDisabled(id) {
    var src = document.getElementById("multiselector_srcSelect_" + id);
    var dst = document.getElementById("multiselector_selectedItems_" + id);
    var hid = document.getElementById("multiselector_selectedIds_" + id);
    var txt = document.getElementById("multiselector_srcText_" + id);
    var txt2 = document.getElementById("multiselector_srcText2_" + id);
    var sel = document.getElementById("multiselector_srcSelect2_" + id);
    var btn = document.getElementById("multiselector_addButton_" + id);

    var doText = multiselectorDuplicate_doText[id];
    var doSelect = multiselectorDuplicate_doSelect[id];

    if (doSelect) {
        if (src.selectedIndex < 0 || src.options[src.selectedIndex].value == ""
            || sel.selectedIndex < 0 || sel.options[sel.selectedIndex].value == "") {
            btn.disabled = true;
            if (btn.className.indexOf(" disabled") < 0) btn.className += " disabled";
        } else {
            btn.disabled = false;
            btn.className = btn.className.replace(" disabled", "");
        }
    }

    if (doText) {
        var subItemDiv = document.getElementById("multiselector_subitem_" + id);
        if (src.selectedIndex < 0 || src.options[src.selectedIndex].value == "") {
            btn.disabled = true;
            if (btn.className.indexOf(" disabled") < 0) btn.className += " disabled";
            subItemDiv.style.display = "none";

        } else {
            btn.disabled = false;
            btn.className = btn.className.replace(" disabled", "");
            subItemDiv.style.display = "";
        }
    }
}

function createPhoneNumberString(pn, ext) {
    return pn + (ext==null||/^[\s]*$/.test(ext)?"":"ext." + ext);
}
function splitPhoneNumber(pnStr) {
    var re = /([\w\-\(\)\s\.]+) ext\.([\w\-\.]+)/;
    var match = re.exec(pnStr);

    var result = { "phoneNumber": null, "ext": null };

    if (match != null) {
        result.phoneNumber = match[1];
        result.ext = match[2];
    } else {
        result.phoneNumber = pnStr;
    }
    return result;
}

function multiselectorDuplicate_initItems(id) {
    var hidInput = document.getElementById("multiselector_selectedIds_" + id);
    var modeShort = multiselectorDuplicate_isShort[id];
    var sel = document.getElementById("multiselector_srcSelect2_" + id);
    var txt = document.getElementById("multiselector_srcText_" + id);
    var txt2 = document.getElementById("multiselector_srcText2_" + id);
    multiselectorDuplicate_initMode[id] = true;

    var doExt = multiselectorDuplicate_doExt[id];

    if (!/^[\s]*$/.test(hidInput.value)) {
        multiselectorDuplicate_selectedCount[id] = 0;

        var items = hidInput.value.split("|");
        hidInput.value = "";
        for (var x in items) {
            var item = items[x].split("=");

            if (item.length == 2) {

                if (doExt) {
                    var pinfo = splitPhoneNumber(item[1]);

                    item[1] = pinfo.phoneNumber;
                    if (pinfo.ext != null) item[2] = pinfo.ext;
                }

                multiselectorDuplicate_setInputs(id, item[0], item[1], (item.length>=3?item[2]:null));
                multiselectorDuplicate_addItem(id, modeShort);
            }

        }
    }
    multiselectorDuplicate_initMode[id] = false;

    multiselectorDuplicate_setButtonDisabled(id);
    if (sel != null) addTitleAttributeToElement(sel);
}

function multiselectorDuplicate_setInputs(id, val1, val2, val3) {
    var src = document.getElementById("multiselector_srcSelect_" + id);
    var sel = document.getElementById("multiselector_srcSelect2_" + id);
    var txt = document.getElementById("multiselector_srcText_" + id);
    var txt2 = document.getElementById("multiselector_srcText2_" + id);

    var doText = multiselectorDuplicate_doText[id];
    var doSelect = multiselectorDuplicate_doSelect[id];
    var doExt = multiselectorDuplicate_doExt[id];

    var index1 = -1;
    var index2 = -1;
    for (var i = 0; index1<0&&i < src.options.length; i++) {
        if (src.options[i].value == val1) {
            index1 = i;
            src.selectedIndex = index1;
        }
    }
    if (doSelect) {
        for (var i = 0; index2 < 0 && i < sel.options.length; i++) {
            if (sel.options[i].value == val2) {
                index2 = i;
                sel.selectedIndex = index2;
            }
        }
    }
    if (doText) {
        txt.value = val2;

        if (doExt && val3 != null) {
            txt2.value = val3;
        }
    }
    
}

function multiselectorDuplicate_addItem(id, isShort) {
    var src = document.getElementById("multiselector_srcSelect_" + id);
    var dst = document.getElementById("multiselector_selectedItems_" + id);
    var hid = document.getElementById("multiselector_selectedIds_" + id);
    var txt = document.getElementById("multiselector_srcText_" + id);
    var txt2 = document.getElementById("multiselector_srcText2_" + id);
    var sel = document.getElementById("multiselector_srcSelect2_"+id);

    var doText = multiselectorDuplicate_doText[id];
    var doSelect = multiselectorDuplicate_doSelect[id];
    var doExt = multiselectorDuplicate_doExt[id];

    if(!multiselectorDuplicate_initMode[id]) doUnloadCheck = true;

    var rowId = multiselectorDuplicate_subitemCount++;

    if (src.selectedIndex < 0) return;

    var txtValue = (doText?(doExt ? createPhoneNumberString(txt.value, txt2.value) : txt.value):null);

    if (src.options[src.selectedIndex].value != "") {
        if (doSelect) {
            if (sel.selectedIndex < 0 || sel.options[sel.selectedIndex].value == "") {
                //alert("Please select from the second dropdown");
                return false;
            } else if (hid.value.indexOf(src.options[src.selectedIndex].value + "=" + sel.options[sel.selectedIndex].value + "|") >= 0) {
                //alert("You have already added that combination");
                return false;
            }
        }

        //find regex
        if (doText) {
            for (var x in multiselectorDuplicate_items[id]) {
                if (x == src.options[src.selectedIndex].value) {
                    if (!multiselectorDuplicate_items[id][x][2].test(txt.value)) {
                        alert("You have entered an invalid value for " + src.options[src.selectedIndex].text);
                        return;
                    }
                }
            }

            if (hid.value.indexOf(src.options[src.selectedIndex].value + "=" + txtValue + "|") >= 0) {
                //alert("You have already added that combination");
                return false;
            }
        }

        var item = document.createElement("div");
        item.className = "multiselector-selected-item" + (isShort ? "-short" : "") + " gradient";
        item.id = "multiselector_" + id + "_" + rowId;
        var lbl = document.createElement("div");
        lbl.className = "multiselector-selected-item-label" + (isShort ? "-short" : "") + "";
        if (doSelect) {
            lbl.appendChild(document.createTextNode(src.options[src.selectedIndex].text + ": " + sel.options[sel.selectedIndex].text));
        }
        if (doText) {
            lbl.appendChild(document.createTextNode(src.options[src.selectedIndex].text + ": " + txtValue));
        }
        item.appendChild(lbl);
        var lnk = document.createElement("a");
        lnk.className = "multiselector-delete-link";
        var dimg = document.createElement("img");
        lnk.appendChild(dimg);
        lnk.href = "#";
        {
            var theId = id;
            var theRowId = rowId
            var theSel = src.options[src.selectedIndex].value + "=";
            if (multiselectorDuplicate_doSelect[theId]) theSel += sel.options[sel.selectedIndex].value;
            if (multiselectorDuplicate_doExt[theId]) theSel += txtValue;
            else if (multiselectorDuplicate_doText[theId]) theSel += txt.value;
            

            lnk.onclick = function () {
                multiselectorDuplicate_removeItem(theId, theSel, theRowId);
                return false;
            }
        }
        item.appendChild(lnk);

        dst.appendChild(item);
        if (doSelect) {
            hid.value += src.options[src.selectedIndex].value + "=" + sel.options[sel.selectedIndex].value + "|";
        } else {
            hid.value += src.options[src.selectedIndex].value + "=" + txtValue + "|";
        }
        if(!multiselectorDuplicate_allowDuplicates[id]) src.options[src.selectedIndex] = null;
        src.selectedIndex = 0;

        if (multiselectorDuplicate_selectedCount[id] == null) multiselectorDuplicate_selectedCount[id] = 0;
        multiselectorDuplicate_selectedCount[id]++;

        document.getElementById("numberAddedLabel_" + id).innerHTML = multiselectorDuplicate_selectedCount[id];

        src.selectedIndex = 0;
        if (doSelect) sel.selectedIndex = 0;
        if (doText) txt.value = "";
        if (doExt) txt2.value = "";
    }

    multiselectorDuplicate_setButtonDisabled(id);
    if (sel != null) addTitleAttributeToElement(sel);
}
function multiselectorDuplicate_removeItem(id, itemId, rowId) {
    //alert(id + "\n" + itemId + "\n" + rowId);
    var src = document.getElementById("multiselector_srcSelect_" + id);
    var item = document.getElementById("multiselector_" + id + "_" + rowId);
    var hid = document.getElementById("multiselector_selectedIds_" + id);
    var sel = document.getElementById("multiselector_srcSelect2_" + id);

    item.parentNode.removeChild(item);

    doUnloadCheck = true;
    hid.value = hid.value.replace(itemId+"|", "");

    multiselectorDuplicate_selectedCount[id]--;

    document.getElementById("numberAddedLabel_" + id).innerHTML = multiselectorDuplicate_selectedCount[id] + " added";
    if (sel != null) addTitleAttributeToElement(sel);
}