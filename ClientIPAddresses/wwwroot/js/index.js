function searchByIP() {   
    const Http = new XMLHttpRequest();
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
    // const url = '/ip/location?ip=123.234.123.234';
    const url = '/ip/location?ip=' + searchInput.value;
    Http.open("GET", url);

    Http.onreadystatechange = (e) => {
        if (Http.readyState === XMLHttpRequest.DONE) {
            console.log(Http.responseText);
            let location = JSON.parse(Http.responseText);
            let tableBody = document.getElementById("coordinates");
            if (Http.status === 200) {
                tableBody.innerHTML = "<tr><td>" + location.latitude + "</td><td>" + location.longitude + "</td></tr>";
            } else {
                tableBody.innerHTML = "<tr><td colspan=2>" + JSON.parse(Http.responseText) + "</td></tr>";
            }
        }
    }
    Http.send();
}

function searchByCity() {
    const Http = new XMLHttpRequest();
    let searchInput = document.getElementById("searchcity");
    if (!searchInput.value)
        return;
    // const url = '/city/locations?city=cit_Gbqw4';
    // const url = '/city/locations?city=cit_Ejid';
    const url = '/city/locations?city=' + searchInput.value;
    Http.open("GET", url);

    Http.onreadystatechange = (e) => {
        if (Http.readyState === XMLHttpRequest.DONE) {
            console.log(Http.responseText);
            let locations = JSON.parse(Http.responseText);
            let tableBody = document.getElementById("locations");
            if (Http.status === 200) {
                tableBody.innerHTML = "";
                Array.prototype.forEach.call(locations, location => {
                    tableBody.innerHTML += "<tr><td>" + location.country + "</td><td>" + location.region + "</td><td>"
                        + location.postal + "</td><td>" + location.city + "</td><td>" + location.organization + "</td><td>"
                        + location.latitude + "</td><td>" + location.longitude + "</td></tr>";
                });
            } else {
                tableBody.innerHTML = "<tr class='error'><td colspan=7>" + JSON.parse(Http.responseText).error + "</td></tr>";
            }
        }
    }
    Http.send();
}

function showContent(id) {
    var contents = document.getElementsByClassName('content');
    Array.prototype.forEach.call(contents, element => element.style.display = 'none');
    const selectedContent = document.getElementById(id);
    selectedContent.style.display = 'block';
}

function validateIPaddress(ipaddress) {
    if (/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(ipaddress)) {
        return true;
    }
    return false;
} 
