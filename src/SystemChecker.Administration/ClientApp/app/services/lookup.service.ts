import { Inject, Injectable } from "@angular/core";
import { Http, Response } from "@angular/http";
import { Observable } from "rxjs/Observable";

import { IDictionaryNumericIndex, ILookupService } from "../interfaces";

@Injectable()
export class LookupService implements ILookupService {
    private comparators: IDictionaryNumericIndex;
    private operators: IDictionaryNumericIndex;

    constructor(private http: Http, @Inject("ORIGIN_URL") private originUrl: string) { }

    public getComparators(): Observable<IDictionaryNumericIndex> {
        if (!this.comparators) {
            return this.http
                .get(this.originUrl + "/api/Lookup/Comparators")
                .map(this.extractData)
                .do(data => this.comparators = data)
                .catch(this.handleError);
        } else {
            return Observable.of(this.comparators);
        }
    }

    public getOperators(): Observable<IDictionaryNumericIndex> {
        if (!this.operators) {
            return this.http
                .get(this.originUrl + "/api/Lookup/Operators")
                .map(this.extractData)
                .do(data => this.operators = data)
                .catch(this.handleError);
        } else {
            return Observable.of(this.operators);
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
