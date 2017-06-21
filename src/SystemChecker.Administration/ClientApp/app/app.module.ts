import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { HttpModule } from "@angular/http";
import { BrowserModule } from "@angular/platform-browser";
import { RouterModule, Routes } from "@angular/router";

import { AppComponent } from "./components/app/app.component";
import { CheckEditorComponent } from "./components/check-editor/check-editor.component";
import { CheckListComponent } from "./components/check-list/check-list.component";
import { HomeComponent } from "./components/home/home.component";
import { NavMenuComponent } from "./components/navmenu/navmenu.component";

import { DictionaryEditorComponent } from "./components/dictionary-editor/dictionary-editor.component";
import { OutcomeEditorComponent } from "./components/outcome-editor/outcome-editor.component";
import { RuleEditorComponent } from "./components/rule-editor/rule-editor.component";

import { CheckService } from "./services/check.service";
import { LookupService } from "./services/lookup.service";

const routes: Routes = [
    { path: "", redirectTo: "home", pathMatch: "full" },
    { path: "home", component: HomeComponent },
    { path: "check-list", component: CheckListComponent },
    { path: "check-editor/:id", component: CheckEditorComponent },
    { path: "**", redirectTo: "home" },
];

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        RouterModule.forRoot(routes),
    ],
    declarations: [
        AppComponent,
        NavMenuComponent,

        CheckListComponent,
        CheckEditorComponent,

        DictionaryEditorComponent,
        OutcomeEditorComponent,
        RuleEditorComponent,

        HomeComponent,
    ],
    providers: [
        CheckService,
        LookupService,
        { provide: "ORIGIN_URL", useValue: location.origin },
    ],
    bootstrap: [AppComponent],
})
export class AppModule { }
