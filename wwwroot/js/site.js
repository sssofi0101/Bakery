function onDecreaseCountByOneClick(url, cartItemId) {
    $.ajax({
        type: 'POST',
        url: url,
        data: { cartItemId: cartItemId },
        success: function (itemCountInfoJson) {
            let itemCountInfo = JSON.parse(itemCountInfoJson)
            if (itemCountInfo.itemCount == 0) {
                window.location.reload()
            }
            else {
                $("#cartItemCount-" + itemCountInfo.cartItemId).text(itemCountInfo.itemCount);
                let formatter = new Intl.NumberFormat("ru", { minimumFractionDigits: 2 });
                $("#cartItemSum-" + itemCountInfo.cartItemId).text(formatter.format(itemCountInfo.newSum.toFixed(2)));
            }
        },
        error: function (error) {
            console.error("Error: " + error);
        }
    })
}
function onIncreaseCountByOneClick(url, cartItemId) {
    $.ajax({
        type: 'POST',
        url: url,
        data: { cartItemId: cartItemId },
        success: function (itemCountInfoJson) {
            let itemCountInfo = JSON.parse(itemCountInfoJson)
            $("#cartItemCount-" + itemCountInfo.cartItemId).text(itemCountInfo.itemCount);
            let formatter = new Intl.NumberFormat("ru", { minimumFractionDigits: 2 });
            $("#cartItemSum-" + itemCountInfo.cartItemId).text(formatter.format(itemCountInfo.newSum.toFixed(2)));
        },
        error: function (error) {
            console.error("Error: " + error);
        }
    })
}