module MediaRequest {

    // **********************************************************
    // ***   SearchHandler                                    ***
    // **********************************************************
    export class SearchHandler {

        public searchResults: KnockoutObservableArray<KnockoutObservable<MediaRequestVm>>;
        public facets: KnockoutObservableArray<server.FacetDto>;
        private zeroResults: KnockoutObservable<boolean>;
        public searchString: KnockoutObservable<string>;
        private currentPage: number;
        public showSearchUi: KnockoutObservable<boolean>;
        public searchActive: KnockoutObservable<boolean>;
        public isFetching: KnockoutObservable<boolean>;
        public searchingError: KnockoutObservable<any>;
        public searchingErrorHtml: KnockoutComputed<string>;
        private searchResultKeys: any;
        private oldsearchstring: string;

        constructor(private pvm: MediaRequestPvm) {
            this.searchResults = ko.observableArray<KnockoutObservable<MediaRequestVm>>();
            this.facets = ko.observableArray<server.FacetDto>();
            this.showSearchUi = ko.observable<boolean>(false); // Show the Search UI.
            this.isFetching = ko.observable<boolean>(false); // Actively waiting on the API call.
            this.zeroResults = ko.observable<boolean>(false);
            this.searchActive = ko.observable<boolean>(false);
            this.searchString = ko.observable<string>();
            this.searchResultKeys = {};

            this.searchingError = ko.observable<any>();
            this.searchingErrorHtml = ko.pureComputed(() => {
                var message: string;
                if (this.searchingError()) {
                    message = '<strong>Network is Unavailable</strong> : Check your network connection.';
                }
                return message;
            });

            this.searchString.subscribe((newValue) => {
                //console.log("newValue", newValue);
                this.zeroResults(false);
            });

        } // End of constructor

        public doSearch = () => {
            var searchStr = this.searchString().trim();

            // if we have a new search string we need to clear the facets array
            if (this.oldsearchstring != searchStr) {
                this.facets([]);
                this.oldsearchstring = searchStr;
            }

            if (searchStr.length < 3) {
                this.pvm.navConfirm().message("That's not much to search with, please add more", () => { });
                return;
            }

            this.setSearchString(searchStr);

            this.getFirstPage((err, results) => {
                if (results && (!results.mediaRequests || results.mediaRequests.length === 0)) {
                    // If no results. show a message.
                    this.zeroResults(true);
                }
            });
            return false;
        }
        public setSearchString = (searchStr: string) => {
            if (searchStr.length == 0) {
                this.facets([]);
            }
            this.searchString(searchStr);
            this.searchResults([]);  // Clear any current results.
            this.searchResultKeys = {}; // discard search result keys
            this.currentPage = 0;
            this.isFetching(false); // Clear the busy indicator.
            this.zeroResults(false);
            this.searchingError(undefined);
        }

        public getFirstPage(callback?: Function) {
            this.searchActive(true);
            return this.addNextPage(callback);
        }

        public addNextPage(callback?: Function) {

            if (this.currentPage == -1 || this.isFetching() || this.searchingError() || (!this.searchActive())) {
                // No more records, already fetching or the request failed.
                //console.log("add page: nope");
                return;
            }

            this.isFetching(true);

            var filters = {};
            this.facets().forEach(facet => {
                for (var fi = 0; fi < facet.filters.length; fi++) {
                    const filter: any = facet.filters[fi];
                    if (filter.isChecked) {
                        filters[facet.name] = filter.name;
                    }
                }
            });


            //console.log("add page: getting data");

            $.ajax({
                url: '/api/mediarequests/search',
                type: "get",
                data: $.extend(filters, { query: this.searchString(), page: this.currentPage}),
                traditional: true,
                success: (results) => {

                    if (results) {
                        if (this.currentPage == 0) {
                            // restore user's filters selections
                            for (var fa = 0; fa < results.facets.length; fa++) {
                                var facet = results.facets[fa];
                                if (facet.name == "leadMinistryDisplayName")
                                    facet.displayName = "Ministry";
                                else if (facet.name == "companyNames")
                                    facet.displayName = "Outlet";
                                else if (facet.name == "contactNames")
                                    facet.displayName = "Contact";
                                else facet.displayName = facet.name;

                                for (var fi = 0; fi < facet.filters.length; fi++) {
                                    const filter = facet.filters[fi];
                                    filter.isChecked = filters[facet.name] === filter.name;
                                }
                                var selectedFilters = facet.filters.filter(filter => filter.isChecked);
                                if (selectedFilters.length != 0) {
                                    // remove confusing extra filters (i.e other reporters belonging to the media requests)
                                    facet.filters = selectedFilters
                                }
                            }
                            this.facets(results.facets)
                            window['snowplow']('trackSiteSearch', this.searchString().split(' '), filters);
                        }
                        if (results.mediaRequests) {
                            // If results.length == 0, There's no more pages to fetch!
                            // currentPage = -1 will disable the continuous fetching of no results.
                            this.currentPage = results.mediaRequests.length ? this.currentPage + 1 : -1;

                            var mrArr = [];
                            for (var i = 0; i < results.mediaRequests.length; i++) {
                                const mReq = new MediaRequestVm(this.pvm.currentUser, results.mediaRequests[i]);

                                // Handle duplicates that can happen after new records are added to the Azure index
                                if (!this.searchResultKeys[mReq.id()]) {
                                    this.searchResultKeys[mReq.id()] = true;
                                    const mcvm = ko.observable(mReq);
                                    mrArr.push(mcvm);
                                }
                            }
                            ko.utils.arrayPushAll(this.searchResults, mrArr);
                        }
                    }

                    this.isFetching(false);
                    if (callback) {
                        callback(undefined, results);
                    }
                },
                error: (err) => {
                    this.searchingError(err);
                    this.isFetching(false);
                    if (callback) {
                        callback(err);
                    }
                }
            });
        }

        public unloadPage = () => {
            this.showSearchUi(false);
            this.searchActive(false);
        }

        public removeMediaRequest = (id: string) => {
            for (var q = 0; q < this.searchResults().length; q++) {
                var mr = this.searchResults()[q];
                if (mr().id() == id) {
                    this.searchResults.splice(q, 1);
                    break;
                }
            }

        }

        public displayFacetUI = () => {
            return this.searchResults().length > 0 && window.innerWidth > 768;
        }
    } // End of SearchHandler
}