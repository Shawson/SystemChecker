// Load required polyfills and testing libraries

import "reflect-metadata";

import "zone.js/dist/zone.js";

import "zone.js/dist/long-stack-trace-zone.js";

import "zone.js/dist/async-test.js";

import "zone.js/dist/fake-async-test.js";

import "zone.js/dist/sync-test.js";

import "zone.js/dist/proxy.js";

import "zone.js/dist/jasmine-patch.js";

import "../global.imports";

import * as testing from "@angular/core/testing";
import * as testingBrowser from "@angular/platform-browser-dynamic/testing";

// There's no typing for the `__karma__` variable. Just declare it as any
declare var __karma__: any;
declare var require: any;

// Prevent Karma from running prematurely
__karma__.loaded = () => undefined;

// First, initialize the Angular testing environment
testing.getTestBed().initTestEnvironment(
    testingBrowser.BrowserDynamicTestingModule,
    testingBrowser.platformBrowserDynamicTesting(),
);

// Then we find all the tests
const context = require.context("../", true, /\.spec\.ts$/);

// And load the modules
context.keys().map(context);

// Finally, start Karma to run the tests
__karma__.start();
