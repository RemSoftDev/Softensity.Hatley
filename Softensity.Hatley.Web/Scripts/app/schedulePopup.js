(function ($) {
    var schedulePopup = $('#schedulePopup').modal({
        keyboard: false,
        show: false,
    });
    var saveSchedule = $('#saveSchedule');
    var openSchedule = $('#openSchedule');
    var lastBackup = $('#lastBackup');
    var nextBackup = $('#nextBackup');
    var scheduleType = $('#scheduleType');

    var scheduleDisable = $('#scheduleDisable');
    var scheduleRadio = $('.schedule');

    var timeSchedule = $('#timeSchedule');
    var dayOfWeekSchedule = $('#dayOfWeekSchedule');
    var dayOfMonthSchedule = $('#dayOfMonthSchedule');
    var scheduleOption = $('.scheduleOption');

    var currentType = 1;
    var currentTypeDisabled = true;
    var onCangeScheduleType = function () {
        if (currentTypeDisabled) {
            scheduleOption.prop('disabled', true);
            scheduleRadio.addClass('disabled');
        } else {
            switch (currentType) {
                case 1:
                default:
                    scheduleOption.prop('disabled', false);
                    scheduleRadio.removeClass('disabled');
                    dayOfWeekSchedule.parent().parent().hide();
                    dayOfMonthSchedule.parent().parent().hide();
                    break;
                case 2:
                    scheduleOption.prop('disabled', false);
                    scheduleRadio.removeClass('disabled');
                    dayOfWeekSchedule.parent().parent().show();
                    dayOfMonthSchedule.parent().parent().hide();
                    break;
                case 3:
                    scheduleOption.prop('disabled', false);
                    scheduleRadio.removeClass('disabled');
                    dayOfWeekSchedule.parent().parent().hide();
                    dayOfMonthSchedule.parent().parent().show();
                    break;
            }
        }
    };
    
    var convertDate = function (ticks) {
        if (ticks != 0) {
            var date = new Date(ticks);
            //date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
            return date.toLocaleString();
        } else {
            return '-';
        }
    };

    var setSchedule = function(data) {
        lastBackup.text(convertDate(data.LastBackup));
        nextBackup.text(convertDate(data.NextBackup));
        scheduleType.text(data.ScheduleTypeString);
    };

    openSchedule.click(function () {
        $.get("/User/Schedule", function (data) {      
            currentTypeDisabled = data.IsDisable;
            if (!currentTypeDisabled) {
                currentType = data.ScheduleType;
                timeSchedule.val(data.Time || 0);
                dayOfWeekSchedule.val(data.DayOfWeek || 0);
                dayOfMonthSchedule.val(data.DayOfMonth || 1);

                $('#scheduleType' + currentType).prop("checked", true);
            }
            onCangeScheduleType();
            schedulePopup.modal('show');
        });
    });

    saveSchedule.click(function () {
        var type;
        if (currentTypeDisabled) {
            type = 0;
        } else {
            type = currentType;
        }
        var currentDate = new Date();
        $.post("/User/Schedule",
            {
                scheduleType: type,
                time: timeSchedule.val(),
                timezone: currentDate.getTimezoneOffset(),
                dayOfWeek: dayOfWeekSchedule.val(),
                dayOfMonth: dayOfMonthSchedule.val()
            },
            function (data) {
                setSchedule(data);
                schedulePopup.modal('hide');
            });
    });
    
    scheduleDisable.change(function () {
        currentTypeDisabled = scheduleDisable.prop('checked');
        onCangeScheduleType();
    });
    
    $("input[name=scheduleType]").change(function() {
        currentType = parseInt($("input[name=scheduleType]:checked").val());
        onCangeScheduleType();
    });
    
    $.get("/User/Schedule", function (data) {
        setSchedule(data);
    });

})(jQuery)