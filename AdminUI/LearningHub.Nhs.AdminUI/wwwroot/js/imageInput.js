$(function () {
    $('.image-input').each(function (i, x) {
        var $x = $(x);
        var value = $x.find('.image-url-value').val();
        var baseUrl = $x.find('.image-url-base').val();
        var $container = $x.find('.image-input-container');
        var $display = $x.find('.image-input-display');
        var $browse = $x.find('.image-input-browse');
        var $change = $x.find('.image-input-change');
        var $remove = $x.find('.image-input-remove');
        var $imageInput = $x.find('.image-input-hidden');
        var $img = $x.find('.image-input-display-image');
        if (value) {
            $display.show();
            $container.hide();
            $img[0].src = baseUrl + value;
        } else {
            $display.hide();
            $container.show();
        }
        $browse.on('click', function () {
            $imageInput[0].click();
        });
        $change.on('click', function () {
            $imageInput.val(null);
            $imageInput[0].click();
        });
        $remove.on('click', function () {
            let type = $remove.attr('name');
            let value1 = 'hdn' + type + 'Value1'
            let value2 = 'hdn' + type + 'Value2'
            $('#' + value1).val(null);
            $('#' + value2).val(null);
            value = null;
            $imageInput.val(null);
            this.files = [];
            $display.hide();
            $container.show();
        });
        $imageInput.on('change', function () {
            var file = this.files[0];
            $img[0].src = URL.createObjectURL(file);
            $display.show();
            $container.hide();
            return false;
        });
    });
});