/// <reference path="MinistryDto.cs.d.ts" />
/// <reference path="UserDto.cs.d.ts" />

declare module server {
	interface EodStatusDto {
		ministry: server.MinistryDto;
		lastActivity: Date;
	}
}
