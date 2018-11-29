ko.bindingHandlers.gcpeOptions = {
    init: function (element, valueAccessor, allBindingsAccessor, data, context) {

        //make sure we're on a select
        var tp = element.nodeName;

        if (!(tp == "SELECT")) {
            throw "Please use gcpeOptions on a select, not on: " + tp;
        }

        //setup the bait and switch pieces
        ko.utils.registerEventHandler(document.getElementById(element.id), "focusin", function (evt) {
            //$("#" + valueInput).focusin(function (evt) {
            $(element).removeClass("gcpeoptionsimple");
        });

        //setup the bait and switch pieces
        $(element).focusout(function (evt) {
            $(element).addClass("gcpeoptionsimple");
        });

        $(element).addClass("gcpeoptionsimple");

        return ko.bindingHandlers.options.init(element, valueAccessor, allBindingsAccessor(), data, context);


    },
    //update the control when the view model changes
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        //no need to do anything here as it is not used to handle changes to the bound option value, only to the list of options.

        //make it validate-able
        ko.bindingHandlers.validationElement.update(element, valueAccessor, allBindings, viewModel, bindingContext);

        //gcpeoptionsNoBorder
        if (valueAccessor()()) {
            if (valueAccessor()().length > 1) {
                $(element).removeClass("gcpeoptionsNoBorder");
            } 
        } else {
            $(element).removeClass("gcpeoptionsNoBorder");
        }

        //forward to the original options binding
        var allBn = allBindings();
        if (valueAccessor()().length == 1) {
            //console.log("removing caption");

            delete allBn.optionsCaption;
        } 
        var fxAllB = function () { return allBn; };
        fxAllB.get = function (key) {
            var ret = undefined;
            if (allBn.hasOwnProperty(key)) {
                ret = allBindings.get(key);
            }
            return ret;
        };
        fxAllB.has = function (key) {
            var ret = undefined;
            if (allBn.hasOwnProperty(key)) {
                ret = allBindings.has(key);
            }
            return ret;
        };

        try {
            //forward the update to the original options binding, replacing the allBindingsAccessor 
            //with our own, to allow the removal of the optionsCaption
            return ko.bindingHandlers.options.update(element, valueAccessor, fxAllB, viewModel, bindingContext);
        } catch (ex) {
            console.log("gcpeOptions binding error", (element.id ? element.id : element), ex);
        }
    }
};

ko.validation.makeBindingHandlerValidatable('gcpeOptions');