document.getElementById("viewMessagesBtn").addEventListener("click", loadMessages);

async function loadMessages() {
    const token = localStorage.getItem('authToken');
    const resp = await fetch('https://localhost:5001/fhir/messages', {
        headers: {
            'Authorization': 'Bearer ' + token
        }
    });

    if (!resp.ok) {
        document.getElementById('messagesList').textContent = `Ошибка: ${resp.status}`;
        return;
    }

    const messages = await resp.json();
    const formatted = messages.map(m =>
        `Получено: ${new Date(m.receivedAtUtc).toLocaleString()}\n${m.rawJson}\n---\n`
    ).join('\n');
    document.getElementById('messagesList').textContent = formatted;
}
