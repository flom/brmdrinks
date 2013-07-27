google.load("visualization", "1", { packages: ["corechart"] });
google.load("visualization", "1", { packages: ["treemap"] });

google.setOnLoadCallback(initialize);

var getBills = $('#getBills').val();
var getSettlements = $('#getSettlements').val();
var getConsumption = $('#getConsumption').val();

function initialize() {
    initHighscoreChart();
    initConsumptionChart();
}

var highscore = {};

function initHighscoreChart() {
    callAjax(getSettlements).success(function (result) {
        $('#highscoreSettlements').html('<option value="-1">Momentane Rechnung</option>');
        for (var index in result) {
            $('#highscoreSettlements').append('<option value="{0}">{1}</option>'.format(result[index]['id'], result[index]['created']));
        }

        initHighscoreProducts();
    });
}

function initHighscoreProducts() {
    var settlementId = $('#highscoreSettlements').val();

    callAjax(getBills, { 'settlementId': settlementId }).success(function (result) {
        computeHightScores(result);
        $('#highscoreProducts').html('');
        for (var product in highscore) {
            $('#highscoreProducts').append('<option value="{0}">{0}</option>'.format(product));
        }
        drawHighscoreChart();
    });
}

function computeHightScores(bills) {
    highscore = {};
    for (var i in bills) {
        var accountSummary = bills[i];
        for (var j in accountSummary['customerBills']) {
            var customerSummary = accountSummary['customerBills'][j];
            for (var k in customerSummary['billSummary']) {
                var order = customerSummary['billSummary'][k];
                if (!highscore[order['productName']]) {
                    highscore[order['productName']] = [{ 'name': customerSummary['customerName'], 'value': order['quantity'] }];
                } else {
                    highscore[order['productName']].push({ 'name': customerSummary['customerName'], 'value': order['quantity'] });
                }
            }
        }
    }
}

function drawHighscoreChart() {
    var settlementId = $('#highscoreSettlements').val();
    var productId = $('#highscoreProducts').val();

    callAjax(getBills, { 'settlementId': settlementId }).success(function (result) {
        computeHightScores(result);
        var chartData = [];
        var treemapData = [];
        for (var product in highscore) {
            if (product == productId) {
                chartData.push(['Person', 'Anzahl getrunken']);
                treemapData.push(['Person', 'Parent', 'Getrunken']);
                treemapData.push(['Global', null, 0]);
                for (var index in highscore[product]) {
                    chartData.push([highscore[product][index]['name'], highscore[product][index]['value']]);
                    treemapData.push([highscore[product][index]['name'], 'Global', highscore[product][index]['value']]);
                }
            }
        }
        chartData.sort(function (a, b) {
            if (parseInt(a[1])) {
                if (parseInt(b[1])) {
                    return b[1] - a[1];
                }
            }
            return 0;
        });

        var data = google.visualization.arrayToDataTable(chartData.slice(0, 11));

        var options = {
            title: 'Besten 10 für {0} {1}'.format($('#highscoreProducts :selected').text(), $('#highscoreSettlements :selected').text()),
        };

        var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));
        chart.draw(data, options);

		var treemapData = google.visualization.arrayToDataTable(treemapData);
		var treemap = new google.visualization.TreeMap(document.getElementById('treemap_chart'));
		treemap.draw(treemapData, {
			minColor: '#f00',
			midColor: '#ddd',
			maxColor: '#0d0',
			headerHeight: 15,
			fontColor: 'black'
		});
    });
}

function initConsumptionChart() {
    var today = new Date();
    today.setDate(today.getDate() + 1);
    var firstDay = new Date();
    firstDay.setDate(today.getDate() - 7);

    $('#consumptionFrom').val('{0}.{1}.{2}'.format(firstDay.getDate(), firstDay.getMonth() + 1, firstDay.getFullYear()));
    $('#consumptionTo').val('{0}.{1}.{2}'.format(today.getDate(), today.getMonth() + 1, today.getFullYear()));

    updateConsumptionChart();
}

function updateConsumptionChart() {
    var from = $('#consumptionFrom').val();
    var to = $('#consumptionTo').val();

    callAjax(getConsumption, { 'firstDay': from, 'lastDay': to }).success(function (result) {
        drawConsumptionChart(result);
    });
}

function drawConsumptionChart(consumption) {
    var labels = ['Datum'];
    var products = getProducts(consumption);
    var productsIndices = {};
    for (var product in products) {
        labels.push(product);
        productsIndices[product] = labels.length - 1;
    }

    var chartData = [labels];
    for (var index in consumption) {
        var oneDay = consumption[index];
        var row = [oneDay['date']];
        for (var i = 1; i < labels.length; i++) {
            row.push(0);
        }
        for (var k in oneDay['products']) {
            var product = oneDay['products'][k];
            row[productsIndices[product['name']]] = product['value'];
        }
        chartData.push(row);
    }
    var data = google.visualization.arrayToDataTable(chartData);

    var options = {
        title: 'Konsum'
    };

    var chart = new google.visualization.LineChart(document.getElementById('consumptionChart'));
    chart.draw(data, options);
}

function getProducts(consumption) {
    var products = {};
    for (var index in consumption) {
        for (var j in consumption[index]['products']) {
            var prod = consumption[index]['products'][j];
            products[prod['name']] = true;
        }
    }
    return products;
}
