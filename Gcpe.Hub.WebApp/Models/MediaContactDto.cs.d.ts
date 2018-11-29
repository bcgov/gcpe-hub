/// <reference path="OutletDto.cs.d.ts" />

declare module server {
	interface MediaContactDto {
		id: any;
		firstName: string;
		lastName: string;
		job: server.MediaJobDto;
		workPhone: string;
		workPhoneExtension: string;
		cellPhone: string;
		email: string;
	}
}
