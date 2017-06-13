import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Check, CheckService } from '../../services/check.service'

@Component({
    selector: 'check-list',
    templateUrl: './check-list.component.html',
    providers: [CheckService]
})
export class CheckListComponent {
    public checks: Check[];

    constructor(private checkService: CheckService, private router: Router) {

        checkService.getAllChecks().subscribe(
            lookup => {
                this.checks = lookup;
            });
    }

    detail(check: Check) {
        this.router.navigate(['/check-editor', check.checkId]);
    }
}