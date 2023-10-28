const loading = `<div class="w-100 d-flex justify-content-center">
                    <div class="loading">
                        <div class="ball1"></div>
                        <div class="ball2"></div>
                        <div class="ball3"></div>
                    </div>
                </div>
                `;

const templateSearch = Handlebars.compile(`
<ul class="user-list container">
  {{#if UserList}}
    {{#each UserList}}
      <li class="user-list-item">
        <div>
          <img src="{{AvatarPath}}" alt="Avatar" class="profile-photo rounded-circle" height="30" />
          <a href="/Game/Friends/profile/{{Id}}" class="user-link">{{UserName}}</a>
        </div>
      </li>
    {{/each}}
  {{else}}
    <li>There are no users</li>
  {{/if}}
</ul>
`);

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
    var resultContainer = $("#received-tab");
    resultContainer.empty();
    resultContainer.append(loading);
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
    var resultContainer = $("#friends-tab");
    resultContainer.empty();
    resultContainer.append(loading);

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
    var resultContainer = $("#sended-tab");
    resultContainer.empty();
    resultContainer.append(loading);

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
        resultContainer.append(templateSearch({ UserList: data }));
            
        }
    }
);