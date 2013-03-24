$(function () {
    $('.date').each(function () {
        this.innerHTML = moment(this.innerHTML).calendar();
    });
});