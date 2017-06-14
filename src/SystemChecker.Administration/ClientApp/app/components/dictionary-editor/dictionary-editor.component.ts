import { Component, Input } from '@angular/core';
import { Dictionary } from '../../services/check.service';

@Component({
    selector: 'dictionary-editor',
    templateUrl: './dictionary-editor.component.html',
})

export class DictionaryEditorComponent {
    @Input() dictionary: Dictionary;

    keys(): Array<string> {
        return Object.keys(this.dictionary);
    }
}