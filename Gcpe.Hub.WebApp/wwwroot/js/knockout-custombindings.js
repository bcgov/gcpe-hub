ko.bindingHandlers.formattedPhone = {
    update: function (element, valueAccessor) {
        var phone = ko.utils.unwrapObservable(valueAccessor());
        var formatPhone = function () {
            var formatted;
            if (phone && phone.length == 10) {
                formatted = phone.replace(/(\d{3})(\d{3})(\d{4})/, "($1) $2-$3");
            } else {
                formatted = phone;
            }
            return formatted;
        }
        ko.bindingHandlers.text.update(element, formatPhone);
    }
};

ko.bindingHandlers.trimmedValue = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);
    },
    update: function (element, valueAccessor) {
        var val = ko.utils.unwrapObservable(valueAccessor());
        if (val && (typeof val === 'string')) {
            val = val.trim();
            valueAccessor()(val);
        }
        ko.bindingHandlers.value.update(element, val);
    }
};


/* Experimental: Should work well as long as we don't need to fire custom code on Cancel. */
ko.bindingHandlers.modal = {
    init: function (element, valueAccessor) {
        $(element).modal({
            show: false
        });

        var value = valueAccessor();
        if (typeof value === 'function') {
            $(element).on('hide.bs.modal', function () {
                value(undefined); // Regardless how it's dismissed.
            });
        }
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).modal("destroy");
        });
    },
    update: function (element, valueAccessor) {
        var value = valueAccessor();
        if (ko.utils.unwrapObservable(value)) {
            $(element).modal('show');
        } else {
            $(element).modal('hide');
        }
    }
};

//Select Mulitples from a list picker binding.
ko.bindingHandlers.selectPicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        if ($(element).is('select')) {
            if (ko.isObservable(valueAccessor())) {
                if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                    // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                    ko.bindingHandlers.selectedOptions.init(element, valueAccessor, allBindingsAccessor);
                } else {
                    // regular select and observable so call the default value binding
                    ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);
                }
            }
            //Revisit this: http://stackoverflow.com/questions/34660500/mobile-safari-multi-select-bug
                var check = false;
                (function (a) { if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) check = true; })(navigator.userAgent || navigator.vendor || window.opera);

            if (check) {
                $(element).addClass('selectpicker').selectpicker('mobile');
            }
            else {
                $(element).addClass('selectpicker').selectpicker();
            }

            //Required to disable the element in the drop down
            $(element).on('show.bs.select', function (e) {
                setTimeout((function () {
                    $(element).find("option[value='']").attr('disabled', 'disabled');
                    $(element).selectpicker('render');
                }).bind(this), 0);
            });
        }
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        if ($(element).is('select')) {
            var selectPickerOptions = allBindingsAccessor().selectPickerOptions;
            if (typeof selectPickerOptions !== 'undefined' && selectPickerOptions !== null) {
                var options = selectPickerOptions.optionsArray,
                    isDisabled = selectPickerOptions.disabledCondition || false, 
                    resetOnDisabled = selectPickerOptions.resetOnDisabled || false;
                if (ko.utils.unwrapObservable(options).length > 0) {
                    // call the default Knockout options binding
                    ko.bindingHandlers.options.update(element, options, allBindingsAccessor);
                }
                if (isDisabled && resetOnDisabled) {
                    // the dropdown is disabled and we need to reset it to its first option
                    $(element).selectpicker('val', $(element).children('option:first').val());
                }
                $(element).prop('disabled', isDisabled);
            }
            if (ko.isObservable(valueAccessor())) {
                if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                    // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                    ko.bindingHandlers.selectedOptions.update(element, valueAccessor);
                } else {
                    // call the default Knockout value binding
                    ko.bindingHandlers.value.update(element, valueAccessor);
                }
            }

            $(element).selectpicker('refresh');
        }
    }
};


ko.bindingHandlers.toggleChecked = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var allB = allBindingsAccessor();
        var initObj = {on:'on', off:'off'};
        if (allB.toggleCheckedOptions){
            if (allB.toggleCheckedOptions.on && allB.toggleCheckedOptions.on.text) {
                if (typeof allB.toggleCheckedOptions.on.text == "string") {
                    initObj.on = allB.toggleCheckedOptions.on.text;
                } else {
                    initObj.on = allB.toggleCheckedOptions.on.text();
                    //subscribe to the observable, so we can propagate the changes to the control
                    allB.toggleCheckedOptions.on.text.subscribe(function (newValue) {
                        initObj.on = newValue;
                        //re-render the control into the dom
                        $(element).bootstrapToggle('destroy');
                        $(element).bootstrapToggle(initObj);
                    });
                }
                
                initObj.onValue = allB.toggleCheckedOptions.on.value;
                
            }
            if (allB.toggleCheckedOptions.off && allB.toggleCheckedOptions.off.text) {
                if (typeof allB.toggleCheckedOptions.off.text == "string") {
                    initObj.off = allB.toggleCheckedOptions.off.text;
                } else {
                    initObj.off = allB.toggleCheckedOptions.off.text();
                    //subscribe to the observable, so we can propagate the changes to the control
                    allB.toggleCheckedOptions.off.text.subscribe(function (newValue) {
                        initObj.off = newValue;
                        //re-render the control into the dom
                        $(element).bootstrapToggle('destroy');
                        $(element).bootstrapToggle(initObj);
                    });
                }
                initObj.offValue = allB.toggleCheckedOptions.off.value;
            }
        }


        $(element).bootstrapToggle(initObj);

        $(element).change(function (evt) {

            if (evt.currentTarget.checked) {
                valueAccessor()(initObj.onValue);
            } else {
                valueAccessor()(initObj.offValue);
            }
           
        });

        if (valueAccessor()() == initObj.onValue) {
            $(element).bootstrapToggle('on');
        } else {
            $(element).bootstrapToggle('false');
        }

        element.toggleBindInit = initObj;
        
        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).bootstrapToggle('destroy');
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor) {

        var initObj = element.toggleBindInit;

        function checkedValueAccessor() {
            return function () {
                var ret = false;
                if (valueAccessor()() == initObj.onValue) {
                    ret = true;
                }

                return ret;
            }
        };

        if (valueAccessor()() == initObj.onValue) {
            $(element).bootstrapToggle('on');
        };
        if (valueAccessor()() == initObj.offValue) {
            $(element).bootstrapToggle('off');
        }

        ko.bindingHandlers.value.update(element, checkedValueAccessor, allBindingsAccessor);
    }
};

ko.bindingHandlers.routerClick = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var allB = allBindingsAccessor();
        var route_p = {}
        if (allB.routerClick) {
            route_p.path = allB.routerClick.path;
            route_p.data = allB.routerClick.data;
            route_p.confirmed = allB.routerClick.confirmed;
            
        }
        
        var va = function () {
            return function () {
                bindingContext.$root.router.navigateTo(route_p);
            };
        };
        ko.bindingHandlers.click.init(element, va, allBindingsAccessor, viewModel, bindingContext);
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

        ko.bindingHandlers.value.update(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
    }
};
