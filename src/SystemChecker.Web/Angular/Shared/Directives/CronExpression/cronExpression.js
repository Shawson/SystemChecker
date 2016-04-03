// making something like http://www.cronmaker.com/
// http://abunchofutils.com/u/computing/cron-format-helper/
// http://www.openjs.com/scripts/jslibrary/demos/crontab.php
(function () {
    'use strict';

    angular
        .module('systemCheckerApp')
        .directive('cronExpression', directive);

    function directive() {

        var controller = function ($scope) {

            var cronParts = {
                SECONDS: 0,
                MINUTES: 1,
                HOURS: 2,
                DAYOFMONTH: 3,
                MONTH: 4,
                DAYOFWEEK: 5
            };

            // interpret the current cron expression
            var expressionBits = $scope.expression.split(' ');
            debugger;
            $scope.Second = expressionBits[cronParts.SECONDS] ? expressionBits[cronParts.SECONDS] : '*';
            $scope.Minute = expressionBits[cronParts.MINUTES] ? expressionBits[cronParts.MINUTES] : '*';
            $scope.Hour = expressionBits[cronParts.HOURS] ? expressionBits[cronParts.HOURS] : '*';
            $scope.DayOfMonth = expressionBits[cronParts.DAYOFMONTH] ? expressionBits[cronParts.DAYOFMONTH] : '*';
            $scope.Month = expressionBits[cronParts.MONTH] ? expressionBits[cronParts.MONTH] : '*';
            $scope.DayOfWeek = expressionBits[cronParts.DAYOFWEEK] ? expressionBits[cronParts.DAYOFWEEK] : '*';
            $scope.Year = '*';
        };

        controller.$inject = ['$scope'];

        return {
            templateUrl: '/Angular/Shared/Directives/CronExpression/cronExpression.html',
            restrict: 'E',
            controller: controller,
            scope: {
                expression: '='
            }
        };
    }

})();