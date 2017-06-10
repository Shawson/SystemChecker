import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'check-editor',
    templateUrl: './check-editor.component.html'
})
export class CheckEditorComponent {
    public checks: Check[];

    constructor(http: Http, @Inject('ORIGIN_URL') originUrl: string) {
        http.get(originUrl + '/api/Check/GetAll').subscribe(result => {
            this.checks = result.json() as Check[];
        });
    }
}

interface Check {
    checkId: number;
    checkSuiteId: number;
    checkTypeId: number;
    disabled: number;
    outcomes: string;
    settings: string;
    systemName: string;
    triggers: string;
    updated: string;
}
