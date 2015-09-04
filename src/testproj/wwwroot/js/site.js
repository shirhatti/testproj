
// Write your Javascript code.
var app = angular.module('adminPage', [
    'ui.bootstrap']);

app.controller('AdminCtrl', function ($scope, $filter, $http, $modal) {

    //Initialize variables
    $scope.alerts = [];
    $scope.data = [];
    $scope.editId = -1;
    $scope.hoverId = -1;
    $scope.temp_user = {};
    $scope.index = -1;
    $scope.filteredData = [];

    // Load the data initially
    $scope.loadData = function () {
        $http.get('../api/url', {
            params: { time: Date.now() }
        }).success(function (data, status) {
            $scope.data = data;
            $scope.filteredData = data;
        })
    };
    $scope.loadData();

    $scope.setHoverId = function (pid) {
        $scope.hoverId = pid;
    };

    $scope.$watch("filterText", function (query) {
        $scope.filteredData = $filter("filter")($scope.data, query);
    });

    // Methods to use alerts
    $scope.addAlert = function (type, message) {
        $scope.alerts.push({ 'type': type, 'msg': message });
    };

    $scope.closeAlert = function (index) {
        $scope.alerts.splice(index, 1);
    };

    $scope.openEditModal = function (index, mode, id) {
        $scope.index = index;
        if (mode == 'edit') {

        } else if (mode == 'new') {
            $scope.temp_user = {};
            $scope.hoverId = -1;
            var modalInstance = $modal.open({
                templateUrl: '/partials/editRow.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    user: function () {
                        return $scope.temp_user;
                    },
                    mode: function () {
                        return mode;
                    }
                }
            });
            modalInstance.result.then(function (message) {
                // Close Function
                $scope.addAlert("success", "Profile information successfully updated");
                $scope.data.push(message)
            }, function (message) {
                // Cancel Function (or Esc keypress)
            });

        }

    }

});

var ModalInstanceCtrl = function ($scope, $modalInstance, $http, user, mode) {
    //$scope.mode = mode;
    //$scope.user = user;
    //$scope.created = false;
    //$scope.delete_ok = function () {
    //    $modalInstance.close('delete');
    //};

    //$scope.cancel = function () {
    //    $modalInstance.dismiss('cancel');
    //};

    //$scope.alerts = [];
    //$scope.addAlert = function (type, message) {
    //    $scope.alerts.push({ 'type': type, 'msg': message });
    //};

    //$scope.closeAlert = function (index) {
    //    $scope.alerts.splice(index, 1);
    //};

    //$scope.submit = function (isValid) {
    //    if (mode == 'edit') {
    //        $http.put('/api/users/' + $scope.user.id, $scope.user)
    //        .success(function (data, status) {
    //            $scope.user = data;
    //            //TODO Disable button during request to prevent double posting
    //            $modalInstance.close($scope.user);
    //        }).error(function (data, status) {
    //            $scope.addAlert("danger", "Uh-oh! Something went wrong. Please try again");
    //        });
    //    }
    //    if (mode == 'new') {
    //        $http.post('../api/users', $scope.user)
    //        .success(function (data, response) {
    //            $scope.user = data;
    //            //TODO Disable button during request to prevent double posting
    //            $scope.created = true;
    //            $modalInstance.close($scope.user);
    //        }).error(function (data, status) {
    //            $scope.addAlert("danger", "Uh-oh! Something went wrong. Please try again");
    //        });
    //    }
    //}
};

