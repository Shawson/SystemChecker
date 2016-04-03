(function() {
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