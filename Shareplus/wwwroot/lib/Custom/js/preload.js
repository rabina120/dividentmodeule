function checkTheme() {
    var gvDarkMode = parseInt(localStorage.getItem('dark-mode')) || 0;
    if (gvDarkMode == 1) document.body.classList.add('dark-mode');
}

document.onload = checkTheme();
