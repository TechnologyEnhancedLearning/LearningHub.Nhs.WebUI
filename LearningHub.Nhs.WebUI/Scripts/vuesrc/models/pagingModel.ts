export type TablePagingModel<T> = {
    results: PagedResultSet<T>
    paging: PagingModel
    sortColumn: string
    sortDirection: string
    filter: PagingColumnFilter[] | undefined
    listPageHeader: ListPageHeaderModel[] | undefined
}

export type ListPageHeaderModel = {
    displayedCount: number
    totalItemCount: number
    filterCount: number
    createRequired: boolean
}

export type PagingColumnFilter = {
    column: string
    value: string
}

export class PagingModel {
    hasItems: boolean
    currentPage: number
    pageSize: number
    totalItems: number
    totalPages: number
}

export type PagedResultSet<T> = {
    items: T[]
    totalItemCount: number
}