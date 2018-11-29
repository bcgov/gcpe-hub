module MediaRequest {

    export interface ChangeTracker {
        isDirty: KnockoutComputed<boolean>;
        markAsClean();
        reInit(newObj: MediaRequestVm);
    }

    export interface ConfirmResult {
        title?: string;
        text?: string;
        yesLabel?: string;
        noLabel?: string;
        hideNoButton?: boolean;
        discardChangesFx: Function;
    }

    // **********************************************************
    // ***   Navigation Confirmation                          ***
    // **********************************************************
    export class NavConfirm {
        public dialogTitle: KnockoutObservable<string>;
        public dialogText: KnockoutObservable<string>;

        public yesLabel: KnockoutObservable<string>;
        public noLabel: KnockoutObservable<string>;
        public noButtonVisible: KnockoutObservable<boolean>;

        private forwardNav: NavParams;
        private confirmResult: ConfirmResult;

        public pvmChangeTracker: KnockoutObservable<ChangeTracker>;
        private independentConfirmActive: boolean = false;

        constructor(private pvm: MediaRequestPvm) {
            this.dialogTitle = ko.observable<string>();
            this.dialogText = ko.observable<string>();

            this.yesLabel = ko.observable<string>();
            this.noLabel = ko.observable<string>();
            this.noButtonVisible = ko.observable<boolean>(true);

            this.pvmChangeTracker = ko.observable(this.getChangeTracker(null));
        }

        public yesResponse() {
            this.pvm.router.modalActive = false;
            if (this.independentConfirmActive) {
                this.independentConfirmActive = false;
                this.confirmResult.discardChangesFx(true);

            } else {
                if (this.confirmResult && this.confirmResult.discardChangesFx) {
                    this.confirmResult.discardChangesFx();
                }

                if (this.forwardNav) {
                    this.forwardNav.confirmed = true;
                    this.pvm.router.navigateTo(this.forwardNav);
                } else {
                    this.pvm.router.confirmedNavBack();
                }

                this.forwardNav = undefined;
                this.confirmResult = undefined
            }
            
           
        }

        public noResponse() {
            this.pvm.router.modalActive = false;
            if (this.independentConfirmActive) {
                this.independentConfirmActive = false;
                this.confirmResult.discardChangesFx(false);

            }
            this.confirmResult = undefined;
           
        }

        // We should refactor this to place this function in a base class
        public displayModal(confResult: ConfirmResult): void {
            //initialize the observables for the title and text
            this.dialogTitle(confResult.title || "Discard changes?");
            this.dialogText(confResult.text || "If you leave this page your changes will not be saved.");
            this.yesLabel(confResult.yesLabel || "Discard changes");
            this.noLabel(confResult.noLabel || "Continue editing");
            
            this.noButtonVisible(!confResult.hideNoButton);
           

            //show the dialog
            this.pvm.router.modalActive = true;
           
            $("#confirmNav").modal({ backdrop: "static", keyboard: false, show: true }); //no esc, skirt click, esc etc.
        }

        public confirmingNavWithModal(np: NavParams, fromState: Route): boolean {
            var retval: boolean = false;
            if (!np.confirmed && fromState && fromState.confirmLeaveFx) {
                this.confirmResult = fromState.confirmLeaveFx();
                if (this.confirmResult) {

                    if (!np.isPop) {
                        this.forwardNav = np;
                    }

                    this.displayModal(this.confirmResult);
                    retval = true;
                }
            }
            return retval;
        }

        private getChangeTracker(mrToTrack: KnockoutObservable<MediaRequestVm>): ChangeTracker {

            function hashCode(inputStr: string): number {
                var hash = 0, i, chr, len;
                if (inputStr.length === 0) return hash;
                for (i = 0, len = inputStr.length; i < len; i++) {
                    chr = inputStr.charCodeAt(i);
                    hash = ((hash << 5) - hash) + chr;
                    hash |= 0; // Convert to 32bit integer
                }
                return hash;
            };

            function sanitizeJS(obj: any, recurseLevel: number = 0): any {
                var ret: any = {};

                if (typeof obj === 'string') return obj;
                if (typeof obj === 'boolean') return obj;
                if (typeof obj === 'number') return obj;
                if (typeof obj === 'object' && obj && obj.constructor === Array) {
                    var retArr = [];
                    for (var i = 0; i < obj.length; i++) {
                        retArr.push(sanitizeJS(obj[i], (recurseLevel + 1)));
                    }
                    return retArr;
                }
                if (typeof obj === 'object' && obj && obj.constructor === Date) {
                    
                    return obj.toISOString();
                }

                //at this point obj should be an object, if the recurseLevel > 0 all we need to return is the id
                if (recurseLevel > 0) {
                    if (obj && obj.id) {
                        // One-off case to address the nesting of an Outlet within a Job.
                        // We *need* to know if the OutletId is dirty, even when the JobId is not.
                        if (obj.job && obj.job.id && obj.job.outlet && obj.job.outlet.id) {
                            var jobId: string = obj.id + "/" + obj.job.id + "/" + obj.job.outlet.id;
                            return jobId;
                        } else {
                           return obj.id;
                        }
                    } else {
                        return undefined;
                    }
                }

                for (let key in obj) {
                    //console.log("key: ", key, " type: ", (typeof obj[key]));
                    if (obj.hasOwnProperty(key)){
                        if (typeof obj[key] === 'object') {
                            if (obj[key] && obj[key].constructor === Array) {
                                //console.log(key, " is an array ");
                                let arr = obj[key];
                                ret[key] = [];
                                for (var i = 0; i < arr.length; i++) {
                                    ret[key].push(sanitizeJS(arr[i], (recurseLevel + 1)));
                                }
                            } else {
                                ret[key] = sanitizeJS(obj[key], (recurseLevel + 1));
                            }
                        } else if (typeof obj[key] === 'function') {
                            //not doing that
                        } else {
                            //console.log("setting: ", key, "of type ", (typeof obj[key]));
                            ret[key] = obj[key];
                        }
                    }
                }

                return ret;
            }

            function getObjectState(obj: MediaRequestVm): number {
                let objJS: any;
                let mr = ko.toJS(obj);
                objJS = sanitizeJS(mr);

                return hashCode(JSON.stringify(objJS));
            }

            let objectToTrack: KnockoutObservable<MediaRequestVm>;
            if (! mrToTrack) {
                objectToTrack = ko.observable<MediaRequestVm>();
            }
            let lastClean = sanitizeJS(ko.toJS(objectToTrack));
            const lastCleanState = ko.observable(getObjectState(objectToTrack()));



            var retval: ChangeTracker = {
                isDirty: ko.pureComputed(function () {
                    // console.log("isDirty Called depcount: ", ko.computedContext.getDependenciesCount());
                    const current = getObjectState(objectToTrack());
                    const currentObj = sanitizeJS(ko.toJS(objectToTrack)); 
                    const isDirtyResult: boolean = current != lastCleanState();

                    // FYI: If a user clicks New or Edit, and changes one value, then hits the 
                    // browser back button without leaving the field, isDirty remains False! (does not happen with the textInput binding)
                    // This happens because the bound Observable has not yet been updated 
                    // and the ViewModel is therefore not considered Dirty.

                    // Save for future debugging.
                    //if (isDirtyResult) {
                    //    console.log("isDirty, lastClean", lastClean);
                    //    console.log("isDirty,   current", currentObj);
                    //}

                    return isDirtyResult;
                }),
                markAsClean: function () {
                    lastCleanState(getObjectState(objectToTrack()));
                },

                reInit: function (newObj: MediaRequestVm) {
                    objectToTrack(newObj);
                    //lastClean = sanitizeJS(ko.toJS(objectToTrack)); //for debug only
                    lastCleanState(getObjectState(objectToTrack()));
                }
            };
            this.pvm.currentMediaRequest.subscribe((newValue) => {
                retval.reInit(newValue);
            });
            //console.log("Init depcount: ", ko.computedContext.getDependenciesCount());
            return retval;
        }

        private  initMRChangeTracker(mediaRequest: MediaRequestVm): void {
           // this.MRChangeTracker = this.initChangeTracker(mediaRequest);
            this.pvmChangeTracker().reInit(mediaRequest);
        }
       

        public confirmPageUnload(): string {
            var retval: string;
            if (this.pvmChangeTracker().isDirty()) {
                retval = "Changes you made will not be saved";
            }

            return retval;
        }

        public confirm = (message: string, yesLabel: string, callback: Function) => {
            let cr: ConfirmResult = {
                title: "Please Confirm",
                text: message,
                yesLabel : yesLabel,
                noLabel : "Cancel",
                discardChangesFx: callback //careful here, we're re-using the discardChangesFx to hold the callback of the confirm!!!
            }
            this.independentConfirmActive = true;
            this.confirmResult = cr;
            this.displayModal(cr);
        }

        public message = (message: string, callback: Function) => {
            let cr: ConfirmResult = {
                title: "Please Confirm",
                text: message,
                yesLabel: "OK",
                noLabel: "Cancel",
                hideNoButton: true,
                discardChangesFx: callback //careful here, we're re-using the discardChangesFx to hold the callback of the confirm!!!
            }
            this.independentConfirmActive = true;
            this.confirmResult = cr;
            this.displayModal(cr);
        }
       
    }
}
