﻿import navbarTemplate from './navbar.html';
import manageDocumentsModalTemplate from 'app/sections/document-manager/manage-modal.html';
import ManageDocumentsModalController from 'app/sections/document-manager/manage-modal';

export default class NavbarController {
    static $inject = ['$uibModal', 'documentListService'];

    constructor(
        private $uibModal: ng.ui.bootstrap.IModalService,
        private documentListService: DocumentListService
    ) {
        this.activate();
    }

    activate() {
        return this.documentListService.get()
            .then(documents => this.documents = documents);
    }

    openManageDocumentsModal() {
        return this.$uibModal
            .open({
                controllerAs: 'manager',
                bindToController: true,
                template: manageDocumentsModalTemplate,
                controller: ManageDocumentsModalController,
                resolve: {
                    documents: () => this.documents
                }
            }).result
            .then(result => console.info(result));
    }
    // TODO: rename to toggleDropdownList
    toggleExpanded() {
        this.expanded = !this.expanded;
    }
    documents: DocumentListItem[];
    expanded = false;
    user: User;
    //logoff() {
    //    return this.userService
    //        .logoff()
    //        .then(() => this.user = undefined)
    //        .finally(() => {
    //            return this.$state.go('app.login', {}, { reload: true });
    //        });
    //}
} 