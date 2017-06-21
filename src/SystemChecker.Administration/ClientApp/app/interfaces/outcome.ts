import { ICondition } from ".";

export interface IOutcome {
    successStatus: number;
    description: string;
    conditions: ICondition[];
}
