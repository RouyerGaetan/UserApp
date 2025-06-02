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
                // On peut aussi mettre à jour l'URL si nécessaire
                // Par exemple si la soumission doit changer la section, à voir
            },
            error: function () {
                $('#dashboard-content').html("<div class='alert alert-danger'>Une erreur est survenue lors de la soumission du formulaire.</div>");
            }
        });
    });

    // Charger la section au chargement initial si paramètre present
    const urlParams = new URLSearchParams(window.location.search);
    const initialSection = urlParams.get('section') || 'profil'; // default section
    loadSection(initialSection);
});

// Fonction globale pour charger dynamiquement une section du dashboard et mettre à jour l'URL
function loadSection(section) {
    fetch('/Dashboard/LoadSection?section=' + section)
        .then(response => {
            if (!response.ok) throw new Error("Erreur de chargement");
            return response.text();
        })
        .then(html => {
            document.getElementById('dashboard-content').innerHTML = html;
            // Met à jour l'URL sans recharger la page
            history.pushState({ section: section }, '', '/Dashboard?section=' + section);
        })
        .catch(error => {
            document.getElementById('dashboard-content').innerHTML =
                "<div class='alert alert-danger'>Erreur de chargement de la section.</div>";
        });
}

// Gestion du bouton back/forward du navigateur
window.onpopstate = function (event) {
    if (event.state && event.state.section) {
        loadSection(event.state.section);
    }
};
