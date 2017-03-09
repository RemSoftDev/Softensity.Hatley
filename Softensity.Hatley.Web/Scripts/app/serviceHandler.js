(function ($) {
    var backupData = $('#backupData');
    window.serviceEnableHandler = function() {
        $.post("/User/Information", function (data) {
            if (data.Sources.length > 0 && data.Receivers.length > 0) {
                backupData.prop('disabled', false);
            } else {
                backupData.prop('disabled', true);
            }
        });
    };
})(jQuery)