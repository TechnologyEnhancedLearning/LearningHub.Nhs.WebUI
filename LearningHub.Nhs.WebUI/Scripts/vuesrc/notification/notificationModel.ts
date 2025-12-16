export type NotificationModel = {
    id: number | undefined
    notificationId: number
    body: string
    date: Date
    title: string
    notificationType: NotificationType
    notificationPriority: NotificationPriority
    readOnDate: Date | undefined
    userDismissable: boolean
}

export enum NotificationType {
    SystemUpdate = 1,
    SystemRelease = 2,
    ActionRequired = 3,
    ResourcePublished = 4,
    ResourceRated = 5,
    UserPermission = 6,
    PublishFailed = 7,
    AccessRequest = 8,
    ReportProcessed = 9,
}

export enum NotificationPriority {
    General = 1,
    Priority = 2,
}

export type TablePagingViewModel<T> = {
    results: PagedResultSet<T>
    paging: PagingViewModel
    sortColumn: string
    sortDirection: string
    filter: PagingColumnFilter[] | undefined
    listPageHeader: ListPageHeaderViewModel[] | undefined
}

export type ListPageHeaderViewModel = {
    displayedCount: number
    totalItemCount: number
    filterCount: number
    createRequired: boolean
}

export type PagingColumnFilter = {
    column: string
    value: string
}

export class PagingViewModel {
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