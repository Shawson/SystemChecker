import * as ExtractTextPlugin from "extract-text-webpack-plugin";
import * as path from "path";
import * as webpack from "webpack";
import { BundleAnalyzerPlugin } from "webpack-bundle-analyzer";

module.exports = (env: any) => {
    const extractCSS = new ExtractTextPlugin("vendor.css");
    const prod = env && env.prod as boolean;
    console.log(prod ? "Production" : "Dev" + " vendor build");
    const analyse = env && env.analyse as boolean;
    if (analyse) { console.log("Analysing build"); }
    const outputDir = "./wwwroot/dist";
    const bundleConfig = {
        stats: { modules: false },
        resolve: {
            extensions: [".js"],
            alias: {
                pace: "pace-progress",
            },
        },
        module: {
            rules: [
                { test: /\.(png|woff|woff2|eot|ttf|svg|gif)(\?|$)/, use: "url-loader?limit=100000" },
                { test: /\.css(\?|$)/, use: extractCSS.extract({ use: prod ? "css-loader?minimize" : "css-loader" }) },
            ],
        },
        entry: {
            vendor: [
                "@angular/animations",
                "@angular/common",
                "@angular/compiler",
                "@angular/core",
                "@angular/forms",
                "@angular/http",
                "@angular/platform-browser",
                "@angular/platform-browser/animations",
                "@angular/platform-browser-dynamic",
                "@angular/router",
                "pace-progress/themes/black/pace-theme-center-simple.css",
                "pace-progress",
                "primeng/resources/primeng.min.css",
                "primeng/resources/themes/cruze/theme.css",
                'bootstrap',
                'bootstrap/dist/css/bootstrap.css',
                "event-source-polyfill",
                "jquery",
                "zone.js",
                "primeng/primeng",
                "reflect-metadata",
                "core-js",
                "rxjs",
                "css-loader/lib/css-base",
                "moment",
                "core-js/es6/string",
                "core-js/es6/array",
                "core-js/es6/object",
                "core-js/es7/reflect",
                "angular2-moment",
            ],
        },
        output: {
            publicPath: "/dist/",
            filename: "[name].js",
            library: "[name]_[hash]",
            path: path.join(__dirname, outputDir),
        },
        node: {
            fs: "empty",
        },
        plugins: [
            new webpack.ProvidePlugin({ $: "jquery", jQuery: "jquery", Hammer: "hammerjs/hammer" }), // Global identifiers
            new webpack.ContextReplacementPlugin(/\@angular\b.*\b(bundles|linker)/, path.join(__dirname, "./ClientApp")), // Workaround for https://github.com/angular/angular/issues/11580
            new webpack.ContextReplacementPlugin(/angular(\\|\/)core(\\|\/)@angular/, path.join(__dirname, "./ClientApp")), // Workaround for https://github.com/angular/angular/issues/14898
            new webpack.IgnorePlugin(/^vertx$/), // Workaround for https://github.com/stefanpenner/es6-promise/issues/100
            extractCSS,
            new webpack.DllPlugin({
                path: path.join(__dirname, outputDir, "[name]-manifest.json"),
                name: "[name]_[hash]",
            }),
        ].concat(prod ? [
            // Plugins that apply in production builds only
            new webpack.optimize.UglifyJsPlugin(),
        ] : [
                // Plugins that apply in development builds only
            ]).concat(analyse ? [
                new BundleAnalyzerPlugin({
                    analyzerMode: "static",
                    reportFilename: "vendor.html",
                    openAnalyzer: false,
                }),
            ] : []),
    };
    return bundleConfig;
};
