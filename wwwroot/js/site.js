function updatePrixValue(val) {
    document.getElementById('prixValue').textContent = val + "€";
}

document.addEventListener("DOMContentLoaded", function () {
    const slider = document.getElementById("prix");
    if (slider) {
        updatePrixValue(slider.value);
    }
});
