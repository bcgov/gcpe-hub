/// <reference path="Typings/jquery.d.ts" />

function dirtyFlag(root, replacer) {

    if (root.__dirty) {
        return;
    }

    // This array will be added to Validated Observables and otherwise trigger dirty!
    root.errors = root.errors || [];

    root.__dirty = function () { };
    root.__dirty._initialState = ko.observable();
    root.__dirty._isInitiallyDirty = false;

    var _replacer;
    if (typeof replacer === 'string') {
        var exclude = replacer.split(',');
        exclude.push("errors"); // ko.validatedObservables will add an "errors" Array if not valid.
        exclude.push("_destroy"); // Filter uses the ko.destroy() method which adds a "_destroy" variable.
        _replacer = function (key, value) {
            // Exclude these from the JSON used by the auto-save (dirty flag) handler.
            if (exclude.indexOf(key) !== -1) {
                return undefined;
            }
            return value;
        };
    } else {
        _replacer = replacer;
    }

    root.__dirty._initialState(ko.toJSON(root, _replacer));

    //console.log("-------- Start  --------");
    //console.log(root.__dirty._initialState());

    root.__dirty.hasError = ko.observable(false);			// To be used externally by the UI.
    root.__dirty.isInitialized = ko.observable(false);  	// To be used externally by the UI.
    root.__dirty.isDirty = ko.computed(function () {
        var dirty = root.__dirty._isInitiallyDirty || root.__dirty._initialState() !== ko.toJSON(root, _replacer);
        if (dirty) {
            var printDifference = false; // DEBUGGING!
            if (!root.__dirty._isInitiallyDirty && printDifference) {
                console.log("-------- Initial --------");
                console.log(root.__dirty._initialState());
                console.log("-------- Current --------");
                console.log(ko.toJSON(root, _replacer));
            }
        }
        return dirty;
    }).extend({ throttle: 300 });

    // Tell the dirtyFlag that there are no changes to be updated.
    root.__dirty.reset = function () {
        root.__dirty._isInitiallyDirty = false;
        root.__dirty._initialState(ko.toJSON(root, _replacer));
        root.__dirty.isInitialized(true);
    };

    // An Update() may not be successful.
    root.__dirty.update = function (success) {
        root.__dirty.hasError(!success);
        root.__dirty.reset();
    };

    root.__dirty.reset();
};
