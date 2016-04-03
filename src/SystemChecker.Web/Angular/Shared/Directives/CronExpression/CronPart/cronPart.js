(function () {
    'use strict';

    angular
        .module('systemCheckerApp')
        .directive('cronPart', directive);

    function directive() {

        var controller = function ($scope) {

            // setup some defaults
            $scope.Specific = $scope.minValue;
            $scope.From = $scope.minValue;
            $scope.To = $scope.maxValue;
            $scope.Offset = $scope.minValue;
            $scope.Increment = $scope.minValue;

            // try and figure out what the current mode is..

            if ($scope.expressionPart.indexOf('*') > -1) {
                $scope.Mode = 'all';
            }
            else if ($scope.expressionPart.indexOf('?') > -1) {
                $scope.Mode = 'unspecified';
            }
            else if ($scope.expressionPart.indexOf('-') > -1) {
                $scope.Mode = 'range';
                $scope.From = $scope.expressionPart.split('-')[0];
                $scope.To = $scope.expressionPart.split('-')[1];
            }
            else if ($scope.expressionPart.indexOf('/') > -1) {
                $scope.Mode = 'offsetIncrement';
                $scope.Offset = $scope.expressionPart.split('/')[0];
                $scope.Increment = $scope.expressionPart.split('/')[1];
            }
            else {
                $scope.Mode = 'specific';
                $scope.Specific = $scope.expressionPart;
            }

            $scope.updateValue = function () {
                switch ($scope.Mode) {
                    case 'all':
                        $scope.expressionPart = '*';
                        break;
                    case 'unspecified':
                        $scope.expressionPart = '?';
                        break;
                    case 'specific':
                        $scope.expressionPart = $scope.Specific;
                        break;
                    case 'range':
                        $scope.expressionPart = $scope.From + '-' + $scope.To;
                        break;
                    case 'offsetIncrement':
                        $scope.expressionPart = $scope.Offset + '/' + $scope.Increment;
                        break;
                };
            }

            $scope.AvailableValues = [];

            for (var i = $scope.minValue; i < $scope.maxValue; i++) {
                $scope.AvailableValues.push(i);
            }
        };

        controller.$inject = ['$scope'];

        return {
            templateUrl: '/Angular/Shared/Directives/CronExpression/CronPart/cronPart.html',
            restrict: 'E',
            transclude: 'true',
            controller: controller,
            scope: {
                expressionPart: '=',
                minValue: '=',
                maxValue: '='
            }
        };
    }

})();