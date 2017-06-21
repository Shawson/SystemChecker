import { Observable } from "rxjs/Observable";

import { IDictionaryNumericIndex, ILookupService } from "../interfaces";

export class MockLookupService implements ILookupService {
    public getComparators(): Observable<IDictionaryNumericIndex> {
       return Observable.of([]);
    }

    public getOperators(): Observable<IDictionaryNumericIndex> {
        return Observable.of([]);
    }
}
