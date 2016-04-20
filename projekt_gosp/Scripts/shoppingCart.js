function showCartData() {
    var actionUrl = 'cart/cart';
    $(".cd-cart-items").empty();
    $.getJSON(actionUrl, function displayData(respone) {
        if (respone != null) {
            var i = 0;
            for (i; 0 != respone[i].Ilosc; i++) {
                $(".cd-cart-items").append('<li><span class="cd-qty">' + respone[i].Ilosc + 'x </span> \n Ziemniaki' +
                                            '<div class="cd-price"> $' + respone[i].Koszt + '</div> ' +
                                             '<a href="#0" class="cd-item-remove cd-img-replace">Usuń</a></li>')
            }
            $(".cd-cart-total").empty();
            $(".cd-cart-total").append('<p>Total <span>$' + respone[i].Koszt + '</span></p>');
        }
    });
}