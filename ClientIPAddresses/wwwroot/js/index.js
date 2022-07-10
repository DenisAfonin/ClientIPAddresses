(() => {
    let previousCity = '';
    let previousIP = '';

    window.onload = () => {
        let ipForm = document.forms.ip;
        let cityForm = document.forms.city;
        ipForm.addEventListener("submit", searchByIP);
        cityForm.addEventListener("submit", searchByCity);

        var menuItems = document.getElementsByClassName("menu-item");
        for (var i = 0; i < menuItems.length; i++) {
            menuItems[i].addEventListener('click', showContent);
        }
    }

    async function searchByIP() {
        let searchInput = this.elements.search;
        let searchInputValue = searchInput.value;
        if (searchInputValue === previousIP)
            return;
        previousIP = searchInputValue;
        if (!validateIPaddress(searchInputValue)) {
            this.classList.add("error");
            return;
        }

        this.classList.remove("error");
        const url = '/ip/location?ip=' + searchInputValue;
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
        let searchInput = this.elements.search;
        let searchInputValue = searchInput.value;
        if (!searchInputValue || searchInputValue == previousCity)
            return;
        // const url = '/city/locations?city=cit_Gbqw4';
        // const url = '/city/locations?city=cit_Ejid';
        const url = '/city/locations?city=' + searchInputValue;
        const response = await getResponse(url);

        let tableBody = document.getElementById("locations");
        if (!response.ok) {
            tableBody.innerHTML = "<tr class='error'><td colspan=7>Местоположения не найдены</td></tr>";
            return;
        }

        const locations = await response.json();
        tableBody.innerHTML = "";
        for (let i = 0; i < locations.length; i++) {
            let location = locations[i];
            tableBody.innerHTML += "<tr><td>" + location.country + "</td><td>" + location.region + "</td><td>"
                + location.postal + "</td><td>" + location.city + "</td><td>" + location.organization + "</td><td>"
                + location.latitude + "</td><td>" + location.longitude + "</td></tr>";
        }
    }

    function showContent() {
        var itemName = this.getAttribute("item");
        let contents = document.getElementsByClassName('content');
        for (let i = 0; i < contents.length; i++) {
            let content = contents[i];
            if (itemName == content.getAttribute('id'))
                content.classList.remove("hidden");
            else
                content.classList.add("hidden");
        }
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
})();