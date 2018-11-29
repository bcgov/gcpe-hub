class ResolutionVm {

    public id: KnockoutObservable<string>;
    public displayAs: KnockoutObservable<string>;

    constructor(data: server.ResolutionDto) {
        this.id = ko.observable(data.id);
        this.displayAs = ko.observable(data.displayAs);
    }

    public static findResolutions(callback: Function): void {
        $.ajax('/api/mediarequests/resolutions', {
            success: function (data) {
                var resolutions = [];
                for (var i = 0; i < data.length; i++) {
                    var resolution = new ResolutionVm(data[i]);
                    resolutions.push(resolution);
                };
                callback(undefined, resolutions);
            },
            error: function (err) {
                callback(err);
            }
        });
    }
}
