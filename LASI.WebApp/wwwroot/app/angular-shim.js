System.register(['jquery', 'angular', 'angular-bootstrap', 'angular-bootstrap-contextmenu', 'angular-file-upload'], function(exports_1) {
    var $, angular, angularBootstrap, angularBootstrapContextmenu, angularFileUpload;
    return {
        setters:[
            function ($_1) {
                $ = $_1;
            },
            function (angular_1) {
                angular = angular_1;
            },
            function (angularBootstrap_1) {
                angularBootstrap = angularBootstrap_1;
            },
            function (angularBootstrapContextmenu_1) {
                angularBootstrapContextmenu = angularBootstrapContextmenu_1;
            },
            function (angularFileUpload_1) {
                angularFileUpload = angularFileUpload_1;
            }],
        execute: function() {
            exports_1("default",{
                $: $,
                angular: angular,
                angularBootstrap: angularBootstrap,
                angularBootstrapContextmenu: angularBootstrapContextmenu,
                angularFileUpload: angularFileUpload
            });
        }
    }
});
