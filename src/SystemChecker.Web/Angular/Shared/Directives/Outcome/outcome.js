(function() {
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