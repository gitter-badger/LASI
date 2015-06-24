var LASI;
(function (LASI) {
    var documentList;
    (function (documentList) {
        'use strict';
        angular
            .module('documentList')
            .directive('documentListTabsetItem', documentListTabsetItem);
        documentListTabsetItem.$inject = ['resultsService'];
        function documentListTabsetItem(resultsService) {
            return {
                restrict: 'E',
                link: function (scope, element, attrs) {
                    element.click(function (event) {
                        event.stopPropagation();
                        resultsService.processDocument(scope.documentId, scope.name);
                        event.preventDefault();
                        var promise = resultsService.processDocument(scope.documentId, scope.name);
                        scope.analysisProgress = resultsService.tasks[scope.documentId].percentComplete;
                        scope.showProgress = true;
                        promise.then(function () { return scope.analysisProgress = resultsService.tasks[scope.documentId].percentComplete; });
                    });
                    console.log(attrs);
                },
                scope: {
                    documentId: '=',
                    name: '=',
                    percentComplete: '='
                },
                templateUrl: '/app/widgets/document-list/document-list-tabset-item.html'
            };
        }
    })(documentList = LASI.documentList || (LASI.documentList = {}));
})(LASI || (LASI = {}));
