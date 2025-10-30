const API_BASE = 'https://localhost:5001';

const token = localStorage.getItem('authToken');
const statusEl = document.getElementById('status');
const logoutBtn = document.getElementById('logoutBtn');

const createBtn = document.getElementById('createBtn');
const deleteBtn = document.getElementById('deleteBtn');

if (!token) {
    window.location.href = '../html/login.html';
}

logoutBtn.addEventListener('click', () => {
    localStorage.removeItem('authToken');
    window.location.href = '../html/login.html';
});

function setStatus(msg) {
    statusEl.textContent = msg;
}

createBtn.addEventListener('click', async () => {
    setStatus('Отправка данных...');

    const firstName  = document.getElementById('firstName').value.trim();
    const middleName = document.getElementById('middleName').value.trim();
    const lastName   = document.getElementById('lastName').value.trim();
    const birthDate  = document.getElementById('birthDate').value;

    if (!firstName || !lastName || !birthDate) {
        setStatus('Заполните имя, фамилию и дату рождения');
        return;
    }

    const fhirPatient = {
        resourceType: "Patient",
        name: [
            {
                family: lastName,
                given: middleName
                    ? [firstName, middleName]
                    : [firstName]
            }
        ],
        birthDate: birthDate
    };

    try {
        const resp = await fetch(`${API_BASE}/fhir/patient`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/fhir+json'
            },
            body: JSON.stringify(fhirPatient)
        });

        if (resp.status === 401 || resp.status === 403) {
            localStorage.removeItem('authToken');
            window.location.href = '../html/login.html';
            return;
        }

        const result = await resp.json();
        setStatus('Ответ: ' + (result?.issue?.[0]?.diagnostics ?? 'ок'));
    } catch (e) {
        console.error(e);
        setStatus('Ошибка сети при создании пациента');
    }
});

deleteBtn.addEventListener('click', async () => {
    setStatus('Удаление...');

    const idStr = document.getElementById('deleteId').value.trim();
    if (!idStr) {
        setStatus('Введите ID пациента');
        return;
    }

    try {
        const resp = await fetch(`${API_BASE}/fhir/patient/${idStr}`, {
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });

        if (resp.status === 401 || resp.status === 403) {
            localStorage.removeItem('authToken');
            window.location.href = '../html/login.html';
            return;
        }

        const result = await resp.json();
        setStatus('Ответ: ' + (result?.issue?.[0]?.diagnostics ?? 'ок'));
    } catch (e) {
        console.error(e);
        setStatus('Ошибка сети при удалении пациента');
    }
});
