// all methods must return strings

function play() {
    $(".js-player-play").click();
}

function previous() {
    $(".js-player-backward").click();
}

function next() {
    $(".js-player-forward").click();
}

function getInfo() {

    try {
        var cover = $(".player-cover__gfx").attr("src");
        var title = $(".player-title")[0].innerText;
        var artist = $(".player-subtitle")[0].innerText;
        var time = $(".player-time-played")[0].innerText;
        var length = $(".player-time-total-inner")[0].innerText;

        return JSON.stringify({ Cover: cover, Title: title, Artist: artist, Time: time, Length: length });

    } catch (e) {
        return "";
    }
}