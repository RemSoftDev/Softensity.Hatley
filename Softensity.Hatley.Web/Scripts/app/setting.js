(function ($) {
    var backupInfo = $('#backupInfo');
    var backupList = $('#backupList');
    var backupListBody = $('#backupListBody');

    var setBackupItem = function (item) {
        var str = '<tr>';
        str += '<td>' + new Date(item.Date).toLocaleString() + '</td>';
        str += '<td>' + item.BackupedFrom + '</td>';
        str += '<td>' + item.BackupedTo + '</td>';
        str += '</tr>';
        return str;
    };

    $.get("/User/BackupInformation", function (array) {
        if (array.length != 0) {
            backupListBody.empty();
            array.forEach(function (item) {
                backupListBody.append(setBackupItem(item));
            });
            backupInfo.hide();
            backupList.show();
        }
    });
})(jQuery)