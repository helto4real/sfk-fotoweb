
function disableLoginButtons() {
    var button = document.getElementById('loginbutton');
    button.disabled = true;
    var buttonGoogle = document.getElementById('button-Google');
    buttonGoogle.disabled = true;

    var spinner = document.getElementById('spinner');
    spinner.style.display = 'block';
}

// document.getElementById("show-password-reset-form-link").addEventListener("click", showResetPasswordForm);
// function showResetPasswordForm() {
//     const div = document.getElementById("reset-password-form");
//     div.style.display = "block";
// }