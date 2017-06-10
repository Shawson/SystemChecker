import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { CheckListComponent } from './components/check-list/check-list.component';
import { CheckEditorComponent } from './components/check-editor/check-editor.component';

import { CheckService } from './services/check.service';

export const sharedConfig: NgModule = {
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,

        CheckListComponent,
        CheckEditorComponent,

        HomeComponent
    ],
    imports: [
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'check-list', component: CheckListComponent },
            { path: 'check-editor/:id', component: CheckEditorComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [
        CheckService
    ]
};
