import { Inject, Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable()
export class CheckService {

    constructor(private http: Http, @Inject('ORIGIN_URL') private originUrl: string) { }

    getAllChecks(): Observable<Check[]> {
        return this.http
            .get(this.originUrl + '/api/Check/GetAll')
            .map(this.extractData)
            .catch(this.handleError);
    }

    getCheck(id: number): Observable<Check> {
        return this.http
            .get(this.originUrl + '/api/Check/GetById/' + id)
            .map(this.extractData)
            .catch(this.handleError);
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
    disabled: number;
    outcomes: string;
    settings: string;
    systemName: string;
    triggers: string;
    updated: string;
}
