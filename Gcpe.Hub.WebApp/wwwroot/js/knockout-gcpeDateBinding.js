ko.bindingHandlers.gcpeDateTimePicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var options = allBindingsAccessor().gcpeDateTimePickerOptions || {};
        //$(element).datepicker(options);

        var classStr = ' class="' + $(element).attr("class") + '" ';
        $(element).attr("class", "");

        //make sure we're on a span or div
        var tp = element.nodeName;

        if (!(tp === "DIV" || tp === "SPAN")) {
            throw "Please use gcpeDateBinding on a span or a div, not on: " + tp;
        }

        var pickerNm = element.id + "_picker";
        var pickerNmWrap = element.id + "_pickerWrap";
        var pickerNmFriendly = pickerNm + "Friendly";
        var userAgent = window.navigator.userAgent;
        //var useKendo = userAgent.indexOf("Mobile") == -1; // We don't like Edge and Desktop Chrome html5 datetime implementation

        //Add Modernizr test for ipad/phone/pod
        Modernizr.addTest('isiosmobile', function () {
            return navigator.userAgent.match(/(iPad|iPhone|iPod)/g);
        });

        // use Kendo on everything but apple iphone / ipad / ipod.
        var useKendo = !Modernizr.isiosmobile;

        var picker;
        var kendoPicker;
        if (useKendo) {
            //let's create the Kendo one
            $(element).before('<div id="' + pickerNmWrap + '"><input id="' + pickerNm + '"' + classStr + ' /></div>');
            picker = $("#" + pickerNm).kendoDateTimePicker({
                animation: false,
                close: function (e) {
                    if (e.view === "date") {
                        picker.data("kendoDateTimePicker").open("time");
                    }
                }
            });
        } else {
            $(element).before('<div id="' + pickerNmWrap + '"><input type="datetime-local" id="' + pickerNm + '"' + classStr + ' autocomplete="on"/></div>');
            //see if we can take the class attribute from element and move it here
            picker = $("#" + pickerNm)[0];
        }

        //now add the friendly date display
        var placeholderStr = "";
        //if (options.placeholder) {
        //    placeholderStr = 'placeholder="' + options.placeholder + '"';
        //} Rendered differently on IE, just setting it to the friendly value in the update.
        $(element).append('<input type="text" id="' + pickerNmFriendly + '" value="" ' + classStr + ' ' + placeholderStr + '/>');
        $("#" + pickerNmWrap).hide();
        $("#" + pickerNmFriendly).show();

        //let's store this reference on the element so we get to play with it in the 'update'
        element.gcpePicker = picker;
        element.gcpePickerFriendly = $("#" + pickerNmFriendly)[0];
        element.gcpePickerUseKendo = useKendo;
        element.gcpePickerPlaceholderText = options.placeholder;

        if (options.enable) {
            function setState(newValue) {
                
                if (newValue) {
                    $("#" + pickerNmFriendly).prop('disabled', false);
                    $("#" + pickerNmFriendly).addClass("disabled");

                } else {
                    $("#" + pickerNmFriendly).prop('disabled', true);
                    $("#" + pickerNmFriendly).removeClass("disabled");

                }
            }
            //we will subscribe to the observable
            options.enable.subscribe(setState);

            //and set the initial state
            setState(options.enable());
        }


        //handle the field changing
        if (useKendo) {
            kendoPicker = picker.data("kendoDateTimePicker");
            kendoPicker.bind("change", function () {
                valueAccessor()(picker.data("kendoDateTimePicker").value());
            });
        } else {
            //note there is this way of doing event also:  ko.utils.registerEventHandler(element, "change", function () 
            $(picker).change(function () {
                var d = moment(picker.value);
                if (d.isValid()) {
                    valueAccessor()(d.toDate());
                } else {

                    valueAccessor()(undefined);
                }
            });
        }

        //setup the bait and switch pieces
        $("#" + pickerNmFriendly).focusin(function () {
            $("#" + pickerNmWrap).show();
            $("#" + pickerNm).focus();
            //$("#" + pickerNmFriendly).hide();
            $(element).hide();

            if (useKendo) {
                var kendoDateTimePicker = picker.data("kendoDateTimePicker");

                var d = moment(kendoDateTimePicker.value());
                if (!d.isValid()) {
                    var currentValue = kendoDateTimePicker.value();
                    if (currentValue == null)
                    {
                        // set the value to noon.
                        var noon = new Date(Date.now());
                        noon.setHours(12);
                        noon.setMinutes(0);
                        noon.setSeconds(0);
                        noon.setMilliseconds(0);
                        kendoDateTimePicker.value(noon);
                        // set the value - this allows the case where the user wants the value to be noon.
                        valueAccessor()(noon);
                    }                    
                    kendoDateTimePicker.open("date");
                }
            } else {
                picker.click();
            }
        });
        //setup the bait and switch pieces
        $("#" + pickerNm).focusout(function () {
            $("#" + pickerNmWrap).hide();
            //$("#" + pickerNmFriendly).show();
            $(element).show();
        })

        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            if (element.gcpePickerUseKendo && kendoPicker) {
                kendoPicker.destroy();
            }
        });

    },
    //update the control when the view model changes
    update: function (element, valueAccessor) {

        var picker = element.gcpePicker;
        var friendly = element.gcpePickerFriendly;
        var useKendo = element.gcpePickerUseKendo;
        if (valueAccessor()()) {
            var d = moment(ko.utils.unwrapObservable(valueAccessor()));
            if (useKendo) {
                picker.data("kendoDateTimePicker").value(ko.utils.unwrapObservable(valueAccessor()));
                // kdp.value(ko.utils.unwrapObservable(valueAccessor()));
            } else {
                picker.value = d.format("YYYY-MM-DDTHH:mm");
            }
            friendly.value = d.format("LLLL");

        } else {
            picker.value = "";
            friendly.value = element.gcpePickerPlaceholderText ? element.gcpePickerPlaceholderText : "";
        }

        //var value = ko.utils.unwrapObservable(valueAccessor());
        //$(element).datepicker("setDate", value);
    }
};

ko.validation.makeBindingHandlerValidatable('gcpeDateTimePicker');