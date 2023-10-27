
//#region Received
function RejectAjax(id) {
    $.ajax({
        type: "POST",
        url: "/Game/Friends/Reject",
        dataType: "json",
        data: { id: id },
        success: function () {
            getReceived();
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

            getReceived();
        },
        error: function (error) {
            console.log(error);
        }
    });
} 

function getReceived() {

    $.ajax({
        type: "POST",
        url: "/Partial/Friends/GetReceived",
        dataType: "text",
        success: function (data) {
            displayReceived(data);
        },
        error: function () {

        }
    });
}
function displayReceived(data) {
    var resultContainer = $("#received-tab");
    resultContainer.empty();

    resultContainer.append(data);
}

//#endregion

//#region Friends
function RemoveFriendAjax(id) {
    $.ajax({
        type: "POST",
        url: "/Game/Friends/RemoveFriend",
        dataType: "json",
        data: { id: id },
        success: function () {
            getFriends();
        },
        error: function () {
        }
    });
}

function getFriends() {
    $.ajax({
        type: "POST",
        url: "/Partial/Friends/GetFriends",
        dataType: "text",
        success: function (data) {
            displayFriends(data);
        },
        error: function () {

        }
    });
}
function displayFriends(data) {
    var resultContainer = $("#friends-tab");
    resultContainer.empty();


    resultContainer.append(data);
    }
//#endregion

//#region Sended
function RecallAjax(id) {
    $.ajax({
        type: "POST",
        url: "/Game/Friends/Recall",
        dataType: "json",
        data: { id: id },
        success: function () {

            getSended();
        },
        error: function (error) {
            console.log(error);
        }
    });
}


function getSended() {

    $.ajax({
        type: "POST",
        url: "/Partial/Friends/GetSended",
        dataType: "text",
        success: function (data) {

            displaySended(data);
        },
        error: function () {
        }
    });

}

function displaySended(data) {
    var resultContainer = $("#sended-tab");
    resultContainer.empty();

    resultContainer.append(data);
}

//#endregion



$(document).ready(function () {
    
    $('.nav-tabs a').click(function (e) {
        e.preventDefault();
        $(this).tab('show');
    });

    $('#received-head').click(function (e) {
        e.preventDefault();

        getReceived();
    })
    $('#sended-head').click(function (e) {
        e.preventDefault();
        getSended();
        
    })
    $('#friends-head').click(function (e) {
        e.preventDefault();
        getFriends();
    })

    getFriends();





    




    $("#usernameInput").on("input", function () {
        var username = $(this).val();
        if (username.length >= 3) {



            $.ajax({
                type: "GET",
                url: "/api/GetUsersByUsername",
                data: { username: username },
                dataType: "json",
                success: function (data) {
                    var parsedData = JSON.parse(data);
                    displayResults(parsedData);
                },
                error: function () {
                }
            });
        } else {
            var resultContainer = $("#resultContainer");
            resultContainer.empty();
        }
    });
    





        function displayResults(data) {
            var resultContainer = $("#resultContainer");
            resultContainer.empty();

            if (data.length > 0) {
                var userList = "<div>";
                $.each(data, function (index, user) {
                    userList += `<img src="${user.AvatarPath}" alt="Avatar" class="profile-photo rounded-circle" height="30" /><a href="/Game/Friends/Profile/${user.Id}">
                
                ${user.UserName}
                </a><br>
                `;
                });
                userList += "</div>";
                resultContainer.append(userList);
            } else {
                resultContainer.text("Users not found");
            }
        }
    }
);