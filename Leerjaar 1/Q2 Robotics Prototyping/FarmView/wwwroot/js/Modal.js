window.showErrorModal = function () {
    var modal = new bootstrap.Modal(document.getElementById('errorModal'));
    modal.show();
};

window.hideErrorModal = function () {
    var modal = bootstrap.Modal.getInstance(
        document.getElementById('errorModal')
    );
    if (modal) {
        modal.hide();
    }
};
