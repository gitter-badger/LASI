﻿import ngf = angular.angularFileUpload;
const log = console.log.bind(console);

export default class {
    static $inject = ['$scope', '$q', 'Upload', 'UserService'];

    constructor(private $scope: angular.IScope,
        private $q: ng.IQService,
        private uploadService: ngf.IUploadService,
        private userService: UserService) {
        this.$scope.$watch('upload.files', this.uploadFiles.bind(this));
    }

    uploadFiles() {
        const files = this.files;
        this.logInvalidFiles();
        return this.$q.when((Array.isArray(files) ? files : [files]).map(file => this.uploadFile(file)));
    }

    logInvalidFiles() {
        const files = this.files;
        (Array.isArray(files) ? files : [files]).filter(file => this.formats.every(format => file.type.localeCompare(format) !== 0))
            .map(file => `File ${file.name} has unaccepted format ${file.type}`)
            .forEach(log);
    }

    uploadFile(file: File) {
        return (this.uploadService.upload
            .call(this.uploadService, {
                data: {
                    [file.name]: file
                },
                url: 'api/UserDocuments',
                method: 'POST',
                userId: this.userService.user.id
            }))
            .progress(data => log(`Progress: ${100.0 * data.progress / data.percentComplete}% ${file.name}`))
            .then(({data}) => log(`File ${file.name} uploaded. \nResponse: ${JSON.stringify(data)}`))
            .catch(reason => log(`Http: ${status} Failed to upload file. \nDetails: ${reason}`));
    }

    removeFile(file: File, index: number) {
        const files = this.files;
        this.files = (Array.isArray(files) ? files : [files]).filter(f => f.name !== file.name);
        $('#file-upload-list').remove(`#upload-list-item-${index}`);
    }

    files: File | File[] = [];

    formats = Object.freeze([
        'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
        'application/msword',
        'application/pdf',
        'text/plain'
    ]);

} 