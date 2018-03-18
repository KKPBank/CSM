$jq(function () {

    $jq(".collapse_container .collapse_header").click(function () {

        var body = $jq(this).parent().find(".collapse_body");

        if ($jq(body).css("display") == "none") {
            $jq(this).find(".collapse_sign").html("[&mdash;]");
        }
        else {
            $jq(this).find(".collapse_sign").html("[+]");
        }

        $jq(body).toggle();
    });

    $jq(".money").autoNumeric("init");
});

function replaceAllIfNotEmpty(original, search, replacement) {
    replacement = replacement.trim();
    if (replacement.length == 0)
        replacement = "";

    return original.split(search).join(replacement);
};

function resetValidation() {
    //Removes validation from input-fields
    $jq('.input-validation-error').addClass('input-validation-valid');
    $jq('.input-validation-error').removeClass('input-validation-error');
    //Removes validation message after input-fields
    $jq('.field-validation-error:not(.custom)').addClass('field-validation-valid');
    $jq('.field-validation-error:not(.custom)').removeClass('field-validation-error');
    //Removes validation summary 
    $jq('.validation-summary-errors').addClass('validation-summary-valid');
    $jq('.validation-summary-errors').removeClass('validation-summary-errors');

}

function checkAccountNoIsMatch() {

    var accountNo = $jq("#hiddenAccountNo").val().trim();
    var contactAccountNo = $jq("#hiddenContactAccountNo").val().trim();

    if (accountNo.length == 0 || contactAccountNo.length == 0 || accountNo == contactAccountNo) {
        $jq("#divAccountNoNotMatch").hide();
    }
    else {
        $jq("#divAccountNoNotMatch").show();
    }
}

function paddy(n, p, c) {
    var pad_char = typeof c !== 'undefined' ? c : '0';
    var pad = new Array(1 + p).join(pad_char);
    return (pad + n).slice(-pad.length);
    // var fu = paddy(14, 5); ==> 00014
    // var bar = paddy(2, 4, '#'); ==> ###2
}



function checkValidEmails(emailList) {

    var emails = emailList.split(",");

    var valid = true;
    var regex = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

    for (var i = 0; i < emails.length; i++) {
        if (emails[i] == "" || !regex.test(emails[i].trim())) {
            valid = false;
        }
    }

    return valid;
}


function doModalWithCloseEvent(placementId, heading, formContent, functionName) {
    var html = '<div id="modalWindow" class="modal fade" data-backdrop="static" data-keyboard="false">';
    html += '<div class="modal-dialog">';
    html += '<div class="modal-content">';
    html += '<div class="modal-header">';
    html += '<a class="close" data-dismiss="modal" onclick="' + functionName + '">×</a>';
    html += '<h4>' + heading + '</h4>';
    html += '</div>';
    html += '<div class="modal-body">';
    html += '<p>';
    html += formContent;
    html += '</p>';
    html += '</div>';
    html += '<div class="modal-footer">';
    html += '<span class="btn btn-gray btn-xsmall btn-sm" data-dismiss="modal" onclick="' + functionName + '">';
    html += 'ปิด';
    html += '</span>'; // close button
    html += '</div>';  // content
    html += '</div>';  // dialog
    html += '</div>';  // footer
    html += '</div>';  // modalWindow
    $jq("#" + placementId).html(html);
    $jq("#modalWindow").modal('show');
}

var invalidScripts = "<>";
function HasInValidScripts(string) {
    for (i = 0; i < invalidScripts.length; i++) {
        if (string.indexOf(invalidScripts[i]) > -1) {
            return true;
        }
    }
    return false;
}
var invalidChars = "<>@!#$%^&*()_+[]{}?:;|'\"\\,./~`-=";
function HasInValidChar(string) {
    for (i = 0; i < invalidChars.length; i++) {
        if (string.indexOf(invalidChars[i]) > -1) {
            return true;
        }
    }
    return false;
}

function addValidateError(selector, errorMessage) {

    inputCtrl = $jq(selector);

    if (inputCtrl.length > 0) {
        if (inputCtrl.parent().hasClass('date')) {
            spanCtrl = inputCtrl.parent().parent().find('span.field-validation-valid');
        } else {
            spanCtrl = inputCtrl.parent().find('span.field-validation-valid');
        }

        $jq(selector).removeClass('input-validation-error');
        $jq(selector).addClass('input-validation-error');
        spanCtrl.html(errorMessage).removeClass('field-validation-valid').addClass('field-validation-error');
    }
}

function removeValidateError(selector) {
    var inputCtrl = $jq(selector);
    if (inputCtrl.length == 0)
        return;

    $jq(inputCtrl.parent()).find('.input-validation-error').removeClass('input-validation-error');
    $jq(inputCtrl.parent()).find('.field-validation-error').addClass('field-validation-valid').removeClass('field-validation-error');
}

function isLoginForm(html) {
    //    return (html.includes("login-username") && html.includes("login-password"));

    if (html.indexOf("login-username") != -1 && html.indexOf("login-password") != -1) {
        return true;
    }
    return false;
}
