const loginBtn = document.getElementById('loginBtn');
const errorEl = document.getElementById('error');

loginBtn.addEventListener('click', async () => {
    errorEl.classList.add('hidden');
    errorEl.textContent = '';

    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value.trim();

    if (!username || !password) {
        errorEl.textContent = 'Введите логин и пароль';
        errorEl.classList.remove('hidden');
        return;
    }

    try {
        const resp = await fetch('https://localhost:5001/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                username,
                password
            })
        });

        if (!resp.ok) {
            errorEl.textContent = 'Неверные учетные данные';
            errorEl.classList.remove('hidden');
            return;
        }

        const data = await resp.json();
        localStorage.setItem('authToken', data.token);

        window.location.href = '/html/chief.html';
    } catch (e) {
        errorEl.textContent = 'Ошибка сети или сервера';
        errorEl.classList.remove('hidden');
        console.error(e);
    }
});
