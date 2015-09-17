﻿namespace LASI.documentViewer {
    'use strict';

    function documentPage(): angular.IDirective {
        function link(scope: angular.IScope, element: angular.IAugmentedJQuery, attrs: angular.IAttributes) {
            log(scope);
            log(element);
            log(attrs);
        }
        return {
            restrict: 'E',
            link: link,
            templateUrl: '/app/document-viewer/document-page.directive.html',
            replace: true,
            scope: {
                page: '=',
                document: '='
            }
        };


    }

    angular
        .module('documentViewer')
        .directive({ documentPage });
}