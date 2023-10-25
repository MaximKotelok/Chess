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
	console.log(getCookie("matchId"));
	connection.on("Started", () => {
		window.location.href = `/Game/Match/Play/${getCookie("matchId")}`;
	});

	connection.on("Count", (a) => {
		console.log(a);
	});

	connection.invoke("JoinLobby", getCookie("matchId"))
		.then(() => {
			console.log("Joined to lobby");
		})
		.catch((error) => {
			console.error("Error joining the lobby:", error);
		});

});