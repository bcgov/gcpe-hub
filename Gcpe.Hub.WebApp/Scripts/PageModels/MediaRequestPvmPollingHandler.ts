
module MediaRequest {

    // **********************************************************
    // ***   PollingHandler                                   ***
    // **********************************************************
    export class PollingHandler {

        private currentPage: number;
        private currentPageSize: number = 40;
        private pollingNewInterval: number = 24000;
        private pollingModInterval: number = 19000;
        private pollingIdsInterval: number = 30000;
        public today: Date = new Date(new Date().setHours(0, 0, 0, 0)); // Updated on polling for New.

        public infiniteScrollFetching: KnockoutObservable<boolean>;

        public pollingError: KnockoutObservable<any>;
        public pollingErrorHtml: KnockoutComputed<string>;

        public pollingNewMediaRequestDate: Date;
        public pollingModifiedMediaRequestDate: Date; 

        public newRequestsTimeout: number;
        public modifyRequestsTimeout: number;
        public requestIdsTimeout: number;

        public filterMinistries: KnockoutObservable<string>;
        public filterResponded: KnockoutObservable<string>;
        private myMinistryIds: Array<string>;

        constructor(private pvm: MediaRequestPvm) {

            this.pollingNewMediaRequestDate = new Date();
            this.pollingModifiedMediaRequestDate = new Date();

            this.infiniteScrollFetching = ko.observable<boolean>(false);

            this.myMinistryIds = [];
            this.filterMinistries = ko.observable<string>("all"); // Default to "all" or "my-ministries"
            this.filterResponded = ko.observable<string>("all"); // Default to "all" or "active";

            this.pollingError = ko.observable<any>();
            this.pollingErrorHtml = ko.pureComputed(() => {
                var message: string;
                if (this.pollingError()) {
                    message = '<strong>Network is Unavailable</strong> : Check your network connection.';
                }
                return message;
            });

            this.filterMinistries.subscribe((newValue) => {
                // clear the list & fetch new!
                //console.log("ministries filter: ", newValue);
                this.getFirstPage((err, data) => {
                    if (err) {
                        return this.pvm.pageError(err);
                    }
                });
            });

            this.filterResponded.subscribe((newValue) => {
                //console.log("responded filter: ", newValue);
                // clear the list & fetch new!
                this.getFirstPage((err, data) => {
                    if (err) {
                        return this.pvm.pageError(err);
                    }
                });
            });
        } // End of constructor

        private getFilterState(): FindRequestsOptions {
           
            let options: FindRequestsOptions = {};
            options.requestsToday = this.today;

            if (this.filterMinistries() === 'my-ministries') {
                options.ministries = this.myMinistryIds;
            }
            if (this.filterResponded()) {
                options.responded = this.filterResponded();
            }


            return options;
        }

        public setMyMinistries(ministries: Array<MinistryVm>) {
            this.myMinistryIds = [];
            for (var i = 0; i < ministries.length; i++) {
                this.myMinistryIds.push(ministries[i].id());
            }
        }

        public pollingOnHold(hold: boolean) {
            var doPolling: string = (hold ? 'none' : 'next');
            this.pollForNewRequests(doPolling);
            this.pollForModifiedRequests(doPolling);
            this.pollForRequestIds(doPolling);
        }

        public pollForNewRequests(doPolling: string = 'now') {

            // Always clear the timeout
            clearTimeout(this.newRequestsTimeout);

            if (doPolling === 'next') {
                this.newRequestsTimeout = setTimeout(() => { this.pollForNewRequests(); }, this.pollingNewInterval);
                return;
            }
            if (doPolling === 'now') {

                if (this.today.getTime() !== new Date(new Date().setHours(0, 0, 0, 0)).getTime()) {
                    // Rolled over the day, do a full-page reload.
                    window.location.reload();
                }

                // Get the data
                var options: FindRequestsOptions = this.getFilterState();
                var newestMediaRequest = this.pvm.mediaRequests()[0]();
                options.requestsAfterDate = new Date(newestMediaRequest.createdAt().getTime() + 1);

                MediaRequestVm.find(options, this.pvm.currentUser, (err, data) => {

                    this.pollingError(err); // undefined or an Error, set it either way.

                    if (!err) {
                        if (data && data.length > 0) {
                            for (var i = 0; i < data.length; i++) {
                                data[i]().modifiedStyle("new-animate");
                                this.pvm.mediaRequests.splice(i, 0, data[i]);
                            }
                            if ($(window).scrollTop() > 300) {
                                var num = this.pvm.numNewRequests() + data.length;
                                this.pvm.numNewRequests(num);
                            }
                        }
                        // *************************************
                        // Check for duplicates!
                        // *************************************
                        var uq = {};
                        for (var i = 0; i < this.pvm.mediaRequests.length; i++) {
                            var mr = this.pvm.mediaRequests()[i];
                            var id = mr().id();
                            if (uq[id]) {
                                console.log('Duplicate at index:' + i, mr());
                            } else {
                                uq[id] = true;
                            }
                        }
                        // *************************************
                    }
                    // Setup the next timeout
                    this.newRequestsTimeout = setTimeout(() => { this.pollForNewRequests(); }, this.pollingNewInterval);
                });
            }
        }

        public pollForRequestIds(doPolling: string = 'now') {

            // Always clear the timeout
            clearTimeout(this.requestIdsTimeout);

            if (doPolling === 'next') {
                this.requestIdsTimeout = setTimeout(() => { this.pollForRequestIds(); }, this.pollingIdsInterval);
                return;
            }
            if (doPolling === 'now') {

                // Get the data using the same Criteria as polling for modified.
                var options: FindRequestsOptions = this.getFilterState();
                var newestMediaRequest = this.pvm.mediaRequests()[0]();
                var oldestMediaRequest = this.pvm.mediaRequests()[this.pvm.mediaRequests().length - 1]();

                options.newestMediaRequestDate = new Date(newestMediaRequest.createdAt().getTime() + 1);
                options.requestsAfterDate = new Date(oldestMediaRequest.createdAt().getTime() - 1); // -1 to include the last one we have!

                MediaRequestVm.findIdsOnly(options, this.pvm.currentUser, (err, data) => {

                    this.pollingError(err); // undefined or an Error, set it either way.

                    if (!err) {

                        if (data && data.length > 0) {

                            // Go through the list of mediaRequest we have in page,
                            // remove the ones not in the list if Ids.
                            for (var q = this.pvm.mediaRequests().length-1; q > -1; q--) {
                                var mr = this.pvm.mediaRequests()[q];
                                var id = mr().id();
                                if (data.indexOf(id) === -1) {
                                    // This mediaRequest should be removed!
                                    this.pvm.mediaRequests.splice(q, 1);
                                }
                            }
                        }
                    }
                    // Setup the next timeout
                    this.requestIdsTimeout = setTimeout(() => { this.pollForRequestIds(); }, this.pollingIdsInterval);
                });
            }
        }

        public pollForModifiedRequests(doPolling: string = 'now') {

            // Always clear the timeout
            clearTimeout(this.modifyRequestsTimeout);

            if (doPolling === 'next') {
                this.modifyRequestsTimeout = setTimeout(() => { this.pollForModifiedRequests(); }, this.pollingModInterval);
                return;
            }
            if (doPolling === 'now') {
                //Get the modified Media Request Data.
                var newestMediaRequest, oldestMediaRequest;

                var options: FindRequestsOptions = this.getFilterState();
                options.modifiedAfterDate = this.pollingModifiedMediaRequestDate;
                // Any new requests appear that satisfy the filter criteria, then they should come through the new poll.
                if (this.pvm.mediaRequests().length > 0) {
                    newestMediaRequest = this.pvm.mediaRequests()[0]();
                    options.newestMediaRequestDate = new Date(newestMediaRequest.createdAt().getTime() + 1);

                    oldestMediaRequest = this.pvm.mediaRequests()[this.pvm.mediaRequests().length - 1]();
                    options.requestsAfterDate = oldestMediaRequest.createdAt();
                }

                MediaRequestVm.find(options, this.pvm.currentUser, (err, data) => {

                    this.pollingError(err); // undefined or an Error, set it either way.

                    if (!err) {
                        if (data && data.length > 0) {
                            var requiresSorting = 0;
                            for (var j = 0; j < data.length; j++) {
                                var modifiedMr: KnockoutObservable<MediaRequestVm> = data[j];
                                var foundMr = false;
                                for (var i = 0; i < this.pvm.mediaRequests().length; i++) {

                                    var request: any = this.pvm.mediaRequests()[i];
                                    if (request().id() == modifiedMr().id()) {
                                        modifiedMr().modifiedStyle("modified-animate");
                                        this.pvm.mediaRequests.splice(i, 1, modifiedMr);
                                        foundMr = true;
                                        break;
                                    }
                                }
                                if (this.pollingModifiedMediaRequestDate < modifiedMr().modifiedAt()) {
                                    // The Server stores & returns dates with fractional milliseconds. 
                                    // The API call recieves ISOString date which goes down to the millisecond only.  
                                    // By adding a single millisecond, we prevent constantly refreshing updates that include partial milliseconds. 
                                    this.pollingModifiedMediaRequestDate = new Date((modifiedMr().modifiedAt().getTime() + 1));
                                }
                                if (!foundMr) {
                                    modifiedMr().modifiedStyle("modified-animate");
                                    this.pvm.mediaRequests.push(modifiedMr);
                                    requiresSorting++;


                                }
                            }
                            // If we stuffed anything on the end, force re-sorting the Array
                            if (requiresSorting > 0) {                                
                                this.pvm.mediaRequests.sort(function (a: KnockoutObservable<MediaRequestVm>, b: KnockoutObservable<MediaRequestVm>) {
                                    if (a().createdAt() < b().createdAt()) { return 1; }
                                    if (a().createdAt() > b().createdAt()) { return -1; }  //descending sort
                                    return 0;
                                });
                            }
                        }
                    }
                    // Setup the next check for modified timeout
                    this.modifyRequestsTimeout = setTimeout(() => { this.pollForModifiedRequests(); }, this.pollingModInterval);
                });
                return;
            }
        }

        public getFirstPage(callback: Function) {
            // this.pvm.mediaRequests([]);
            this.pvm.mediaRequests.removeAll();

            this.currentPage = 0;

            //reset all the dates.
            // this.mostRecentMediaRequestDate = null;
            this.pollingNewMediaRequestDate = null;
            this.pollingModifiedMediaRequestDate = null;

            return this.addNextPage((err, data) => {
                if (data && data.length > 0) {
                    // Figure out the dates to be used for fetching New & Modified MediaRequests.
                    for (var i = 0; i < data.length; i++) {
                        if (i === 0) {
                            // this.mostRecentMediaRequestDate = data[i]().createdAt();
                            this.pollingNewMediaRequestDate = new Date((data[i]().createdAt().getTime() + 1));
                            this.pollingModifiedMediaRequestDate = new Date((data[i]().modifiedAt().getTime() + 1));
                        } else {
                            if (this.pollingModifiedMediaRequestDate < data[i]().modifiedAt()) {
                                this.pollingModifiedMediaRequestDate = new Date((data[i]().modifiedAt().getTime() + 1));
                            }
                        }
                    }
                    // Start delayed polling at the regular intervals.
                    this.pollForNewRequests('next');
                    this.pollForModifiedRequests('next');
                    this.pollForRequestIds('next');
                }
                callback(err, data);
            });
        }

        public addNextPage(callback?: Function) {

            if (this.currentPage == -1 || this.infiniteScrollFetching() || this.pollingError()) {
                // No more records, already fetching or the request failed.
                return;
            }

            let t: any = $("#dom-toggle-active-filter");
            t.bootstrapToggle('disable');
            t = $("#dom-toggle-ministry-filter");
            t.bootstrapToggle('disable');
            this.infiniteScrollFetching(true);

            setTimeout(() => {

                let options = this.getFilterState();
                // options.skip = (this.currentPage * this.currentPageSize);
                options.skip = this.pvm.mediaRequests().length; // Consider that items may have been removed!
                options.limit = this.currentPageSize;
                // options.mostRecentMediaRequestDate = this.mostRecentMediaRequestDate;
                

                MediaRequestVm.find(options, this.pvm.currentUser, (err: any, data: Array<KnockoutObservable<MediaRequestVm>>) => {
                    this.pollingError(err); // Always set or clear an err
                    if (data) {

                        ko.utils.arrayPushAll<KnockoutObservable<MediaRequestVm>>(this.pvm.mediaRequests, data);

                        // If data.length == 0, There's no more pages to fetch!
                        // currentPage = -1 will disable the continuous fetching of no data.
                        this.currentPage = data.length ? this.currentPage + 1 : -1;
                    }

                    $("#filter-buttons").removeClass("disabled-content");
                    this.infiniteScrollFetching(false);
                    let t: any = $("#dom-toggle-active-filter");
                    t.bootstrapToggle('enable');
                    t = $("#dom-toggle-ministry-filter");
                    t.bootstrapToggle('enable');

                    if (callback) {
                        callback(err, data);
                    }
                });
            }, 200); // limit how fast we trigger infinite scrolling API calls.           
        }

    }  // End of PollingHandler
}