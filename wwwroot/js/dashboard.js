$(document).ready(function () {
    // Soumission de formulaire AJAX (déjà ok)
    $(document).on('submit', 'form[data-ajax="true"]', function (e) {
        e.preventDefault();

        const $form = $(this);
        const url = $form.attr('action');
        const data = $form.serialize();

        $.ajax({
            url: url,
            method: 'POST',
            data: data,
            success: function (html) {
                $('#dashboard-content').html(html);
            },
            error: function () {
                $('#dashboard-content').html("<div class='alert alert-danger'>Une erreur est survenue lors de la soumission du formulaire.</div>");
            }
        });
    });

    // Charger la section au chargement initial **seulement si paramètre 'section' existe**
    const urlParams = new URLSearchParams(window.location.search);
    const initialSection = urlParams.get('section');
    if (initialSection) {
        loadSection(initialSection);
    }
});

function loadSection(section) {
    $.ajax({
        url: '/Dashboard/LoadSection',
        method: 'GET',
        data: { section: section },
        success: function (html) {
            $('#dashboard-content').html(html);
            history.pushState({ section: section }, '', '/Dashboard?section=' + section);

            if (section === 'avis') {
                activerFiltreAvis(); // appelée ici, fonction globale
            }
        },
        error: function () {
            $('#dashboard-content').html("<div class='alert alert-danger'>Erreur de chargement de la section.</div>");
        }
    });
}

window.onpopstate = function (event) {
    if (event.state && event.state.section) {
        loadSection(event.state.section);
    }
};
