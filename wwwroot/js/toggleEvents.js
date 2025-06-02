function initToggleEvents() {
    $('#dashboard-content').off('click', '.event-header');

    $('#dashboard-content').on('click', '.event-header', function () {
        const content = $(this).next('.event-content');
        const arrow = $(this).find('.toggle-arrow');

        if (content.is(':visible')) {
            content.slideUp();
            arrow.text('▶');
        } else {
            content.slideDown();
            arrow.text('▼');
        }
    });
}
