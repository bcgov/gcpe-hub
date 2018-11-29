function resetEverything() {
    $('fieldset').removeClass('helpActive');
    $('.leftHelpDiv').css('display', 'none');
    $('.leftHelpDiv').removeClass('widerColumn');
    $('.rightHelpDiv').css('display', 'none');
    $('.rightHelpDiv').removeClass('widerColumn');
    $('.col1').removeClass('narrowerColumn');
    $('.col2').removeClass('narrowerColumn');
}

function onCloseClick() {
    resetEverything();
}

function onQuestionClick(helpLinkElement, openLeft) {
    if (openLeft) {
        var helpDiv = $('.leftHelpDiv');
    }
    else {
        var helpDiv = $('.rightHelpDiv');
    }

    //check if current fieldset is active already and set a variable
    var parentFieldset = $(helpLinkElement).parents('fieldset');
    if (parentFieldset.hasClass('helpActive')) {
        var parentFieldsetWasActive = true;
    }

    resetEverything();

    // if the current one was not active, make it active 
    if (!parentFieldsetWasActive) {
        parentFieldset.addClass('helpActive');
        helpDiv.css('display', 'block');
        helpDiv.addClass('widerColumn');
        $('.col1').addClass('narrowerColumn');
        $('.col2').addClass('narrowerColumn');
    }

    var helpFilePath: string = 'ContextualHelp/';
    switch (parentFieldset[0].id) {
        case 'ministryFieldset':
            helpFilePath += 'ministry.html';
            break;
        case 'eventFieldset':
            helpFilePath += 'event.html';
            break;
        case 'overviewFieldset':
            helpFilePath += 'overview.html';
            break;
        case 'scheduleFieldset':
            helpFilePath += 'schedule.html';
            break;
        case 'planningFieldset':
            helpFilePath += 'planning.html';
            break;
        case 'releaseFieldset':
            helpFilePath += 'release.html';
            break;
        default:
            break;
    }

    var fieldsetHeight: number = parentFieldset[0].clientHeight;
    var topOfFieldset: number = parentFieldset[0].offsetTop;
    var bottomOfFieldset: number = parentFieldset[0].offsetTop + parentFieldset.outerHeight(true);

    helpDiv.load(helpFilePath, function () {
        var helpDivHeight: number = helpDiv.outerHeight();

        //Vertically align center of helpdiv with center of fieldset.
        //if (fieldsetHeight > helpDivHeight) {
        //    var topOfHelp = topOfFieldset - (helpDivHeight - fieldsetHeight) / 2;
        //} else {
        //    var topOfHelp = topOfFieldset + (fieldsetHeight - helpDivHeight) / 2;
        //}
        var topOfHelp: number = topOfFieldset;

        //to make sure the bottom edge of the help box doesn't disappear
        var bottomOfHelp: number = topOfHelp + helpDivHeight;
        if (bottomOfHelp > $('.main').height()) {
            // bottomOfFieldset is a more reliable measure than main div height
            // for fieldsets in the bottom row
            topOfHelp -= (bottomOfHelp - bottomOfFieldset);
            topOfHelp -= 32; //to account for bottom margins
        }
        //to make sure the top edge of the help box doesn't disappear
        if (topOfHelp < 0) {
            topOfHelp = 0;
        }
        // TODO: deal with situation where the content of the help is taller than the entire screen 
        // this should probably involve forcing the length of the main div to be longer
        helpDiv.css('top', topOfHelp);
    });
}