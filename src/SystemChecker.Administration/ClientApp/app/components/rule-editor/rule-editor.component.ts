import { Component, Input, OnInit } from "@angular/core";

import { IDictionaryNumericIndex, IRule } from "../../interfaces";
import { LookupService } from "../../services/lookup.service";

@Component({
    selector: "rule-editor",
    templateUrl: "./rule-editor.component.html",
})
export class RuleEditorComponent implements OnInit {
    @Input() public rule: IRule;

    public comparators: IDictionaryNumericIndex;
    public operators: IDictionaryNumericIndex;

    constructor(private lookupService: LookupService) { }

    public ngOnInit() {
        this.lookupService.getComparators().subscribe(data => this.comparators = data);
        this.lookupService.getOperators().subscribe(data => this.operators = data);
    }
}
