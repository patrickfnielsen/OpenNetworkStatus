window.Status = (function() {
    if (!Array.prototype.last){
        Array.prototype.last = function(){
            return this[this.length - 1];
        };
    }   
    
    function getColor(name) {
        let style = getComputedStyle(document.documentElement);
        return style.getPropertyValue("--" + name).trim();
    }

    function hook(name) {
        return document.querySelectorAll("[data-js-hook='" + name + "']");
    }
    
    return {
        getColor: getColor,
        hook: hook
    }
})();

Status.Metric = (function () {
    let timers = [];
    let charts = {};
    let timespan = "day";

    function init(options) {
        Status.Metric.options = options;

        Chart.Tooltip.positioners.custom = function(elements, eventPosition) {
            return { x: 5, y: 5};
        };

        hookTimespanButtons();
        renderAllCharts();
    }

    function hookTimespanButtons() {
        let buttons = Status.hook("metric-timespan");

        buttons.forEach(item => {
            item.addEventListener("click", onTimespanClick);
        });
    }

    function onTimespanClick() {
        let newTimespan = this.attributes["data-timespan"].value;
        let allMetrics = Status.hook("metric");
        let timespanButtons = Status.hook("metric-timespan");

        if (timespan == newTimespan) {
            return;
        }

        //Remove 'is-selected' class
        timespanButtons.forEach(button => {
            button.classList.remove("is-selected")
        });

        //Add 'is-selected' to the new timespan button
        this.classList.add("is-selected");

        //Set new timespan and re-render all charts
        allMetrics.forEach(item => {
            let metricId = item.attributes["data-metric-id"].value;
            let metricSuffix = item.attributes["data-metric-suffix"].value;
            let decimalPlaces = item.attributes["data-metric-decimal-places"].value;

            item.setAttribute("data-metric-timespan", newTimespan);
            timespan = newTimespan;

            refreshData(charts[metricId], metricId, metricSuffix, decimalPlaces);
        });
    }
    
    function renderAllCharts() {
        let metrics = Status.hook("metric");

        metrics.forEach(item => {           
            let metricId = item.attributes["data-metric-id"].value;
            let metricSuffix = item.attributes["data-metric-suffix"].value;
            let decimalPlaces = item.attributes["data-metric-decimal-places"].value;

            let renderContext = item.querySelectorAll("[data-metric-chart]")[0];
            let chart = renderChart(renderContext, metricId, metricSuffix);

            refreshData(chart, metricId, metricSuffix, decimalPlaces);

            //Refresh data every minutes
            let timer = setInterval(() => {
                refreshData(chart, metricId, metricSuffix, decimalPlaces);
            }, 60 * 1000)

            charts[metricId] = chart;
            timers.push(timer);
        });
    }

    function calculateTicks(ticks) {
        if (timespan == "week") {
            return calculateTicksForWeek(ticks);
        } else {
            return calculateTicksForDay(ticks);
        }
    }

    function calculateTicksForDay(ticks) {
        //This is a hack, and I'm sure there is a better way to do this, fell free to refactor it
        //we do this to control the ticks, and the major unit is always 3 hours a part as i've found that to look best
        ticks = [];
        let lastDay = moment.utc().startOf("hour").subtract(1, "days");
        let modulo = lastDay.local().hour() % 3;

        if (modulo === 0) {
            lastDay.add(3, "hours")
        } else if (modulo === 1) {
            lastDay.add(2, "hours")
        } else {
            lastDay.add(1, "hours")
        }

        ticks.push({ major: lastDay.local().hour() == 0, value: lastDay.valueOf() });


        for (x = 1; x < 8; x++) {
            lastDay.add(3, "hours")

            ticks.push({ major: lastDay.local().hour() == 0, value: lastDay.valueOf() });
        }
        return ticks;
    }

    function calculateTicksForWeek(ticks) {
        //This is a hack, and I'm sure there is a better way to do this, fell free to refactor it
        ticks = [];
  
        let lastDay = moment().startOf("day").utc().subtract(6, "days");
        ticks.push({ major: true, value: lastDay.valueOf() });

        for (x = 1; x < 7; x++) {
            lastDay.add(1, "day")

            ticks.push({ major: true, value: lastDay.valueOf() });
        }
        return ticks;
    }

    function renderChart(ctx, metricId, metricSuffix) {
        let chartLineColor = Status.getColor("chart-line");
        let chartTickColor = Status.getColor("chart-ticks");

        return new Chart(ctx, {
            type: "line",
            data: {
                datasets: [
                    {
                        backgroundColor: chartLineColor,
                        borderColor: chartLineColor,
                        pointRadius: 0,
                        fill: false,
                        spanGaps: true,
                        lineTension: 0.1,
                        borderWidth: 2,
                        data: []
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                legend: {
                    display: false
                },
                animation: {
                    duration: 0
                },
                hover: {
                    intersect: false
                },
                scales: {
                    xAxes: [{
                        type: "time",
                        distribution: "series",
                        time: {
                            isoWeekday: true,
                            minUnit: "hour",
                            tooltipFormat: "dddd, MMM D, HH:mm",
                            displayFormats: {
                                minute: "HH:mm",
                                hour: "HH:mm",
                            }
                        },
                        gridLines: {
                            drawOnChartArea: false,
                            drawBorder: false,
                            drawTicks: true,
                            zeroLineColor: "rgba(0, 0, 0, 0.1)",
                            tickMarkLength: 6
                        },
                        ticks: {
                            major: {
                                enabled: true,
                            },
                            source: "data",
                            padding: 1,
                            fontColor: chartTickColor,
                            autoSkip: true,
                            autoSkipPadding: 75,
                            maxRotation: 0,
                            sampleSize: 100
                        },
                        afterBuildTicks: function(_, ticks) {
                            return calculateTicks(ticks);
                        }
                    }],
                    yAxes: [{
                        gridLines: {
                            drawBorder: false
                        },
                        ticks: {
                            beginAtZero: true,
                            lineHeight: 3,
                            padding: 8,
                            fontColor: chartTickColor
                        }
                    }]
                },
                tooltips: {
                    mode: "index",
                    enabled: false,
                    intersect: false,
                    callbacks: {
                        label: (item) => item.yLabel + " " + metricSuffix,
                    },
                    custom: function(tooltipModel) {
                        var tooltipId = "chart-tooltip-" + metricId;
                        var tooltipEl = document.getElementById(tooltipId);

                        // Create element on first render
                        if (!tooltipEl) {
                            tooltipEl = document.createElement('div');
                            tooltipEl.id = tooltipId;
                            document.body.appendChild(tooltipEl);
                        }

                        // Hide if no tooltip
                        if (tooltipModel.opacity === 0) {
                            tooltipEl.style.opacity = 0;
                            return;
                        }

                        // Set caret Position
                        tooltipEl.classList.remove('above', 'below', 'no-transform');
                        if (tooltipModel.yAlign) {
                            tooltipEl.classList.add(tooltipModel.yAlign);
                        } else {
                            tooltipEl.classList.add('no-transform');
                        }

                        function getBody(bodyItem) {
                            return bodyItem.lines;
                        }

                        // Set Text
                        if (tooltipModel.body) {
                            var titleLines = tooltipModel.title || [];
                            var bodyLines = tooltipModel.body.map(getBody);

                            var innerHtml = "<div class='metric-chart-tooltip'>";
                            bodyLines.forEach(function(body, i) {
                                var colors = tooltipModel.labelColors[i];
                                var style = "background:" + colors.backgroundColor + ";";

                                innerHtml += "<span class='tooltip-title'>" + titleLines[i] + "</span>";
                                innerHtml += "<span class='tooltip-color' style='" + style + "'></span>";
                                innerHtml += "<span class='tooltip-value'>" + body + "</span>";
                            });

                            tooltipEl.innerHTML = innerHtml + "</div>";
                        }

                        // `this` will be the overall tooltip
                        var position = this._chart.canvas.getBoundingClientRect();

                        // Display, position, and set styles for font
                        tooltipEl.style.opacity = 1;
                        tooltipEl.style.position = "absolute";
                        tooltipEl.style.left = position.left + window.pageXOffset + "px";
                        tooltipEl.style.top = position.top + window.pageYOffset + "px";
                        tooltipEl.style.fontFamily = tooltipModel._bodyFontFamily;
                        tooltipEl.style.fontSize = tooltipModel.bodyFontSize + "px";
                        tooltipEl.style.fontStyle = tooltipModel._bodyFontStyle;
                        tooltipEl.style.padding = tooltipModel.yPadding + "px " + tooltipModel.xPadding + "px";
                        tooltipEl.style.pointerEvents = "none";
                    }
                },
                plugins: {
                    crosshair: {
                        line: {
                            color: "rgba(0, 0, 0, 0.3)"
                        },
                        sync: {
                            enabled: false,
                        },
                        zoom: {
                            enabled: false
                        }
                    }
                }
            }
        });
    }
    
    function refreshData(chart, metricId, metricSuffix, decimalPlaces) {
        url = generateMetricApiUrl(metricId);

        fetch(url)
            .then(data => {
                return data.json();
            })
            .then(result => {
                let dataset = chart.data.datasets[0];
                dataset.data = [];

                result.forEach(dataPoint => {
                    dataset.data.push({ x: moment.utc(dataPoint.created_at), y: dataPoint.value?.toFixed(decimalPlaces) });
                });

                if(dataset.data.length > 0) {
                    updateLastValue(metricId, dataset.data.last().y, metricSuffix);
                    chart.update();
                }
            });
    }

    function generateMetricApiUrl(metricId) {
        let url = window.location.origin + "/api/v1/metrics/" + metricId + "/datapoints";
        if (timespan == "week") {
            url += "/week";
        }

        return url;
    }
    
    function updateLastValue(metricId, newValue, metricSuffix) {
        if (newValue !== null) {
            newValue = newValue + " " + metricSuffix;
        } else {
            newValue = "N/A";
        }

        let items = document.querySelectorAll("[data-metric-last-value='" + metricId + "']");
        items.forEach(item => {
            item.innerHTML = newValue;
        });
    }

    return {
        init: init,
    }
})();

Status.Collapsible = (function() {
    function init(options) {
        Status.Collapsible.options = options;
        initCollapsibleItems();
    }

    function initCollapsibleItems() {
        let collapsibles = Status.hook("collapse");
        collapsibles.forEach(item => {
            let container = item.querySelectorAll("[data-collapse-container]")[0];
            container.addEventListener("click", onClick);
        });
    }
    
    function onClick() {
        let icon = this.querySelectorAll(".icon ion-icon")[0];
        let status = this.querySelectorAll(".component-status")[0];
        let content = this.nextElementSibling;
        let isActive = content.classList.toggle("is-active");
        
        if (isActive) {
            icon.setAttribute("name", "remove-circle-outline");
            status.style.display = "none";
        } else {
            icon.setAttribute("name", "add-circle-outline");
            status.style.display = "inline-block";
        } 
    }
    
    return {
        init: init,
    }
})();