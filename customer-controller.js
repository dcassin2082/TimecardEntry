angular.module('customerApp', []).controller('customerController', function ($scope, $http) {
    $scope.getCustomers = function () {
        $http.get('/api/Customers')
        .success(function (data) {
            $scope.customers = data;
        })
        .error(function (data) {
            $scope.error('An error occurred loading customers');
        });
    };

    $scope.getCustomer = function (id) {
        $http.get('/api/Customers/' + id)
        .success(function (data) {
            $scope.customer = data;
        })
        .error(function (data) {
            $scope.error('An error occurred loading customer');
        });
    };

    $scope.addCustomer = function (customer) {

        $http.post('/api/Customers',
            {
                'Street': 'test street',
                'City': customer.City,
                'State': customer.State,
                'Zip': customer.Zip,
                'Phone': customer.Phone,
                'Name': customer.Name
            })
        .success(function (data) {

            $('#addModal').modal('hide');
            $scope.customers.push(customer);
            $http.get('/api/Customers')
            .success(function (data) {
                $scope.customers = data;
                alert('Customer added successfully');
            })
            .error(function (data) {
                $scope.error('An error occurred adding customer');
            });
            $scope.addcustomerform.$setPristine();
            $scope.addcustomerform.$setUntouched();
            $scope.currentRecord = {};
        });
    };

    $scope.editCustomer = function (customer) {
        $http.get('/api/Customers/' + customer.CustomerId)
        .success(function (data) {
            $scope.customer = data;
        })
        .error(function (data) {
            $scope.error('An error occurred loading customer');
        });
        $scope.Name = customer.Name;
        $scope.Street = customer.Street;
        $scope.City = customer.City;
        $scope.State = customer.State;
        $scope.Zip = customer.Zip;
        $scope.Phone = customer.Phone;
    };

    $scope.updateCustomer = function (id, customer) {
        $scope.CustomerId = id;
        $scope.Name = customer.Name;
        $scope.Street = customer.Street;
        $scope.City = customer.City;
        $scope.State = customer.State;
        $scope.Zip = customer.Zip;
        $scope.Phone = customer.Phone;
        $http.put('/api/Customers/' + id, {
            'CustomerId': customer.CustomerId,
            'Street': customer.Street,
            'City': customer.City,
            'State': customer.State,
            'Zip': customer.Zip,
            'Phone': customer.Phone,
            'Name': customer.Name
        })
        .success(function (data) {
            $('#editModal').modal('hide');
            $http.get('/api/Customers')
                .success(function (data) {
                    $scope.customers = data;
                    $scope.refresh();

                })
            .error(function (data) {
                $scope.error('An error occurred updating customer');
            });
        });
        $scope.customer.Street = '';
        $scope.customer.Name = '';
        $scope.customer.City = '';
        $scope.customer.State = '';
        $scope.customer.Zip = '';
        $scope.customer.Phone = '';
        $scope.editcustomerform.$setPristine();
        $scope.editcustomerform.$setUntouched();
        $scope.currentRecord = {};
        alert('Update successful');
    };

    $scope.deleteCustomer = function (id) {
        $http.delete('/api/Customers/' + id)
            .success(function (data) {
                $scope.refresh();
                $('#deleteModal').modal('hide');
            })
        .error(function (data) {
            $scope.error('An error occurred deleting customer');
        });
        $scope.customer.Street = '';
        $scope.customer.Name = '';
        $scope.customer.City = '';
        $scope.customer.State = '';
        $scope.customer.Zip = '';
        $scope.customer.Phone = '';
        alert('Deleted Successfully');
    };

    $scope.refresh = function () {
        $http.get('/api/Customers/')
            .success(function (data) { $scope.customers = data; })
    };

    $scope.cancelAdd = function () {
        $('#addModal').modal('hide');
    };

    $scope.cancelEdit = function () {
        $('#editModal').modal('hide');
        $scope.customer.Street = '';
        $scope.customer.Name = '';
        $scope.customer.City = '';
        $scope.customer.State = '';
        $scope.customer.Zip = '';
        $scope.customer.Phone = '';
        $scope.editcustomerform.$setPristine();
        $scope.editcustomerform.$setUntouched();
        $scope.currentRecord = {};
    };

    $scope.cancelAdd = function () {
        $('#addModal').modal('hide');
        $scope.customer.Street = '';
        $scope.customer.Name = '';
        $scope.customer.City = '';
        $scope.customer.State = '';
        $scope.customer.Zip = '';
        $scope.customer.Phone = '';
        $scope.addcustomerform.$setPristine();
        $scope.addcustomerform.$setUntouched();
        $scope.currentRecord = {};
    };

    $scope.cancelDelete = function () {
        $('#deleteModal').modal('hide');
        $scope.customer.Street = '';
        $scope.customer.Name = '';
        $scope.customer.City = '';
        $scope.customer.State = '';
        $scope.customer.Zip = '';
        $scope.customer.Phone = '';
    };

    $scope.phoneNumberPattern = (function () {
        var regexp = /^\(?(\d{3})\)?[ .-]?(\d{3})[ .-]?(\d{4})$/;
        return {
            test: function (value) {
                if ($scope.requireTel === false) {
                    return true;
                }
                return regexp.test(value);
            }
        };
    })();
});