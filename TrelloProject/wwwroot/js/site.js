function init() {
    dragula([document.querySelector("#pending"),
    document.querySelector("#progress"),
    document.querySelector("#blocked"),
    document.querySelector("#done")], {
        revertOnSpill: true
    })
    .on('drop', function (el, current, prev) {
        if (current.id !== prev.id) {
            var data = new FormData();
            data.append('id', $(el)[0].children[0].name);
            data.append('status', current.id);

            var request = new XMLHttpRequest();
            var url = "/Home/Move";
            request.open("POST", url, true);
            request.send(data);
        }
    });
};

function showModal(url) {
    $.get(url,
        function (data) {
            $('.modal-body').html(data);
        });

    $("#myModal").modal("show");
};
