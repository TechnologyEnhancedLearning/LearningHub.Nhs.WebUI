<template>
        <div class="notifications-panel lh-container-xl">
            <div :class="[showMessage ? 'd-none' : 'd-block']">
                <h2 class="nhsuk-heading-l nhsuk-u-padding-bottom-5">Notifications</h2>
                <notification ref="priorityNotification"
                              :priorityType="this.NotificationPriority.Priority"
                              :showContent="this.showPriorityContent"
                              @toggleContent="toggleContent"
                              @selected="onSelect"
                              @showNotification="showNotification">
                </notification>
                <notification ref="generalNotification"
                              :priorityType="this.NotificationPriority.General"
                              :showContent="!this.showPriorityContent"
                              @toggleContent="toggleContent"
                              @selected="onSelect"
                              @showNotification="showNotification">
                </notification>
            </div>

            <div v-if="showMessage" :class="[showMessage ? 'd-block' : 'd-none', 'nhsuk-u-margin-bottom-7']">
                <div class="nhsuk-back-link">
                    <a class="nhsuk-back-link__link" href="#" @click="showNotificationList($event)">
                        <svg class="nhsuk-icon nhsuk-icon__chevron-left" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M8.5 12c0-.3.1-.5.3-.7l5-5c.4-.4 1-.4 1.4 0s.4 1 0 1.4L10.9 12l4.3 4.3c.4.4.4 1 0 1.4s-1 .4-1.4 0l-5-5c-.2-.2-.3-.4-.3-.7z"></path>
                        </svg>
                        Back to: Notifications
                    </a>
                </div>

                <div class="row">
                    <div class="col-md-8">
                        <h2>{{this.selectedNotification.title}}</h2>
                    </div>
                </div>

                <div class="row nhsuk-u-font-size-19">
                    <div class="col-md-8" v-html="selectedNotification.body" />

                    <div class="col-md-4">
                        <div class="d-block d-md-none mx-n4">
                            <hr class="mt-3" />
                        </div>

                        <div class="row">
                            <div class="col-4">Added on:</div>
                            <div class="col">
                                {{selectedNotification.date | formatDate('DD MMM YYYY')}}
                            </div>
                            <div class="w-100 py-2"></div>
                            <div class="col-4">Type:</div>
                            <div class="col" v-for="item in notificationTypeContent()">
                                <i :class="[item.className + ' pr-3']"></i>{{ item.text }}
                            </div>
                            <div class="w-100 py-2"></div>
                            <div class="col-4">Status:</div>
                            <div class="col">
                                <i class="fa-regular fa-envelope-open text-success pr-3"></i>Read
                            </div>
                        </div>

                        <div class="d-none d-md-block">
                            <hr class="py-2 mt-3" />
                        </div>

                        <template v-if="selectedNotification.userDismissable">
                            <div class="d-block d-md-none mx-n4">
                                <hr class="py-2 mt-4" />
                            </div>
                            <button data-target="#deleteModal" data-toggle="modal" class="nhsuk-button">Delete</button>
                        </template>
                    </div>
                </div>
            </div>

            <div class="modal" tabindex="-1" role="dialog" id="deleteModalButton">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content text-center">
                        <a href="#" @click="deleteConfirmation($event)">Delete</a>
                    </div>
                </div>
            </div>

            <div class="modal fade" tabindex="-1" role="dialog" id="deleteModal">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header pr-0 pt-0">
                            <button type="button" class="close" @click="closeConfirmation" aria-label="Close">
                                <i class="fa-solid fa-xmark"></i>
                            </button>
                        </div>
                        <div class="modal-body text-center">
                            <p>
                                Deleting this notification cannot be undone.<br />
                                Are you sure you want to delete it?
                            </p>
                        </div>
                        <div class="modal-footer justify-content-center">
                            <button type="button" class="nhsuk-button nhsuk-button--secondary mr-4" @click="closeConfirmation">Cancel</button>
                            <button type="button" class="nhsuk-button" @click="deleteNotification">Delete</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import '../filters';
    import notification from './notification.vue';
    import { NotificationPriority, NotificationType, NotificationModel } from './notificationModel';

    export default Vue.extend({
        components: {
            'notification': notification,
        },
        data() {
            return {
                NotificationType,
                NotificationPriority,
                showPriorityContent: true,
                showMessage: false,
                selectedNotification: undefined as NotificationModel,
            }
        },
        methods: {
            toggleContent() {
                this.showPriorityContent = !this.showPriorityContent;
            },
            onSelect(notification: NotificationModel) {
                this.selectedNotification = notification;
            },
            getSelectedComponent(): any {
                return this.selectedNotification.notificationPriority == NotificationPriority.Priority
                    ? this.$refs.priorityNotification : this.$refs.generalNotification;
            },
            deleteConfirmation(event: any) {
                event.preventDefault();
                $('#deleteModalButton').modal('hide');
                $('#deleteModal').modal();
            },
            closeConfirmation() {
                if (document.activeElement instanceof HTMLElement) {
                    document.activeElement.blur();
                }
                document.body.focus();
                setTimeout(() => { $('#deleteModal').modal('hide'); }, 10);
            },
            showNotification(notification: NotificationModel) {
                this.selectedNotification = notification;
                this.showMessage = true;
                setTimeout(() => { $('html,body').scrollTop($(".nhsuk-back-link").offset().top); }, 100);

            },
            showNotificationList(event: any) {
                event.preventDefault();
                this.showMessage = false;
            },
            async deleteNotification() {
                await this.getSelectedComponent().deleteNotification();
                this.closeConfirmation();
                this.showMessage = false;
            },
            notificationTypeContent() {
                switch (this.selectedNotification.notificationType) {
                    case NotificationType.SystemUpdate:
                        return [{ text: 'System update', className: 'fa-solid fa-gear text-dark' }];
                    case NotificationType.SystemRelease:
                        return [{ text: 'Service release', className: 'fa-solid fa-upload text-dark' }];
                    case NotificationType.ActionRequired:
                        return [{ text: 'Action required', className: 'fa-solid fa-triangle-exclamation text-warning' }];
                    case NotificationType.ResourcePublished:
                        return [{ text: 'Published', className: 'fa-solid fa-circle-check text-success' }];
                    case NotificationType.ResourceRated:
                        return [{ text: 'Rated', className: 'fas fa-star text-dark' }];
                    case NotificationType.UserPermission:
                        return [{ text: 'Permissions', className: 'fas fa-user text-dark' }];
                    case NotificationType.PublishFailed:
                        return [{ text: 'Action required', className: 'fa-solid fa-triangle-exclamation text-warning' }];
                    case NotificationType.AccessRequest:
                        return [{ text: 'Access request', className: 'fas fa-lock-alt text-dark pt-1' }];
                    default:
                        return [{ text: 'unknown', className: '' }];
                }
            },
        },
    })
</script>