import { Component, Input } from '@angular/core';
import { Outcome, Condition, Rule } from '../../services/check.service';

@Component({
    selector: 'outcome-editor',
    templateUrl: './outcome-editor.component.html',
})

export class OutcomeEditorComponent {
    @Input() outcome: Outcome;
}