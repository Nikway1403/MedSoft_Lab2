const token = localStorage.getItem('authToken');
const tbody = document.getElementById('patientsBody');
const statusEl = document.getElementById('status');
const logoutBtn = document.getElementById('logoutBtn');

if (!token) {
    window.location.href = '/html/login.html';
}

logoutBtn.addEventListener('click', () => {
    localStorage.removeItem('authToken');
    window.location.href = '/html/login.html';
});

function renderPatients(list) {
    tbody.innerHTML = '';

    if (!list || list.length === 0) {
        tbody.innerHTML = `<tr><td colspan="3">Нет пациентов</td></tr>`;
        return;
    }

    for (const p of list) {
        const tr = document.createElement('tr');

        const tdName = document.createElement('td');
        tdName.textContent = p.firstName;

        const tdLast = document.createElement('td');
        tdLast.textContent = p.lastName;

        const tdBirth = document.createElement('td');
        tdBirth.textContent = p.birthDate;

        tr.appendChild(tdName);
        tr.appendChild(tdLast);
        tr.appendChild(tdBirth);

        tbody.appendChild(tr);
    }
}

async function loadPatients() {
    statusEl.textContent = 'Обновление списка...';

    try {
        const resp = await fetch('https://localhost:5001/fhir/patients', {
            headers: {
                'Authorization': 'Bearer ' + token,
                'Accept': 'application/fhir+json'
            }
        });

        if (resp.status === 401) {
            localStorage.removeItem('authToken');
            window.location.href = '/html/login.html';
            return;
        }

        const bundle = await resp.json();

        const patients = [];

        if (bundle.entry && Array.isArray(bundle.entry)) {
            for (const e of bundle.entry) {
                const r = e.resource;
                if (!r || r.resourceType !== "Patient") continue;

                let family = "";
                let given = "";

                if (r.name && r.name.length > 0) {
                    family = r.name[0].family || "";
                    if (r.name[0].given && r.name[0].given.length > 0) {
                        given = r.name[0].given[0] || "";
                    }
                }

                patients.push({
                    firstName: given,
                    lastName: family,
                    birthDate: r.birthDate || ""
                });
            }
        }

        renderPatients(patients);
        statusEl.textContent = 'Список актуален.';
    } catch (e) {
        console.error(e);
        statusEl.textContent = 'Ошибка при обновлении.';
    }
}


loadPatients();
setInterval(loadPatients, 3000);
