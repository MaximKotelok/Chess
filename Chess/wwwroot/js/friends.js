$(document).ready(function () {
    getFriends();
    $('.nav-tabs a').click(function (e) {
        e.preventDefault();
        $(this).tab('show');
    });

    $('#received-head').click(function (e) {
        e.preventDefault();

        $.ajax({
            type: "GET",
            url: "/api/GetReceived",
            dataType: "json",
            success: function (data) {
                var parsedData = JSON.parse(data);
                displayReceived(parsedData);
            },
            error: function () {
            }
        });
    })
    $('#sended-head').click(function (e) {
        e.preventDefault();

        $.ajax({
            type: "GET",
            url: "/api/GetSended",
            dataType: "json",
            success: function (data) {
                var parsedData = JSON.parse(data);
                displaySended(parsedData);
            },
            error: function () {
            }
        });
    })



    $('#friends-head').click(function (e) {
        e.preventDefault();
        getFriends();
    })

    function getFriends() {


        $.ajax({
            type: "GET",
            url: "/api/GetFriends",
            dataType: "json",
            success: function (data) {
                var parsedData = JSON.parse(data);
                displayFriends(parsedData);
            },
            error: function () {
            }
        });
    }




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

    function displayFriends(data) {
        var resultContainer = $("#friends-tab");
        resultContainer.empty();

        if (data.length > 0) {
            var userList = "<div>";
            $.each(data, function (index, user) {
                userList += `
        <form action="/Game/Friends/RemoveFriend/${user.Id}" method="POST">
            <div>${user.UserName}</div>
            <a class="btn btn-primary" href="/Game/Friends/Profile/${user.Id}">Open Profile</a>
            <button class="btn btn-danger" type="submit">Remove</button>
        </form>
        <br>
        `;
            });
            userList += "</div>";
            resultContainer.append(userList);
        } else {
            resultContainer.text("There are no sended requests");
        }
    }


        function displaySended(data) {
            var resultContainer = $("#sended-tab");
            resultContainer.empty();

            if (data.length > 0) {
                var userList = "<div>";
                $.each(data, function (index, user) {
                    userList += `
                <form action="/Game/Friends/Recall/${user.Id}" method="POST">
                <img src="${user.AvatarPath}" alt="Avatar" class="profile-photo rounded-circle" height="30" /><a href="/Game/Friends/Profile/${user.Id}">
                ${user.UserName}                                
                
					<button class="btn btn-danger" type="submit">Recall</button>
				</form>
                <br>
                `;
                });
                userList += "</div>";
                resultContainer.append(userList);
            } else {
                resultContainer.text("There are no sended requests");
            }
        }

        function displayReceived(data) {
            var resultContainer = $("#received-tab");
            resultContainer.empty();

            if (data.length > 0) {
                var userList = "<div>";
                $.each(data, function (index, user) {
                    userList += `
                <img src="${user.AvatarPath}" alt="Avatar" class="profile-photo rounded-circle" height="30" /><a href="/Game/Friends/Profile/${user.Id}">
                ${user.UserName}                
                <form action="/Game/Friends/Accept/${user.Id}" method="POST">
					<button class="btn btn-success" type="submit">Accept</button>
				</form>
                <form action="/Game/Friends/Reject/${user.Id}" method="POST">
					<button class="btn btn-danger" type="submit">Cancel</button>
				</form>
                <br>
                `;
                });
                userList += "</div>";
                resultContainer.append(userList);
            } else {
                resultContainer.text("There are no received requests");
            }
        }


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