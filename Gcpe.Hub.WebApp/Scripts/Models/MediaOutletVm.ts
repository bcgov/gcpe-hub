class MediaOutletVm {

    public id: KnockoutObservable<string>;
    public name: KnockoutObservable<string>;
    public isMajor: KnockoutObservable<boolean>;

    constructor(data?: server.OutletDto) { 
        this.id = ko.observable<string>();
        this.name = ko.observable<string>();
        this.isMajor = ko.observable<boolean>();

        if (data) {
            this.init(data);
        }
    }

    private init(data: server.OutletDto) {
        this.id(data.id);
        this.name(data.name);
        this.isMajor(data.isMajor);
    }
}