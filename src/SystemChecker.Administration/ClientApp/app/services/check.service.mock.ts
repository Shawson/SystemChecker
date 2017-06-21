import { Observable } from "rxjs/Observable";

import { ICheck, ICheckService, ICheckSuite } from "../interfaces";

export class MockCheckService implements ICheckService {
    public getAllChecks(): Observable<ICheck[]> {
        const checks = [];
        for(let i=1;i<5;i++) {
            checks.push(this.getFakeCheck(i));
        }
        return Observable.of(checks);
    }

    public getCheck(id: number): Observable<ICheck> {
        return Observable.of(this.getFakeCheck(id));
    }

    public getCheckSuites(): Observable<ICheckSuite[]> {
        const suites = [];
        for(let i=1;i<5;i++) {
            suites.push(this.getFakeSuite(i));
        }
        return Observable.of(suites);
    }

    private getFakeCheck(id: number): ICheck {
        return {
            checkId: id,
            checkSuiteId: 0,
            checkTypeId: 0,
            disabled: new Date(),
            outcomes: [],
            settings: {},
            systemName: "Test",
            triggers: "",
            updated: "",
        };
    }

    private getFakeSuite(id: number): ICheckSuite {
        return {
            checkSuiteId: id,
            suiteName: "Test Suite",
        };
    }
}
