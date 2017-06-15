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
export class LookupService {

    constructor(private http: Http, @Inject('ORIGIN_URL') private originUrl: string) { }

    comparators: DictionaryNumericIndex;
    operators: DictionaryNumericIndex;

    getComparators(): Observable<DictionaryNumericIndex> {
        if (!this.comparators) {
            return this.http
                .get(this.originUrl + '/api/Lookup/Comparators')
                .map(this.extractData)
                .do(data => this.comparators = data)
                .catch(this.handleError);
        }
        else {
            return Observable.of(this.comparators);
        }
    }

    getOperators(): Observable<DictionaryNumericIndex> {
        if (!this.operators) {
            return this.http
                .get(this.originUrl + '/api/Lookup/Operators')
                .map(this.extractData)
                .do(data => this.operators = data)
                .catch(this.handleError);
        }
        else {
            return Observable.of(this.operators);
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

export interface DictionaryNumericIndex {
    [index: number]: string
}