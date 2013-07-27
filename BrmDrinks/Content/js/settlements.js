
// ajax api
var getAllSettlements = $('#getAllSettlements').val(); 
var getBills = $('#getBills').val(); 

function loadAllSettlements() {
    callAjax(getAllSettlements).success(function (result) {
        $('#settlements').html('');
        for (var index in result) {
            $('#settlements').append('<li><a href="/Home/ShowSettlement?id={0}">Abrechnung am {1}</a></li>'.format(result[index]['id'], result[index]['created']));
        }
    });
}

function getCurrentBills(targetTable) {
    var table = $('#' + targetTable);
    table.html('');

    callAjax(getBills, {}).success(function (result) {
        var totalAllAccounts = 0;
        for (i in result) {
            var customerBills = result[i]['customerBills'];
            var totalAccount = 0;
            table.append('<tr class="info"><td><strong>Konto {0} Summe:</strong></td> <td id="tblAccountSum{1}"></td></tr>'.format(result[i]['accountName'], i));
            for (j in customerBills) {
                table.append('<tr><td><strong>{0}</strong></td> <td id="tblCustomerSum{1}"></td></tr>'.format(customerBills[j]['customerName'], i + '' + j));
                var billSummary = customerBills[j]['billSummary'];
                var totalCustomer = 0;
                for (k in billSummary) {
                    table.append('<tr><td>{0}x {1}</td> <td>{2} €</td></tr>'.format(billSummary[k]['quantity'], billSummary[k]['productName'], billSummary[k]['totalPrice']));
                    totalAllAccounts += billSummary[k]['totalPrice'];
                    totalAccount += billSummary[k]['totalPrice'];
                    totalCustomer += billSummary[k]['totalPrice'];
                }
                totalCustomer = Math.round(totalCustomer * 100) / 100;
                $('#tblCustomerSum' + i + '' + j).html('<strong>{0} €</strong>'.format(totalCustomer));
            }
            totalAccount = Math.round(totalAccount * 100) / 100;
            $('#tblAccountSum' + i).html('<strong>{0} €</strong>'.format(totalAccount));
        }

		totalAllAccounts = Math.round(totalAllAccounts * 100) / 100;
        $('#totalAllAccounts').text(totalAllAccounts);
    });
}

function getSpecificBills(settlementId, targetTable) {
    var table = $('#' + targetTable);
    table.html('');

    callAjax(getBills, { 'settlementId': settlementId }).success(function (result) {
        var totalAllAccounts = 0;
        for (i in result) {
            var customerBills = result[i]['customerBills'];
            var totalAccount = 0;
            table.append('<tr class="info"><td><strong>Konto {0} Summe:</strong></td> <td id="tblAccountSum{1}"></td></tr>'.format(result[i]['accountName'], i));
            for (j in customerBills) {
                table.append('<tr><td><strong>{0}</strong></td> <td id="tblCustomerSum{1}"></td></tr>'.format(customerBills[j]['customerName'], i + '' + j));
                var billSummary = customerBills[j]['billSummary'];
                var totalCustomer = 0;
                for (k in billSummary) {
                    table.append('<tr><td>{0}x {1}</td> <td>{2} €</td></tr>'.format(billSummary[k]['quantity'], billSummary[k]['productName'], billSummary[k]['totalPrice']));
                    totalAllAccounts += billSummary[k]['totalPrice'];
                    totalAccount += billSummary[k]['totalPrice'];
                    totalCustomer += billSummary[k]['totalPrice'];
                }
                $('#tblCustomerSum' + i + '' + j).html('<strong>{0} €</strong>'.format(totalCustomer));
            }
            $('#tblAccountSum' + i).html('<strong>{0} €</strong>'.format(totalAccount));
        }

        $('#totalAllAccounts').text(totalAllAccounts);
    });
}