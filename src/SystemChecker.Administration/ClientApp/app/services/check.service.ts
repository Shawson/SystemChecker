import { Inject, Injectable } from "@angular/core";
import { Http, Response } from "@angular/http";
import { Observable } from "rxjs/Observable";

import { ICheck, ICheckService, ICheckSuite } from "../interfaces";

@Injectable()
export class CheckService implements ICheckService {
    private checkSuiteCache: ICheckSuite[];

    constructor(private http: Http, @Inject("ORIGIN_URL") private originUrl: string) { }

    public getAllChecks(): Observable<ICheck[]> {
        return this.http
            .get(this.originUrl + "/api/Check/GetAll")
            .map(this.extractData)
            .catch(this.handleError);
    }

    public getCheck(id: number): Observable<ICheck> {
        return this.http
            .get(this.originUrl + "/api/Check/GetById/" + id)
            .map((res: Response) => {
                const body = res.json();

                // get rid of tabs, these are one liners anyway
                let settingsJson = body.settings;
                if (settingsJson) {
                    settingsJson = settingsJson.replace(/\t/g, "");
                    settingsJson = settingsJson.replace(/\r\n/g, "");
                }

                body.settings = JSON.parse(settingsJson);
                body.outcomes = JSON.parse(body.outcomes);
                body.triggers = JSON.parse(body.triggers);
                return body || {};
            })
            .catch(this.handleError);
    }

    public getCheckSuites(): Observable<ICheckSuite[]> {
        if (!this.checkSuiteCache) {
            return this.http
                .get(this.originUrl + "/api/CheckSuite/GetAll")
                .map(this.extractData)
                .do(checkSuites => this.checkSuiteCache = checkSuites)
                .catch(this.handleError);
        } else {
            return Observable.of(this.checkSuiteCache);
        }
    }

    private extractData(res: Response) {
        const body = res.json();
        return body || {};
    }

    private handleError(error: Response | any) {
        // In a real world app, you might use a remote logging infrastructure
        let errMsg: string;
        if (error instanceof Response) {
            const body = error.json() || "";
            const err = body.error || JSON.stringify(body);
            errMsg = `${error.status} - ${error.statusText || ""} ${err}`;
        } else {
            errMsg = error.message ? error.message : error.toString();
        }
        console.error(errMsg);
        return Observable.throw(errMsg);
    }
}
