module MediaRequest {

    // **********************************************************
    // ***   MediaContactHandler                              ***
    // **********************************************************
    export class MediaContactHandler {

        public pageError: KnockoutObservable<any>;
        public editMediaContact: KnockoutObservable<MediaContactVm>;
        public contactTitles: KnockoutObservableArray<string>;
        public outlets: KnockoutObservableArray<MediaOutletVm>;
        public selectedMediaContact: KnockoutObservable<MediaContactVm>;
        public outletSearch: KnockoutObservable<string>;
        public saveMediaContactErrorHtml: KnockoutComputed<string>;

        public static capitalise(name): string {
            // treat name as an array of char.
            return name && name[0].toUpperCase() + name.slice(1).toLowerCase();
        }

        constructor(private pvm: MediaRequestPvm) {
            this.pageError = ko.observable();
            this.editMediaContact = ko.observable<MediaContactVm>();
            this.contactTitles = ko.observableArray<string>();
            this.outlets = ko.observableArray<MediaOutletVm>(); // Active Outlets only!
            this.selectedMediaContact = ko.observable<MediaContactVm>();
            this.outletSearch = ko.observable<string>();

            this.saveMediaContactErrorHtml = ko.pureComputed(function () {
                if (!this.pageError()) {
                    // If there's no Error or we're editing a mediaContact, do nothing.
                    return undefined;
                }
                return this.pvm.formatErrorAsHtml(this.pageError());
            }, this);

            this.selectedMediaContact = ko.observable<MediaContactVm>();
            this.selectedMediaContact.subscribe(function (newValue: MediaContactVm) {
                if (this.pvm.currentMediaRequest() && newValue) {
                    var alreadyExists = false;
                    for (var i = 0; i < this.pvm.currentMediaRequest().mediaContacts().length; i++) {
                        if (newValue.id() === this.pvm.currentMediaRequest().mediaContacts()[i].id()) {
                            alreadyExists = true;
                            break;
                        }
                    }
                    if (!alreadyExists) {
                        this.pvm.currentMediaRequest().mediaContacts.push(newValue);
                    }
                }
            }, this);


        }

        public addNewMediaContact = (searchString: string) => {
            // Add a new MediaContact!
            var newContact: KnockoutObservable<MediaContactVm> = ko.observable(new MediaContactVm());

            // Initialize the new MediaContactVm with the First & Last names as entered.
            if (searchString && searchString.trim().length > 0) {
                var searchParts = searchString.split(' ');
                newContact().firstName(MediaContactHandler.capitalise(searchParts[0]));
                if (searchParts.length > 1) {
                    // Add remaining parts back with spaces between each.
                    searchParts.splice(0, 1);
                    newContact().lastName(MediaContactHandler.capitalise(searchParts.join(' ')));
                }
            }
            const np: NavParams = {
                path: 'request/' + (this.pvm.currentMediaRequest().id() ? this.pvm.currentMediaRequest().id() : 'new') + '/contact/new',
                data: newContact(),
                confirmed: true
            }
            this.pvm.router.navigateTo(np)
            //this.pvm.initMediaContactModal(newContact());
        }

        public submitEditContact = () => {
            // Save new & edited Media Contacts!
            this.pageError(undefined);

            var validatable = ko.validatedObservable(this.editMediaContact());
            if (validatable().isValid()) {

                $("#dom-edit-mediacontact-save").addClass("disabled");
                this.editMediaContact().save((err: JQueryXHR, result: server.MediaContactDto) => {
                    $("#dom-edit-mediacontact-save").removeClass("disabled");
                    if (err) {
                        return this.pageError(err);
                    }
                    var contactOutlet = this.editMediaContact().job().outlet();
                    if (!contactOutlet.id()) {
                        contactOutlet.id(result.job.outlet.id);
                        // Add the outlet to the list of known Outlets
                        this.outlets.push(contactOutlet);
                    }
                    // Set it as selected on the PVM.
                    this.selectedMediaContact(this.editMediaContact());

                    //go back
                    window.history.back();
                });
            }
            else {
                validatable().errors.showAllMessages(true);
            }
        }

        public cancelEditContact = () => {
            if (this.editMediaContact()) {
                this.editMediaContact().cancelEdit();
            }

            window.history.back();
        }

        public searchContacts = (options) => {
            this.pageError(undefined);

            if (options.text && options.text.trim().length > 0) {
                var me = this;
                $('#dom-mediaContactSearch-busy').show();
                MediaContactVm.findMediaContacts(options.text, (err, data) => {
                    $('#dom-mediaContactSearch-busy').hide();
                    if (err) {
                        return me.pageError(err);
                    }

                    options.total = data.length;
                    options.callback({ data: data, total: options.total });
                });
            }
        };

        public changeOutlet = () => {
            this.editMediaContact().job().outlet(undefined);

            $('#dom-outlet-search-combobox input').focus();
        }


        public createNewOutlet = (outletName: string) => {
            var newOutlet = new MediaOutletVm({ id: undefined, name: outletName, isMajor: false });

            // Apply it to the MediaContact being edited
            this.editMediaContact().job().outlet(newOutlet);

            // And re-sort the outlets list...
            this.outlets.sort(function compare(a, b) {
                if (a.name < b.name) return -1;
                if (a.name > b.name) return 1;
                return 0;
            });
        }
    } // End of MediaContactHandler
}