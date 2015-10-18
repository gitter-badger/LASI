System.config({
  baseURL: "/",
  defaultJSExtensions: true,
  transpiler: "none",
  paths: {
    "github:*": "jspm_packages/github/*",
    "npm:*": "jspm_packages/npm/*"
  },
  bundles: {
    "app.build.js": [
      "app/app.js",
      "github:angular/bower-angular@1.4.7",
      "npm:font-awesome@4.4.0",
      "app/debug/debug.module.js",
      "app/widgets/widgets.module.js",
      "github:twbs/bootstrap@3.3.5",
      "app/document-upload/document-upload.module.js",
      "app/document-viewer/search/search.module.js",
      "app/LASI.js",
      "app/document-list/document-list.module.js",
      "app/document-viewer/document-viewer.module.js",
      "app/utilities/augmentations.js",
      "app/angular-shim.js",
      "app/debug/debug-panel.directive.js",
      "app/widgets/document-list.js",
      "github:twbs/bootstrap@3.3.5/js/bootstrap",
      "app/document-viewer/search/search-box.js",
      "app/document-upload/upload-panel.js",
      "app/document-list/documents.service.js",
      "app/document-list/results.service.js",
      "app/document-list/document-list-service.provider.js",
      "app/document-list/tasks-list-service.provider.js",
      "github:angular/bower-angular@1.4.7/angular",
      "app/document-list/list.controller.js",
      "app/document-list/document-list-menu-item.directive.js",
      "app/document-viewer/document.controller.js",
      "app/document-viewer/document-model.service.js",
      "app/document-list/document-list-tabset-item.directive.js",
      "app/document-viewer/directives/paragraph.js",
      "app/document-viewer/directives/page.js",
      "app/document-viewer/directives/document-viewer.js",
      "app/document-viewer/lexical-menu-builder.service.js",
      "app/document-viewer/directives/sentence.js",
      "app/document-viewer/directives/phrase.js",
      "dist/app.css!github:systemjs/plugin-css@0.1.18",
      "github:twbs/bootstrap@3.3.5/css/bootstrap.css!github:systemjs/plugin-css@0.1.18",
      "npm:font-awesome@4.4.0/css/font-awesome.css!github:systemjs/plugin-css@0.1.18",
      "github:components/jquery@2.1.4",
      "github:angular/bower-angular-resource@1.4.7",
      "github:angular-ui/bootstrap-bower@0.14.2",
      "github:danialfarid/ng-file-upload-bower@9.0.13",
      "github:Templarian/ui.bootstrap.contextMenu@0.9.4",
      "app/document-upload/upload.controller.js",
      "github:angular/bower-angular-resource@1.4.7/index",
      "github:angular-ui/bootstrap-bower@0.14.2/index",
      "github:Templarian/ui.bootstrap.contextMenu@0.9.4/contextMenu",
      "github:danialfarid/ng-file-upload-bower@9.0.13/ng-file-upload",
      "github:components/jquery@2.1.4/jquery",
      "app/document-viewer/search/search-box.html",
      "app/debug/debug-panel.directive.html",
      "github:angular/bower-angular-resource@1.4.7/angular-resource",
      "app/document-viewer/directives/document-viewer.html",
      "app/document-viewer/directives/page.html",
      "app/document-viewer/directives/paragraph.html",
      "app/document-viewer/directives/sentence.html",
      "app/document-viewer/directives/phrase.html",
      "app/document-list/document-list-menu-item.directive.html",
      "github:angular-ui/bootstrap-bower@0.14.2/ui-bootstrap-tpls",
      "app/document-upload/upload-panel.html"
    ]
  },

  packages: {
    "app": {
      "main": "app.js",
      "defaultExtension": "js",
      "modules": {
        "*.html": {
          "loader": "text"
        }
      }
    },
    "bootstrap-css": {
      "css": "github:systemjs/plugin-css@0.1.18",
      "bootstrap-css": "github:twbs/bootstrap@3.3.5/css/bootstrap.css!"
    }
  },

  map: {
    "angular": "github:angular/bower-angular@1.4.7",
    "angular-bootstrap": "github:angular-ui/bootstrap-bower@0.14.2",
    "angular-bootstrap-contextmenu": "github:Templarian/ui.bootstrap.contextMenu@0.9.4",
    "angular-file-upload": "github:danialfarid/ng-file-upload-bower@9.0.13",
    "angular-resource": "github:angular/bower-angular-resource@1.4.7",
    "bootstrap": "github:twbs/bootstrap@3.3.5",
    "clean-css": "npm:clean-css@3.4.6",
    "css": "github:systemjs/plugin-css@0.1.18",
    "font-awesome": "npm:font-awesome@4.4.0",
    "jquery": "github:components/jquery@2.1.4",
    "jquery-validation": "github:jzaefferer/jquery-validation@1.14.0",
    "jquery-validation-unobtrusive": "github:aspnet/jquery-validation-unobtrusive@3.2.3",
    "text": "github:systemjs/plugin-text@0.0.2",
    "github:aspnet/jquery-validation-unobtrusive@3.2.3": {
      "jquery-validation": "github:jzaefferer/jquery-validation@1.14.0"
    },
    "github:jspm/nodelibs-assert@0.1.0": {
      "assert": "npm:assert@1.3.0"
    },
    "github:jspm/nodelibs-buffer@0.1.0": {
      "buffer": "npm:buffer@3.5.1"
    },
    "github:jspm/nodelibs-events@0.1.1": {
      "events": "npm:events@1.0.2"
    },
    "github:jspm/nodelibs-http@1.7.1": {
      "Base64": "npm:Base64@0.2.1",
      "events": "github:jspm/nodelibs-events@0.1.1",
      "inherits": "npm:inherits@2.0.1",
      "stream": "github:jspm/nodelibs-stream@0.1.0",
      "url": "github:jspm/nodelibs-url@0.1.0",
      "util": "github:jspm/nodelibs-util@0.1.0"
    },
    "github:jspm/nodelibs-https@0.1.0": {
      "https-browserify": "npm:https-browserify@0.0.0"
    },
    "github:jspm/nodelibs-os@0.1.0": {
      "os-browserify": "npm:os-browserify@0.1.2"
    },
    "github:jspm/nodelibs-path@0.1.0": {
      "path-browserify": "npm:path-browserify@0.0.0"
    },
    "github:jspm/nodelibs-process@0.1.2": {
      "process": "npm:process@0.11.2"
    },
    "github:jspm/nodelibs-stream@0.1.0": {
      "stream-browserify": "npm:stream-browserify@1.0.0"
    },
    "github:jspm/nodelibs-url@0.1.0": {
      "url": "npm:url@0.10.3"
    },
    "github:jspm/nodelibs-util@0.1.0": {
      "util": "npm:util@0.10.3"
    },
    "github:jzaefferer/jquery-validation@1.14.0": {
      "jquery": "github:components/jquery@2.1.4"
    },
    "github:twbs/bootstrap@3.3.5": {
      "jquery": "github:components/jquery@2.1.4"
    },
    "npm:amdefine@1.0.0": {
      "fs": "github:jspm/nodelibs-fs@0.1.2",
      "module": "github:jspm/nodelibs-module@0.1.0",
      "path": "github:jspm/nodelibs-path@0.1.0",
      "process": "github:jspm/nodelibs-process@0.1.2"
    },
    "npm:assert@1.3.0": {
      "util": "npm:util@0.10.3"
    },
    "npm:buffer@3.5.1": {
      "base64-js": "npm:base64-js@0.0.8",
      "ieee754": "npm:ieee754@1.1.6",
      "is-array": "npm:is-array@1.0.1"
    },
    "npm:clean-css@3.4.6": {
      "buffer": "github:jspm/nodelibs-buffer@0.1.0",
      "commander": "npm:commander@2.8.1",
      "fs": "github:jspm/nodelibs-fs@0.1.2",
      "http": "github:jspm/nodelibs-http@1.7.1",
      "https": "github:jspm/nodelibs-https@0.1.0",
      "os": "github:jspm/nodelibs-os@0.1.0",
      "path": "github:jspm/nodelibs-path@0.1.0",
      "process": "github:jspm/nodelibs-process@0.1.2",
      "source-map": "npm:source-map@0.4.4",
      "url": "github:jspm/nodelibs-url@0.1.0",
      "util": "github:jspm/nodelibs-util@0.1.0"
    },
    "npm:commander@2.8.1": {
      "child_process": "github:jspm/nodelibs-child_process@0.1.0",
      "events": "github:jspm/nodelibs-events@0.1.1",
      "fs": "github:jspm/nodelibs-fs@0.1.2",
      "graceful-readlink": "npm:graceful-readlink@1.0.1",
      "path": "github:jspm/nodelibs-path@0.1.0",
      "process": "github:jspm/nodelibs-process@0.1.2"
    },
    "npm:core-util-is@1.0.1": {
      "buffer": "github:jspm/nodelibs-buffer@0.1.0"
    },
    "npm:font-awesome@4.4.0": {
      "css": "github:systemjs/plugin-css@0.1.18"
    },
    "npm:graceful-readlink@1.0.1": {
      "fs": "github:jspm/nodelibs-fs@0.1.2"
    },
    "npm:https-browserify@0.0.0": {
      "http": "github:jspm/nodelibs-http@1.7.1"
    },
    "npm:inherits@2.0.1": {
      "util": "github:jspm/nodelibs-util@0.1.0"
    },
    "npm:os-browserify@0.1.2": {
      "os": "github:jspm/nodelibs-os@0.1.0"
    },
    "npm:path-browserify@0.0.0": {
      "process": "github:jspm/nodelibs-process@0.1.2"
    },
    "npm:process@0.11.2": {
      "assert": "github:jspm/nodelibs-assert@0.1.0"
    },
    "npm:punycode@1.3.2": {
      "process": "github:jspm/nodelibs-process@0.1.2"
    },
    "npm:readable-stream@1.1.13": {
      "buffer": "github:jspm/nodelibs-buffer@0.1.0",
      "core-util-is": "npm:core-util-is@1.0.1",
      "events": "github:jspm/nodelibs-events@0.1.1",
      "inherits": "npm:inherits@2.0.1",
      "isarray": "npm:isarray@0.0.1",
      "process": "github:jspm/nodelibs-process@0.1.2",
      "stream-browserify": "npm:stream-browserify@1.0.0",
      "string_decoder": "npm:string_decoder@0.10.31"
    },
    "npm:source-map@0.4.4": {
      "amdefine": "npm:amdefine@1.0.0",
      "process": "github:jspm/nodelibs-process@0.1.2"
    },
    "npm:stream-browserify@1.0.0": {
      "events": "github:jspm/nodelibs-events@0.1.1",
      "inherits": "npm:inherits@2.0.1",
      "readable-stream": "npm:readable-stream@1.1.13"
    },
    "npm:string_decoder@0.10.31": {
      "buffer": "github:jspm/nodelibs-buffer@0.1.0"
    },
    "npm:url@0.10.3": {
      "assert": "github:jspm/nodelibs-assert@0.1.0",
      "punycode": "npm:punycode@1.3.2",
      "querystring": "npm:querystring@0.2.0",
      "util": "github:jspm/nodelibs-util@0.1.0"
    },
    "npm:util@0.10.3": {
      "inherits": "npm:inherits@2.0.1",
      "process": "github:jspm/nodelibs-process@0.1.2"
    }
  }
});
