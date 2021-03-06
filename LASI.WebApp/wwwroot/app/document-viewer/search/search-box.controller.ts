﻿export default class SearchBoxController {
    static $inject = ['$q'];
    constructor(private $q: angular.IQService) { }
    phrases: PhraseModel[];
    words: WordModel[];

    getWords() {
        return (this.phrases || []).flatMap(p => p.words);
    }
    find: SearchModel;
    searchContext: TextFragmentModel[];

    search(searchOptions: SearchOptions, searchContext: TextFragmentModel[]) {
        const deferred = this.$q.defer<SearchModel[]>();
        const value = searchOptions.value;
        const term = typeof value === 'string'
            ? value
            : typeof value !== 'undefined'
                ? value.detailText
                : undefined;
        if (!term) {
            deferred.reject('search term was undefined');
        } else if (!searchContext) {
            deferred.reject('nothing to search');
            this.phrases.forEach(phrase => phrase.style.cssClass = phrase.style.cssClass.replace('matched-by-search', ''));
        } else {
            this.phrases = this.phrases || searchContext
                .flatMap(m => m.paragraphs)
                .flatMap(p => p.sentences)
                .flatMap(s => s.phrases);

            const results: PhraseModel[] = [];
            this.phrases.forEach(phrase => {
                const matched = phrase.words.some(word => word.text === value);
                if (!matched) {
                    phrase.style.cssClass = phrase.style.cssClass.replace('matched-by-search', '');
                } else {
                    phrase.style.cssClass += ' matched-by-search';
                    results.push(phrase);
                }
            });
            deferred.resolve(results.map(r => r.text));
        }
        return deferred.promise;
    }
}

type SearchModel = string | LexicalModel;
interface SearchOptions {
    value: SearchModel;
    lifted?: boolean;
}