function activerFiltreAvis() {
    const $input = $('#avisSearch');
    const $items = $('.avis-item');
    const $noMatchMessage = $('#noMatchMessage');

    if ($input.length === 0 || $items.length === 0) return;

    $input.off('input').on('input', function () {
        const searchTerm = $(this).val().toLowerCase();
        let anyVisible = false;

        $items.each(function () {
            const text = $(this).text().toLowerCase();
            const match = text.includes(searchTerm);
            $(this).toggle(match);
            if (match) anyVisible = true;
        });

        $noMatchMessage.toggle(!anyVisible);
    });
}
