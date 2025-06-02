function initToggleEvents() {
    $('#dashboard-content').off('click', '.event-header');

    $('#dashboard-content').on('click', '.event-header', function () {
        const content = $(this).next('.event-content');
        const header = $(this);

        if (content.is(':visible')) {
            content.slideUp();
            header.removeClass('active');
        } else {
            content.slideDown();
            header.addClass('active');
        }
    });
}