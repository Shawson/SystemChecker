/// <binding BeforeBuild='restore' />
"use strict";

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    htmlmin = require("gulp-htmlmin"),
    uglify = require("gulp-uglify"),
    merge = require("merge-stream"),
    del = require("del"),
    bundleconfig = require("./bundleconfig.json"); // make sure bundleconfig.json doesn't contain any comments

gulp.task("min", ["min:js", "min:css", "min:html"]);

gulp.task("min:js", function () {
    var tasks = getBundles(".js").map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(uglify())
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("min:css", function () {
    var tasks = getBundles(".css").map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(cssmin())
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("min:html", function () {
    var tasks = getBundles(".html").map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(htmlmin({ collapseWhitespace: true, minifyCSS: true, minifyJS: true }))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("clean", function () {
    var files = bundleconfig.map(function (bundle) {
        return bundle.outputFileName;
    });

    return del(files);
});

gulp.task("watch", function () {
    getBundles(".js").forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:js"]);
    });

    getBundles(".css").forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:css"]);
    });

    getBundles(".html").forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:html"]);
    });
});

gulp.task('restore', function () {

    var filesToCopy = [
            { from: 'node_modules/@angular/**/*.js', to: '@angular' },
            { from: 'node_modules/angular-in-memory-web-api/*.js', to: 'angular-in-memory-web-api' },
            { from: 'node_modules/rxjs/**/*.js', to: 'rxjs' },
            { from: 'node_modules/systemjs/dist/*.js', to: 'systemjs' },
            { from: 'node_modules/zone.js/dist/*.js', to: 'zone.js' },
            { from: 'node_modules/core-js/client/*.js', to: 'core-js' },
            { from: 'node_modules/reflect-metadata/reflect.js', to: 'reflect-metadata' },
            { from: 'node_modules/primeng/**/*.*', to: 'primeng' },
            { from: 'node_modules/quill/**/*.*', to: 'quill' }
    ];

    for (var i = 0; i < filesToCopy.length; i++) {

        gulp.src([
            filesToCopy[i].from, '!node_modules/**/node_modules/**/*.*'
        ]).pipe(gulp.dest('wwwroot/lib/' + filesToCopy[i].to + ''));

    }

});

function getBundles(extension) {
    return bundleconfig.filter(function (bundle) {
        return new RegExp(extension).test(bundle.outputFileName);
    });
}