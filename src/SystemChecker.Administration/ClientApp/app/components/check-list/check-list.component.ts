import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { ICheck } from "../../interfaces";
import { CheckService } from "../../services/check.service";

@Component({
    selector: "check-list",
    templateUrl: "./check-list.component.html",
})
export class CheckListComponent implements OnInit {
    public checks: ICheck[];

    constructor(private checkService: CheckService, private router: Router) { }

    public ngOnInit() {
        this.checkService.getAllChecks().subscribe(
            lookup => {
                this.checks = lookup;
            });
    }

    public detail(check: ICheck) {
        this.router.navigate(["/check-editor", check.checkId]);
    }
}
