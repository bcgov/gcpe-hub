/// <reference path="UserDto.cs.d.ts" />
/// <reference path="MinistryDto.cs.d.ts" />
/// <reference path="MediaContactDto.cs.d.ts" />

declare module server {
	interface MediaRequestDto {
		id: any;
		createdAt: Date;
		createdBy: server.UserDto;
		modifiedAt: Date;
		modifiedBy: server.UserDto;
		leadMinistry: server.MinistryDto;
        sharedMinistries: server.MinistryDto[];
        takeOverRequestMinistry: server.MinistryDto;
		responsibleUser: server.UserDto;
		mediaContacts: server.MediaContactDto[];
		deadlineAt: Date;
		requestTopic: string;
		requestContent: string;
		requestedAt: Date;
		acknowledgedAt: Date;
		response: string;
		respondedAt: Date;
		parentRequest: server.MediaRequestDto;
		eodReportWith: any;
		resolution: {
			id: any;
			displayAs: string;
		};
	}
}
