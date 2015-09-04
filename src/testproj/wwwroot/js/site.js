
// Write your Javascript code.
var app = angular.module('adminPage', [
    'ui.bootstrap']);

app.controller('AdminCtrl', function ($scope, $filter, $http, $modal) {

    //Initialize variables
    $scope.alerts = [];
    $scope.data = [];
    $scope.editId = -1;
    $scope.hoverId = -1;
    $scope.temp_link = {};
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
            $http.get('/api/url/' + id)
            .success(function (data, status) {
                $scope.temp_link = data;
                $scope.hoverId = -1;
                var modalInstance = $modal.open({
                    templateUrl: '/partials/editRow.html',
                    controller: ModalInstanceCtrl,
                    resolve: {
                        link: function () {
                            return $scope.temp_link;
                        },
                        mode: function () {
                            return mode;
                        },
                        addAlert: function () {
                            return $scope.addAlert;
                        }
                    }
                });
                modalInstance.result.then(function (message) {
                    //Close Function
                    $scope.addAlert("success", "Profile information successfully updated");
                    for (var i = 0; i < $scope.data.length; i++) {
                        if ($scope.data[i].id == id) {
                            $scope.data[i] = angular.copy(message);
                        }
                    }
                }, function (message) {
                    //Cancel Function (or Esc keypress)
                });
            }).error(function (data, status) {
                $scope.addAlert("danger", "Uh-oh! Something went wrong. Please try again");
            });
        } else if (mode == 'new') {
            $scope.temp_link = {};
            $scope.hoverId = -1;
            var modalInstance = $modal.open({
                templateUrl: '/partials/editRow.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    link: function () {
                        return $scope.temp_link;
                    },
                    mode: function () {
                        return mode;
                    },
                    addAlert: function () {
                        return $scope.addAlert;
                    }
                }
            });
            modalInstance.result.then(function (message) {
                // Close Function
                $scope.data.push(message)
            }, function (message) {
                // Cancel Function (or Esc keypress)
            });

        }

    }

});

var ModalInstanceCtrl = function ($scope, $modalInstance, $http, link, mode, addAlert) {
    $scope.test_data = "dfadsfs";
    $scope.mode = mode;
    $scope.link = link;
    $scope.created = false;
    $scope.delete_ok = function () {
        $modalInstance.close('delete');
    };

    $scope.alerts = [];
    $scope.addAlert = function (type, message) {
        $scope.alerts.push({ 'type': type, 'msg': message });
    };

    $scope.closeAlert = function (index) {
        $scope.alerts.splice(index, 1);
    };

    $scope.submit = function (isValid) {
        if (mode == 'edit') {
            $scope.addAlert("warning", "This feature has not been implemented");
            //$http.put('/api/users/' + $scope.user.id, $scope.user)
            //.success(function (data, status) {
            //    $scope.user = data;
            //    //TODO Disable button during request to prevent double posting
            //    $modalInstance.close($scope.user);
            //}).error(function (data, status) {
            //    $scope.addAlert("danger", "Uh-oh! Something went wrong. Please try again");
            //});
        }
        if (mode == 'new') {
            $http.post('../api/url', $scope.link)
            .success(function (data, response) {
                $scope.link = data;
                $scope.created = true;
                addAlert("success", "New URL created");
                mode = 'edit'
            }).error(function (data, status) {
                addAlert("danger", "Uh-oh! Something went wrong. Please try again");
                $modalInstance.close($scope.link);
            });
        }
    }
};

