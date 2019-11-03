var images = {
    init: function () {
        images.regEvents();
    },
    //Delete all list images
    regEvents: function () {
        $('#btnDeleteAll').off('click').on('click', function () {
            if (window.confirm("Do you want delete all images?")) {
                $.ajax({
                    url: '/Images/DeleteAll',
                    dataType: 'json',
                    type: 'POST',
                    success: function (res) {
                        window.location.href = "/Images/Index";
                    }
                })
            }
        });
        $('.btn-delete').off('click').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                data: { id: $(this).data('id') },
                url: '/Images/Delete',
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/Images/Index";
                    }
                }
            })
        });
        $('#btnDone').off('click').on('click', function () {
            if (window.confirm("Do you want apply this list for schedule?")) {
                $.ajax({
                    url: '/Images/Done',
                    dataType: 'json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            window.location.href = "/";
                        }
                        else {
                            console.log(res);
                        }
                    }
                })
            }
        });

        $('.btn-edit').off('click').on('click', function (e) {
            e.preventDefault();
            $(".editTab").load('/Images/Edit?id=' + $(this).data('id'));
            $("#editActive").tab('show');
        });
    }
}
images.init();