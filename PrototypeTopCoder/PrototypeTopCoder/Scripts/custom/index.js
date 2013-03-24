$(function () {
    $('.date').each(function () {
        this.innerHTML = moment(this.innerHTML).calendar();
    });
    $('.tile.contest').click(function () {
        window.location.href = $(this).find('.action').data('url');
    });
});