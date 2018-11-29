class UserVm {

    public id: KnockoutObservable<string>;
    public displayAs: KnockoutObservable<string>;
   // public isCurrent: KnockoutObservable<boolean>;
    public isEditor: KnockoutObservable<boolean>;
    public isAdvanced: KnockoutObservable<boolean>;
    public mobileTelephone: KnockoutObservable<string>;
    public workTelephone: KnockoutObservable<string>;
    public workTelephoneExtension: KnockoutObservable<string>;
    public emailAddress: KnockoutObservable<string>;

    constructor(data: server.UserDto) {
        this.id = ko.observable(data.id);
        this.displayAs = ko.observable(data.displayAs);
        //this.isCurrent = ko.observable(false);
        this.isEditor = ko.observable(data.isEditor);
        this.isAdvanced = ko.observable(data.isAdvanced);
        this.mobileTelephone = ko.observable(data.mobileTelephone);
        this.workTelephone = ko.observable(data.workTelephone);
        this.workTelephoneExtension = ko.observable(data.workTelephoneExtension); // Maybe, future!
        this.emailAddress = ko.observable(data.emailAddress);

    }//Constructor

    public formattedMobilePhone = () => {
        var formatted:string = "";
        if (this.mobileTelephone() && this.mobileTelephone().length == 10) {
            formatted = this.mobileTelephone().replace(/(\d{3})(\d{3})(\d{4})/, "$1-$2-$3"); // Using 250-555-1234 format.
        } else {
            formatted = this.mobileTelephone();
        }
        return formatted;
    }

    public formattedWorkPhone = () => {
        var formatted:string = "";
        if (this.workTelephone() && this.workTelephone().length == 10) {
            formatted = this.workTelephone().replace(/(\d{3})(\d{3})(\d{4})/, "$1-$2-$3"); // Using 250-555-1234 format.
        } else {
            formatted = this.workTelephone();
        }
        if (this.workTelephoneExtension()) {
            formatted += " ext ";
            formatted += this.workTelephoneExtension();
        }
        return formatted;
    }


    public initiateWorkPhoneCall = (data, event) => {
        window.location.href = 'tel:' + this.workTelephone() + (this.workTelephoneExtension() ? ',,,' + this.workTelephoneExtension() : '');
    }

    public initiateMobilePhoneCall = (data, event) => {
        window.location.href = 'tel:' + this.mobileTelephone();
    }

    public static getCurrentUser(callback: Function): void {
        $.ajax('/api/users/me', {
            success: function (data) {
                var obj = new UserVm(data);
                callback(undefined, obj);
        },
            error: function (err) {
                callback(err);
            }
       });
    }

} //User
