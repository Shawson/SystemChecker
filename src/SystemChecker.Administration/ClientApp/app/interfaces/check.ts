import { IDictionaryStringIndex, IOutcome } from ".";

export interface ICheck {
    checkId: number;
    checkSuiteId: number;
    checkTypeId: number;
    disabled: Date;
    outcomes: IOutcome[];
    settings: IDictionaryStringIndex;
    systemName: string;
    triggers: string;
    updated: string;
}
