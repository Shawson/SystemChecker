import { Component, Input } from '@angular/core';
import { Condition, Rule } from '../../services/check.service';

import { LookupService, DictionaryNumericIndex } from '../../services/lookup.service'

@Component({
    selector: 'rule-editor',
    templateUrl: './rule-editor.component.html',
    providers: [ LookupService ]
})

export class RuleEditorComponent {
    @Input() rule: Rule;

    public comparators: DictionaryNumericIndex;
    public operators: DictionaryNumericIndex;

    constructor(private lookupService: LookupService) {

        lookupService.getComparators().subscribe(data => this.comparators = data);
        lookupService.getOperators().subscribe(data => this.operators = data);

    }
}