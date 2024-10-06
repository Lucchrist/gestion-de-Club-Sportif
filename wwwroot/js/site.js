// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

<script>
    document.addEventListener('scroll', function() {
    const heroSection = document.querySelector('.hero-section');
    const trainingSection = document.querySelector('.training-section');

    // Position de la section training par rapport au haut de la page
    const trainingPosition = trainingSection.getBoundingClientRect().top;

    // Si la section training atteint le milieu de la page, on cache la section hero
    if (trainingPosition < window.innerHeight / 2) {
        heroSection.classList.add('hero-hidden');
    } else {
        heroSection.classList.remove('hero-hidden');
    }
});
</script>
