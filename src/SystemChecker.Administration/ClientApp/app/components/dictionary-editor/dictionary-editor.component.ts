import { Component, Input } from "@angular/core";
import { IDictionaryStringIndex } from "../../interfaces";

@Component({
    selector: "dictionary-editor",
    templateUrl: "./dictionary-editor.component.html",
})
export class DictionaryEditorComponent {
    @Input() public dictionary: IDictionaryStringIndex;

    public keys(): string[] {
        return Object.keys(this.dictionary);
    }
}
