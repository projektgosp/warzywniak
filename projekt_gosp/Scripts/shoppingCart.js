function showCartData() {
    var actionUrl = '/cart/cart';
    $(".cd-cart-items").empty();
    $.getJSON(actionUrl, function displayData(respone) {
        var total = respone.total;
        console.log(respone);
        if (respone.productsList != null) {
            console.log(respone.productsList);
            var i = 0;
            var responeLength = respone.productsList.length;
            for (i; responeLength > i; i++) {
                $(".cd-cart-items").append('<li><span class="cd-qty">' + respone.productsList[i].Ilosc + 'x </span> \n' + respone.productsList[i].Nazwa +
                                            '<div class="cd-price"> ' + respone.productsList[i].Koszt.toFixed(2) + ' pln </div> ' +
                                             '<a href="#0" onclick=(removeItem(' + respone.productsList[i].ID_Towaru + '))>Usuń</a></li><hr>')
            }
            $(".cd-cart-total").empty();
            $(".cd-cart-total").append('<p>Łącznie do zapłaty <span>' + total.toFixed(2) + ' pln </span></p>');
        }
    });
}

$(".cd-item-remove").click(function () {

});

var removeItem = function (id) {


    console.log(typeof id);

    var actionUrl = "/cart/delete";
    $.ajax({
        url: actionUrl,
        type: 'POST',
        data: JSON.stringify({
            id: id
        }),
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        success: function (data) {
            showCartData();
        },
        error: function () {
            alert("error");
        }
    });
};

var addToCart = function () {

    var link = window.location.href;

    var id_param = link.match(/id\/\d+/)[0];

    var id = id_param.match(/\d+/)[0];

    var produktId = $("#produktId").text();

    var count = $("#count").val();

    if (count <= 0) {
        alert("Ilość musi być dodatnia.");
        return;
    }

    var actionUrl = "/cart/info";

    $.ajax({
        url: actionUrl,
        type: 'POST',
        data: JSON.stringify({
            ID_towaru: parseInt(id),
            Ilosc: parseInt(count)
        }),
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        success: function (data) {
            alert("produkt został pomyślnie dodany do koszyka");
            showCartData();
        },
        error: function () {
            alert("error");
        }
    });

}

var addPromotionToCart = function (id) {
    var count = $("#count-" + id.toString()).val();

    if (count <= 0) {
        alert("Cena musi być dodatnia.");
        return;
    }

    $.ajax({
        url: "/cart/info",
        type: 'POST',
        data: JSON.stringify({
            ID_towaru: id,
            Ilosc: count
        }),
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        success: function (data) {
            alert("produkt został pomyślnie dodany do koszyka");
            showCartData();
        },
        error: function () {
            alert("error");
        }
    });
}

var addProductToCart = function (id) {
    var count = $("#count-" + id.toString()).val();
    if (count <= 0) {
        alert("Cena musi być dodatnia.");
        return;
    }
    $.ajax({
        url: "/cart/info",
        type: 'POST',
        data: JSON.stringify({
            ID_towaru: id,
            Ilosc: count
        }),
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        success: function (data) {
            alert("produkt został pomyślnie dodany do koszyka");
            showCartData();
        },
        error: function () {
            alert("error");
        }
    });
}