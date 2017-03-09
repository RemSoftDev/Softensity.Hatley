IconRotation = function (items) {
    var self = this;
    var count = 0;
    var stopRotate = true;
    var rotate = function () {
        items.css({
            '-webkit-transform': 'rotate(' + count + 'deg)',
            '-moz-transform': 'rotate(' + count + 'deg)',
            '-ms-transform': 'rotate(' + count + 'deg)',
            '-o-transform': 'rotate(' + count + 'deg)',
            'transform': 'rotate(' + count + 'deg)'
        });
        if (count == 360) {
            count = 0;
        }
        count += 5;
        if (!stopRotate) {
            window.setTimeout(rotate, 10);
        }
    };
    self.start = function () {
        stopRotate = false;
        rotate();
    };
    self.stop = function () {
        stopRotate = true;
    };
};