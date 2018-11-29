var multiselector_items = new Array();
var multiselector_subitems = new Array();
var multiselector_doText = new Array();
var multiselector_doSelect = new Array();
var multiselector_selectedCount = new Array();
var multiselector_allowDuplicates = new Array();
var multiselector_isShort = new Array();
var multiselector_initMode = new Array();

function multiselector_setButtonDisabled(id) {
    var src = document.getElementById("multiselector_srcSelect_" + id);
    var dst = document.getElementById("multiselector_selectedItems_" + id);
    var hid = document.getElementById("multiselector_selectedIds_" + id);
    var txt = document.getElementById("multiselector_srcText_" + id);
    var sel = document.getElementById("multiselector_srcSelect2_" + id);
    var btn = document.getElementById("multiselector_addButton_" + id);
    var doText = multiselector_doText[id];
    var doSelect = multiselector_doSelect[id];
    var showsub = false;
    var itm = multiselector_findItem(id, src.options[src.selectedIndex].value);
    if (itm != null) showsub = itm[4];

    if (src.selectedIndex < 0 || src.options[src.selectedIndex].value == ""
        || doSelect && showsub && (sel.selectedIndex < 0 || sel.options[sel.selectedIndex].value == "")) {
        btn.disabled = true;
        if(btn.className.indexOf(" disabled")<0) btn.className += " disabled";
    } else {
        btn.disabled = false;
        btn.className = btn.className.replace(" disabled", "");
       
    }
}

function multiselector_initItems(id) {
    var modeShort = multiselector_isShort[id];

    var src = document.getElementById("multiselector_srcSelect_" + id);
    var dst = document.getElementById("multiselector_selectedItems_" + id);
    var hid = document.getElementById("multiselector_selectedIds_" + id);
    var txt = document.getElementById("multiselector_srcText_" + id);
    var sel = document.getElementById("multiselector_srcSelect2_" + id);
    var btn = document.getElementById("multiselector_addButton_" + id);
    var doText = multiselector_doText[id];
    var doSelect = multiselector_doSelect[id];
    var showsub = false;
    
    multiselector_initMode[id] = true;

    if (!/^[\s]*$/.test(hid.value)) {
        multiselector_selectedCount[id] = 0;
        var items = hid.value.split("|");
        hid.value = "";
        for (var x in items) {
            if (multiselector_doText[id] || multiselector_doSelect[id]) {
                var item = items[x].split("=");
                if (item.length == 2) {
                    multiselector_setInputs(id, item[0], item[1]);
                    multiselector_addItem(id, modeShort);
                } else if (!/^[\s]*$/.test(items[x])) {
                    multiselector_setInputs(id, items[x], null);
                    multiselector_addItem(id, modeShort);
                }
            } else {
                if (!/^[\s]*$/.test(items[x])) {
                    multiselector_setInputs(id, items[x], null);
                    multiselector_addItem(id, modeShort);
                }
            }
        }
    }
    multiselector_initMode[id] = false;

    multiselector_setButtonDisabled(id);
    if (sel != null) addTitleAttributeToElement(sel);
}

function multiselector_setInputs(id, val1, val2) {
    var src = document.getElementById("multiselector_srcSelect_" + id);
    var sel = document.getElementById("multiselector_srcSelect2_" + id);
    var txt = document.getElementById("multiselector_srcText_" + id);
    var index1 = -1;
    var index2 = -1;
    for (var i = 0; index1 < 0 && i < src.options.length; i++) {
        if (src.options[i].value == val1) {
            index1 = i;
            src.selectedIndex = index1;
        }
    }
    if (multiselector_doText[id]) {
        txt.value = val2;
    } else if (multiselector_doSelect[id]) {
        for (var i = 0; index2 < 0 && i < sel.options.length; i++) {
            if (sel.options[i].value == val2) {
                index2 = i;
                sel.selectedIndex = index2;
            }
        }
    }
}

function multiselector_addItem(id, isShort) {
    var src = document.getElementById("multiselector_srcSelect_" + id);
    var dst = document.getElementById("multiselector_selectedItems_" + id);
    var hid = document.getElementById("multiselector_selectedIds_" + id);
    var txt = document.getElementById("multiselector_srcText_" + id);
    var sel = document.getElementById("multiselector_srcSelect2_" + id);
    var btn = document.getElementById("multiselector_addButton_" + id);

    var doText = multiselector_doText[id];
    var doSelect = multiselector_doSelect[id];
    var showsub = false;

    if(!multiselector_initMode[id]) doUnloadCheck = true;

    if (src.selectedIndex < 0) return;

    var itm = multiselector_findItem(id, src.options[src.selectedIndex].value);
    if (itm != null) showsub = itm[4];

    if (src.options[src.selectedIndex].value != "") {

        if (doSelect && showsub) {
            if (sel.selectedIndex<0||sel.options[sel.selectedIndex].value == "") {
                return false;
            }
        }

        //find regex
        if (doText) {
            for (var x in multiselector_items[id]) {
                if (x == src.options[src.selectedIndex].value) {
                    if (!multiselector_items[id][x][2].test(txt.value)) {
                        alert("You have entered an invalid value for "+src.options[src.selectedIndex].text);
                        return;
                    }
                }
            }
        }

        var item = document.createElement("div");
        item.className = "multiselector-selected-item"+(isShort?"-short":"")+" gradient";
        item.id = "multiselector_" + id + "_" + src.options[src.selectedIndex].value;
        var lbl = document.createElement("div");
        lbl.className = "multiselector-selected-item-label" + (isShort ? "-short" : "") + "";
        lbl.appendChild(document.createTextNode(src.options[src.selectedIndex].text+(doText?(": "+txt.value):"")+(doSelect&&showsub?": "+sel.options[sel.selectedIndex].text:"")));
        item.appendChild(lbl);
        var lnk = document.createElement("a");
        lnk.className = "multiselector-delete-link";
        lnk.href = "#";
        {
            var theId = id;
            var theSel = src.options[src.selectedIndex].value
            lnk.onclick = function () {
                multiselector_removeItem(theId, theSel);
                return false;
            }
        }
        item.appendChild(lnk);

        dst.appendChild(item);
        hid.value += src.options[src.selectedIndex].value + (doText?"="+txt.value:"") + (doSelect&&showsub?"="+sel.options[sel.selectedIndex].value:"") + "|";
        if(!multiselector_allowDuplicates[id]) src.options[src.selectedIndex] = null;
        src.selectedIndex = 0;

        if (multiselector_selectedCount[id] == null) multiselector_selectedCount[id] = 0;
        multiselector_selectedCount[id]++;

        document.getElementById("numberAddedLabel_" + id).innerHTML = multiselector_selectedCount[id];

        multiselector_checkSubItem(src);

        if (doText) txt.value = "";
        src.selectedIndex = 0;
        if(doSelect) sel.selectedIndex = 0;
    }

    multiselector_setButtonDisabled(id);

    if(sel!=null) addTitleAttributeToElement(sel);
}
function multiselector_removeItem(id, itemId) {
    var src = document.getElementById("multiselector_srcSelect_" + id);
    var item = document.getElementById("multiselector_" + id + "_" + itemId);
    var hid = document.getElementById("multiselector_selectedIds_" + id);
    var sel = document.getElementById("multiselector_srcSelect2_" + id);

    var itm = multiselector_findItem(id, itemId);

    item.parentNode.removeChild(item);

    doUnloadCheck = true;

    var re = new RegExp(itemId + "\\=.*?\\|");

    var doText = multiselector_doText[id];
    var doSelect = multiselector_doSelect[id];
    if (doText||(doSelect&&itm[4])) {
        hid.value = hid.value.replace(re, "");
    } else {
        hid.value = hid.value.replace(itemId + "|", "");
    }

    multiselector_selectedCount[id]--;

    document.getElementById("numberAddedLabel_" + id).innerHTML = multiselector_selectedCount[id] + " added";

    src.options.length = 0;

    var emptyOpt = new Option("Select", "", false);
    src.options[src.options.length] = emptyOpt;
    for (var x in multiselector_items[id]) {
        var itm2 = multiselector_findItem(id, x);
        var re2 = new RegExp(x + "\\=.*?\\|");
        if (doText || (doSelect && itm2[4])) {
            if (!re2.test(hid.value)) {
                var opt = new Option(multiselector_items[id][x][0], x, false);
                src.options[src.options.length] = opt;
            }
        } else {
            if (hid.value.indexOf(x + "|") < 0) {
                var opt = new Option(multiselector_items[id][x][0], x, false);
                src.options[src.options.length] = opt;
            }
        }
    }
    if (sel != null) addTitleAttributeToElement(sel);
    multiselector_checkSubItem(src);
}
function multiselector_findItem(id, itemId) {
    for (var x in multiselector_items[id]) {
        if (x == itemId) return multiselector_items[id][x];
    }
    return null;
}
function multiselector_checkSubItem(elem) {
    var id = elem.id.replace("multiselector_srcSelect_", "");
    var con = document.getElementById("multiselector_subitem_" + id);
    if (multiselector_doText[id] || multiselector_doSelect[id]) {
        if (elem.selectedIndex >= 0) {
        
            var val = elem.options[elem.selectedIndex].value;

            var itm = multiselector_findItem(id, val);
            if (itm != null) {
                if (itm[4]) con.style.display = "";
                else con.style.display = "none";
            } else {
                con.style.display = "none";
            }
        } else {
            con.style.display = "none";
        }
    } else {
        con.style.display = "none";
    }

    multiselector_setPlaceholder(id);

    multiselector_setButtonDisabled(id);
}

function multiselector_setPlaceholder(id) {
    var src = document.getElementById("multiselector_srcSelect_" + id);
    var txt = document.getElementById("multiselector_srcText_" + id);
    try {
        txt.setAttribute("placeholder", "Enter "+src.options[src.selectedIndex].text);
    } catch (e) { }
}