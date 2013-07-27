// ajax api
var getCustomers = $('#getCustomers').val();
var getProducts = $('#getProducts').val();
var getAccounts = $('#getAccounts').val();
var getArchivedCustomers = $('#getArchivedCustomers').val();
var getProductCount = $('#getProductCount').val();
var orderProduct = $('#orderProduct').val();
var orderSpendedProduct = $('#orderSpendedProduct').val();
var getPastOrdersForProduct = $('#getPastOrdersForProduct').val();
var undoOrderProduct = $('#undoOrderProduct').val();
var undoSpecificOrder = $('#undoSpecificOrder').val();
var addAccount = $('#addAccount').val();
var addCustomer = $('#addCustomer').val();
var addProduct = $('#addProduct').val();
var spendProduct = $('#spendProduct').val();
var getProductSpendings = $('#getProductSpendings').val();
var getSpendingCount = $('#getSpendingCount').val();
var getOrdersForSpending = $('#getOrdersForSpending').val();
var getOrdersForCustomer = $('#getOrdersForCustomer').val();
var closeProductSpending = $('#closeProductSpending').val();

var customers = [];
var products = [];
var accounts = [];
var customerSortOption = 0;
var spendings = [];
var selectedCustomer = undefined;
var selectedSpending = undefined;
var selectedProduct = undefined;
var selectedProductPastOrders = undefined;
var quantity = 1;

initialize();

function initialize() {
    loadCustomers();
    loadProducts();
    loadSpendings();
    loadAccounts();
    selectCustomer(-1);
}

function setCustomerSort(option) {
    customerSortOption = option;
    initialize();
}

function loadCustomers() {
    callAjax(getCustomers, { 'sortOption': customerSortOption }).success(function (result) {
        fillCustomerList(result);
        customers = result;
    });
}

function fillCustomerList(customers) {
    $('#personList').html('');
    for (var index in customers) {
        var customer = customers[index];
        if (customer['spending']) {
            $('#personList').append('<button id="spendingBtn{0}" class="btn btn-large btn-spending" onclick="updateSelectSpending({0})">{1}</button>'.format(customers[index]['id'], customers[index]['name']));
        } else {
            $('#personList').append('<button id="userBtn{0}" class="btn btn-large btn-name" onclick="updateSelectCustomer({0})">{1}</button>'.format(customers[index]['id'], customers[index]['name']));
        }
    }
}

function loadProducts() {
    callAjax(getProducts).success(function (result) {
        fillProductList(result);
        products = result;
    });
}

function fillProductList(products, enabled) {
    var mostConsumedProductButton = ['none', 0];
    $('#productList').html('');
    for (var index in products) {
        if (products[index]['consumed'] > mostConsumedProductButton[1]) {
            mostConsumedProductButton = ['#productBtn{0}'.format(products[index]['id']), products[index]['consumed']];
        }
        if (enabled) {
            $('#productList').append('<button id="productBtn{0}" class="btn btn-large btn-product" onclick="selectProduct({0})">{1} - {2} &euro;</button>'.format(products[index]['id'], products[index]['name'], products[index]['price']));
        } else {
            $('#productList').append('<button id="productBtn{0}" disabled class="btn btn-large btn-product" onclick="selectProduct({0})">{1} - {2} &euro;</button>'.format(products[index]['id'], products[index]['name'], products[index]['price']));
        }
    }
    if (enabled) {
        $(mostConsumedProductButton[0]).removeClass('btn-product');
        $(mostConsumedProductButton[0]).addClass('btn-product-most-consumed');
    }
}

function loadSpendings() {
    callAjax(getProductSpendings).success(function (result) {
        spendings = result;
        newcustomers = []
        for (i in result) {
            newcustomers = newcustomers.concat([{
                'id': result[i]['id'],
                'name': 'Klammer [{0}] von {1}'.format(result[i]['quantity'], result[i]['customerName']),
                'spending': true,
            }]);
        }
        newcustomers = newcustomers.concat(customers);
        fillCustomerList(newcustomers);
    });
}

function loadAccounts() {
    $('#availableAccounts').html('');
    callAjax(getAccounts).success(function (result) {
        for (var index in result) {
            $('#availableAccounts').append('<option value="{0}">{1}</option>'.format(result[index]['id'], result[index]['name']));
        }
    });
    callAjax(getArchivedCustomers).success(function (result) {
        for (var index in result) {
            $('#archivedCustomers').append('<option value="{0}">{1}</option>'.format(result[index]['id'], result[index]['name']));
        }
    });
}

function updateSelectCustomer(userId) {
    selectSpending(-1);
    selectedSpending = undefined;
    selectCustomer(userId);
}

function updateSelectSpending(spendingId) {
    selectCustomer(-1);
    selectedCustomer = undefined;
    selectSpending(spendingId);
}

function selectCustomer(userId) {
    // check if same customer is clicked twice
    if (selectedCustomer && userId == selectedCustomer['id']) {
        userId = -1;
        selectedCustomer = undefined;
    }

    // refresh css classes of buttons and set selectedCustomer (if there is any)
    for (var index in customers) {
        var btnId = '#userBtn{0}'.format(customers[index]['id'])
        $(btnId).removeClass('btn-name-selected');
        $(btnId).addClass('btn-name');
        if (customers[index]['id'] == userId) {
            selectedCustomer = customers[index];
            $(btnId).removeClass('btn-name');
            $(btnId).addClass('btn-name-selected');
        }
    }

    if (selectedCustomer) {
        fillProductList(products, true);
        $('#selectedCustomer').text(selectedCustomer['name']);
        if (selectedProduct) {
            selectProduct(selectedProduct['id']);
        }
        showCustomerInfo(selectedCustomer);
    } else {
        fillProductList(products);
        $('#infoPanel').css('display', 'none');
        $('#btnCustomerInfo').css('display', 'none');
    }
}

function showCustomerInfo(customer) {
    $('#customerInfoLabel').text(customer['name']);
    $('#linkEditCustomer').attr('href', '/Customer/Edit/{0}'.format(customer['id']));
    $('#linkArchiveCustomer').attr('href', '/Customer/ToggleArchive/{0}'.format(customer['id']));

    callAjax(getOrdersForCustomer, { 'customerId': customer['id'] }).success(function (result) {
        $('#customerInfoBody').html('');
        for (var index in result) {
            $('#customerInfoBody').append('<button class="btn btn-large" onclick="undoSelectedOrder({0})">{1}</button>'.format(result[index]['id'], result[index]['text']));
            $('#customerInfoBody').append('<br /><br />');
        }
    });

    $('#btnCustomerInfo').css('display', '');
}

function selectSpending(spendingId) {
    if (selectedSpending && spendingId == selectedSpending['id']) {
        spendingId = -1;
        selectedSpending = undefined;
        console.log('deselect');
    }

    for (var index in spendings) {
        var btnId = '#spendingBtn{0}'.format(spendings[index]['id'])
        $(btnId).removeClass('btn-spending-selected');
        $(btnId).addClass('btn-spending');
        if (spendings[index]['id'] == spendingId) {
            selectedSpending = spendings[index];
            $(btnId).removeClass('btn-spending');
            $(btnId).addClass('btn-spending-selected');
        }
    }

    if (selectedSpending) {
        var spendedProduct = undefined;
        for (var index in products) {
            if (products[index]['id'] == selectedSpending['productId']) {
                spendedProduct = [products[index]];
            }
        }
        fillProductList(spendedProduct, true);
        $('#selectedCustomer').text('Klammer von {0}'.format(selectedSpending['customerName']));
        selectProduct(spendedProduct[0]['id']);
    }
}

function selectProduct(productId) {
    for (var index in products) {
        var btnId = '#productBtn{0}'.format(products[index]['id'])
        $(btnId).removeClass('btn-product-selected');
        if (products[index]['id'] == productId) {
            selectedProduct = products[index];
            $(btnId).addClass('btn-product-selected');
        }
    }

    if (selectedProduct) {
        // show order panel
        $('#infoPanel').css('display', '');

        // get current drink count of customer or spending for this product
        setQuantity(1);
        if (selectedCustomer) {
            $('#btnNewSpending').css('display', '');
            $('#btnCloseSpending').css('display', 'none');

            callAjax(getProductCount, { "customerId": selectedCustomer['id'], "productId": selectedProduct['id'] }).success(function (result) {
                $('#productCount').text(result);
            });

            // get past orders
            callAjax(getPastOrdersForProduct, { 'customerId': selectedCustomer['id'], 'productId': selectedProduct['id'] }).success(function (result) {
                fillPastOrders(result);
            });
        } else {
            $('#btnNewSpending').css('display', 'none');
            $('#btnCloseSpending').css('display', '');

            callAjax(getSpendingCount, { "spendingId": selectedSpending['id'], }).success(function (result) {
                $('#productCount').text('{0}/{1}'.format(result['alreadyConsumed'], result['maxQuantity']));
            });

            callAjax(getOrdersForSpending, { 'spendingId': selectedSpending['id'] }).success(function (result) {
                fillPastOrders(result);
            });
        }
    }
}

function fillPastOrders(orders) {
    $('#pastOrders').html('');
    for (var index in orders) {
        $('#pastOrders').append('<li>{0}</li>'.format(orders[index]));
    }
}

function addQuantity(quantity) {
    var currentQuantity = parseInt($('#currentQuantity').text());
    var newQuantity = currentQuantity + quantity;
    if (newQuantity >= 1) {
        setQuantity(newQuantity);
    }
}

function setQuantity(newQuantity) {
    if (selectedCustomer) {
        quantity = newQuantity;
        $('#currentQuantity').text(newQuantity);
        $('#addedQuantity').text(' +{0}'.format(newQuantity));
    } else {
        quantity = Math.min(selectedSpending['quantity'] - selectedSpending['currentQuota'], newQuantity);
        quantity = Math.max(0, quantity);
        $('#currentQuantity').text(quantity);
        $('#addedQuantity').text(' +{0}'.format(quantity));
    }
}

function confirmOrder() {
    if (selectedCustomer) {
        callAjax(orderProduct, { 'customerId': selectedCustomer['id'], 'productId': selectedProduct['id'], 'quantity': quantity }).success(function () {
            selectProduct(selectedProduct['id']);
        });
    } else {
        callAjax(orderSpendedProduct, { 'spendingId': selectedSpending['id'], 'quantity': quantity }).success(function (result) {
            selectedSpending = result;
            if (selectedSpending['currentQuota'] < selectedSpending['quantity']) {
                selectProduct(selectedSpending['productId']);
            } else {
                $('#successLabel').text('Klammer [{0}] von {1} erfolgreich ausgetrunken'.format(selectedSpending['quantity'], selectedSpending['customerName']));
                $('#successLabel').css('display', '');
                initialize();
            }
        });
    }
}

function undoOrder() {
    if (selectedCustomer) {
        callAjax(undoOrderProduct, { 'customerId': selectedCustomer['id'], 'productId': selectedProduct['id'] }).success(function () {
            selectProduct(selectedProduct['id']);
        });
    } else {
        callAjax(undoOrderProduct, { 'customerId': selectedSpending['customerId'], 'productId': selectedProduct['id'] }).success(function () {
            selectProduct(selectedProduct['id']);
        });
    }
}

function undoSelectedOrder(orderId) {
    if (confirm('Bestellung wirklich entfernen?')) {
        callAjax(undoSpecificOrder, { 'orderId': orderId }).success(function () {
            initialize();
        });
    }
}

function newAccountName(value) {
    if (value.length > 0) {
        $('#availableAccounts').prop('disabled', true);

        if (value.indexOf(' ') > 0) {
            $('#newCustomerFirstName').val(value.substring(0, value.indexOf(' ')));
            $('#newCustomerLastName').val(value.substring(value.indexOf(' ') + 1, value.length));
        } else {
            $('#newCustomerFirstName').val(value);
        }
    } else {
        $('#availableAccounts').prop('disabled', false);
    }
}

function addNewCustomer() {
    var firstName = $('#newCustomerFirstName').val();
    var lastName = $('#newCustomerLastName').val();

    if ($('#availableAccounts').prop('disabled')) {
        var newAccountName = $('#newAccountName').val();
        callAjax(addAccount, { 'name': newAccountName }).success(function (result) {
            callAjax(addCustomer, { 'firstName': firstName, 'lastName': lastName, 'accountId': result }).success(function () {
                initialize();
            });
        });
    } else {
        var accountId = $('#availableAccounts').find(':selected').val();
        callAjax(addCustomer, { 'firstName': firstName, 'lastName': lastName, 'accountId': accountId }).success(function () {
            initialize();
        });
    }

    $('#newCustomerFirstName').val('');
    $('#newCustomerLastName').val('');
    $('#newAccountName').val('');
}

function addNewProduct() {
    var productName = $('#newProductName').val();
    var productPrice = parseFloat($('#newProductPrice').val().replace(',', '.'));

    if (productPrice) {
        callAjax(addProduct, { 'productName': productName, 'price': productPrice }).success(function () {
            initialize();
        });
    } else {
        $('#errorLabel').text('Ungültiger Preis');
        $('#errorLabel').css('display', '');
    }
}

function spendChosenProduct() {
    callAjax(spendProduct, { 'customerId': selectedCustomer['id'], 'productId': selectedProduct['id'], 'quantity': quantity }).success(function () {
        initialize();
    });
}

function closeSpending() {
    callAjax(closeProductSpending, { 'spendingId': selectedSpending['id'] }).success(function () {
        initialize();
    });
}

function archivedCustomerSelection(id) {
    $('#btnReactivate').css('display', id != -1 ? '' : 'none');
    $('#btnReactivate').attr('href', '/Customer/ToggleArchive/{0}'.format(id));
}