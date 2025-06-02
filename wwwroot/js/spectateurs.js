function activerFiltreSpectateurs() {
    const $input = $('#spectateurSearch');
    const $eventGroups = $('.event-group');
    const $noMatchMessage = $('#noMatchMessage');

    if ($input.length === 0 || $eventGroups.length === 0) return;

    $input.off('input').on('input', function () {
        const searchTerm = $(this).val().toLowerCase();
        let anyEventVisible = false;

        $eventGroups.each(function () {
            const $group = $(this);
            const eventTitle = $group.find('.event-header').text().toLowerCase();

            // Filtrer les lignes dans le tbody de cet événement
            const $rows = $group.find('tbody tr');
            let anyRowVisible = false;

            $rows.each(function () {
                const $row = $(this);
                const nom = $row.find('td').eq(0).text().toLowerCase();
                const prenom = $row.find('td').eq(1).text().toLowerCase();

                const rowMatch = nom.includes(searchTerm) || prenom.includes(searchTerm);
                $row.toggle(rowMatch);

                if (rowMatch) anyRowVisible = true;
            });

            // Afficher l'événement s'il y a correspondance dans le titre ou dans au moins une ligne
            const eventMatch = eventTitle.includes(searchTerm) || anyRowVisible;
            $group.toggle(eventMatch);

            if (eventMatch) anyEventVisible = true;
        });

        $noMatchMessage.toggle(!anyEventVisible);
    });
}