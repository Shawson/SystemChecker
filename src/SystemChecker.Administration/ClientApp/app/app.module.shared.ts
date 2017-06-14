import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { CheckListComponent } from './components/check-list/check-list.component';
import { CheckEditorComponent } from './components/check-editor/check-editor.component';

import { DictionaryEditorComponent } from './components/dictionary-editor/dictionary-editor.component'

import { CheckService } from './services/check.service';

export const sharedConfig: NgModule = {
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,

        CheckListComponent,
        CheckEditorComponent,

        DictionaryEditorComponent,

        HomeComponent
    ],
    imports: [
        BrowserModule,
        FormsModule,
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
