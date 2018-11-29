class MinistryVm {

    public id: KnockoutObservable<string>;
    public displayAs: KnockoutObservable<string>;
    public abbreviation: KnockoutObservable<string>;
    public users: Array<UserVm>;
    public currentUserVm: UserVm;
    public eodFinalizedDateTime: KnockoutObservable<Date>;
    public eodLastRunUser: KnockoutObservable<UserVm>;
    public eodLastRunDateTime: KnockoutObservable<Date>;
    public primaryContact: KnockoutObservable<UserVm>;
    public secondaryContact: KnockoutObservable<UserVm>;
    public afterHoursPhone: KnockoutObservable<string>;
    public afterHoursPhoneExtension: KnockoutObservable<string>;
    public formattedAfterHoursPhone: KnockoutComputed<string>;

    private origVal: any;

    // Used client-side only in the MinistryContact edit list.
    public isMyMinistry: KnockoutObservable<boolean>;

    constructor(data: server.MinistryDto, currentUser?: UserVm) {
        if (!data) {
            throw new Error("data is needed on the constructor of MinistryVM");
        }
        this.id = ko.observable<string>();
        this.displayAs = ko.observable<string>();
        this.abbreviation = ko.observable<string>();
        this.users = new Array<UserVm>();
        this.eodFinalizedDateTime = ko.observable<Date>();
        this.eodLastRunUser = ko.observable<UserVm>();
        this.eodLastRunDateTime = ko.observable<Date>();
        this.primaryContact = ko.observable<UserVm>();
        this.secondaryContact = ko.observable<UserVm>();
        this.afterHoursPhone = ko.observable<string>();
        this.afterHoursPhoneExtension = ko.observable<string>();
        this.isMyMinistry = ko.observable(false); // We'll set this after init fires.

        this.formattedAfterHoursPhone = ko.pureComputed({
            read: function () {
                var formatted = "";
                if (this.afterHoursPhone() && this.afterHoursPhone().length == 10) {
                    formatted = this.afterHoursPhone().replace(/(\d{3})(\d{3})(\d{4})/, "$1-$2-$3"); // Using 250-555-1234 format.
                } else {
                    formatted = this.afterHoursPhone();
                }
                if (this.afterHoursPhoneExtension()) {
                    formatted += " ext ";
                    formatted += this.afterHoursPhoneExtension();
                }
                return formatted;
            },
            write: function (value: string) {
                var teleDigits = '';
                var extDigits = '';
                if (value) {
                    var parts = value.split('ext');
                    teleDigits = parts[0].match(/\d+/g).join(''); // Strip, non-digits and join without a delimiter.
                    teleDigits = teleDigits.replace(/(\d{3})(\d{3})(\d{4})/, "$1-$2-$3"); // Using 250-555-1234 format.
                    if (parts.length > 0) {
                        var extDigitsMatch = parts[1].match(/\d+/g);
                        if (extDigitsMatch) {
                            extDigits = extDigitsMatch.join(''); // Strip, non-digits and join without a delimiter.
                        }
                    }
                }
                this.afterHoursPhone(teleDigits);
                this.afterHoursPhoneExtension(extDigits);
            },
            owner: this
        });

        // Empty function is required for cancel edit feature.
        this.origVal = function () {};
        

        if (data) {
            // Saved for the undo.
            this.origVal.data = data;
            this.origVal.currentUser = currentUser;

            this.init(data, currentUser);
        }
    }

    public initiateAfterHoursPhoneCall = (data, event) => {
        window.location.href = 'tel:' + this.afterHoursPhone() + (this.afterHoursPhoneExtension() ? ',,,' + this.afterHoursPhoneExtension() : '');
    }

    // Brute force dirty checking 'cause it's only a few fields.
    public __minIsDirty = () => {

        let retval: boolean = false;
        let origMinistry: server.MinistryDto = this.origVal.data;
        if (origMinistry) {

            let origPrimaryId = (origMinistry.primaryContact ? origMinistry.primaryContact.id : undefined);
            let primaryId = (this.primaryContact() ? this.primaryContact().id() : undefined);
            if (origPrimaryId != primaryId) {
                retval =  true;
            }

            let origSecondaryId = (origMinistry.secondaryContact ? origMinistry.secondaryContact.id : undefined);
            let secondaryId = (this.secondaryContact() ? this.secondaryContact().id() : undefined);
            if (origSecondaryId != secondaryId) {
                retval =  true;
            }
            let ahPhone = (this.afterHoursPhone() ? this.afterHoursPhone() : '');
            let phone = (origMinistry.afterHoursPhone ? origMinistry.afterHoursPhone : '');
            if (phone != ahPhone) {
                retval = true;
            }
            let ahPhoneExt = (this.afterHoursPhoneExtension() ? this.afterHoursPhoneExtension() : '');
            let phoneExt = (origMinistry.afterHoursPhoneExtension ? origMinistry.afterHoursPhoneExtension : '');
            if (phoneExt != ahPhoneExt) {
                retval = true;
            }
        }
        return retval;

    };


    private init(data: server.MinistryDto, currentUser?: UserVm) {

        this.id(data.id);
        this.displayAs(data.displayAs);
        this.abbreviation(data.abbreviation);
        this.eodFinalizedDateTime(data.eodFinalizedDateTime ? new Date(data.eodFinalizedDateTime) : undefined);
        this.eodLastRunDateTime(data.eodLastRunDateTime ? new Date(data.eodLastRunDateTime) : undefined);
        this.afterHoursPhone(data.afterHoursPhone ? data.afterHoursPhone : undefined);
        this.afterHoursPhoneExtension(data.afterHoursPhoneExtension ? data.afterHoursPhoneExtension : undefined);


        if (this.users.length === 0) {
            // Only initialze the list on initial load - we don't edit them *anywhere*.
            this.initializeUsers(data, currentUser);
        } else {
            // Set using the existing Users array.
            this.primaryContact(data.primaryContact ? this.getUser(data.primaryContact.id) : undefined);
            this.secondaryContact(data.secondaryContact ? this.getUser(data.secondaryContact.id) : undefined);
        }

        if (data.eodLastRunUser) {
            this.eodLastRunUser(new UserVm(data.eodLastRunUser));
        }
    }

    private getUser = (id: string):UserVm => {
        let foundUser: UserVm;
        for (var i = 0; i < this.users.length; i++) {
            if (this.users[i].id() === id) {
                foundUser = this.users[i];
            }
        }
        return foundUser;
    }

    private initializeUsers(data: server.MinistryDto, currentUser?: UserVm): void {
        // Create Primary & Secondary UserVm's from the Ministry record.
        let primary: UserVm;
        if (data.primaryContact) {
            primary = new UserVm(data.primaryContact);
        }

        let secondary: UserVm;
        if (data.secondaryContact) {
            secondary = new UserVm(data.secondaryContact);
        }
        if (currentUser) {
            for (var i = 0; i < data.users.length; i++) {
                var u = new UserVm(data.users[i]);
                if (u.id() == currentUser.id()) {
                    this.currentUserVm = u;
                }
                if (primary && u.id() == primary.id()) {
                    this.primaryContact(u);
                }
                if (secondary && u.id() == secondary.id()) {
                    this.secondaryContact(u);
                }
                this.users.push(u);
            }
        }
        // If the selected Contact is no longer in 
        // this Ministry, force them into the list!
        if (!this.primaryContact()) {
            if (primary) {
                this.primaryContact(primary);
                this.users.push(primary);
            }
        }
        if (!this.secondaryContact()) {
            if (secondary) {
                this.secondaryContact(secondary);
                this.users.push(secondary);
            }
        }
    }

    public cancelEdit = () => {
        if (this.origVal.data && this.__minIsDirty()) {
            this.init(this.origVal.data, this.origVal.currentUser);
        }
    }

    public toString(): string {
        return this.id();
    }
    public valueOf(): string {
        return this.id();
    }

    public static findMinistries( currentUser: UserVm, callback: Function): void {
        $.ajax('/api/ministries', {
            success: function (data) {
                var allMinArr = [];
                var myMinArr = [];
                for (var i = 0; i < data.length; i++) {
                    var minrec = new MinistryVm(data[i], currentUser);
                    allMinArr.push(minrec);
                    // Create a separate array for *my* Ministries & flag them as such.
                    if (minrec.currentUserVm != null || currentUser.isAdvanced()) {
                        minrec.isMyMinistry(true);
                        myMinArr.push(minrec);
                    }
                };

                callback(undefined, allMinArr, myMinArr);
            },
            error: function (err) {
                callback(err);
            }
        });
    }

    public saveMinistryContacts(callback: Function): void {
        var me: MinistryVm = this;

        // Only allow updating, not insert!
        if (!this.id()) {
            return callback(new Error("Id is required for saveMinistryContacts()"));
        }

        // Whitelist the properties to send back, not the whole VM.
        let putData:any = {
            id : this.id(),
            primaryContact: { id: (this.primaryContact() ? this.primaryContact().id() : undefined) },
            secondaryContact: { id: (this.secondaryContact() ? this.secondaryContact().id() : undefined) },
            afterHoursPhone: this.afterHoursPhone(), 
            afterHoursPhoneExtension : this.afterHoursPhoneExtension()
        };
        $.ajax('/api/ministries/' + this.id(), {
            type: 'PUT',
            contentType: 'application/json',
            data: ko.toJSON(putData),
            success: (data) => {
                //now make sure the record is not dirty
                this.origVal.data.primaryContact = putData.primaryContact;
                this.origVal.data.secondaryContact = putData.secondaryContact;
                this.origVal.data.afterHoursPhone = putData.afterHoursPhone;
                this.origVal.data.afterHoursPhoneExtension = putData.afterHoursPhoneExtension;
                callback(undefined, true);
            },
            error: (err) => {
                callback(err);
            }
        });
    }
}