$(document).ready(function () {
   
    $("#usernameInput").on("input", function () {
        var username = $(this).val();
        if (username != "") {



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
            resultContainer.text("");
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
            resultContainer.text("Пользователи не найдены.");
        }
    }
});