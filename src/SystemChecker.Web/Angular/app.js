(function () {
    'use strict';
    
    config.$inject = ['$routeProvider', '$locationProvider'];

    angular.module('systemCheckerApp', [
        'ngRoute'
    ]).config(config);

    function config($routeProvider, $locationProvider) {
        /*
        $routeProvider
            .when('/', {
                templateUrl: '/Angular/Views/list.html',
                controller: 'CheckListController'
            })
            .when('/checks/add', {
                templateUrl: '/Angular/Views/add.html',
                controller: 'CheckAddController'
            })
            .when('/checks/edit/:id', {
                templateUrl: '/Angular/Views/edit.html',
                controller: 'CheckEditController'
            })
            .when('/checks/delete/:id', {
                templateUrl: '/Angular/Views/delete.html',
                controller: 'CheckDeleteController'
            });
            */
        $locationProvider.html5Mode(true);
    }

}());