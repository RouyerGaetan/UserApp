$(document).ready(function () {
    // Gestion des soumissions de formulaires dynamiques via AJAX
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
});

// Fonction globale pour charger dynamiquement une section du dashboard
function loadSection(section) {
    fetch('/Dashboard/LoadSection?section=' + section)
        .then(response => {
            if (!response.ok) throw new Error("Erreur de chargement");
            return response.text();
        })
        .then(html => {
            document.getElementById('dashboard-content').innerHTML = html;
        })
        .catch(error => {
            document.getElementById('dashboard-content').innerHTML =
                "<div class='alert alert-danger'>Erreur de chargement de la section.</div>";
        });
}

