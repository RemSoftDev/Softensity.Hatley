(function ($) {
    var start = $('#backupData');
    var backupDone = $('#backupDone');
    var backupInfo = $('#backupInfo');
    var receiversAccounts = $('#receiversAccounts');
    var sourcesAccounts = $('#sourcesAccounts');
    var backupErrors = $('#backupErrors');
    
    var setAccount = function (id, name) {
        return '<div id="' + id + '" class="accountItem">' +
                   '<span class="statusBar">' +
                        '<span class="status glyphicon glyphicon-refresh" style="display: none;"></span>' +
                        '<span class="statusOk glyphicon glyphicon-ok" style="display: none;"></span>' +
                        '<span class="statusError glyphicon glyphicon-remove" style="display: none;"></span>' +
                   '</span>' +
                   '<span>&nbsp;' + name + '</span>' +
               '</div>';
    };
    var setErrors = function (id) {
        return '<div class="alert alert-warning" class="row">' +
                        '<a href="#" class="close" data-dismiss="alert">×</a>' +
                        '<span>' + id + ' fail. Please connect to the account again.</span>' +
                   ' </div>';
    };

    var backupPopup = $('#backupPopup').modal({
        keyboard: false,
        show: false,
        backdrop: 'static'
    });
    var errors = false;
    backupDone.click(function () {
        if (errors) {
            window.location.replace('/User/Index');
        }
        backupPopup.modal('hide');
        errors = false;
        backupInfo.text('Connection...');
        receiversAccounts.empty();
        sourcesAccounts.empty();
        backupErrors.empty();
    });

    var iconRotation = null;

    var hub = $.connection.backupDataHub;
    hub.client.accountStart = function (account) {
        $('#' + account + ' .status').show();
    };
    hub.client.accountComplete = function (account) {
        $('#' + account + ' .status').hide();
        $('#' + account + ' .statusOk').show();
    };
    hub.client.backupComplete = function (message) {
        $.connection.hub.stop();
        iconRotation.stop();
        backupDone.prop("disabled", false);

        console.log(message);
    };
    hub.client.progress = function (message) {
        backupInfo.text(message);
    };
    hub.client.showError = function (account) {
        errors = true;
        $('#' + account + ' .status').hide();
        $('#' + account + ' .statusOk').hide();
        $('#' + account + ' .statusError').show();
        backupErrors.append(setErrors(account));
    };

    start.click(function () {
        backupDone.prop("disabled", true);
        backupPopup.modal('show');
        $.connection.hub.start().done(function () {
            $.post("/User/Information", function (data) {
                var user = data;
                user.Sources.forEach(function (item) {
                    sourcesAccounts.append(setAccount(item, item));
                });
                user.Receivers.forEach(function (item) {
                    receiversAccounts.append(setAccount(item, item));
                });
                iconRotation = new IconRotation($('.status'));
                iconRotation.start();
                hub.server.start(user.UserId);
            });
        });
    });
})(jQuery)