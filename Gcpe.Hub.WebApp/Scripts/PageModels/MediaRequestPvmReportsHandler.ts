module MediaRequest {

    //interface MediaContact {
    //    firstname: string;
    //    lastname: string;
    //    cellPhone: string;
    //    title: string;
    //}

    //interface EodMediaRequest {
    //    requestTopic: string;
    //    requestedAt: string;
    //    requestContent: string;
    //    mediaContacts: Array<MediaContact>;
    //    value1: boolean;
    //    value2: boolean;

    //}
    // **********************************************************
    // ***   ReportsHandler                                   ***
    // **********************************************************
    export class ReportsHandler {

        public showEodReport: KnockoutObservable<boolean>;
        public showEodSummaryReport: KnockoutObservable<boolean>;
        public eodMediaRequests: KnockoutObservableArray<KnockoutObservable<MediaRequestVm>>;
        public eodSummaryMinistries: KnockoutObservableArray<EodStatusVm>;
        public fetchingData: KnockoutObservable<boolean>;
        private evergreenTimeout: number;

        constructor(private pvm: MediaRequestPvm) {
            this.showEodReport = ko.observable<boolean>(false);
            this.showEodSummaryReport = ko.observable<boolean>(false);
            this.eodMediaRequests = ko.observableArray([]);
            this.eodSummaryMinistries = ko.observableArray([]);
            this.fetchingData = ko.observable<boolean>(false);
        }

        public unloadPage = () => {
            this.showEodReport(false);
        }

        public unloadSummaryPage = () => {
            this.showEodSummaryReport(false);
            clearTimeout(this.evergreenTimeout);
        }

        public confirmLeave = (): ConfirmResult => {
            let ret: ConfirmResult;
            let changed: number = 0;
            ko.utils.arrayForEach(this.eodMediaRequests(), (mr: KnockoutObservable<MediaRequestVm>) => {
                if (mr().eodReportWith.isModified()) {
                    changed++;
                };
            });
            if (changed > 0) {
                ret = {discardChangesFx: this.unloadData};
            }
            return ret;
        }

        private unloadData = () => {
            this.eodMediaRequests([]);
        }

        

        public fetchReportData = (user: CurrentUserVm) => {
            this.eodMediaRequests([]);
            this.fetchingData(true);

            MediaRequestVm.getOpenMediaRequests(user, (err, data: Array<KnockoutObservable<MediaRequestVm>>) => {
                this.fetchingData(false);
                if (err) {
                    this.pvm.pageError(err);
                    return;
                }
                for (let i = 0; i < data.length; i++) {
                    let mr: KnockoutObservable<MediaRequestVm> = data[i];
                    mr().eodReportWith.extend({
                        required: {
                            message: "The Eod 'Report With' value is required"
                        }
                    });
                    mr().eodReportWith.clearError();
                    mr().eodReportWith.isModified(false);
                }
                ko.utils.arrayPushAll<KnockoutObservable<MediaRequestVm>>(this.eodMediaRequests, data);
            });
        }

        public fetchEodSummaryReportData = () => {
            this.fetchingData(true);
            EodStatusVm.findEodReportSummaryMinistries((err, data) => {
                this.fetchingData(false);
                if (err) {
                    this.pvm.pageError(err);
                    return;
                }
                this.eodSummaryMinistries([]);
                ko.utils.arrayPushAll<EodStatusVm>(this.eodSummaryMinistries, data);
            });

            this.evergreenTimeout = setTimeout(() => {this.fetchEodSummaryReportData()}, 30000)
        }

        public collectAndSubmit = () => {
            //make sure all rows in the page have a value for eodWith
            let goodToGo: boolean = true;
            ko.utils.arrayForEach(this.eodMediaRequests(), (mr: KnockoutObservable<MediaRequestVm>) => {
               
                if (!(mr().eodReportWith())) {
                    // Set any error to highlight the border. The record is not modified, so validations don't fire automatically.
                    mr().eodReportWith.setError("Required"); 
                    goodToGo = false;
                    this.pvm.pageError({ responseJSON: { message: "Please select a value for every row" }, statusText: "Incomplete" });
                    this.pvm.scrollToTop();
                };
                mr().errors.showAllMessages();

            });

            if (goodToGo) {
                $(".disable-on-busy").addClass("disabled");
                MediaRequestVm.postEndOfDayUpdates(this.eodMediaRequests, (err, result) => {

                    $(".disable-on-busy").removeClass("disabled");
                    if (err) {
                        return this.pvm.pageError(err)
                    }
                    this.unloadData();
                    window.history.back();

                    
                })
            }
        }


        public collectAndSubmitSummary = () => {

            $(".disable-on-busy").addClass("disabled");
            MediaRequestVm.postEndOfDaySummary( (err, result) => {

                $(".disable-on-busy").removeClass("disabled");
                if (err) {
                    return this.pvm.pageError(err)
                }
                window.history.back();
               
            })
        }

        
    }
}