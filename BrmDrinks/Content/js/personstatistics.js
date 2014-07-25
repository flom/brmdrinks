function personStatisticsCtrl($scope, $http) {
  $scope.customers = [];
  $scope.selectedCustomers = [];
  $scope.searchStr = '';
  $scope.alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".split('');

  $scope.loadData = function () {
    $scope.loadCustomers();
    $scope.initConsumptionChart();
  };

  $scope.loadCustomers = function (sortOption) {
    sortOption = typeof sortOption !== 'undefinded' ? sortOption : 0;
    $scope.customers = [];
    $scope.loadingCustomers = true;
    $http.post('/Ajax/GetAllCustomers', {
      sortOption: sortOption
    }).success(function (data, status, headers, config) {
      $scope.customers = data;
      $scope.loadingCustomers = false;
    });
  };

  $scope.initConsumptionChart = function () {
    var today = new Date();
    today.setDate(today.getDate() + 1);
    var firstDay = new Date();
    firstDay.setDate(today.getDate() - 7);

    $('#from').val('{0}.{1}.{2}'.format(firstDay.getDate(), firstDay.getMonth() + 1, firstDay.getFullYear()));
    $('#to').val('{0}.{1}.{2}'.format(today.getDate(), today.getMonth() + 1, today.getFullYear()));
  }

  $scope.updateConsumptionChart = function () {
    var selected = [];

    for (var i in $scope.selectedCustomers) {
      selected.push($scope.selectedCustomers[i].id);
    }

    var from = $('#from').val();
    var to = $('#to').val();
    $http.post('/Ajax/GetPeopleConsumption',
      { 'firstDay': from, 'lastDay': to, 'peopleIds': selected}).success(function (data) {
        $scope.drawConsumptionChart(data);
      });
  };

  $scope.drawConsumptionChart = function (consumption) {

    var data = google.visualization.arrayToDataTable(consumption);

    var options = {
      title: 'Konsum'
    };

    var chart = new google.visualization.LineChart(document.getElementById('consumptionChart'));
    chart.draw(data, options);
  }

  $scope.search = function (letter) {
    $scope.searchStr += letter;
  };

  $scope.selectCustomer = function (customer) {
    var index = $scope.selectedCustomers.indexOf(customer);
    if (index > -1) {
      $scope.selectedCustomers.splice(index, 1);
    } else {
      $scope.selectedCustomers.push(customer);
    }
  };

  $scope.selectAll = function () {
    $scope.selectedCustomers = $scope.customers;
  };

  $scope.deselectAll = function () {
    $scope.selectedCustomers = [];
  };

}