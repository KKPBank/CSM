$jq(function () {

    $jq(".collapse_container .collapse_header").click(function () {

        var body = $jq(this).parent().find(".collapse_body");

        if ($jq(body).css("display") === "none") {
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
    if (replacement.length === 0)
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

    if (accountNo.length === 0 || contactAccountNo.length === 0 || accountNo === contactAccountNo) {
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
        var emailAddr = emails[i].trim().replace('.@', '@'); //ให้มี ".@" ได้
        if (emails[i] === "" || !regex.test(emailAddr)) {
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
    if (inputCtrl.length === 0)
        return;

    $jq(inputCtrl.parent()).find('.input-validation-error').removeClass('input-validation-error');
    $jq(inputCtrl.parent()).find('.field-validation-error').addClass('field-validation-valid').removeClass('field-validation-error');
}

function isLoginForm(html) {
    //    return (html.includes("login-username") && html.includes("login-password"));

    if (html.indexOf("login-username") !== -1 && html.indexOf("login-password") !== -1) {
        return true;
    }
    return false;
}

/*
* 0 = actionUrl, 1 = customerId, 2 = customerNumber, 3 = CustomerCardNo, 4 = CustomerSubscriptionTypeCode, 5 = CustomerSubscriptionName, 6 = CustomerSubscriptionTypeCode, 7 = CustomerType, 8 = CustomerBirthDate
, 9 = CustomerTitleTh, 10 = CustomerFirstNameTh, 11 = CustomerLastNameTh, 12 = CustomerTitleEn, 13 = CustomerFirstNameEn, 14 = CustomerLastNameEn
, 15 = CustomerPhoneNo1, 16 = CustomerFax, 17 = CustomerEmail, 18 = CustomerEmployeeCode
, 19 = AccountId, 20 = AccountNo, 21 = AccountStatusName 22 = AccountStatusId, 23 = ProductGroup, 24 = Product, 25 = BranchName, 26 = CarNo
, 27 = ContactId
*/
function initNewSRWithDefaData(actionUrl, customerId, customerNo, cardNo, cbsCardTypeCode, cbsCardTypeName, custType, birthDate
    , titleTh, fNameTh, lNameTh, titleEn, fNameEn, lNameEn
    , phone, fax, email, emplCode
    , accountId, accountNo, accountStat, accountStatId, productGroup, product, branchName, carNo
    , contactId) {

    $jq('#dvTarget').html('');
    var inputCustId = $jq("<input>").attr("type", "hidden").attr("name", "CustomerId").val(customerId);
    var inputCustNo = $jq("<input>").attr("type", "hidden").attr("name", "CustomerNumber").val(customerNo);
    var inputCardNo = $jq("<input>").attr("type", "hidden").attr("name", "CustomerCardNo").val(cardNo);
    var inputCardTypeCode = $jq("<input>").attr("type", "hidden").attr("name", "CustomerCardTypeCode").val(cbsCardTypeCode);
    var inputCardTypeName = $jq("<input>").attr("type", "hidden").attr("name", "CustomerCardTypeName").val(cbsCardTypeName);
    var inputCustType = $jq("<input>").attr("type", "hidden").attr("name", "CustomerType").val(custType);
    var inputBirthDate = $jq("<input>").attr("type", "hidden").attr("name", "CustomerBirthDate").val(birthDate);
    var inputTitleTh = $jq("<input>").attr("type", "hidden").attr("name", "CustomerTitleTh").val(titleTh);
    var inputFirstNameTh = $jq("<input>").attr("type", "hidden").attr("name", "CustomerFirstNameTh").val(fNameTh);
    var inputLastNameTh = $jq("<input>").attr("type", "hidden").attr("name", "CustomerLastNameTh").val(lNameTh);
    var inputTitleEn = $jq("<input>").attr("type", "hidden").attr("name", "CustomerTitleEn").val(titleEn);
    var inputFirstNameEn = $jq("<input>").attr("type", "hidden").attr("name", "CustomerFirstNameEn").val(fNameEn);
    var inputLastNameEn = $jq("<input>").attr("type", "hidden").attr("name", "CustomerLastNameEn").val(lNameEn);
    //var inputPhone = $jq("<input>").attr("type", "hidden").attr("name", "CustomerPhoneNo1").val(phone);
    //var inputFax = $jq("<input>").attr("type", "hidden").attr("name", "CustomerFax").val(fax);
    //var inputEmail = $jq("<input>").attr("type", "hidden").attr("name", "CustomerEmail").val(email);
    var inputEmplCode = $jq("<input>").attr("type", "hidden").attr("name", "CustomerEmployeeCode").val(emplCode);
    var inputAccountId = $jq("<input>").attr("type", "hidden").attr("name", "AccountId").val(accountId);
    var inputAccountNo = $jq("<input>").attr("type", "hidden").attr("name", "AccountNo").val(accountNo);
    var inputAccountStatId = $jq("<input>").attr("type", "hidden").attr("name", "AccountStatusId").val(accountStatId);
    var inputAccountStat = $jq("<input>").attr("type", "hidden").attr("name", "AccountStatusName").val(accountStat);
    var inputProductGroup = $jq("<input>").attr("type", "hidden").attr("name", "AccountProductGroupName").val(productGroup);
    var inputProduct = $jq("<input>").attr("type", "hidden").attr("name", "AccountProductName").val(product);
    var inputBranchName = $jq("<input>").attr("type", "hidden").attr("name", "AccountBranchName").val(branchName);
    var inputCarNo = $jq("<input>").attr("type", "hidden").attr("name", "AccountCarNo").val(carNo);
    var inputContactId = $jq("<input>").attr("type", "hidden").attr("name", "ContactId").val(contactId);

    $jq('#dvTarget').append(`<form action="${actionUrl}" method="POST" class="hidden" target="_blank"></form>`);
    var inputToken = $jq("<input>").attr("type", "hidden").attr("name", "__RequestVerificationToken").val(getAntiForgeryToken());

    $jq('#dvTarget form').append($jq(inputToken));
    $jq('#dvTarget form').append($jq(inputCustId));
    $jq('#dvTarget form').append($jq(inputCustNo));
    $jq('#dvTarget form').append($jq(inputCardNo));
    $jq('#dvTarget form').append($jq(inputCardTypeCode));
    $jq('#dvTarget form').append($jq(inputCardTypeName));
    $jq('#dvTarget form').append($jq(inputCustType));
    $jq('#dvTarget form').append($jq(inputBirthDate));
    $jq('#dvTarget form').append($jq(inputTitleTh));
    $jq('#dvTarget form').append($jq(inputFirstNameTh));
    $jq('#dvTarget form').append($jq(inputLastNameTh));
    $jq('#dvTarget form').append($jq(inputTitleEn));
    $jq('#dvTarget form').append($jq(inputFirstNameEn));
    $jq('#dvTarget form').append($jq(inputLastNameEn));
    //$jq('#dvTarget form').append($jq(inputPhone));
    //$jq('#dvTarget form').append($jq(inputFax));
    //$jq('#dvTarget form').append($jq(inputEmail));
    $jq('#dvTarget form').append($jq(inputEmplCode));
    $jq('#dvTarget form').append($jq(inputAccountId));
    $jq('#dvTarget form').append($jq(inputAccountNo));
    $jq('#dvTarget form').append($jq(inputAccountStatId));
    $jq('#dvTarget form').append($jq(inputAccountStat));
    $jq('#dvTarget form').append($jq(inputProductGroup));
    $jq('#dvTarget form').append($jq(inputProduct));
    $jq('#dvTarget form').append($jq(inputBranchName));
    $jq('#dvTarget form').append($jq(inputCarNo));
    $jq('#dvTarget form').append($jq(inputContactId));
    $jq('#dvTarget form').submit();
}
