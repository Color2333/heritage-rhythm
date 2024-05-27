document.addEventListener('DOMContentLoaded', function () {
    var ctx = document.getElementById('fitChart').getContext('2d');

    // 示例数据
    var scatterData = {
        datasets: [{
            label: '用户评分',
            data: [{x: 1, y: 4.5}, {x: 2, y: 3.5}, {x: 3, y: 5}, {x: 4, y: 4}, {x: 5, y: 5}],
            backgroundColor: 'rgba(255, 99, 132, 0.5)'
        }]
    };

    // 线性回归函数
    function linearRegression(data){
        let n = data.length;
        let sum_x = 0, sum_y = 0, sum_xy = 0, sum_xx = 0;

        data.forEach((point) => {
            sum_x += point.x;
            sum_y += point.y;
            sum_xy += point.x * point.y;
            sum_xx += point.x * point.x;
        });

        let slope = (n * sum_xy - sum_x * sum_y) / (n * sum_xx - sum_x * sum_x);
        let intercept = (sum_y - slope * sum_x) / n;

        return {slope, intercept};
    }

    var {slope, intercept} = linearRegression(scatterData.datasets[0].data);

    // 添加拟合线
    scatterData.datasets.push({
        label: '拟合线',
        type: 'line',
        data: [{x: 1, y: intercept + slope * 1}, {x: 5, y: intercept + slope * 5}],
        borderColor: 'rgba(75, 192, 192, 1)',
        backgroundColor: 'rgba(0, 0, 0, 0)',
        fill: false
    });

    var scatterChart = new Chart(ctx, {
        type: 'scatter',
        data: scatterData,
        options: {
            scales: {
                x: {beginAtZero: true},
                y: {beginAtZero: true}
            }
        }
    });
});
