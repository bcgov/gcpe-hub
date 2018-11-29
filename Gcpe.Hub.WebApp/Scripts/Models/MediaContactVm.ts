class MediaContactVm {
    private origVal: any; //hide the value under a function so it's excluded from serializing to json.

    public id: KnockoutObservable<string>;
    public firstName: KnockoutObservable<string>;
    public lastName: KnockoutObservable<string>;
    public name: KnockoutComputed<string>;
    public job: KnockoutObservable<MediaJobVm>;
    public workPhone: KnockoutObservable<string>;
    public workPhoneExtension: KnockoutObservable<string>;
    public cellPhone: KnockoutObservable<string>;
    public formattedWorkPhone: KnockoutComputed<string>;
    public formattedCellPhone: KnockoutComputed<string>;
    public email: KnockoutObservable<string>;
    public searchParts: KnockoutObservableArray<string>;
    public searchFormattedName: KnockoutComputed<string>;
    public searchFormattedOutletName: KnockoutComputed<string>;

    public errors;



    constructor(data?: server.MediaContactDto) {
        var self = this;
        this.id = ko.observable<string>();
        this.firstName = ko.observable<string>().extend({ required: { message: "Please enter a first name" } });
        this.lastName = ko.observable<string>().extend({ required: { message: "Please enter a last name" } });
        this.workPhone = ko.observable<string>();
        this.workPhoneExtension = ko.observable<string>();
        this.email = ko.observable<string>();
        this.cellPhone = ko.observable<string>().extend({
            required: {
                message: "Please enter a phone # OR e-mail",
                onlyIf: function () { return ko.validation.utils.isEmptyVal(self.workPhone()) && ko.validation.utils.isEmptyVal(self.email()); }
            }
        });
        this.email.extend({
            required: {
                message: "Please enter a phone # OR e-mail",
                onlyIf: function () { return ko.validation.utils.isEmptyVal(self.cellPhone()) && ko.validation.utils.isEmptyVal(self.workPhone()); }
            }
        });
        this.searchParts = ko.observableArray<string>();
        this.job = ko.observable<MediaJobVm>();

        this.name = ko.pureComputed(() => {
            return (this.firstName() ? this.firstName() + " " : "") + (this.lastName() ? this.lastName() : "");
        });

        this.origVal = function () {
            // Empty function is required for cancel edit feature.
        };

        this.workPhone.extend({
            required: {
                message: "Please enter a phone # OR e-mail",
                onlyIf: function () { return ko.validation.utils.isEmptyVal(self.cellPhone()) && ko.validation.utils.isEmptyVal(self.email()); }
            }
        });

        this.searchFormattedName = ko.pureComputed(() => {
            /* Search splits the input & does a 'starts-with' on first & last names.
                Test with "cr joh", should return "<b>Jo</b>hn <b>Cr</b>ow" */
            if (this.searchParts()) {
                var lastNm = this.lastName() || '';
                var firstNm = this.firstName() || '';

                for (var i = 0; i < this.searchParts().length; i++) {
                    lastNm = lastNm.replace(new RegExp('^(' + this.searchParts()[i] + ')', 'i'), '<b>$1</b>');
                    firstNm = firstNm.replace(new RegExp('^(' + this.searchParts()[i].substr(0, 2) + ')', 'i'), '<b>$1</b>');
                }
                return firstNm + ' ' + lastNm;
            } else {
                return this.name();
            }
        });

        this.searchFormattedOutletName = ko.pureComputed(() => {
            var outletName = this.job().outlet() ? this.job().outlet().name() : '' || '';
            if (this.searchParts()) {
                for (var i = 0; i < this.searchParts().length; i++) {
                    outletName = outletName.replace(new RegExp('(' + this.searchParts()[i] + ')', 'i'), '<b>$1</b>');
                }
            }
            return outletName;
        });

        this.formattedWorkPhone = ko.pureComputed({
            read: function () {
                var formatted = "";
                if (this.workPhone() && this.workPhone().length == 10) {
                    formatted = this.workPhone().replace(/(\d{3})(\d{3})(\d{4})/, "$1-$2-$3"); // Using 250-555-1234 format.
                } else {
                    formatted = this.workPhone();
                }
                if (this.workPhoneExtension()) {
                    formatted += " ext ";
                    formatted += this.workPhoneExtension();
                }
                return formatted;
            },
            write: function (value: string) {
                var teleDigits = '';
                var extDigits = '';
                if (value) {
                    var parts = value.split('ext');
                    teleDigits = parts[0].match(/\d+/g).join(''); // Strip, non-digits and join without a delimiter.
                    if (parts.length > 0) {
                        var extDigitsMatch = parts[1].match(/\d+/g);
                        if (extDigitsMatch) {
                            extDigits = extDigitsMatch.join(''); // Strip, non-digits and join without a delimiter.
                        }
                    }
                }
                this.workPhone(teleDigits);
                this.workPhoneExtension(extDigits);
            },
            owner: this
        });

        this.formattedCellPhone = ko.pureComputed({
            read: function () {
                var formatted = "";
                if (this.cellPhone() && this.cellPhone().length == 10) {
                    formatted = this.cellPhone().replace(/(\d{3})(\d{3})(\d{4})/, "$1-$2-$3"); // Using 250-555-1234 format.
                } else {
                    formatted = this.cellPhone();
                }
                return formatted;
            },
            write: function (value: string) {
                var teleDigits = '';
                if (value) {
                    teleDigits = value.match(/\d+/g).join(''); // Strip, non-digits and join without a delimiter.
                }
                this.cellPhone(teleDigits);
            },
            owner: this
        });

        if (data) {
            this.init(data);
        } else {
            this.job(new MediaJobVm());
        }
        this.errors = ko.validation.group(this, { deep: true });
    }

    private init(data: server.MediaContactDto): void {
        this.id(data.id);
        this.firstName(data.firstName);
        this.lastName(data.lastName);
        this.workPhone(data.workPhone);
        this.workPhoneExtension(data.workPhoneExtension);
        this.cellPhone(data.cellPhone);
        this.email(data.email);
        this.job(new MediaJobVm(data.job as server.MediaJobDto));

        this.origVal.value = data;
    }

    public cancelEdit = () => {
        if (this.origVal.value) {
            this.init(this.origVal.value);
        }
    }

    public initiateWorkPhoneCall = (data, event) => {
        window.location.href = 'tel:' + this.workPhone() + (this.workPhoneExtension() ? ',,,' + this.workPhoneExtension() : '');
    }

    public initiateCellPhoneCall = (data, event) => {
        window.location.href = 'tel:' + this.cellPhone();
    }

    public static findMediaContacts(queryString: string, callback: Function): void {
        //var queryStringEncoded = encodeURI(queryString);

        // Make sure we don't call the api without a filter parameter
        queryString = queryString.trim() || "dummy" + new Date().getMilliseconds();

        $.ajax('/api/mediacontacts?filter=' + queryString, {
            success: function (data) {
                var arr = [];
                var searchParts = queryString.toLowerCase().split(' ');
                for (var i = 0; i < data.length; i++) {
                    var obj = new MediaContactVm(data[i]);
                    obj.searchParts(searchParts); // Allows us to show what part of the name matched!
                    arr.push(obj);
                };
                callback(undefined, arr);
            },
            error: function (err) {
                callback(err);
            }
        });
    }

    public isValid(): boolean {
        var isValid = this.errors().length === 0;
        if (!isValid) {
            this.errors.showAllMessages();
        }
        return isValid;
    }


    public save(callback: Function): void {
        var me: MediaContactVm = this;
        const saveData = ko.toJSON(this);

        //we need to sort out if we are inserting or updating.
        var updateContactId = this.id() ? "/" + this.id() : '';

        $.ajax('/api/mediacontacts' + updateContactId, {
            type: updateContactId ? 'PUT' : 'POST',
            contentType: 'application/json',
            data: saveData,
            success: function (data: server.MediaContactDto) {
                // We need to fake up the 'origVal' for future Undo operations.
                me.origVal.value = ko.utils.parseJson(saveData);
                if (!updateContactId) {
                    // For new mediaContacts, we need to set the new Id
                    me.id(data.id);
                    me.origVal.value.id = data.id;
                    // .... and also the job id!
                    me.job().id(data.job.id);
                }
                callback(undefined, data);
            },
            error: function (err) {
                callback(err);
            }
        });
    }

}