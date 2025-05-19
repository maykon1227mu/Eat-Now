document.addEventListener('DOMContentLoaded', () => {
    const barCtx = document.getElementById('barChart')?.getContext('2d');
    if (barCtx) {
        new Chart(barCtx, {
            type: 'bar',
            data: {
                labels: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun'],
                datasets: [{
                    label: 'Lucro (R$)',
                    data: [1200, 1900, 3000, 2500, 4000, 3500],
                    backgroundColor: '#34d399'
                }]
            },
            options: {
                responsive: true,
                plugins: { legend: { display: false } }
            }
        });
    }

    const pieCtx = document.getElementById('pieChart')?.getContext('2d');
    if (pieCtx) {
        new Chart(pieCtx, {
            type: 'pie',
            data: {
                labels: ['Bebidas', 'Lanches', 'Sobremesas'],
                datasets: [{
                    data: [35, 45, 20],
                    backgroundColor: ['#fbbf24', '#60a5fa', '#f472b6']
                }]
            }
        });
    }

    const lineCtx = document.getElementById('lineChart')?.getContext('2d');
    if (lineCtx) {
        new Chart(lineCtx, {
            type: 'line',
            data: {
                labels: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun'],
                datasets: [{
                    label: 'Vendas',
                    data: [100, 120, 150, 170, 200, 250],
                    borderColor: '#4ade80',
                    tension: 0.3,
                    fill: false
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: { beginAtZero: true }
                }
            }
        });
    }
});
