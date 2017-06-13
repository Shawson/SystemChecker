import { Component, Input } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { Check, CheckService } from '../../services/check.service'

@Component({
    selector: 'check-editor',
    templateUrl: './check-editor.component.html',
    providers: [CheckService]
})

export class CheckEditorComponent {

    public check: Check;

    constructor(private checkService: CheckService, private route: ActivatedRoute) {

        this.route.params.forEach((params: Params) => {

            let checkId = +params['id']; // (+) converts string 'id' to a number
            if (checkId > 0) {
                checkService.getCheck(checkId).subscribe(
                    data => {
                        this.check = data;
                    });
            }
        });
    }

    save() {
        console.log('saving...');
    }
}