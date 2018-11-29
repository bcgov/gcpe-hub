declare module server {
	interface OutletDto {
		id: any;
		name: string;
		isMajor: boolean;
	}
	interface MediaJobDto {
		id: any;
		outlet: server.OutletDto;
		title: string;
	}
}
