var connectionId = getCookie("userId");
function getIdFromUrl() {
	const nUrl = new URL(window.location.href);
	const path = nUrl.pathname; 
	const parts = path.split('/'); 
	const value = parts.pop();

	return value;
}

function getCookie(cname) {
	let name = cname + "=";
	let decodedCookie = decodeURIComponent(document.cookie);
	let ca = decodedCookie.split(';');
	for (let i = 0; i < ca.length; i++) {
		let c = ca[i];
		while (c.charAt(0) == ' ') {
			c = c.substring(1);
		}
		if (c.indexOf(name) == 0) {
			return c.substring(name.length, c.length);
		}
	}
	return "";
}

const connection = new signalR.HubConnectionBuilder().withUrl("/signalr").build();


connection.start().then(() => {
	
	connection.on("Started", () => {
		window.location.href = `/Game/Match/Play/${getIdFromUrl()}`;
	});

	connection.on("ReceiveMessage", (a,b) => {
		console.log(a + " " + b);
	});

	
	connection.invoke("JoinLobby", getIdFromUrl(), connectionId)
		.then(() => {
			console.log("Joined to lobby");
		})
		.catch((error) => {
			console.error("Error joining the lobby:", error);
		});

});

window.addEventListener("beforeunload", function (e) {
	connection.invoke("LeaveLobby", getIdFromUrl(), connectionId);

});