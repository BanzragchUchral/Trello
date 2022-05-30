function init() {
    dragula([document.querySelector("#pending"),
    document.querySelector("#progress"),
    document.querySelector("#blocked"),
    document.querySelector("#done")], {
        revertOnSpill: true,
        moves: function (el, container, handle) {
            return el.localName !== "h2";
        }
    })
    .on('drop', function (el, current, prev) {
        if (current.id !== prev.id) {
            var data = new FormData();
            data.append('id', $(el)[0].children[0].name);
            data.append('status', current.id);

            fetch('/Home/Move', {
                method: 'POST',
                body: data
            })
            .catch((error) => {
                console.error('Error:', error);
            });
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

function deleteCard(id) {
    var element = $('[name=' + id + ']').parent()[0];

    element.remove();
};

function renderAll() {
    var allCards = $.get('https://localhost:44348/api/Card/GetAllCards');

    $.each($(allCards.responseJSON), function (asd) {
        console.log(asd);
    });
};