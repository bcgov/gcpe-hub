declare module server {
	interface UserDto {
		id: any;
		displayAs: string;
		isEditor: boolean;
		isAdvanced: boolean;
		workTelephone: string;
		workTelephoneExtension: string;
		mobileTelephone: string;
		emailAddress: string;
		userDomainName: string;
	}
}
