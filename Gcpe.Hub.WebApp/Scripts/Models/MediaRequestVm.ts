interface FindRequestsOptions {
    responded?: string;
    ministries?: Array<string>;
    skip?: number;
    limit?: number;
    newestMediaRequestDate?: Date; // newest (createdAt)
    requestsToday?: Date;       // Today according to the client.
    requestsAfterDate?: Date;
    modifiedAfterDate?: Date;
    raw?: boolean;
}

interface SystemUser {
    id: string,
    displayAs: string
}

interface OrigFunctionMR extends Function {
    value?: server.MediaRequestDto;
}

class MediaRequestVm {
    private origVal: OrigFunctionMR; //hide the value under a function so it's excluded from serializing to json.
    public id: KnockoutObservable<string>;
    public subject: KnockoutObservable<string>;
    public content: KnockoutObservable<string>;

    public createdAt: KnockoutObservable<Date>;
    public createdBy: KnockoutObservable<SystemUser>;
    public modifiedAt: KnockoutObservable<Date>;
    public modifiedBy: KnockoutObservable<SystemUser>;

    public leadMinistry: KnockoutObservable<MinistryVm>;
    public responsibleUser: KnockoutObservable<UserVm>;
    public mediaContacts: KnockoutObservableArray<MediaContactVm>;
    public sharedMinistries: KnockoutObservableArray<MinistryVm>;
    public takeOverRequestMinistry: KnockoutObservable<MinistryVm>;
    public deadlineAt: KnockoutObservable<Date>;
    public requestTopic: KnockoutObservable<string>;
    public requestContent: KnockoutObservable<string>;
    public requestedAt: KnockoutObservable<Date>;
    public acknowledgedAt: KnockoutObservable<Date>;
    public respondedAt: KnockoutObservable<Date>;
    public response: KnockoutObservable<string>;
    public parentRequest: KnockoutObservable<MediaRequestVm>;
    public eodReportWith: KnockoutObservable<number>;
    public requestStatus: KnockoutComputed<string>;
    private gotModified: KnockoutObservable<string>; // Used internally only
    public modifiedStyle: KnockoutComputed<string>;
    public completed: KnockoutComputed<boolean>;
    public editPageHeader: KnockoutComputed<string>;
    public triggerEmail: KnockoutObservable<boolean>;
    public onlyEmailMyself: KnockoutObservable<boolean>;
    public isMajorOutlet: KnockoutComputed<boolean>;
    public isOverdue: KnockoutComputed<boolean>;
    public isMyResponsiblity: KnockoutComputed<boolean>;
    public myMinistries: Array<MinistryVm>;
    public currentUserData: CurrentUserVm;
    public resolution: KnockoutObservable<ResolutionVm>;
    public commContacts: KnockoutObservableArray<UserVm>;


    public errors;
    public __dirty;

    constructor(currentUserData: CurrentUserVm, dto?: server.MediaRequestDto) {
        this.id = ko.observable<string>();
        this.createdAt = ko.observable<Date>();
        this.createdBy = ko.observable<SystemUser>();
        this.modifiedAt = ko.observable<Date>();
        this.modifiedBy = ko.observable<SystemUser>();
        this.leadMinistry = ko.observable<MinistryVm>();
        this.responsibleUser = ko.observable<UserVm>();
        this.mediaContacts = ko.observableArray<MediaContactVm>();
        this.sharedMinistries = ko.observableArray<MinistryVm>();
        this.takeOverRequestMinistry = ko.observable<MinistryVm>();
        this.deadlineAt = ko.observable<Date>();
        this.requestTopic = ko.observable<string>();
        this.requestContent = ko.observable<string>();
        this.requestedAt = ko.observable<Date>();
        this.acknowledgedAt = ko.observable<Date>();
        this.respondedAt = ko.observable<Date>();
        this.response = ko.observable<string>();
        this.parentRequest = ko.observable<MediaRequestVm>();
        this.eodReportWith = ko.observable<number>();
        this.resolution = ko.observable<ResolutionVm>();
        this.commContacts = ko.observableArray<UserVm>();

        this.currentUserData = currentUserData;

        this.editPageHeader = ko.pureComputed({
            read: () => {
                if (!this.id()) {
                    if (this.parentRequest()) {
                        return "Follow-up Media Request";
                    }
                    return "New Media Request";
                }
                return "Edit Media Request";
            }
        });
        this.triggerEmail = ko.observable<boolean>();
        this.onlyEmailMyself = ko.observable<boolean>();
        this.errors = ko.validation.group(this);
        
        this.gotModified = ko.observable<string>();
        this.modifiedStyle = ko.pureComputed<string>({
            read: function () {
                return this.gotModified();
            },
            write: function (value) {
                this.gotModified(value);
                if (value) {
                    setTimeout(() => {
                        // if it hasn't changed, clear it!
                        if (value === this.gotModified()) {
                            this.gotModified(undefined);
                        }
                    }, 3200); // Css animation duration is 3 seconds.
                }
            },
            owner: this
        });

        this.requestStatus = ko.pureComputed<string>(function () {
            var retval = "Open";
            if (this.acknowledgedAt()) {
                retval = "In Progress";
            }
            if (this.respondedAt()) {
                retval = "Closed";
            }
            return retval;
        }, this);

        this.completed = ko.pureComputed<boolean>({
            read: function () {
                return this.respondedAt() ? true : false;
            },
            write: function (value) {
                if (value) {
                    this.respondedAt(new Date());
                } else {
                    this.respondedAt(undefined);
                    this.response(undefined);
                }
            }
        }, this);

        this.isMajorOutlet = ko.pureComputed(function () {
            var match = false;
            //this type of loop need be tested. Maybe For..In or just use a for loop.
            //could also loose the parenthesis
            for (var contact of this.mediaContacts()) {
                var job = contact.job();
                if (job && job.outlet() && job.outlet().isMajor()) {
                    match = true;
                    break;
                }
            }
            return match;
        }, this);

        this.isOverdue = ko.pureComputed(function () {
            var overdue = false;
            var now = new Date();
            if (this.deadlineAt()) {
                if (this.deadlineAt() < now) {
                    overdue = true;
                }
            }
            return overdue;
        }, this);
        this.origVal = function () {
            // Empty function is required for cancel edit feature.
        };
        this.isMyResponsiblity = ko.pureComputed(function () {
            var isMine = false;
            if (currentUserData.user().isAdvanced()) {
                return true;
            }
            else if (currentUserData.userMinistries() && this.leadMinistry()) {
                for (let i = 0; i < currentUserData.userMinistries().length; i++) {
                    let ministry = currentUserData.userMinistries()[i];
                    if (ministry.id() == this.leadMinistry().id()) {
                        isMine = true;
                        break;
                    }
                }
            }
            return isMine;
        }, this);

        this.leadMinistry.extend({
            required: {
                message: "Please select a Ministry."
            }
        });
        this.mediaContacts.extend({
            required: {
                message: "Choose a Reporter from the list, or add a new one."
            }
        });
        this.requestedAt.extend({
            required: {
                message: "Please enter the Received date/time."
            }
        });
        this.requestTopic.extend({
            required: { 
                message: "Topic is a required field."
            }, 
            maxLength: 250
        });
        this.requestContent.extend({
            required: {
                message: "The Request details are required."
            },
            maxLength: 4000
        });
        this.responsibleUser.extend({
            required: {
                message: "A Responsible user is required."
            }
        });

        this.response.extend({
            required: {
                message: "Response is required when closing the request.  To re-open the request, remove the date and time Responded.",
                onlyIf: () => {
                    return (!ko.validation.utils.isEmptyVal(this.respondedAt()));
                }
            }
        });

        this.resolution.extend({
            required: {
                message: "Resolution is required when closing the request.  To re-open the request, remove the date and time Responded.",
                onlyIf: () => {
                    return (!ko.validation.utils.isEmptyVal(this.respondedAt()));
                }
            }
        });

        if (dto) {
            this.init(dto);
        }
        else {
            this.applyDefaults();
        }
    }

    public applyDefaults(): void {

        this.requestedAt(new Date(Date.now()));
    }


    private init(data: server.MediaRequestDto): void {
        
        this.id(data.id);
        this.createdAt(data.createdAt ? new Date(data.createdAt) : undefined);
        this.createdBy(data.createdBy);
        this.modifiedAt(data.modifiedAt ? new Date(data.modifiedAt) : undefined);
        this.modifiedBy(data.modifiedBy);

        if (data.leadMinistry) {
            this.leadMinistry(new MinistryVm(data.leadMinistry));
        }
        if (data.takeOverRequestMinistry) {
            this.takeOverRequestMinistry(new MinistryVm(data.takeOverRequestMinistry));
        }
        
        if (data.responsibleUser) {
            this.responsibleUser(new UserVm(data.responsibleUser));
        }

        var contacts: Array<MediaContactVm> = [];
        for (var i = 0; (data.mediaContacts && i < data.mediaContacts.length); i++) {
            contacts.push(new MediaContactVm(data.mediaContacts[i]));
        }
        this.mediaContacts([]);
        ko.utils.arrayPushAll<MediaContactVm>(this.mediaContacts, contacts);

        this.deadlineAt(data.deadlineAt ? new Date(data.deadlineAt) : undefined);
        this.requestTopic(data.requestTopic); 
        this.requestContent(data.requestContent);
        this.requestedAt(data.requestedAt ? new Date(data.requestedAt) : undefined);
        this.acknowledgedAt(data.acknowledgedAt ? new Date(data.acknowledgedAt) : undefined);
        this.respondedAt(data.respondedAt ? new Date(data.respondedAt) : undefined);
        this.response(data.response);
        if (data.parentRequest) {
            var pr: MediaRequestVm = new MediaRequestVm(this.currentUserData, data.parentRequest);
            this.parentRequest(pr);
        }
        this.eodReportWith(data.eodReportWith);

        if (data.resolution)
            this.resolution(new ResolutionVm(data.resolution));

        this.origVal.value = data;
        if (data.sharedMinistries) {
            var mins: Array<MinistryVm> = [];
            for (var i = 0; i < data.sharedMinistries.length; i++) {
                mins.push(new MinistryVm(data.sharedMinistries[i]));
            }
            this.sharedMinistries([]);
            ko.utils.arrayPushAll<MinistryVm>(this.sharedMinistries, mins);
        } else {
            this.sharedMinistries([]);
        }

        
    }

    private static sanitizeFindRequestsOptions(findOptions: FindRequestsOptions): any {
        var ajaxData: any = {};
        ajaxData._skip = findOptions.skip ? findOptions.skip : undefined;
        ajaxData._limit = findOptions.limit ? findOptions.limit : undefined;
        ajaxData.ministries = findOptions.ministries ? findOptions.ministries.join(',') : undefined;
        ajaxData.responded = findOptions.responded ? findOptions.responded : undefined;
        ajaxData.requestsToday = findOptions.requestsToday ? findOptions.requestsToday.toISOString() : undefined;
        ajaxData.requestsBefore = findOptions.newestMediaRequestDate ? findOptions.newestMediaRequestDate.toISOString() : undefined;
        ajaxData.requestsAfter = findOptions.requestsAfterDate ? findOptions.requestsAfterDate.toISOString() : undefined;
        ajaxData.modifiedAfter = findOptions.modifiedAfterDate ? findOptions.modifiedAfterDate.toISOString() : undefined;
        return ajaxData;
    }

    public static find(findOptions: FindRequestsOptions, curUserData: CurrentUserVm, callback: Function): void {

        var ajaxData: any = MediaRequestVm.sanitizeFindRequestsOptions(findOptions);

        $.ajax({
            url: '/api/mediarequests',
            type: "get",
            data: ajaxData,
            success: function (data) {

                if (findOptions.raw) {
                    return callback(undefined, data);
                }

                var mrArr = [];
                for (var i = 0; i < data.length; i++) {

                    var mcvm = ko.observable(new MediaRequestVm(curUserData, data[i]));
                    mrArr.push(mcvm);
                };
                callback(undefined, mrArr);

            },
            error: function (err) {
                callback(err);
            }
        });

    }

    public static getOpenMediaRequests(curUserData: CurrentUserVm, callback: Function): void {
        $.ajax({
            url: '/api/mediarequests/openforme',
            type: "get",
            success: function (data) {
                var arrayMr = [];
                for (var i = 0; i < data.length; i++) {
                    var mrvm = ko.observable(new MediaRequestVm(curUserData, data[i]));
                    arrayMr.push(mrvm);
                };
                callback(undefined, arrayMr)
            },
            error: function (err) {
                callback(err);
            }
        });
    }

    public static findIdsOnly(findOptions: FindRequestsOptions, curUserData: CurrentUserVm, callback: Function): void {

        var ajaxData: any = MediaRequestVm.sanitizeFindRequestsOptions(findOptions);
        ajaxData.idsOnly = true;

        $.ajax({
            url: '/api/mediarequests',
            type: "get",
            data: ajaxData,
            success: function (data) {

                var mrArr = [];
                for (var i = 0; i < data.length; i++) {
                    mrArr.push(data[i].id);
                };
                callback(undefined, mrArr);
            },
            error: function (err) {
                callback(err);
            }
        });

    }

    public static findById(id: string, curUserData: CurrentUserVm, callback: Function): void {
        if (id) {
            // Cleanup url parts like: undefined#contact=undefined" etc.
            id = id.replace('#', '?').split('?')[0];
            if (id === 'null' || id === 'undefined') {
                id = undefined;
            }
            if (id) {
                $.ajax('/api/mediarequests/' + id, {
                    success: function (data) {
                        var mrvm = ko.observable(new MediaRequestVm(curUserData, data));
                        callback(undefined, mrvm);
                    },
                    error: function (err) {
                        callback(err);
                    }
                });
            }
        }
    }

    public static postEndOfDayUpdates(eodRequests: KnockoutObservableArray<KnockoutObservable<MediaRequestVm>>, callback): void {
        //create the post object
        let postData: Array<any> = [];
        ko.utils.arrayForEach(eodRequests(), (mr: KnockoutObservable<MediaRequestVm>, i: number) => {
            let el: any = {};
            el.id = mr().id();
            el.eodReportWith = mr().eodReportWith();
            postData.push(el);
        });

        $.ajax('/api/mediarequests/postendofdayupdates', {
            type: 'POST',
            contentType: 'application/json',
            data: ko.toJSON(postData),
            success: function (data) {
                
                callback(undefined, true);
            },
            error: function (err) {
                callback(err, false);
            }
        });
    }

    public static postEndOfDaySummary( callback): void {
        

        $.ajax('/api/mediarequests/postendofdaysummary', {
            type: 'POST',
            contentType: 'application/json',
            data: {},
            success: function (data) {

                callback(undefined, true);
            },
            error: function (err) {
                callback(err, false);
            }
        });
    }

    public cancelEdit = () => {
        if (this.origVal.value) {
            this.init(this.origVal.value);
            if (this.__dirty) {
                this.__dirty.reset();
            }
        }
    }

    public isValid(): boolean {
        var isValid = this.errors().length === 0;
        if (!isValid) {
            this.errors.showAllMessages();
        } 
        return isValid;
    }

    private static mediaRequestToJSON(mr: MediaRequestVm) {
        return ko.toJSON(mr, function (key, value) {
            if (key == "currentUserData") {
                // currentUserData is used by neither the API, nor the Cancel/Undo.
                return null;
            }
            return value;
        });
    }

    public save(callback: Function): void {
        var me: MediaRequestVm = this;

        var qryString = "";
        if (this.triggerEmail()) {
            qryString = "?triggerEmail=true";

        }
        if (this.onlyEmailMyself()) {
            qryString = "?onlyEmailMyself=true";
        }

        //we need to sort out if we are inserting or updating.
        if (this.id()) {

            // If we're re-opening a closed MediaRequest, discard the Response.
            if (this.response() && !this.respondedAt()) {
                this.response("");
            }

            //update
            const putData = MediaRequestVm.mediaRequestToJSON(this);

            $.ajax('/api/mediarequests/' + this.id() + qryString, {
                type: 'PUT',
                contentType: 'application/json',
                data: putData,
                success: function (data) {
                    callback(undefined, true);
                    me.origVal.value = ko.utils.parseJson(putData);
                    if (me.__dirty) {
                        me.__dirty.reset();
                    }
                },
                error: function (err) {
                    callback(err);
                }
            });
        } else {
            //insert
            $.ajax('/api/mediarequests' + qryString, {
                type: 'POST',
                contentType: 'application/json',
                data: MediaRequestVm.mediaRequestToJSON(this), 
                success: function (data) {
                    // Don't create a new object, initialize the one we have.
                    me.id(data);
                    me.createdAt(new Date());
                    if (me.__dirty) {
                        me.__dirty.reset();
                    }
                    callback(undefined, data);
                },
                error: function (err) {
                    callback(err);
                }
            });
        }
    }

    public delete(callback: Function): void {
        $.ajax('/api/mediarequests/' + this.id(), {
            type: 'DELETE',
            contentType: 'application/json',
            success: function (data) {
                callback(undefined, true);
            },
            error: function (err) {
                callback(err);
            }
        });
    }
}
