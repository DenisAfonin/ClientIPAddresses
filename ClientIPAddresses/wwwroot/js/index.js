function searchByIP() {
    const Http = new XMLHttpRequest();
    const url = '/ip/location?ip=123.234.123.234';
    Http.open("GET", url);  

    Http.onreadystatechange = (e) => {
        if (Http.readyState === XMLHttpRequest.DONE) {
            console.log(Http.responseText);
        }
    }
    Http.send();
}

function searchByCity() {
    const Http = new XMLHttpRequest();
    const url = '/city/locations?city=cit_Gbqw4';
    Http.open("GET", url);

    Http.onreadystatechange = (e) => {
        if (Http.readyState === XMLHttpRequest.DONE) {
            console.log(Http.responseText);
        }
    }
    Http.send();
}

function showContent(id) {
    var contents = document.getElementsByClassName('content');
    Array.prototype.forEach.call( contents, element => element.style.display = 'none');
    const selectedContent = document.getElementById(id);
    selectedContent.style.display = 'block';
}
