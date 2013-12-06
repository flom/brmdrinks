function drinksController($scope, $log, $http) {
  $scope.alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".split('');
  $scope.searchStr = '';
  $scope.search = function (letter) {
    $scope.searchStr += letter;

    for (var i in $scope.customers) {
      if ($scope.customers[i].name.indexOf($scope.searchStr) == 0) {
        if ($scope.selectedCustomer != $scope.customers[i]) {
          $scope.selectCustomer($scope.customers[i], true);
          break;
        }
      }
    }
  };

  $scope.customers = [];
  $scope.spendings = [];
  $scope.products = [];
  $scope.selectedCustomer = null;
  $scope.selectedSpending = null;
  $scope.selectedProduct = null;
  $scope.mostConsumedProduct = null;
  $scope.priorityProductId = null;
  $scope.productCount = 0;
  $scope.spendingCount = 0;
  $scope.pastOrders = [];
  $scope.ordersForCustomer = [];
  $scope.lastCustomers = [];

  $scope.spendingAmount = 0;

  $scope.loadingCustomers = false;
  $scope.loadingSpendings = false;
  $scope.loadingProducts = false;
  $scope.loadingProductCount = false;
  $scope.loadingSpendingCount = false;
  $scope.loadingPastOrders = false;

  $scope.loadData = function () {
    $log.info('loading data');
    $scope.loadSpendings();
    $scope.loadCustomers();
    $scope.loadProducts();
  };

  $scope.loadSpendings = function () {
    $scope.loadingSpendings = true;
    $http.post('/Ajax/GetProductSpendings').success(function (data, status, headers, config) {
      $scope.spendings = data;
      $scope.loadingSpendings = false;
    });
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

  $scope.loadProducts = function () {
    $scope.loadingProducts = true;
    $http.post('/Ajax/GetAllProducts').success(function (data, status, headers, config) {
      $scope.products = data;
      for (var index in $scope.products) {
        var product = $scope.products[index];

        if (product.priority) {
          $scope.mostConsumedProduct = product;
        }
      }
      $scope.loadingProducts = false;
    });
  };

  $scope.selectCustomer = function (customer, fromSearch) {
    if (fromSearch === undefined) {
      fromSearch = false;
    }

    $scope.selectedSpending = null;
    if ($scope.selectedCustomer == customer) {
      $scope.selectedCustomer = null;
    } else {
      $scope.selectedCustomer = customer;

      if ($scope.selectedProduct) {
        $scope.getProductCount();
        $scope.getPastOrders();
      }

      $scope.loadOrdersForCustomer();

      if (!fromSearch) {
        $scope.searchStr = '';
      }
    }
  };

  $scope.selectSpending = function (spending) {
    $scope.spendingAmount = 1;
    $scope.selectedCustomer = null;
    if ($scope.selectedSpending == spending) {
      $scope.selectedSpending = null;
    } else {
      $scope.selectedSpending = spending;
      for (var index in $scope.products) {
        if ($scope.products[index]['id'] == $scope.selectedSpending['productId']) {
          $scope.selectedProduct = $scope.products[index];
        }
      }
      $scope.getSpendingCount();
      $scope.getPastOrders();
    }
  };

  $scope.selectProduct = function (product) {
    if ($scope.selectedProduct == product) {
      $scope.selectedProduct = null;
    } else {
      $scope.selectedProduct = product;

      if ($scope.selectedCustomer) {
        $scope.getProductCount();
      } else {
        $scope.getSpendingCount();
      }
      $scope.getPastOrders();
    }
  };

  $scope.getProductCount = function () {
    $scope.loadingProductCount = true;
    $http.post('/Ajax/GetProductCount',
        {
          customerId: $scope.selectedCustomer['id'],
          productId: $scope.selectedProduct['id']
        }).success(function (data, status, headers, config) {
          $scope.productCount = data;
          $scope.loadingProductCount = false;
        });
  };

  $scope.getSpendingCount = function () {
    $scope.loadingSpendingCount = true;
    $http.post('/Ajax/GetSpendingCount',
        {
          spendingId: $scope.selectedSpending['id'],
        }).success(function (data, status, headers, config) {
          $scope.spendingCount = data['alreadyConsumed'];
          $scope.loadingSpendingCount = false;
          if (data['alreadyConsumed'] == data['maxQuantity']) {
            $scope.selectedSpending = null;
            $scope.loadSpendings();
            $scope.successAlertText = 'Klammer erfolgreich ausgetrunken';
            $scope.showSuccessAlert = true;
          }
        });
  };

  $scope.getPastOrders = function () {
    $scope.loadingPastOrders = true;
    if ($scope.selectedCustomer) {
      $http.post('/Ajax/GetPastOrdersForProduct', {
        customerId: $scope.selectedCustomer['id'],
        productId: $scope.selectedProduct['id']
      }).success(function (data, status, headers, config) {
        $scope.pastOrders = data;
        $scope.loadingPastOrders = false;
      })
    } else if ($scope.selectedSpending) {
      $http.post('/Ajax/GetOrdersForSpending', {
        spendingId: $scope.selectedSpending['id'],
      }).success(function (data, status, headers, config) {
        $scope.pastOrders = data;
        $scope.loadingPastOrders = false;
      })
    }
  };

  $scope.undoOrder = function () {
    $http.post('/Ajax/UndoOrderProduct', {
      customerId: $scope.selectedCustomer['id'],
      productId: $scope.selectedProduct['id']
    }).success(function (data, status, headers, config) {
      $scope.getProductCount();
      $scope.getPastOrders();
    })
  };

  $scope.undoSpendingOrder = function () {
    $http.post('/Ajax/UndoOrderProduct', {
      customerId: $scope.selectedSpending['customerId'],
      productId: $scope.selectedProduct['id']
    }).success(function (data, status, headers, config) {
      $scope.getSpendingCount();
      $scope.getPastOrders();
    })
  };

  $scope.orderProduct = function (quantity) {
    $scope.searchStr = '';
    if ($scope.selectedCustomer) {
      $http.post('/Ajax/OrderProduct', {
        customerId: $scope.selectedCustomer['id'],
        productId: $scope.selectedProduct['id'],
        quantity: quantity
      }).success(function (data, status, headers, config) {
        $scope.getProductCount();
        $scope.getPastOrders();

        // add customer to lastCustomers list
        var index = $scope.lastCustomers.indexOf($scope.selectedCustomer);
        if (index >= 0) {
          $scope.lastCustomers.splice(index, 1);
        }
        $scope.lastCustomers.splice(0, 0, $scope.selectedCustomer);
        if ($scope.lastCustomers.length > 5) {
          $scope.lastCustomers.splice($scope.lastCustomers.length - 1, 1);
        }

      })
    } else if ($scope.selectedSpending) {
      $http.post('/Ajax/OrderSpendedProduct', {
        spendingId: $scope.selectedSpending['id'],
        quantity: quantity
      }).success(function (data, status, headers, config) {
        $scope.getSpendingCount();
        $scope.spendingAmount = 1;
        $scope.getPastOrders();
      })
    }
  };

  $scope.spendChosenProduct = function () {
    $http.post('/Ajax/SpendProduct', {
      customerId: $scope.selectedCustomer['id'],
      productId: $scope.selectedProduct['id'],
      quantity: $scope.spendingAmount
    }).success(function (data, status, headers, config) {
      $scope.loadSpendings();
      $scope.spendingAmount = 0;
    })
  };

  $scope.setSpendingAmount = function (quantity) {
    var newVal = $scope.spendingAmount + quantity;
    if (newVal > 0 && newVal <= $scope.selectedSpending['quantity']) {
      $scope.spendingAmount = newVal;
    } else if (newVal <= 0) {
      $scope.spendingAmount = 1;
    } else {
      $scope.spendingAmount = $scope.selectedSpending['quantity'] - $scope.selectedSpending['currentQuota'];
    }
  }

  $scope.selectedConsumer = function () {
    if ($scope.selectedCustomer && $scope.selectedProduct) {
      return "customer";
    } else if ($scope.selectedSpending && $scope.selectedProduct) {
      return "spending";
    }
    return null;
  };

  $scope.closeSpending = function () {
    $http.post('/Ajax/RemoveProductSpending', {
      spendingId: $scope.selectedSpending['id'],
    }).success(function (data, status, headers, config) {
      $scope.loadSpendings();
      $scope.selectedSpending = null;
      $scope.spendingAmount = 0;
    })
  };

  $scope.loadOrdersForCustomer = function () {
    $http.post('/Ajax/GetOrdersForCustomer', {
      customerId: $scope.selectedCustomer['id']
    }).success(function (data, status, headers, config) {
      $scope.ordersForCustomer = data;
    })
  };

  $scope.removeOrder = function (order) {
    $http.post('/Ajax/UndoSpecificOrder', {
      orderId: order['id']
    }).success(function (data, status, headers, config) {
      $scope.loadOrdersForCustomer();
    })
  };

  $scope.quickOrder = function () {
    if ($scope.selectedProduct != $scope.mostConsumedProduct) {
      $scope.selectProduct($scope.mostConsumedProduct);
    }
    $scope.orderProduct(1);
  };
}

function addCustomerController($scope, $log, $http) {
  $scope.accounts = [];
  $scope.archivedCustomers = [];

  $scope.init = function () {
    $scope.loadAccounts();
    $scope.loadArchivedCustomers();
  };

  $scope.loadAccounts = function () {
    $http.post('/Ajax/GetAllAccounts').success(function (data, status, headers, config) {
      $scope.accounts = data;
    })
  };

  $scope.loadArchivedCustomers = function () {
    $http.post('/Ajax/GetArchivedCustomers').success(function (data, status, headers, config) {
      $scope.archivedCustomers = data;
    })
  };

  $scope.createCustomer = function () {
    if (!($scope.firstName || $scope.lastName)) { return; }

    if ($scope.newAccountName) {
      $http.post('/Ajax/AddAccount', {
        accountName: $scope.firstName,
      }).success(function (data, status, headers, config) {
        $http.post('/Ajax/AddCustomer', {
          firstName: $scope.firstName,
          lastName: $scope.lastName,
          accountId: data
        }).success(function (data, status, headers, config) {
          window.location.reload();
        })
      })
    } else if ($scope.selectedAccount) {
      $http.post('/Ajax/AddCustomer', {
        firstName: $scope.firstName,
        lastName: $scope.lastName,
        accountId: $scope.selectedAccount
      }).success(function (data, status, headers, config) {
        window.location.reload();
      })
    }
  };
}
