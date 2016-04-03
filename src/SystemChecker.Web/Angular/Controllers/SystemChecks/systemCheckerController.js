(function () {
    'use strict';

    angular
        .module('systemCheckerApp')
        .factory('systemChecksService', systemChecksService)
        .controller('systemCheckerController', systemCheckerController);

    systemCheckerController.$inject = ['$scope', 'systemChecksService'];

    function systemCheckerController($scope, systemChecksService) {

        $scope.Checks = [];

        activate();

        function activate() {
            systemChecksService
                .getChecks()
                .then(function (result) {

                    $scope.Checks = result.data;

                    for (var i = 0; i < $scope.Checks.length; i++) {
                        var check = $scope.Checks[i];
                        if (check.Outcomes !== null)
                            check.Outcomes = angular.fromJson(check.Outcomes.replace(/\n/g, '').replace(/\t/g, '').replace(/\r/g, ''));

                        if (check.Settings !== null)
                            check.Settings = angular.fromJson(check.Settings.replace(/\n/g, '').replace(/\t/g, '').replace(/\r/g, ''));

                        populateTriggerClosure(check)
                    }
                });
        };

        function populateTriggerClosure(check) {
            systemChecksService.getTriggersForCheck(check.CheckId).then(function (trigger) {
                check.Triggers = trigger.data;
            });
        };
    }

    systemChecksService.$inject = ['$http'];

    function systemChecksService($http) {

        var _getChecks = function () {
            return $http({
                method: 'GET',
                url: 'api/Checks'
            }).then(function (response) {
                return response;
            });
        };

        var _getCheckTypes = function () {
            return $http({
                method: 'GET',
                url: 'api/CheckTypes'
            }).then(function (response) {
                return response;
            });
        }

        var _getTriggersForCheck = function (checkId) {
            return $http({
                method: 'GET',
                url: 'api/Triggers/' + checkId
            }).then(function (response) {
                return response;
            });
        }

        return {
            getChecks: _getChecks,
            getCheckTypes: _getCheckTypes,
            getTriggersForCheck: _getTriggersForCheck
        };
    }
}());
