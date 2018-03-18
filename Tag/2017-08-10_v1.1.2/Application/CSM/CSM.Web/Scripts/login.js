var $jq = jQuery.noConflict();

function doModal(placementId, heading, formContent, strSubmitFunc, btnText) {
    var html = '<div id="modalWindow" class="modal fade">';
    html += '<div class="modal-dialog">';
    html += '<div class="modal-content">';
    html += '<div class="modal-header">';
    html += '<a class="close" data-dismiss="modal">×</a>';
    html += '<h4>' + heading + '</h4>';
    html += '</div>';
    html += '<div class="modal-body">';
    html += '<p>';
    html += formContent;
    html += '</p>';
    html += '</div>';
    html += '<div class="modal-footer">';
    if (btnText != '') {
        html += '<span class="btn btn-blue btn-xsmall btn-sm"';
        html += ' onClick="' + strSubmitFunc + '">' + btnText;
        html += '</span>';
    }
    html += '<span class="btn btn-gray btn-xsmall btn-sm" data-dismiss="modal">';
    html += 'ปิด';
    html += '</span>'; // close button
    html += '</div>';  // content
    html += '</div>';  // dialog
    html += '</div>';  // footer
    html += '</div>';  // modalWindow
    $jq("#" + placementId).html(html);
    $jq("#modalWindow").modal('show');
}

function hideModal(placementId) {
    // Using a very general selector - this is because $('#modalDiv').hide
    // will remove the modal window but not the mask
    $jq('#' + placementId).modal('hide');
}