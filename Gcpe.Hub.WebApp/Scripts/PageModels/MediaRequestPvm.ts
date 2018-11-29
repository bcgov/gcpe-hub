module MediaRequest {

    // **********************************************************
    // ***   MediaRequestPvm                                  ***
    // **********************************************************
    export class MediaRequestPvm {

        public currentMediaRequest: KnockoutObservable<MediaRequestVm>;
        public specificEditMediaRequest: KnockoutObservable<MediaRequestVm>;
        public mediaRequests: KnockoutObservableArray<KnockoutObservable<MediaRequestVm>>;
        public allMinistries: KnockoutObservableArray<MinistryVm>;
        public allMinistriesExceptCurrentLead: KnockoutObservableArray<MinistryVm>; // AllButCurrentLeadMinistries
        public myMinistries: KnockoutObservableArray<MinistryVm>;
        public users: KnockoutObservableArray<UserVm>;
        public userMe: KnockoutObservable<UserVm>;
        public pageError: KnockoutObservable<any>;
        public saveErrorHtml: KnockoutComputed<string>;
        public isNotAcknowledged: KnockoutComputed<boolean>;
        public isShared: KnockoutComputed<boolean>;
        public isTakeOverRequest: KnockoutComputed<boolean>;
        public singleMinistry: KnockoutComputed<boolean>;
        public MyMinistriesFilterText: KnockoutComputed<string>;
        public canClaimMinistry: KnockoutComputed<boolean>;
        public sharingWithMinistries: KnockoutObservable<boolean>;
        public takeOverRequestActive: KnockoutObservable<boolean>;
        public listScrollPos: number;
        private pollingHandler: KnockoutObservable<MediaRequest.PollingHandler>;
        public router: MediaRequest.PvmRouter;
        public mediaContactHandler: KnockoutObservable<MediaRequest.MediaContactHandler>;
        public navConfirm: KnockoutObservable<MediaRequest.NavConfirm>;
        public searchHandler: KnockoutObservable<MediaRequest.SearchHandler>;
        public reportsHandler: KnockoutObservable<MediaRequest.ReportsHandler>;
        public ministryContactsHandler: KnockoutObservable<MediaRequest.MinistryContactsHandler>;
        public leadMinistrySubscription: KnockoutSubscription;
        public sharedMinistrySubscription: KnockoutSubscription;
        public numNewRequests: KnockoutObservable<number>;
        public ministryClaimInProgress: KnockoutObservable<boolean>;
        public takeOverRequestInProgress: KnockoutObservable<boolean>; // true if the user is being asked to confirm a takeover.
        public currentUi: KnockoutComputed<string>;
        public sharedMinistries: KnockoutObservableArray<string>;
        public currentUser: CurrentUserVm;
        public resolutions: KnockoutObservableArray<ResolutionVm>;
        public sharedMinsOnInit: Array<string>;
        public takeOverMinsOnInit: Array<string>;

        constructor(user: server.UserDto) {
            this.currentMediaRequest = ko.observable<MediaRequestVm>();
            this.specificEditMediaRequest = ko.observable<MediaRequestVm>();
            this.mediaRequests = ko.observableArray<KnockoutObservable<MediaRequestVm>>();
            this.allMinistries = ko.observableArray<MinistryVm>();
            this.allMinistriesExceptCurrentLead = ko.observableArray<MinistryVm>();
            this.myMinistries = ko.observableArray<MinistryVm>();
            this.users = ko.observableArray<UserVm>();
            this.userMe = ko.observable<UserVm>(new UserVm(user));

            this.currentUser = new CurrentUserVm();
            //TODO: Right now just setting a reference to the observable of userMe and ministries. 
            //userme and ministries should either be subscribed to (in case they are changed elsewhere).
            //Or refactored right out of the code. 
            this.currentUser.user = this.userMe;
            this.currentUser.userMinistries = this.myMinistries;
            this.numNewRequests = ko.observable<number>(0);
            this.ministryClaimInProgress = ko.observable<boolean>(false);
            this.takeOverRequestInProgress = ko.observable<boolean>(false);

            this.router = new MediaRequest.PvmRouter(this);
            this.pollingHandler = ko.observable<PollingHandler>(new PollingHandler(this));
            this.mediaContactHandler = ko.observable<MediaContactHandler>(new MediaContactHandler(this));
            this.navConfirm = ko.observable<NavConfirm>(new NavConfirm(this));
            this.searchHandler = ko.observable<SearchHandler>(new SearchHandler(this));
            this.reportsHandler = ko.observable<ReportsHandler>(new ReportsHandler(this));
            this.ministryContactsHandler = ko.observable<MinistryContactsHandler>(new MinistryContactsHandler(this));
            this.sharedMinistries = ko.observableArray<string>([]);
            this.sharingWithMinistries = ko.observable<boolean>(false);
            this.takeOverRequestActive = ko.observable<boolean>(false);
            this.resolutions = ko.observableArray<ResolutionVm>();

            this.currentUi = ko.pureComputed(() => {
                if (this.currentMediaRequest()) {
                    if (!this.currentMediaRequest().id()) {
                        return "new";
                    }
                    return "edit";
                }
                if (this.searchHandler().showSearchUi()) {
                    return "search";
                }
                if (this.reportsHandler().showEodReport()) {
                    return "eodreport";
                }
                if (this.ministryContactsHandler().showMinistryContactUi()) {
                    return "ministrycontacts";
                }
                if (this.reportsHandler().showEodSummaryReport()) {
                    return "eodsummaryreport";
                }
                return "dashboard";
            });

            this.pageError = ko.observable<any>();
            this.saveErrorHtml = ko.pureComputed(function () {
                if (!this.pageError() || this.mediaContactHandler().editMediaContact()) {
                    // If there's no Error or we're editing a mediaContact, do nothing.
                    return undefined;
                }
                return this.formatErrorAsHtml(this.pageError());
            }, this);


            this.isNotAcknowledged = ko.pureComputed<boolean>(function () {
                let ack: boolean = true;
                if (this.currentMediaRequest() && this.currentMediaRequest().acknowledgedAt()) {
                    ack = false;
                }
                return ack;

            }, this);

            this.singleMinistry = ko.pureComputed(() => {
                return this.myMinistries().length == 1;
            });

            this.MyMinistriesFilterText = ko.pureComputed(() => {
                return this.myMinistries().length > 1 ? "My Ministries" : "My Ministry";
            });

            this.isShared = ko.pureComputed(() => {
                return !((this.sharedMinistries().length == 0) || this.sharedMinistries().join() === this.sharedMinsOnInit.join());
            });

            this.isTakeOverRequest = ko.pureComputed(() => {
                return this.takeOverRequestActive();
            }); 

            this.canClaimMinistry = ko.pureComputed(() => {
                let allowClaim: boolean = false;
                if (this.currentMediaRequest() && this.currentMediaRequest().leadMinistry()) {
                    if (this.myMinistries().length === 1) {
                        if (this.myMinistries()[0].id() === this.currentMediaRequest().leadMinistry().id()) {
                            allowClaim = true;
                        }
                    } else {
                        // A user with multiple Ministries, should always be allowed to change the Ministry.
                        allowClaim = true;
                    }
                }
                return allowClaim;
            });

            this.currentMediaRequest.subscribe((newValue) => {

                // Cleanup any existing subscription.
                if (this.leadMinistrySubscription) {
                    this.leadMinistrySubscription.dispose();
                }

                if (this.sharedMinistrySubscription) {
                    this.sharedMinistrySubscription.dispose();
                }
               
                this.sharedMinistries([]);
                this.sharedMinsOnInit = [];

                if (newValue) {
                    //clear the ministryclaiminprogress, so we display the proper bait and switch
                    this.ministryClaimInProgress(false);
                    this.takeOverRequestInProgress(false);
                    // We need to wait until the field is displayed before we can configure autogrow.
                    setTimeout(function () {
                        var element: any = $('#dom-edit-content');
                        element.autogrow({ vertical: true, horizontal: false, flickering: false });
                    }, 0);

                    // Subscribe to changes on leadMinistry so we can 
                    // update the list of users for the ResponsibleUser select.
                    this.leadMinistrySubscription = newValue.leadMinistry.subscribe((newMinistry) => {
                        const noUsers = new Array<UserVm>();

                        this.initAllMinistriesExceptCurrentLead(newMinistry);

                        if (newMinistry) {

                            this.users(newMinistry.users);

                            // When we're changing a ministry, set the responsible user to the Current User.
                            // Loop through the users, if the current user is in the list, then set that one to be the selected user.
                            if (newMinistry.primaryContact() != null) {
                                newValue.responsibleUser(newMinistry.primaryContact());
                            }
                        } else {
                            // If the Ministry is cleared, clear the list of Ministry Users.
                            this.users(noUsers);
                            // Clear the ResponsibleUser.
                            newValue.responsibleUser(undefined);
                        }
                    });

                    this.initAllMinistriesExceptCurrentLead(newValue.leadMinistry());

                    var ministryIds: Array<string> = [];
                    for (var m = 0; m < newValue.sharedMinistries().length; m++) {
                        ministryIds.push(newValue.sharedMinistries()[m].id());
                    }
                    this.sharedMinistries(ministryIds);
                    this.sharedMinsOnInit = ministryIds;
                    this.sharingWithMinistries((ministryIds.length > 0));

                    this.sharedMinistrySubscription = this.sharedMinistries.subscribe((newValue) => {
                        var mins: Array<MinistryVm> = [];
                        for (var i = 0; i < newValue.length; i++) {
                            for (var m = 0; this.allMinistries().length; m++) {
                                if (newValue[i] === this.allMinistries()[m].id()) {
                                    mins.push(this.allMinistries()[m]);
                                    break;
                                }
                            }
                        }
                        this.currentMediaRequest().sharedMinistries(mins);
                    });
                    
                    this.takeOverRequestActive(newValue.takeOverRequestMinistry() != null);
                    if (newValue.takeOverRequestMinistry() != null)
                    {                        
                        for (var m = 0; m < this.currentUser.userMinistries().length; m++) {
                            if (newValue.takeOverRequestMinistry().id() == this.currentUser.userMinistries()[m].id()) {
                                this.takeOverRequestInProgress(true);
                            }
                        }   

                        
                        var currId = newValue.takeOverRequestMinistry().id();
                            
                        for (var i = 0; i < this.allMinistriesExceptCurrentLead().length; i++) {
                            if (currId == this.allMinistriesExceptCurrentLead()[i].id()) {
                                newValue.takeOverRequestMinistry(this.allMinistriesExceptCurrentLead()[i]);
                                break;
                            }
                        }                        
                    }                                
                }
            });
            this.setupRoutes();
        }

        private setupRoutes = () => {
            this.router.addRoute({
                id: 'all',
                url: 'all',
                windowTitle: 'Queue',
                routeInit: this.navToDashboard,
                confirmLeaveFx: (): ConfirmResult => {
                    //just save the scroll position, always return null, we can always leave
                    var retval: ConfirmResult;
                    this.listScrollPos = $(window).scrollTop();
                    return retval;
                },
                isDefault: true
            });

            this.router.addRoute({
                id: 'search',
                url: 'search',
                windowTitle: 'Search',
                routeInit: this.searchMediaRequests,
                confirmLeaveFx: null,
                pageUnload: this.searchHandler().unloadPage
            });

            this.router.addRoute({
                id: 'new',
                url: 'request/new',
                windowTitle: 'New Media Request',
                routeInit: this.addNewMediaRequest,
                confirmLeaveFx: (): ConfirmResult => {
                    //check to see if should leave
                    // if not then return an object to init the dialog
                    var retval: ConfirmResult;
                    if (this.navConfirm().pvmChangeTracker().isDirty()) {
                        retval = { discardChangesFx: this.cancelEditMediaRequest };
                    }
                    return retval;
                },
                pageUnload: this.unloadEditPage
            });

            this.router.addRoute({
                id: 'editmr',
                url: 'request/:id',
                windowTitle: 'Edit Media Request',
                routeInit: this.navToEdit,
                confirmLeaveFx: (): ConfirmResult => {
                    //check to see if should leave
                    // if not then return an object to init the dialog
                    var retval: ConfirmResult;
                    if (this.navConfirm().pvmChangeTracker().isDirty()) {
                        retval = { discardChangesFx: this.cancelEditMediaRequest };
                    }
                    return retval;
                },
                pageUnload: this.unloadEditPage
            });

            this.router.addRoute({
                id: 'editcontact',
                url: 'request/:id/contact/:id',
                windowTitle: 'Edit Media Contact',
                routeInit: this.navToMediaContact,
                confirmLeaveFx: null,
                pageUnload: this.unloadMediaContactPage
            });

            this.router.addRoute({
                id: 'editeod',
                url: 'reports/eod',
                windowTitle: 'End of Day Report',
                routeInit: this.navToEodReport,
                confirmLeaveFx: this.reportsHandler().confirmLeave,
                pageUnload: this.reportsHandler().unloadPage
            });

            this.router.addRoute({
                id: 'eodsummary',
                url: 'reports/eodsummary',
                windowTitle: 'End of Day Report - Summary',
                routeInit: this.navToEodSummaryReport,
                confirmLeaveFx: null,
                pageUnload: this.reportsHandler().unloadSummaryPage
            });

            this.router.addRoute({
                id: 'ministrycontacts',
                url: 'contacts',
                windowTitle: 'Ministry Contacts',
                routeInit: this.navToMinistryContacts,
                confirmLeaveFx: this.ministryContactsHandler().confirmLeave,
                pageUnload: this.ministryContactsHandler().unloadPage
            });
        }

        private initAllMinistriesExceptCurrentLead = (currentLead: MinistryVm) => {

            // Take a copy of All Ministries, but don't add the current LeadMinistry.
            var tmpMinistries: Array<MinistryVm> = new Array<MinistryVm>();
            for (var i = 0; i < this.allMinistries().length; i++) {
                // If there is one, it's not to include the LeadMinistry!
                if (!currentLead || this.allMinistries()[i].id() !== currentLead.id()) {
                    tmpMinistries.push(this.allMinistries()[i]);
                }
            }
            this.allMinistriesExceptCurrentLead([]);
            ko.utils.arrayPushAll<MinistryVm>(this.allMinistriesExceptCurrentLead, tmpMinistries);
        }

        /**
         * Internal PVM function to parse and format errors 
         * into something we can show in the page header.
         * @param err
         */
        public formatErrorAsHtml(err) {
            // console.log(err);
            var errorMessage: string = "Oh no! That didn't work.";
            if (err.responseJSON && err.responseJSON.message) {
                errorMessage = this.htmlEncode(err.responseJSON.message);
                if (err.responseJSON.exceptionMessage) {
                    errorMessage += ' ' + this.htmlEncode(err.responseJSON.exceptionMessage);
                }
            } else {
                if (err.status) {
                    errorMessage = "Server replied with an HTTP " + err.status + " status.";
                }
            }

            var errorTitle: string = "Error";
            if (err.statusText) {
                errorTitle = this.htmlEncode(err.statusText);
            }
            return '<strong>' + errorTitle + '</strong> : ' + errorMessage;
        }


        public removeMediaContact(mediaContact: MediaContactVm): void {
            this.navConfirm().confirm("Are you sure you want to remove that reporter?", "Remove", (result) => {
                if (result) {
                    this.currentMediaRequest().mediaContacts.remove(mediaContact);
                }
            });
        }

        public navToMediaContact = (rp: RouteParams) => {

            // Explicitly setup the MediaRequest first
            const medReqRp: RouteParams = {
                urlParams: rp.urlParams,
                data: undefined,
                isPush: false
            };
            this.navToEditWithCallback(medReqRp, () => {
                // ...then setup the Contact.
                this.initMediaContactModal(rp);
            });
        }

        public unloadMediaContactPage = () => {
            $('#editMediaContact').modal('hide');
        }

        private initMediaContactModal(rp: RouteParams): void {

            this.pageError(undefined);
            let mContact: MediaContactVm;

            const mediaContactId = rp.urlParams[1];
            if (mediaContactId) {
                if (mediaContactId == 'new') {
                    mContact = rp.data;
                } else {
                    for (var i = 0; i < this.currentMediaRequest().mediaContacts().length; i++) {
                        const mc: MediaContactVm = this.currentMediaRequest().mediaContacts()[i];
                        if (mc.id() == mediaContactId) {
                            // Found the matching Contact on the current MediaRequest!
                            mContact = mc;
                            break;
                        }
                    }
                }
            }
            if (mContact) {
                // this.mediaContactHandler().editMediaContactSelectedOutletString(undefined);
                this.mediaContactHandler().editMediaContact(mContact);
                // No dismiss on Overlay click or Esc.
                $('#editMediaContact').modal({ backdrop: "static", keyboard: false, show: true });
            }
        }

        private applyMinistryDefault(): void {
            const noUsers = new Array<UserVm>();
            this.users(noUsers);
            this.currentMediaRequest().responsibleUser.isModified(false);
            if (this.singleMinistry()) {
                if (!this.currentMediaRequest().id()) { // New record
                    //default the selected value
                    this.currentMediaRequest().leadMinistry(this.myMinistries()[0]);
                }
            }
        }

        /**
         * Given that Titles is a simple Array<string>, there's no VM for it.
         * Put the Ajax fetching method for it in here.
         */
        private findTitles = (callback: Function) => {
            $.ajax('/api/mediacontacts/titles', {
                success: function (data) {
                    callback(undefined, data);
                },
                error: function (err) {
                    callback(err);
                }
            });
        };

        private findOutlets = (callback: Function) => {
            $.ajax('/api/mediacontacts/outlets', {
                success: function (data) {
                    let dataArr: Array<MediaOutletVm> = [];
                    for (let i = 0; i < data.length; i++) {
                        let mo: MediaOutletVm = new MediaOutletVm(data[i]);
                        dataArr.push(mo);
                    }
                    callback(undefined, dataArr);
                },
                error: function (err) {
                    callback(err);
                }
            });
        };


        public initialize = (initRoute: string) => {

            // Initialize PVM observables.
            this.pageError(undefined);
            this.numNewRequests(0);

            // Start the nested callbacks to initialize our in-page data.
            ResolutionVm.findResolutions((err: any, resolutions: Array<ResolutionVm>) => {
                if (err) {
                    return this.pageError(err);
                }
                this.resolutions(resolutions);

                MinistryVm.findMinistries(this.userMe(), (err: any, allMin: Array<MinistryVm>, myMin: Array<MinistryVm>) => {
                    if (err) {
                        return this.pageError(err);
                    }
                    this.allMinistries(allMin);

                    this.myMinistries(myMin);
                    this.pollingHandler().setMyMinistries(myMin); // these are *my* Ministries

                    // Fetch the first page of MediaRequests.
                    this.pollingHandler().getFirstPage((err, data) => {
                        if (err) {
                            return this.pageError(err);
                        }

                        this.findTitles((err: any, data: Array<string>) => {
                            if (err) {
                                return this.pageError(err);
                            }
                            this.mediaContactHandler().contactTitles(data);

                            this.findOutlets((err: any, data: Array<MediaOutletVm>) => {
                                if (err) {
                                    return this.pageError(err);
                                }
                                this.mediaContactHandler().outlets(data);

                                if (initRoute) {

                                    // Brute force discard the contact portion of the url
                                    // to avoid navigating to a child route, after which our 
                                    // back button is non-functional.
                                    let urlparts = initRoute.split('/');
                                    if (urlparts.length >= 3 && urlparts[2] == 'contact') {
                                        initRoute = urlparts.splice(0, 2).join('/');
                                    }

                                    let np: NavParams = {
                                        path: initRoute,
                                        data: null
                                    };
                                    this.router.navigateTo(np);

                                }
                            });
                        }); // End of Titles

                    });
                });
            });
        }

        public initInfiniteScroll = () => {
            $(window).scroll(() => {
                if (this.currentMediaRequest()) {
                    return;
                }
                //console.log("doc: ", $(document).height(), "  win: ", $(window).height(), "  scrollPos: ", $(window).scrollTop());
                if (($(window).scrollTop() > $(document).height() - $(window).height() - 700) && ($(document).height() > 800)) { // 1500px to trigger at about the 20 last items

                    if (this.currentUi() === 'dashboard') {
                        this.pollingHandler().addNextPage();
                    } else if (this.currentUi() === 'search') {
                        // console.log("Get Next Page");
                        this.searchHandler().addNextPage();
                    }
                }
                // Make '# New Media Requests' pill disappear after scrolling to top.
                if ($(window).scrollTop() < 300) {
                    this.numNewRequests(0);

                    if (this.currentUi() === 'dashboard' && this.mediaRequests().length > 80) {
                        this.mediaRequests.splice(80);
                    }
                }
            });
        }

        public scrollToTop = () => {
            $(window).scrollTop(0);
            this.numNewRequests(0);
        }

        public navToDashboard = (rp: RouteParams) => {
            // Back to the list view
            //this.historyPush({ page: 'all' }, null, "Dashboard", "all");
            this.searchHandler().showSearchUi(false);

            if (!rp.isPush) {
                //we popped back
                setTimeout(() => {
                    $(window).scrollTop(this.listScrollPos);
                    if (this.listScrollPos > 0) {
                        this.listScrollPos = 0;
                    }}, 0); //this needs to be done after the current event is done processing.
            }
            return false;
        }

        public unloadEditPage = () => {
            this.currentMediaRequest(undefined);
        }

        public addNewMediaRequest = (rp: RouteParams) => {
            let mr: KnockoutObservable<MediaRequestVm>;

            if (rp.data) { //did we get a predefined new record from the router?
                mr = rp.data;
            } else {
                mr = ko.observable(new MediaRequestVm(this.currentUser));
            }

            this.currentMediaRequest(mr());
            this.applyMinistryDefault();
            this.navConfirm().pvmChangeTracker().markAsClean();

            this.setDefaultFocus();
            this.specificEditMediaRequest(undefined);
            return false;
        }

        public searchMediaRequests = (rp: RouteParams) => {
            this.specificEditMediaRequest(undefined);
            this.setDefaultFocus();

            if (rp.isPush) {
                this.searchHandler().setSearchString(""); // new search with reset facets
            }
            this.searchHandler().showSearchUi(true);

            return false;
        }

        public submitAndSendEditMediaRequest = () => {
            this._submitEditMediaRequest(true);
        }

        public submitEditMediaRequest = () => {
            this._submitEditMediaRequest(false);
        }

        // internal only, don't use this in a click binding.
        private _submitEditMediaRequest = (sendEmail?: boolean) => {
            this.pageError(undefined);

            this.currentMediaRequest().triggerEmail(sendEmail);

            var validatable = ko.validatedObservable(this.currentMediaRequest());
            if (validatable().isValid()) {
                $(".disable-on-busy").addClass("disabled");

                // Stop polling while we attempt a save().
                this.pollingHandler().pollingOnHold(true);

                const isNew = !this.currentMediaRequest().id();
                const isFollowup = this.currentMediaRequest().parentRequest() ? true : false;

                this.currentMediaRequest().save((err, result) => {

                    // Re-start polling after save(), regardless the result.
                    this.pollingHandler().pollingOnHold(false);
                    if (isNew) {
                        this.pollingHandler().pollForNewRequests('now');
                    } else {
                        this.pollingHandler().pollForModifiedRequests('now');
                    }

                    $(".disable-on-busy").removeClass("disabled"); // re-enable the Save button
                    if (err) {
                        return this.pageError(err);
                    }
                    // Back to where we came from 
                    this.navConfirm().pvmChangeTracker().markAsClean();
                    if (isNew && isFollowup) {
                        window.history.go(-2);
                    } else {
                        window.history.back();
                    }
                });
            }
            else {
                validatable().errors.showAllMessages(true);
            }
        }

        private htmlEncode(value): string {
            return $('<div />').text(value).html();
        }

        public cancelEditMediaRequest = () => {

            if (this.currentMediaRequest()) {
                var copyMr = this.currentMediaRequest();
                this.currentMediaRequest(undefined);
                // We need to clear the currentMediaRequest BEFORE doing cancelEdit()
                // otherwise we end up whacking the Ministry & Assigned User!
                copyMr.cancelEdit();
            }
            // Should never be a Contact if we've cancelled a MediaRequest - maybe.
            //this.mediaContactHandler().editMediaContact(undefined);

            //this.navToDashboard();
        };

        public navToEodReport = (rp: RouteParams) => {
            this.reportsHandler().showEodReport(true);
            this.reportsHandler().fetchReportData(this.currentUser);

        }
        public navToEodSummaryReport = (rp: RouteParams) => {
            this.reportsHandler().showEodSummaryReport(true);
            this.reportsHandler().fetchEodSummaryReportData();
        }

        public navToMinistryContacts = (rp: RouteParams) => {
            this.ministryContactsHandler().showMinistryContactUi(true);
            //this.ministryContactsHandler().isFetching(false);
            //this.ministryContactsHandler().isEditing(false);
            this.ministryContactsHandler().initializeUi();
            // using existing ministry data...
        }

        public navToEdit = (rp: RouteParams) => {
            const callback = () => { };
            this.navToEditWithCallback(rp, callback);
        }

        /**
         * Allows us to re-use this Async initialization when 
         * navigating to a child route of mediaRequest.
         */
        private navToEditWithCallback = (rp: RouteParams, callback: Function) => {
            if (rp.data) {
                this.navToRequest(rp.data, callback);
            } else {

                let recordIsCurrent: boolean = false;
                if (!rp.isPush) {
                    //we got here from a pop, maybe the requested record is already here
                    if (this.currentMediaRequest()) {
                        const requestIsNew = (!this.currentMediaRequest().id() && 'new' == rp.urlParams[0]);
                        const requestIdMatchesUrl = (this.currentMediaRequest().id() == rp.urlParams[0]);
                        if (requestIsNew || requestIdMatchesUrl) {
                            //just have to close the media contact dialog if it's there, and carry on
                            recordIsCurrent = true;
                        }
                    }
                }
                if (recordIsCurrent) {
                    // Tell our caller that we're finished!
                    callback();
                } else {
                    //go through the list to see if we have this ID, before we decide to go get it.
                    let data: KnockoutObservable<MediaRequestVm>;
                    ko.utils.arrayForEach<KnockoutObservable<MediaRequestVm>>(this.mediaRequests(), (mr: KnockoutObservable<MediaRequestVm>) => {
                        if (mr().id() === rp.urlParams[0]) {
                            data = mr;
                        }
                    });
                    if (data) {
                        this.navToRequest(data(), callback);

                    } else {
                        //we have to go get it.
                        MediaRequestVm.findById(rp.urlParams[0], this.currentUser, (err: any, data: KnockoutObservable<MediaRequestVm>) => {
                            if (err) {
                                return this.pageError(err);
                            }
                            if (data && data()) {
                                this.specificEditMediaRequest(data());
                                // Launch into the edit page!
                                this.navToRequest(this.specificEditMediaRequest(), callback);
                            }
                        });
                    }                   
                }
            }

        }

        private navToRequest = (req: MediaRequestVm, callback: Function) => {
            // If navigating to a MR that is NOT the specificEditMediaRequest, clear it.
            if (this.specificEditMediaRequest() && this.specificEditMediaRequest().id() !== req.id()) {
                this.specificEditMediaRequest(undefined);
            }

            // We need to populate the Pvm's users array with Users from the 
            // Ministry selected on the record we're editing.
            if (req.leadMinistry()) {
                let ministryFound: boolean = false;
                for (var i = 0; i < this.allMinistries().length; i++) {
                    if (req.leadMinistry().id() == this.allMinistries()[i].id()) {
                        ministryFound = true;
                        this.users(this.allMinistries()[i].users);

                        //now make sure that the selected user on the record is in the array.
                        let userFound: boolean = false;
                        for (var j = 0; j < this.users().length; j++) {
                            if (req.responsibleUser().id() == this.users()[j].id()) {
                                userFound = true;
                                break;
                            }
                        }
                        if (!userFound) {
                            this.users.push(req.responsibleUser()); //just add the current selected user to the array, so we can show historical data.
                        }
                        break;
                    }
                }
                if (!ministryFound) {
                    this.allMinistries().push(req.leadMinistry());
                    this.users([]);
                    this.users().push(req.responsibleUser());
                }
            }

            // Set the responsibleUser to the same object as the one in the user list to it can be re-selected.
            if (req.responsibleUser()) {
                var currId = req.responsibleUser().id();
                for (var i = 0; i < this.users().length; i++) {
                    if (currId == this.users()[i].id()) {
                        req.responsibleUser(this.users()[i]);
                        break;
                    }
                }
            }

            // Set the resolution to the same object as the one in the resolution list to it can be re-selected.
            if (req.resolution()) {
                var currId = req.resolution().id();
                for (var i = 0; i < this.resolutions().length; i++) {
                    if (currId == this.resolutions()[i].id()) {
                        req.resolution(this.resolutions()[i]);
                        break;
                    }
                }
            }
            this.currentMediaRequest(req);           

            $(window).scrollTop(0);

            // Tell our caller that we're finished!
            callback();
        }

        public setDefaultFocus = () => {
            if (this.currentUi() === 'edit') {
                $("#dom-edit-content").focus();
                return;
            }
            if (this.currentUi() === 'search') {
                $("#dom-search-text").focus();
                return;
            }
            if (this.currentUi() === 'new') {
                var reporter = $("#dom-mediaContactSearch input.searchable-combo");
                if (reporter && reporter.length === 1 && this.currentMediaRequest().mediaContacts().length === 0) {
                    reporter[0].focus();
                    $(window).scrollTop(0);
                    return;
                }
                if (this.singleMinistry()) {
                    $("#dom-edit-content").focus();
                } else {
                    $("#dom-new-ministry").focus();
                }
            }
        }

        public claimMinistry = () => {
            this.ministryClaimInProgress(true);
            // add the current ministry to the fyi list
            // this must be done BEFORE lead Ministry changes.
            //Also, first check that's it's not already there.
            if (this.sharedMinistries.indexOf(this.currentMediaRequest().leadMinistry().id()) == -1) {
                this.sharedMinistries.push(this.currentMediaRequest().leadMinistry().id());
            }
            if (this.singleMinistry()) {
                //default the selected value
                this.currentMediaRequest().leadMinistry(this.myMinistries()[0]);
            } else {
                this.currentMediaRequest().leadMinistry(undefined);
                //there is no way to open the select, which we would want to do here.
            }
            // clear out takeOver Request Ministries  
            this.currentMediaRequest().takeOverRequestMinistry(undefined);
            
            this.takeOverRequestActive(false);
            $("#dom-new-ministry").focus();
        }

        public startSharingWithMinistries = () => {
            this.sharingWithMinistries(true);
            //$("#dom-shared-ministries").focus();
            let el: any = $("#dom-shared-ministries");
            el.selectpicker('toggle');
            return false;
        }

        public sendTakeOverRequest = () => {
            this.takeOverRequestActive(true);
            let el: any = $("#dom-takeoverMinistry");
            el.focus();
            return false;
        }


        public cloneCurrentRequest = () => {
            if (this.currentMediaRequest() && this.currentMediaRequest().id()) {
                if (this.navConfirm().pvmChangeTracker().isDirty()) {
                    //this was confirmed, go ahead and cancel the edits
                    this.currentMediaRequest().cancelEdit();
                    //now we need to setup the record again as if we just got to it
                    // TODO, see if we need this statement this.navToEdit(this.currentMediaRequest(), false);
                }
                var mr: any = ko.toJS(this.currentMediaRequest());

                var newMedReq: any = {};
                newMedReq.parentRequest = JSON.parse(JSON.stringify(mr));
                // If multiple reporters clear the list, else leave it as-is.
                if (mr.mediaContacts.length === 1) {
                    newMedReq.mediaContacts = mr.mediaContacts;
                }
                newMedReq.leadMinistry = mr.leadMinistry;
                newMedReq.requestTopic = mr.requestTopic;
                newMedReq.responsibleUser = mr.responsibleUser;
                newMedReq.sharedMinistries = mr.sharedMinistries;

                const mrNew: KnockoutObservable<MediaRequestVm> = ko.observable(new MediaRequestVm(this.currentUser, newMedReq));
                mrNew().applyDefaults(); // Set requestedAt()

                let np: NavParams = {
                    path: 'request/new',
                    data: mrNew
                }

                this.router.navigateTo(np);

                this.setDefaultFocus();
                this.applyMinistryDefault();

            }
        }

        public deleteCurrentRequest = () => {
            if (this.currentMediaRequest()) {
                //First we check if there is children.
                let self = this;
                //TODO: Handle this server-side.
                $.ajax('/api/mediarequests/children/' + this.currentMediaRequest().id(), {
                    type: 'GET',
                    contentType: 'application/json',
                    success: (data) => {
                        //callback(undefined, true);
                        if (data > 0) {
                            this.navConfirm().message("This media request cannot be deleted because it is linked to other records.", () => { });
                            //self.pageError({ responseJSON: { message: "This media request cannot be deleted because it is linked to other records." }, statusText: "Delete Failed" });
                            return;
                        }
                        this.navConfirm().confirm("Are you sure you want to delete this media request?", "Delete", (result) => {
                            if (result) {
                                
                                if (self.currentMediaRequest()) {
                                    let copyMr: KnockoutObservable<MediaRequestVm>;
                                    copyMr = ko.observable(self.currentMediaRequest());

                                    
                                    //Delete the User.
                                    copyMr().delete((err, result) => {
                                        if (err) {
                                            return self.pageError(err);
                                        }
                                        //Then remove the deleted mediaRequest from our client side list. 
                                        self.removeMediaRequest(copyMr().id());
                                        // Navigate to from whence I came. 
                                         
                                        self.currentMediaRequest(undefined);
                                        window.history.back();
                                    });
                                }
                            }
                        });

                    },
                    error: function (err) {
                        return this.pageError(err);
                    }
                });
            }
        }
        public removeMediaRequest = (id: string) => {
            for (var q = 0; q < this.mediaRequests().length ; q++) {
                var mr = this.mediaRequests()[q];
                if (mr().id() == id) {
                    this.mediaRequests.splice(q, 1);
                    break;
                }
            }

            this.searchHandler().removeMediaRequest(id);
        }


    }
}
