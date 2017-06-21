import { Observable } from "rxjs/Observable";
import { IDictionaryNumericIndex } from ".";

export interface ILookupService {
    getComparators(): Observable<IDictionaryNumericIndex>;
    getOperators(): Observable<IDictionaryNumericIndex>;
}
