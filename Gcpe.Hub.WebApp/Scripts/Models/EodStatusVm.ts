class EodStatusVm {
    public ministry: KnockoutObservable<MinistryVm>;
    public lastActivity: KnockoutObservable<Date>;
    public eodState: KnockoutComputed<string>;
    public eodStateText: KnockoutComputed<string>;

    constructor(data: server.EodStatusDto) {
        this.lastActivity = ko.observable(undefined);
        this.ministry = ko.observable(new MinistryVm(data.ministry));

        if (data.lastActivity)
        {
            this.lastActivity(new Date(data.lastActivity));
        }

        //Keep MediaRequestsApiController.AutoSendEodSummary logic in sync with this computed
        this.eodState = ko.pureComputed<string>(() => {
            let retval: string = "";
            if (!this.lastActivity()) {
                retval = "OK";
            } else {
                retval = "ERR";
                if (this.ministry() && this.ministry().eodLastRunDateTime() && (this.lastActivity() < this.ministry().eodLastRunDateTime())) {
                    //AND that lastRunDate was today, then we are good.
                    var nowDtStr: string = moment(new Date()).format("YYYY-MM-DD");
                    var lrd: moment.Moment = moment(this.ministry().eodLastRunDateTime());
                    if (nowDtStr == lrd.format("YYYY-MM-DD")) {
                        retval = "OK";
                    }
                }
            }
            return retval;
        });


        this.eodStateText = ko.pureComputed<string>(() => {
            let retval: string = "";
            if (this.eodState() === "OK") {
                retval = "No new media requests";
            } else {
                
                if (this.ministry() && this.ministry().eodLastRunDateTime()) {
                    var nowDtStr: string = moment(new Date()).format("YYYY-MM-DD");
                    var lrd: moment.Moment = moment(this.ministry().eodLastRunDateTime());
                    if (nowDtStr == lrd.format("YYYY-MM-DD")) {
                        //go get the user who ran it
                        retval = "Run by " + this.ministry().eodLastRunUser().displayAs() + " today (" + lrd.fromNow() + ")";
                    } else {
                        retval = "Run " + lrd.calendar(null, {
                            sameDay: '[today at] LT',
                            nextDay: '[tomorrow at] LT',
                            nextWeek: 'dddd [at] LT',
                            lastDay: '[yesterday at] LT ',
                            lastWeek: '[last] dddd [at] LT',
                            sameElse: 'dddd, MMM D, h:mm A'
                        });
                    }
                }
                else {
                    retval = "Never been generated";
                }

            }

            return retval;
        });

        
    }

    public static findEodReportSummaryMinistries(callback: Function): void {
        $.ajax('/api/ministries/eodsummary', {
            success: function (data) {

                var allMinArr = [];

                for (var i = 0; i < data.length; i++) {
                    var minrec = new EodStatusVm(data[i]);
                    allMinArr.push(minrec);
                };
                callback(undefined, allMinArr);
            },
            error: function (err) {
                callback(err);
            }
        });
    }
}