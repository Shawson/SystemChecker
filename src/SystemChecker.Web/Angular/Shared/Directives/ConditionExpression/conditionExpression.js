(function() {
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