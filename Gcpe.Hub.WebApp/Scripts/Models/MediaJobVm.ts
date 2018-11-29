class MediaJobVm {

    public id: KnockoutObservable<string>;
    public title: KnockoutObservable<string>;
    public outlet: KnockoutObservable<MediaOutletVm>;

    constructor(data?: server.MediaJobDto) { 
        this.id = ko.observable<string>();
        this.title = ko.observable<string>().extend({ required: { message: "Please select a Title" } });
        this.outlet = ko.observable<MediaOutletVm>().extend({ required: { message: "Please select a Media Outlet" } });

        if (data) {
            this.init(data);
        }
    }

    private init(data: server.MediaJobDto) {
        this.id(data.id);
        this.title(data.title);
        this.outlet(new MediaOutletVm(data.outlet as server.OutletDto));
    }



}