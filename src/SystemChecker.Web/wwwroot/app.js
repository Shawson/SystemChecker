/*! Shaw Young - SYSTEMCHECKER - 03-04-2016*/ 

/*IMPORTANT! This code was generated by a tool. Changes to this file may cause incorrect behavior and will be lost if the code is regenerated. */
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

    'use strict';

    angular
        .module('systemCheckerApp')
        .factory('RecursionHelper', ['$compile', function ($compile) {
            return {
            /**
             * Manually compiles the element, fixing the recursion loop.
             * @param element
             * @param [link] A post-link function, or an object with function(s) registered via pre and post properties.
             * @returns An object containing the linking functions.
             */
            compile: function (element, link) {
                // Normalize the link parameter
                if (angular.isFunction(link)) {
                    link = { post: link };
                }

                // Break the recursion loop by removing the contents
                var contents = element.contents().remove();
                var compiledContents;
                return {
                    pre: (link && link.pre) ? link.pre : null,
                    /**
                     * Compiles and re-adds the contents
                     */
                    post: function (scope, element) {
                        // Compile the contents
                        if (!compiledContents) {
                            compiledContents = $compile(contents);
                        }
                        // Re-add the compiled contents to the element
                        compiledContents(scope, function (clone) {
                            element.append(clone);
                        });

                        // Call the post-linking function, if any
                        if (link && link.post) {
                            link.post.apply(null, arguments);
                        }
                    }
                };
            }
        };
    }]);

})();

    'use strict';

    angular
        .module('systemCheckerApp')
        .directive('condition', directive);
    
    directive.$inject = ['RecursionHelper'];

    function directive(RecursionHelper) {

        var controller = function ($scope) {

            // todo : move to a lookup
            $scope.Comparators = [
                { Value: -1, Description: 'NotEquals' },
                { Value: 0, Description: 'Equals' },
                { Value: 10, Description: 'GreaterThan' },
                { Value: 20, Description: 'LessThan' },
                { Value: 100, Description: 'Contains' },
                { Value: 110, Description: 'StartsWith' },
                { Value: 200, Description: 'IsNull' }
            ];
            $scope.OperatorDescriptions = ['AND','OR'];
           
            $scope.AddCondition = function () {

                if (!$scope.item.Rules)
                    $scope.item.Rules = [];

                $scope.item.Rules.push({
                    // create a default instance of a condition
                    ValueA: '',
                    Comparator: 0,
                    ValueB: '',
                    Operator: 0,
                    Description: '',
                    Rules: false
                });
            };

            $scope.RemoveCondition = function (item) {
                debugger;
            }
            
        };

        controller.$inject = ['$scope', 'RecursionHelper']

        return {
            templateUrl: '/Angular/Shared/Directives/Condition/condition.html',
            restrict: 'E',
            controller: controller,
            scope: {
                item: '=',
                first: '='
            },
            compile: function (element) {
                // Use the compile function from the RecursionHelper,
                // And return the linking function(s) which it returns
                return RecursionHelper.compile(element);
            }
        };
    }

})();

    'use strict';

    angular
        .module('systemCheckerApp')
        .directive('conditionExpression', directive);
    
    directive.$inject = ['RecursionHelper'];

    function directive(RecursionHelper) {

        var controller = function ($scope) {

            // todo : move to a lookup
            $scope.Comparators = [
                { Value: -1, Description: 'NotEquals' },
                { Value: 0, Description: 'Equals' },
                { Value: 10, Description: 'GreaterThan' },
                { Value: 20, Description: 'LessThan' },
                { Value: 100, Description: 'Contains' },
                { Value: 110, Description: 'StartsWith' }
            ];
            $scope.OperatorDescriptions = ['AND','OR'];
                       
        };

        controller.$inject = ['$scope', 'RecursionHelper']

        return {
            templateUrl: '/Angular/Shared/Directives/ConditionExpression/conditionExpression.html',
            restrict: 'E',
            controller: controller,
            scope: {
                item: '=',
                first: '='
            },
            compile: function (element) {
                // Use the compile function from the RecursionHelper,
                // And return the linking function(s) which it returns
                return RecursionHelper.compile(element);
            }
        };
    }

})();

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

    'use strict';

    angular
        .module('systemCheckerApp')
        .directive('outcome', directive);

    function directive() {

        var controller = function ($scope) {
            console.log($scope.item);

            $scope.GetSuccessStatusDescription = function (item) {
                switch (item) {
                    case 10:
                        return "Success";
                    case -10:
                        return "Failure";
                    case -20:
                        return "Timeout";
                    default:
                        return item;
                }
            };
        };

        controller.$inject = ['$scope'];

        return {
            templateUrl: '/Angular/Shared/Directives/Outcome/outcome.html',
            restrict: 'E',
            controller: controller,
            scope: {
                item: '='
            }
        };
    }

})();

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


    'use strict';

    angular
        .module('systemCheckerApp')
        .controller('testBuilderController', controller);

    controller.$inject = ['$scope', 'systemChecksService']; 

    function controller($scope, systemChecksService) {
        $scope.title = 'controller';

        activate();

        function activate() { }
    }

    
})();