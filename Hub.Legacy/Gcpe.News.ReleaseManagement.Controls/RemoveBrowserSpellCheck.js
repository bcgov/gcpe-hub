//ensure built-in browser spellcheckers are turned off
$(document).ready(function () {
    $('#@ClientID@ iframe').contents().find('body').attr('spellcheck', 'false');
});