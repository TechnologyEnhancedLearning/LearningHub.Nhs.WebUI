<template>
    <div>
        <div :class="['d-none d-md-block ' + (this.pagingModel.totalPages <= 1 ? 'pb-5' : '')]">
            <h2>{{priorityTypeText}}</h2>
            <hr class="nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-0" />
            <div class="table-responsive d-none d-md-block">
                <table class="table table-hover table-bordered table-borderless-header small mb-0">
                    <thead>
                        <tr>
                            <th v-if="isHighPriority" class="p-0">
                                &nbsp;
                            </th>
                            <th class="px-sm-3 py-sm-3" style="width:14%" @click="sortByDate" role=button>
                                Date
                                <i :class="['pl-2 pt-2 text-dark '+ sortDirectionIcon]"></i>
                            </th>
                            <th class="px-sm-3 py-sm-3" style="width:44%">Title</th>
                            <th class="px-sm-3 py-sm-3" style="width:18%">Type</th>
                            <th class="px-sm-3 py-sm-3" style="width:12%">Status</th>
                            <th class="px-sm-3 py-sm-3" style="width:12%">Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="notification in this.notificationModel">
                            <td v-if="isHighPriority" class="p-0" style="background:#FCEC60;">
                                &nbsp;
                            </td>
                            <td class="px-sm-3 py-sm-3">{{ notification.date | formatDate('DD MMM YYYY') }}</td>
                            <td class="px-sm-3 py-sm-3">
                                <a href="#" @click="showNotification(notification)">{{ notification.title }}</a>
                            </td>
                            <td class="px-sm-3 py-sm-3">
                                <div class="d-flex justify-content-between" 
                                     v-for="item in notificationTypeContent(notification.notificationType)">
                                    {{ item.text }}&nbsp;
                                    <i :class="item.className"></i>
                                </div>
                            </td>
                            <td class="px-sm-3 py-sm-3">
                                <div class="d-flex justify-content-between"
                                     v-for="item in notificationStatusContent(notification.readOnDate)">
                                    {{ item.text }}
                                    <i :class="[item.className + ' pl-3']"></i>
                                </div>
                            </td>
                            <td class="px-sm-3 py-sm-3">
                                <div class="d-flex justify-content-between" v-if="notification.userDismissable">
                                    <a href="#deleteModal" data-toggle="modal" @click="selectNotification(notification)">Delete</a>
                                    &nbsp;<i class="fa-solid fa-trash-can-alt pt-1"></i>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <paging v-bind="this.pagingModel" @loadPage="loadPage"></paging>
        </div>

        <div class="d-block d-md-none mx-n4 pb-1">

            <div class="d-flex justify-content-between p-4 m-0"
                 :style="[isHighPriority ? {background:'#FCEC60'}:{background:'#435563', color:'#fff'}]"
                 @click="toggleContent" role=button>
                <h2 class="m-0">{{priorityTypeText}}</h2>
                <i :class="showContentIcon"
                   :style="[isHighPriority ? {color:'#4c6272'}:{color:'#fff'}] "></i>
            </div>

            <template v-if="showContent">
                <div v-for="notification in this.notificationModel"
                     class="d-flex justify-content-between p-4 border-bottom">

                    <div class="text-truncate pr-4">
                        <i :class="['fal fa-envelope' + (notification.readOnDate != null ? '-open text-success' : '')]">&nbsp;</i>
                        <a href="#" @click="showNotification(notification)">{{ notification.title }}</a>
                    </div>

                    <a href="#deleteModalButton" data-toggle="modal" @click="deleteNotification()" aria-label="Delete" v-if="notification.userDismissable">
                        <i class="fas fa-ellipsis-h fa-lg" style="color: #435563;"></i>
                    </a>
                </div>

                <paging v-bind="this.pagingModel" @loadPage="loadPage"></paging>
            </template>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import '../filters';
    import paging from '../globalcomponents/pagingcomponent.vue';
    import axios from 'axios';
    import { NotificationModel, NotificationType, NotificationPriority } from './notificationModel';
    import { TablePagingModel, PagingModel } from '../models/pagingModel';

    export default Vue.extend({
        name: 'notification',
        components: {
            'paging': paging,
        },
        props: {
            priorityType: { Type: NotificationPriority } as PropOptions<NotificationPriority>,
            showContent: { Type: Boolean } as PropOptions<boolean>,
        },
        data() {
            return {
                selectedNotification: undefined as NotificationModel,
                NotificationPriority,
                pageIndex: 1,
                sortAscending: false,
                pagingModel: {} as PagingModel,
                notificationModel: [] as NotificationModel[],
            }
        },
        computed: {
            sortDirectionIcon() {
                return (this as any).sortAscending ? 'fa-solid fa-chevron-up' : 'fa-solid fa-chevron-down';
            },
            isHighPriority() {
                return this.priorityType == NotificationPriority.Priority;
            },
            priorityTypeText() {
                return (this as any).isHighPriority ? 'High priority' : 'Other';
            },
            showContentIcon() {
                return this.showContent ? 'fa-solid fa-chevron-up fa-lg' : 'fa-solid fa-chevron-down fa-lg';
            },
        },
        async created() {
            await this.loadNotification();
        },
        methods: {
            async loadNotification() {
                var data = {
                    page: this.pageIndex,
                    sortColumn: 'date',
                    sortDirection: this.sortAscending ? 'A' : 'D'
                };
                await axios.post<TablePagingModel<NotificationModel>>('/api/notification?priorityType=' + this.priorityType, data)
                    .then(response => {
                        this.pageIndex = response.data.paging.currentPage;
                        this.pagingModel = response.data.paging;
                        this.notificationModel = response.data.results.items;
                    })
                    .catch(e => console.log(e));
            },
            async loadPage(pageIndex: number) {
                this.pageIndex = pageIndex;
                await this.loadNotification();
            },
            async sortByDate() {
                this.sortAscending = !this.sortAscending;
                await this.loadNotification();
            },
            toggleContent() {
                this.$emit('toggleContent');
            },
            selectNotification(notification: NotificationModel) {
                this.selectedNotification = notification;
                this.$emit('selected', notification);
            },
            async showNotification(notification: NotificationModel) {
                event.preventDefault();
                var markRead = notification.readOnDate == null;
                notification.readOnDate = new Date();
                this.selectedNotification = notification;
                this.$emit('showNotification', notification);

                if (markRead) {
                    await axios.post(
                        '/api/notification/' + this.selectedNotification.notificationId + '/mark-as-read?userNotificationId=' + this.selectedNotification.id)
                        .catch(e => console.log(e));
                }
            },
            async deleteNotification() {                
                await axios.delete(
                    '/api/notification/' + this.selectedNotification.notificationId + '?userNotificationId=' + this.selectedNotification.id)
                    .catch(e => console.log(e));

                await this.loadNotification();
            },
            notificationTypeContent(type: NotificationType) {
                switch (type) {
                    case NotificationType.SystemUpdate:
                        return [{ text: 'System update', className: 'fa-solid fa-gear text-dark pt-1' }];
                    case NotificationType.SystemRelease:
                        return [{ text: 'Service release', className: 'fa-solid fa-upload text-dark pt-1' }];
                    case NotificationType.ActionRequired:
                        return [{ text: 'Action required', className: 'fa-solid fa-triangle-exclamation text-warning pt-1' }];
                    case NotificationType.ResourcePublished:
                        return [{ text: 'Published', className: 'fa-solid fa-circle-check text-success pt-1' }];
                    case NotificationType.ResourceRated:
                        return [{ text: 'Rated', className: 'fa-regular fa-star text-dark pt-1' }];
                    case NotificationType.UserPermission:
                        return [{ text: 'Permissions', className: 'fa-regular fa-user text-dark pt-1' }];
                    case NotificationType.PublishFailed:
                        return [{ text: 'Action required', className: 'fa-solid fa-triangle-exclamation text-warning pt-1' }];
                    case NotificationType.AccessRequest:
                        return [{ text: 'Access request', className: 'fa-solid fa-lock text-dark pt-1' }];
                    default:
                        return [{ text: 'unknown', className: '' }];
                }
            },
            notificationStatusContent(readOnDate: Date | undefined) {
                if (readOnDate != null) {
                    return [{ text: 'Read', className: 'fa-regular fa-envelope-open text-success pt-1' }]
                } else {
                    return [{ text: 'Unread', className: 'fa-regular fa-envelope pt-1' }]
                }
            },
        },
    })
</script>