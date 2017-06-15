import { Inject, Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/do';
import 'rxjs/add/observable/of';
import 'rxjs/add/observable/throw'

@Injectable()
export class CheckService {

    constructor(private http: Http, @Inject('ORIGIN_URL') private originUrl: string) { }

    checkSuiteCache: CheckSuite[];

    getAllChecks(): Observable<Check[]> {
        return this.http
            .get(this.originUrl + '/api/Check/GetAll')
            .map(this.extractData)
            .catch(this.handleError);
    }

    getCheck(id: number): Observable<Check> {
        return this.http
            .get(this.originUrl + '/api/Check/GetById/' + id)
            .map((res: Response) => {
                let body = res.json();

                // get rid of tabs, these are one liners anyway
                var settingsJson = body.settings;
                if (settingsJson) {
                    settingsJson = settingsJson.replace(/\t/g, '');
                    settingsJson = settingsJson.replace(/\r\n/g, '');
                }

                body.settings = JSON.parse(settingsJson);
                body.outcomes = JSON.parse(body.outcomes);
                body.triggers = JSON.parse(body.triggers);
                return body || {};
            })
            .catch(this.handleError);
    }

    getCheckSuites(): Observable<CheckSuite[]> {
        if (!this.checkSuiteCache) {
            return this.http
                .get(this.originUrl + '/api/CheckSuite/GetAll')
                .map(this.extractData)
                .do(checkSuites => this.checkSuiteCache = checkSuites)
                .catch(this.handleError);
        }
        else {
            return Observable.of(this.checkSuiteCache);
        }
    }

    private extractData(res: Response) {
        let body = res.json();
        return body || {};
    }

    private handleError(error: Response | any) {
        // In a real world app, you might use a remote logging infrastructure
        let errMsg: string;
        if (error instanceof Response) {
            const body = error.json() || '';
            const err = body.error || JSON.stringify(body);
            errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
        } else {
            errMsg = error.message ? error.message : error.toString();
        }
        console.error(errMsg);
        return Observable.throw(errMsg);
    }
}

export class Check {
    checkId: number;
    checkSuiteId: number;
    checkTypeId: number;
    disabled: Date;
    get disabledChecked(): boolean {
        return this.disabled != null;
    }
    set disabledChecked(newval: boolean) 
    {
        this.disabled = new Date();
    }
    outcomes: Outcome[];
    settings: DictionaryStringIndex;
    systemName: string;
    triggers: string;
    updated: string;
}

export class Outcome {
    successStatus: number;
    description: string;
    conditions: Condition[];
}

export class Condition {
    rules: Rule[];
}

export class Rule {
    rules: Rule[];
    operator: number;
    valueA: string;
    valueB: string;
    comparator: number;
}

export interface DictionaryStringIndex {
    [index: string]: string
}

export class CheckSuite {
    checkSuiteId: number;
    suiteName: string;
}