import { Component, Input } from '@angular/core';
import { DictionaryStringIndex } from '../../services/check.service';

@Component({
    selector: 'dictionary-editor',
    templateUrl: './dictionary-editor.component.html',
})

export class DictionaryEditorComponent {
    @Input() dictionary: DictionaryStringIndex;

    keys(): Array<string> {
        return Object.keys(this.dictionary);
    }
}