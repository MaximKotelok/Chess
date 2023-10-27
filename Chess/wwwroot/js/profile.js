function RemoveFriendAjax(id) {
    $.ajax({
        type: "POST",
        url: "/Game/Friends/RemoveFriend",
        dataType: "json",
        data: { id: id },
        success: function () {
            location.reload();
        },
        error: function () {
        }
    });
}

function RejectAjax(id) {
    $.ajax({
        type: "POST",
        url: "/Game/Friends/Reject",
        dataType: "json",
        data: { id: id },
        success: function () {
            location.reload();
        },
        error: function () {
        }
    });
}
function AcceptAjax(id) {
    $.ajax({
        type: "POST",
        url: "/Game/Friends/Accept",
        dataType: "json",
        data: { id: id },
        success: function () {

            location.reload();
        },
        error: function (error) {
            console.log(error);
        }
    });
} 

function RecallAjax(id) {
    $.ajax({
        type: "POST",
        url: "/Game/Friends/Recall",
        dataType: "json",
        data: { id: id },
        success: function () {

            location.reload();
        },
        error: function (error) {
            console.log(error);
        }
    });
}

