﻿namespace LASI.documentViewer {
    'use strict';

    export interface DocumentModelService {
        processDocument(documentId: string): angular.IHttpPromise<models.DocumentModel>;
    }
    documentModelService.$inject = ['$http'];
    function documentModelService($http: angular.IHttpService): DocumentModelService {

        return {
            processDocument: (documentId) => $http.get(`Analysis/${documentId}`, { cache: false })
        };
    }

    angular.module('documentViewer').factory({ documentModelService });
}