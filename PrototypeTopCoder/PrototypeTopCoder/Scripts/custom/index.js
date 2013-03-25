// Load the Visualization API and the piechart package.
google.load('visualization', '1.0', { 'packages': ['corechart'] });


$(function () {
    $('.date').each(function () {
        this.innerHTML = moment(this.innerHTML).calendar();
    });
    $('.tile.contest').click(function () {
        window.location.href = $(this).find('.action').data('url');
    });
});