String.prototype.toHHMMSS = function () {
    sec_numb = parseInt(this);
    var hours = Math.floor(sec_numb / 3600);
    var minutes = Math.floor((sec_numb - (hours * 3600)) / 60);
    var seconds = sec_numb - (hours * 3600) - (minutes * 60);

    if (hours < 10) { hours = "0" + hours; }
    if (minutes < 10) { minutes = "0" + minutes; }
    if (seconds < 10) { seconds = "0" + seconds; }
    var time = hours + ':' + minutes + ':' + seconds;
    return time;
}

$(function () {
    var timeLeftSpan = $('#timeLeft');
    var timeLeft = timeLeftSpan.data('timeleft');
    var decreaseTimeLeft = function () {
        --timeLeft;
        if (timeLeft != 0) {
            timeLeftSpan.text(("" + timeLeft).toHHMMSS());
            setTimeout(decreaseTimeLeft, 1000);
        } else {
            $('#countdown').text("Contest is over!");
        }
    };
    setTimeout(decreaseTimeLeft, 1000);
});