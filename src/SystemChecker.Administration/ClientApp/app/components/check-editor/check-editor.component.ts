import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";

import { ICheck, ICheckSuite } from "../../interfaces";
import { CheckService } from "../../services/check.service";

@Component({
    selector: "check-editor",
    templateUrl: "./check-editor.component.html",
})
export class CheckEditorComponent implements OnInit {

    public check: ICheck;
    public checkSuites: ICheckSuite[];

    constructor(private checkService: CheckService, private route: ActivatedRoute) { }

    public ngOnInit() {
        this.route.params.forEach((params: Params) => {

            const checkId = parseInt(params.id);
            if (checkId > 0) {
                this.checkService.getCheck(checkId).subscribe(
                    data => {
                        this.check = data;
                    });
            }
        });

        this.checkService.getCheckSuites().subscribe(data => this.checkSuites = data);
    }

    public save() {
        console.log("saving...");
    }
}
