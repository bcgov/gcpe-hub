ko.bindingHandlers.momentDateDisplay = {
    init: function (element, valueAccessor, allBindingsAccessor, data, context) {

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            // This will be called when the element is removed by Knockout or
            // if some other part of your code calls ko.removeNode(element)
            clearTimeout(element.timeout);
        });

        var textValueAccessor = function () {
            return valueAccessor().value;
        }
        //forward to the original options binding
        return ko.bindingHandlers.text.init(element, textValueAccessor, allBindingsAccessor, data, context);

    },
    //update the control when the view model changes
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

        //This sets the custom date format for the 'Calendar' option so that the default is our wanted date format.
        moment.updateLocale('en', {
            calendar: {
                sameElse: 'ddd, MMM D, h:mm A'
            }
        });

        function getMomentString() {
            var retval = "";
            if (valueAccessor().value()) {
                element.style.color = "";
                var d = moment(valueAccessor().value());
                if (valueAccessor().format) {
                    if (valueAccessor().format.toUpperCase() == 'FROMNOW') {
                        retval = d.fromNow();
                    } else if (valueAccessor().format.toUpperCase() == 'CALENDAR') {
                        retval = d.calendar();
                    } else {
                        retval = d.format(valueAccessor().format);
                    }

                } else {
                    retval = d.toString();
                }

            }
            else {
                if (valueAccessor().placeholder) {
                    retval = valueAccessor().placeholder.displayText;
                    //if using a placeholder check for the color attribute
                    if (valueAccessor().placeholder.color) {
                        element.style.color = valueAccessor().placeholder.color;
                    }
                }
            }
            return retval;

        }

        function loopUpdate() {

            clearTimeout(element.timeout);
            $(element).text(getMomentString());

            element.timeout = setTimeout(loopUpdate, 10000);


        }

        var textValueAccessor = function () {
            if (valueAccessor().format) {
                if (valueAccessor().format.toUpperCase() == 'FROMNOW') {
                    clearTimeout(element.timeout);
                    element.timeout = setTimeout(loopUpdate, 10000);
                }
            }
            return getMomentString();
        }
        //forward the update to the original options binding
        return ko.bindingHandlers.text.update(element, textValueAccessor, allBindings, viewModel, bindingContext);
    }
};

ko.validation.makeBindingHandlerValidatable('momentDateDisplay');