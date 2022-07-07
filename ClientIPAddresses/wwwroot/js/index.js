function sendAjax() {
    const Http = new XMLHttpRequest();
    const url = '/ip/location?ip=123.234.123.234';
    Http.open("GET", url);
    Http.send();

    Http.onreadystatechange = (e) => {
        console.log(Http.responseText)
    }
}