import { Component, Input } from "@angular/core";
import { IOutcome } from "../../interfaces";

@Component({
    selector: "outcome-editor",
    templateUrl: "./outcome-editor.component.html",
})
export class OutcomeEditorComponent {
    @Input() public outcome: IOutcome;
}
