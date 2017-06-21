import { Observable } from "rxjs/Observable";
import { ICheck, ICheckSuite } from ".";

export interface ICheckService {
    getAllChecks(): Observable<ICheck[]>;
    getCheck(id: number): Observable<ICheck>;
    getCheckSuites(): Observable<ICheckSuite[]>;
}
