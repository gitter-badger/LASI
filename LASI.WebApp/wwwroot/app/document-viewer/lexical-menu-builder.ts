﻿module LASI.documentViewer {
    'use strict';

    angular
        .module('documentViewer')
        .factory('lexicalMenuBuilder', lexicalMenuBuilder);


    lexicalMenuBuilder.$inject = [];
    import contextMenu = ui.bootstrap.contextMenu;
    export interface ILexicalMenuBuilderFactory {
        buildAngularMenu: (source: IVerbalContextmenuDataSource | IReferencerContextmenuDataSource) => contextMenu.ContextMenu;
    }
    function lexicalMenuBuilder(): ILexicalMenuBuilderFactory {
        var [buildForVerbal, buildForReferencer] = [createForVerbalMenuBuilder({}), createForReferencerMenuBuilder({})];

        return {
            buildAngularMenu: source =>
                referencerMenuIsViable(<IReferencerContextmenuDataSource>source) ?
                    buildForReferencer(<IReferencerContextmenuDataSource>source) :
                    verbalMenuIsViable(<IVerbalContextmenuDataSource>source) ?
                        buildForVerbal(<IVerbalContextmenuDataSource>source) :
                        undefined
        };

        function verbalMenuIsViable(menu: IVerbalContextmenuDataSource) {
            return !!(menu && (menu.directObjectIds || menu.indirectObjectIds || menu.subjectIds));
        }
        function referencerMenuIsViable(menu: IReferencerContextmenuDataSource) {
            return !!(menu && menu.refersToIds);
        }

    } function createForReferencerMenuBuilder(menuActionTargets: { [id: string]: JQuery }) {
        return (source: IReferencerContextmenuDataSource): contextMenu.ContextMenu => {
            return [
                ['View Referred To', (itemScope, event) => {
                    resetReferencerAsssotionCssClasses();
                    source.refersToIds
                        .forEach(id => menuActionTargets[id] = $(`#${id}`).addClass('referred-to-by-current'));
                }]
            ];
        };
        function resetReferencerAsssotionCssClasses() {
            Object.keys(menuActionTargets)
                .map(key => menuActionTargets[key])
                .forEach($e => $e.removeClass('referred-to-by-current'));
        }
    }
    function createForVerbalMenuBuilder(menuActionTargets: { [id: string]: JQuery }) {
        return (function (verbalMenuCssClassMap: { [mapping: string]: string }) {
            return (source: IVerbalContextmenuDataSource) => {
                var menuItems: contextMenu.ContextMenu = [];
                if (source.subjectIds) {
                    menuItems.push(['View Subjects', (itemScope, event) => {
                        resetVerbalAssociationCssClasses();
                        source.subjectIds
                            .forEach(id => {
                            menuActionTargets[id] = $(`#${id}`).addClass(verbalMenuCssClassMap['View Subjects']);
                        });
                    }]);
                }
                if (source.directObjectIds) {
                    menuItems.push(['View Direct Objects', (itemScope, event) => {
                        resetVerbalAssociationCssClasses();
                        source.directObjectIds
                            .forEach(id => menuActionTargets[id] = $(`#${id}`).addClass(verbalMenuCssClassMap['View Direct Objects']));
                    }]);
                }
                if (source.indirectObjectIds) {
                    menuItems.push(['View Indirect Objects', (itemScope, event) => {
                        resetVerbalAssociationCssClasses();
                        source.indirectObjectIds.forEach(id => {
                            menuActionTargets[id] = $(`#${id}`).addClass(verbalMenuCssClassMap['View Indirect Objects']);
                        });
                    }]);
                }
                return menuItems;
            };
            function resetVerbalAssociationCssClasses() {
                Object.keys(menuActionTargets)
                    .map(key => menuActionTargets[key])
                    .forEach($e =>
                    Object.keys(verbalMenuCssClassMap)
                        .map((k: string): string => verbalMenuCssClassMap[k])
                        .forEach(cssClass => $e.removeClass(cssClass)));
            }
        })({
            'View Subjects': 'subject-of-current',
            'View Direct Objects': 'direct-object-of-current',
            'View Indirect Objects': 'indirect-object-of-current'
        });
    }
}