(function () {
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
