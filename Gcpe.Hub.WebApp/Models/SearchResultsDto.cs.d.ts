declare module server {
	interface FilterDto {
		name: string;
		count: number;
		isChecked: boolean;
	}
	interface FacetDto {
        name: string;
        filters: server.FilterDto[];
	}
    interface SearchResultsDto {
        mediaRequests: server.MediaRequestDto[];
        facets: FacetDto[];
	}
}
