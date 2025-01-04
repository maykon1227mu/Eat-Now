async function fetchTempoRestante() {
    const response = await fetch(`/api/ProdutoController/tempo-restante/`);
    if (response.ok) {
        return await response.json();
    } else {
        throw new Error("Não foi possível obter o tempo restante.");
    }
}

async function deletarPromocao() {
    const response = await fetch(`/api/ProdutoController/deletar-promocao/`, {
        method: 'DELETE'
    });
    if (response.ok) {
        console.log("Promoção deletada com sucesso.");
    } else {
        console.error("Erro ao deletar a promoção.");
    }
}

function startTimer(duration) {
    let timer = duration, days, hours, minutes, seconds;
    const countdown = setInterval(() => {
        days = parseInt(timer / (60 * 60 * 24), 10);
        hours = parseInt((timer % (60 * 60 * 24)) / (60 * 60), 10);
        minutes = parseInt((timer % (60 * 60)) / 60, 10);
        seconds = parseInt(timer % 60, 10);

        days = days < 10 ? "0" + days : days;
        hours = hours < 10 ? "0" + hours : hours;
        minutes = minutes < 10 ? "0" + minutes : minutes;
        seconds = seconds < 10 ? "0" + seconds : seconds;

        document.getElementById('timer').textContent = `${days}d ${hours}h ${minutes}m ${seconds}s`;

        if (--timer < 0) {
            clearInterval(countdown);
            // Chama a API para deletar a promoção
            deletarPromocao();
        }
    }, 1000);
}

document.addEventListener("DOMContentLoaded", async () => {
    try {
        const tempoRestante = await fetchTempoRestante();
        startTimer(tempoRestante, promocaoId);
    } catch (error) {
        console.error("Erro ao obter o tempo restante da promoção:", error);
    }
});