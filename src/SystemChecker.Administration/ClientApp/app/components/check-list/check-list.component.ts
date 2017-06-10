import { Component } from '@angular/core';
import { Check, CheckService } from '../../services/check.service'

@Component({
    selector: 'check-list',
    templateUrl: './check-list.component.html',
    providers: [CheckService]
})
export class CheckListComponent {
    public checks: Check[];

    constructor(private checkService: CheckService) {

        checkService.getAllChecks().subscribe(
            lookup => {
                this.checks = lookup;
            });
    }
}