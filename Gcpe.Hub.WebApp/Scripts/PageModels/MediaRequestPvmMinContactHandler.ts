module MediaRequest {

    // **********************************************************
    // ***   MinistryContactsHandler                           ***
    // **********************************************************
    export class MinistryContactsHandler {

        public ministryContacts: KnockoutObservableArray<MinistryVm>;
        public showMinistryContactUi: KnockoutObservable<boolean>;
        public isFetching: KnockoutObservable<boolean>;
        public isEditing: KnockoutObservable<boolean>;

        private saveCount: number;

        constructor(private pvm: MediaRequestPvm) {
            this.ministryContacts = ko.observableArray<MinistryVm>();
            this.showMinistryContactUi = ko.observable<boolean>(false); // Show the UI.
            this.isFetching = ko.observable<boolean>(false); // Actively waiting on the API call.
            this.isEditing = ko.observable<boolean>(false); 
        } // End of constructor

        public initializeUi = () => {
            this.isFetching(false);
            this.pvm.pageError(undefined);
            // others?

            // Initialize so that non-Advanced users *start* in Edit mode.
            // Advanced users, are required to toggle the context menu.
            if (this.pvm.currentUser && this.pvm.currentUser.user() && !this.pvm.currentUser.user().isAdvanced()) {
                this.isEditing(true);
            }
        }

        public toggleEdit = () => {
            this.isEditing(!this.isEditing());
        }

        public isDirty = () => {
            // We have one Edit page & one Save button for all 
            // my Ministries, if any of my ministries are dirty...
            for (var i = 0; i < this.pvm.myMinistries().length; i++) {
                var ministry: MinistryVm = this.pvm.myMinistries()[i];
                if (ministry.__minIsDirty()) {
                    return true;
                }
            }
            return false;
        }

        public cancelEdit = () => {
            // Cancel Edit on only the dirty ones. 
            for (var i = 0; i < this.pvm.myMinistries().length; i++) {
                var ministry: MinistryVm = this.pvm.myMinistries()[i];
                ministry.cancelEdit();
            }
            this.isEditing(false);
        }

        public confirmLeave = (): ConfirmResult => {
            let ret: ConfirmResult;
            let changed: number = 0;

            for (var i = 0; i < this.pvm.myMinistries().length; i++) {
                var ministry: MinistryVm = this.pvm.myMinistries()[i];
                if (ministry.__minIsDirty()) {
                    changed++;
                }
            }

            if (changed > 0) {
                ret = { discardChangesFx: this.cancelEdit };
            }
            return ret;
        }

        public saveMinistryContacts = () => {
            this.isFetching(true);
            this.pvm.pageError(undefined);
            this.saveCount = 0;

            // Count the dirty ones.
            for (var i = 0; i < this.pvm.myMinistries().length; i++) {
                var ministry: MinistryVm = this.pvm.myMinistries()[i];
                if (ministry.__minIsDirty()) {
                    this.saveCount++;
                }
            }
            if (this.saveCount === 0) {
                return;
            }
            $(".disable-on-busy").addClass("disabled");

            let hasErrors: boolean = false;
            for (var i = 0; i < this.pvm.myMinistries().length; i++) {
                var ministry: MinistryVm = this.pvm.myMinistries()[i];
                if (ministry.__minIsDirty()) {
                    ministry.saveMinistryContacts((err, data) => {
                        // Once we're at 0, they're all back!
                        this.isFetching(--this.saveCount !== 0);
                        if (!this.isFetching()) {
                            // Re-enable the Save & Cancel buttons
                            $(".disable-on-busy").removeClass("disabled");

                            // If we're finished without any Errors, do something.
                            if (!hasErrors) {
                                // Turn off the input fields.
                                this.isEditing(false);

                                if (this.pvm.currentUser && this.pvm.currentUser.user() && !this.pvm.currentUser.user().isAdvanced()) {
                                    // Bounce Regular Users back where they came from.
                                    window.history.back();
                                }
                            }
                        }
                        if (err) {
                            this.pvm.pageError(err);
                            hasErrors = true;
                        }
                    });
                }
            }
        }

        public unloadPage = () => {
            this.pvm.pageError(undefined);
            this.showMinistryContactUi(false);
        }

    } // End of MinistryContactsHandler
}