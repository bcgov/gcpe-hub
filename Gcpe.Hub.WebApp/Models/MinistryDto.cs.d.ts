/// <reference path="UserDto.cs.d.ts" />

declare module server {
	interface MinistryDto {
		id: any;
		displayAs: string;
		users: server.UserDto[];
		abbreviation: string;
		eodFinalizedDateTime: Date;
		eodLastRunUser: server.UserDto;
		eodLastRunDateTime: Date;
		primaryContact: server.UserDto;
		secondaryContact: server.UserDto;
		afterHoursPhone: string;
		afterHoursPhoneExtension: string;
	}
}
