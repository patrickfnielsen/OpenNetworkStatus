window.Status = (function() {
    if (!Array.prototype.last){
        Array.prototype.last = function(){
            return this[this.length - 1];
        };
    }
    
    function hook(name) {
        return document.querySelectorAll("[data-js-hook='" + name + "']");
    }
    
    return {
        hook: hook
    }
})();

Status.Metric = (function() {    
    function init(options) {
        Status.Metric.options = options;

        Chart.Tooltip.positioners.custom = function(elements, eventPosition) {
            return { x: 5, y: 5};
        };
        
        renderAllCharts();
    }
    
    function renderAllCharts() {
        let metrics = Status.hook("metric");
        
        metrics.forEach(item => {           
            let metricId = item.attributes["data-metric-id"].value;
            let metricSuffix = item.attributes["data-metric-suffix"].value;

            let renderContext = item.querySelectorAll("[data-metric-chart]")[0];
            let chart = renderChart(renderContext, metricId, metricSuffix);

            refreshData(chart, metricId, metricSuffix);
        });
    }
    
    function renderChart(ctx, metricId, metricSuffix) {   
        return new Chart(ctx, {
            type: "line",
            data: {
                datasets: [
                    {
                        backgroundColor: "#7a7a7a",
                        borderColor: "#7a7a7a",
                        pointRadius: 0,
                        fill: false,
                        lineTension: 0,
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
                            tooltipFormat: "dddd, MMM D, HH:mm",
                            displayFormats: {
                                minute: "mm:ss",
                                hour: "HH:mm"
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
                            fontColor: "#99aab5",
                            autoSkip: true,
                            autoSkipPadding: 75,
                            maxRotation: 0,
                            sampleSize: 100
                        },
                        afterBuildTicks: function(scale, ticks) {
                            if(ticks == null) return;

                            var majorUnit = scale._majorUnit;
                            var firstTick = ticks[0];
                            var i, ilen, val, tick, currMajor, lastMajor;

                            val = moment(ticks[0].value);
                            lastMajor = val.get(majorUnit);

                            for (i = 1, ilen = ticks.length; i < ilen; i++) {
                                tick = ticks[i];
                                val = moment(tick.value);
                                currMajor = val.get(majorUnit);
                                tick.major = currMajor !== lastMajor;
                                lastMajor = currMajor;
                            }
                            return ticks;
                        }
                    }],
                    yAxes: [{
                        gridLines: {
                            drawBorder: false
                        },
                        ticks: {
                            lineHeight: 3,
                            padding: 8,
                            fontColor: "#99aab5"
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
    
    function refreshData(chart, metricId, metricSuffix) {
        fetch(window.location.origin + "/api/v1/metrics/" + metricId + "/datapoints")
            .then(data => {
                return data.json();
            })
            .then(result => {
                let dataset = chart.data.datasets[0];
                result.forEach(dataPoint => {
                    dataset.data.push({x: moment.utc(dataPoint.createdOn), y: dataPoint.value});
                });

                if(dataset.data.length > 0) {              
                    updateLastValue(metricId, dataset.data.last().y, metricSuffix);
                    chart.update();
                }
            });
    }
    
    function updateLastValue(metricId, newValue, metricSuffix) {
        let items = document.querySelectorAll("[data-metric-last-value='" + metricId + "']");
        items.forEach(item => {
            item.innerHTML = newValue + " " + metricSuffix;
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
        let content = this.nextElementSibling;
        let isActive = content.classList.toggle("is-active");
        
        if (isActive) {
            icon.setAttribute("name", "remove-circle-outline");
        } else {
            icon.setAttribute("name", "add-circle-outline");
        } 
    }
    
    return {
        init: init,
    }
})();