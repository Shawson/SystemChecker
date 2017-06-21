/// <reference types="jasmine" />

import { async, ComponentFixture, TestBed } from "@angular/core/testing";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { RouterTestingModule } from "@angular/router/testing";
import { Observable } from "rxjs/Observable";
import { CheckService, LookupService } from "../../services";
import { MockCheckService, MockLookupService } from "../../services/mock";
import { DictionaryEditorComponent } from "../dictionary-editor/dictionary-editor.component";
import { OutcomeEditorComponent } from "../outcome-editor/outcome-editor.component";
import { RuleEditorComponent } from "../rule-editor/rule-editor.component";
import { CheckEditorComponent } from "./check-editor.component";

let fixture: ComponentFixture<CheckEditorComponent>;

describe("Check editor component", () => {
    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [FormsModule, RouterTestingModule],
            declarations: [CheckEditorComponent, DictionaryEditorComponent, OutcomeEditorComponent, RuleEditorComponent],
            providers: [
                { provide: CheckService, useClass: MockCheckService },
                { provide: LookupService, useClass: MockLookupService },
                { provide: ActivatedRoute, useValue: {params: Observable.of({id: 15})}},
            ],
        }).compileComponents();
        fixture = TestBed.createComponent(CheckEditorComponent);
        fixture.detectChanges();
    });

    it("Territory code", async(() => {
        fixture.whenStable().then(() => {
            expect(fixture.componentInstance.check.systemName).toEqual("Test");
            expect(fixture.componentInstance.check.checkId).toEqual(15);
        });
    }));
});
