async function searchByIP() {
    let searchInput = document.getElementById("searchip");
    searchInput.style.borderColor = null;
    searchInput.style.borderWidth = null;
    let ipForm = document.getElementById("ip_form");
    ipForm.classList.remove("error");
    if (!validateIPaddress(searchInput.value)) {
        searchInput.style.borderColor = "#c00000";
        searchInput.style.borderWidth = "2px";
        ipForm.classList.add("error");
        return;
    }

    const url = '/ip/location?ip=' + searchInput.value;
    const response = await getResponse(url);
    let tableBody = document.getElementById("coordinates");

    if (!response.ok) {
        tableBody.innerHTML = "<tr><td colspan=2>Координаты не найдены</td></tr>";
        return;
    }
    const location = await response.json();
    tableBody.innerHTML = "<tr><td>" + location.latitude + "</td><td>" + location.longitude + "</td></tr>";
}

async function searchByCity() {
    let searchInput = document.getElementById("searchcity");
    if (!searchInput.value)
        return;
    // const url = '/city/locations?city=cit_Gbqw4';
    // const url = '/city/locations?city=cit_Ejid';
    const url = '/city/locations?city=' + searchInput.value;
    const response = await getResponse(url);
    let tableBody = document.getElementById("locations");

    if (!response.ok) {
        tableBody.innerHTML = "<tr class='error'><td colspan=7>Местоположения не найдены</td></tr>";
        return;
    }

    const locations = await response.json();
    console.log(locations);

    tableBody.innerHTML = "";
    Array.prototype.forEach.call(locations, location => {
        tableBody.innerHTML += "<tr><td>" + location.country + "</td><td>" + location.region + "</td><td>"
            + location.postal + "</td><td>" + location.city + "</td><td>" + location.organization + "</td><td>"
            + location.latitude + "</td><td>" + location.longitude + "</td></tr>";
    });
}

function showContent(id) {
    let contents = document.getElementsByClassName('content');
    Array.prototype.forEach.call(contents, element => element.style.display = 'none');
    let selectedContent = document.getElementById(id);
    selectedContent.style.display = 'block';
}

function validateIPaddress(ipaddress) {
    if (/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(ipaddress)) {
        return true;
    }
    return false;
}

async function getResponse(url) {
    return await fetch(url, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });
}