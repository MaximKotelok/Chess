
/*=========глобальні змінні=========================*/
var isHistory = false;
var isWhitePlayer = false;
var isWhitePlayerMove = true;
var sessionId = getIdFromUrl()
console.log(sessionId);
var connectionId = getCookie("userId");
var square_class = document.getElementsByClassName("square");
var white_checker_class = document.getElementsByClassName("white_checker");
var black_checker_class = document.getElementsByClassName("black_checker");
var history_checker = "white_checker";
var table = document.getElementById("table");
var score = document.getElementById("score");
/*var black_background = document.getElementById("black_background");*/
var moveSound = document.getElementById("moveSound");
var winSound = document.getElementById("winSound");
var windowHeight = window.innerHeight
	|| document.documentElement.clientHeight
	|| document.body.clientHeight;;
var windowWidth = window.innerWidth
	|| document.documentElement.clientWidth
	|| document.body.clientWidth;
var moveLength = 80;
var moveDeviation = 10;
var Dimension = 1;
var selectedPiece, selectedPieceindex;
var fromX, fromY;
var upRight, upLeft, downLeft, downRight;  // всі можливі варіанти ходів для шашки
var contor = 0, gameOver = 0;
var bigScreen = 1;

var block = [];
var w_checker = [];
var b_checker = [];
var the_checker;
var oneMove;
var anotherMove;
var mustAttack = false;
var multiplier = 1 //2, якщо він стрибає, 1 у разі контракту

var tableLimit, reverse_tableLimit, moveUpLeft, moveUpRight, moveDownLeft, moveDownRight, tableLimitLeft, tableLimitRight;

const connection = new signalR.HubConnectionBuilder().withUrl("/signalr").build();


/*================================*/
getDimension();
if (windowWidth > 640) {
	moveLength = 80;
	moveDeviation = 10;
}
else {
	moveLength = 50;
	moveDeviation = 6;
}

/*================декларація класу=========*/

var square_p = function (square, index) {
	this.id = square;
	this.ocupied = false;
	this.pieceId = undefined;
	this.id.onclick = function () {
		makeMove(index);
	}
}

var checker = function (piece, color, square) {
	this.id = piece;
	this.color = color;
	this.king = false;
	this.ocupied_square = square;
	this.alive = true;
	this.attack = false;
	if (square % 8) {
		this.coordX = square % 8;
		this.coordY = Math.floor(square / 8) + 1;
	}
	else {
		this.coordX = 8;
		this.coordY = square / 8;
	}
	this.id.onclick = function () {
		showMoves(piece);
	}
}

checker.prototype.setCoord = function (X, Y) {
	var x = (this.coordX - 1) * moveLength + moveDeviation;
	var y = (this.coordY - 1) * moveLength + moveDeviation;
	this.id.style.top = y + 'px';
	this.id.style.left = x + 'px';
}

checker.prototype.changeCoord = function (X, Y) {
	this.coordY += Y;
	this.coordX += X;
}

checker.prototype.checkIfKing = function () {
	if (this.coordY == 8 && !this.king && this.color == "white") {
		this.king = true;
		this.id.style.border = "4px solid #FFFF00";
	}
	if (this.coordY == 1 && !this.king && this.color == "black") {
		this.king = true;
		this.id.style.border = "4px solid #FFFF00";
	}
}

/*===============Ініціалізація ігрових полів=================================*/


for (var i = 1; i <= 64; i++)
	block[i] = new square_p(square_class[i], i);

/*==================================================*/


/*================ініціалізація шашок=================================*/

// білі 
for (var i = 1; i <= 4; i++) {
	w_checker[i] = new checker(white_checker_class[i], "white", 2 * i - 1);
	w_checker[i].setCoord(0, 0);
	block[2 * i - 1].ocupied = true;
	block[2 * i - 1].pieceId = w_checker[i];
}

for (var i = 5; i <= 8; i++) {
	w_checker[i] = new checker(white_checker_class[i], "white", 2 * i);
	w_checker[i].setCoord(0, 0);
	block[2 * i].ocupied = true;
	block[2 * i].pieceId = w_checker[i];
}

for (var i = 9; i <= 12; i++) {
	w_checker[i] = new checker(white_checker_class[i], "white", 2 * i - 1);
	w_checker[i].setCoord(0, 0);
	block[2 * i - 1].ocupied = true;
	block[2 * i - 1].pieceId = w_checker[i];
}

//чорні
for (var i = 1; i <= 4; i++) {
	b_checker[i] = new checker(black_checker_class[i], "black", 56 + 2 * i);
	b_checker[i].setCoord(0, 0);
	block[56 + 2 * i].ocupied = true;
	block[56 + 2 * i].pieceId = b_checker[i];
}

for (var i = 5; i <= 8; i++) {
	b_checker[i] = new checker(black_checker_class[i], "black", 40 + 2 * i - 1);
	b_checker[i].setCoord(0, 0);
	block[40 + 2 * i - 1].ocupied = true;
	block[40 + 2 * i - 1].pieceId = b_checker[i];
}

for (var i = 9; i <= 12; i++) {
	b_checker[i] = new checker(black_checker_class[i], "black", 24 + 2 * i);
	b_checker[i].setCoord(0, 0);
	block[24 + 2 * i].ocupied = true;
	block[24 + 2 * i].pieceId = b_checker[i];
}

/*========================================================*/



/*================ВИБІР ЧАСТИНИ==============*/
the_checker = w_checker;

function showMoves(piece) {

	var match = false;
	mustAttack = false;
	if (selectedPiece) {
		erase_roads(selectedPiece);
	}
	selectedPiece = piece;
	var i, j; // зберегти індекс шашки
	for (j = 1; j <= 12; j++) {
		if (the_checker[j].id == piece) {
			i = j;
			selectedPieceindex = j;
			match = true;
		}
	}

	if (oneMove && !attackMoves(oneMove)) {
		changeTurns(oneMove);
		oneMove = undefined;
		return false;
	}
	if (oneMove && oneMove != the_checker[i]) {
		return false;
	}

	if (!match) {
		return 0; // якщо збіг не знайдено; це буває, коли, наприклад, червоний рухається, а ви натискаєте чорний
	}

	/*===тепер, залежно від їх кольору, я встановлюю краї та рухи шашки===*/
	if (the_checker[i].color == "white") {
		tableLimit = 8;
		tableLimitRight = 1;
		tableLimitLeft = 8;
		moveUpRight = 7;
		moveUpLeft = 9;
		moveDownRight = - 9;
		moveDownLeft = -7;
	}
	else {
		tableLimit = 1;
		tableLimitRight = 8;
		tableLimitLeft = 1;
		moveUpRight = -7;
		moveUpLeft = -9;
		moveDownRight = 9;
		moveDownLeft = 7;
	}
	/*===========Перевірка, чи можна атакувати====*/


	attackMoves(the_checker[i]); // перевірка, чи є якісь атакуючі рухи


	/*========ЯКЩО Я НЕ МОЖУ АТАКУВАТИ, Я ПЕРЕВІРЮЮ, ЧИ МОЖУ Я ЙТИ======*/

	if (!mustAttack) {
		downLeft = checkMove(the_checker[i], tableLimit, tableLimitRight, moveUpRight, downLeft);
		downRight = checkMove(the_checker[i], tableLimit, tableLimitLeft, moveUpLeft, downRight);
		if (the_checker[i].king) {
			upLeft = checkMove(the_checker[i], reverse_tableLimit, tableLimitRight, moveDownRight, upLeft);
			upRight = checkMove(the_checker[i], reverse_tableLimit, tableLimitLeft, moveDownLeft, upRight)
		}
	}
	if (downLeft || downRight || upLeft || upRight) {
		return true;
	}
	return false;

}


function erase_roads(piece) {
	if (downRight) block[downRight].id.style.background = "#BA7A3A";
	if (downLeft) block[downLeft].id.style.background = "#BA7A3A";
	if (upRight) block[upRight].id.style.background = "#BA7A3A";
	if (upLeft) block[upLeft].id.style.background = "#BA7A3A";
}

/*=============ПЕРЕМІЩЕННЯ ЧАСТИНИ======*/

function makeMove(index) {
	var isMove = false;
	if (!selectedPiece) // якщо гра почалася, але трек не вибрано
		return false;
	if (index != upLeft && index != upRight && index != downLeft && index != downRight) {
		erase_roads(0);
		selectedPiece = undefined;
		return false;
	}

	/* =========перспектива — це гравець, який рухається ======*/
	if (the_checker[1].color == "white") {
		cpy_downRight = upRight;
		cpy_downLeft = upLeft;
		cpy_upLeft = downLeft;
		cpy_upRight = downRight;
	}
	else {
		cpy_downRight = upLeft;
		cpy_downLeft = upRight;
		cpy_upLeft = downRight;
		cpy_upRight = downLeft;
	}

	if (mustAttack)  // щоб знати, пропускаю я лише один рядок чи 2
		multiplier = 2;
	else
		multiplier = 1;


	if (index == cpy_upRight) {
		isMove = true;
		if (the_checker[1].color == "white") {
			// перемістити шашку
			executeMove(multiplier * 1, multiplier * 1, multiplier * 9);
			//видалити доріжку, якщо було виконано стрибок
			if (mustAttack) eliminateCheck(index - 9);
		}
		else {
			executeMove(multiplier * 1, multiplier * -1, multiplier * -7);
			if (mustAttack) eliminateCheck(index + 7);
		}
	}

	if (index == cpy_upLeft) {

		isMove = true;
		if (the_checker[1].color == "white") {
			executeMove(multiplier * -1, multiplier * 1, multiplier * 7);
			if (mustAttack) eliminateCheck(index - 7);
		}
		else {
			executeMove(multiplier * -1, multiplier * -1, multiplier * -9);
			if (mustAttack) eliminateCheck(index + 9);
		}
	}

	if (the_checker[selectedPieceindex].king) {

		if (index == cpy_downRight) {
			isMove = true;
			if (the_checker[1].color == "white") {
				executeMove(multiplier * 1, multiplier * -1, multiplier * -7);
				if (mustAttack) eliminateCheck(index + 7);
			}
			else {
				executeMove(multiplier * 1, multiplier * 1, multiplier * 9);
				if (mustAttack) eliminateCheck(index - 9);
			}
		}

		if (index == cpy_downLeft) {
			isMove = true;
			if (the_checker[1].color == "white") {
				executeMove(multiplier * -1, multiplier * -1, multiplier * -9);
				if (mustAttack) eliminateCheck(index + 9);
			}
			else {
				executeMove(multiplier * -1, multiplier * 1, multiplier * 7);
				if (mustAttack) eliminateCheck(index - 7);
			}
		}
	}

	erase_roads(0);
	the_checker[selectedPieceindex].checkIfKing();

	if (isMove) {

		const toX = block[the_checker[selectedPieceindex].ocupied_square].pieceId.coordX;
		const toY = block[the_checker[selectedPieceindex].ocupied_square].pieceId.coordY;
		const move = `${selectedPieceindex}:${fromX}${fromY}-${toX}${toY}`;
		// alert(moveDescription);

		//playSound(moveSound);
		anotherMove = undefined;
		if (mustAttack) {
			anotherMove = attackMoves(the_checker[selectedPieceindex]);
		}
		if (anotherMove) {
			oneMove = the_checker[selectedPieceindex];
			showMoves(oneMove);
		}
		else {
			oneMove = undefined;
			changeTurns(the_checker[1]);	
		}

		if (!isHistory) {
			connection.invoke("SendMove", sessionId, connectionId, move)
				.then(() => {
					checkWinAfterMove();
				})
				.catch((error) => {
					console.error("Error sending the move:", error);
				});
		}

		selectedPieceindex = null;
		selectedPiece = null;
	}
}

/*===========ПЕРЕМІЩЕННЯ ЧАСТИНИ – ЗМІНА КООРДИНАТ======*/

function executeMove(X, Y, nSquare) {

	// змінити координати переміщеної частини
	fromX = block[the_checker[selectedPieceindex].ocupied_square].pieceId.coordX;
	fromY = block[the_checker[selectedPieceindex].ocupied_square].pieceId.coordY;

	the_checker[selectedPieceindex].changeCoord(X, Y);
	the_checker[selectedPieceindex].setCoord(0, 0);

	// Я звільняю поле, зайняте фігурою, і займаю те, яке вона займе
	block[the_checker[selectedPieceindex].ocupied_square].ocupied = false;
	block[the_checker[selectedPieceindex].ocupied_square + nSquare].ocupied = true;
	block[the_checker[selectedPieceindex].ocupied_square + nSquare].pieceId = block[the_checker[selectedPieceindex].ocupied_square].pieceId;
	block[the_checker[selectedPieceindex].ocupied_square].pieceId = undefined;
	the_checker[selectedPieceindex].ocupied_square += nSquare;

}

function checkMove(Apiece, tLimit, tLimit_Side, moveDirection, theDirection) {
	if (Apiece.coordY != tLimit) {
		if (Apiece.coordX != tLimit_Side && !block[Apiece.ocupied_square + moveDirection].ocupied) {
			block[Apiece.ocupied_square + moveDirection].id.style.background = "#704923";
			theDirection = Apiece.ocupied_square + moveDirection;
		}
		else
			theDirection = undefined;
	}
	else
		theDirection = undefined;
	return theDirection;
}



function checkAttack(check, X, Y, negX, negY, squareMove, direction) {
	if (check.coordX * negX >= X * negX && check.coordY * negY <= Y * negY && block[check.ocupied_square + squareMove].ocupied && block[check.ocupied_square + squareMove].pieceId.color != check.color && !block[check.ocupied_square + squareMove * 2].ocupied) {
		mustAttack = true;
		direction = check.ocupied_square + squareMove * 2;
		block[direction].id.style.background = "#704923";
		return direction;
	}
	else
		direction = undefined;
	return direction;
}

function eliminateCheck(indexx) {
	if (indexx < 1 || indexx > 64)
		return 0;

	var x = block[indexx].pieceId;
	x.alive = false;
	block[indexx].ocupied = false;
	x.id.style.display = "none";
}


function attackMoves(ckc) {

	upRight = undefined;
	upLeft = undefined;
	downRight = undefined;
	downLeft = undefined;

	if (ckc.king) {
		if (ckc.color == "white") {
			upRight = checkAttack(ckc, 6, 3, -1, -1, -7, upRight);
			upLeft = checkAttack(ckc, 3, 3, 1, -1, -9, upLeft);
		}
		else {
			downLeft = checkAttack(ckc, 3, 6, 1, 1, 7, downLeft);
			downRight = checkAttack(ckc, 6, 6, -1, 1, 9, downRight);
		}
	}
	if (ckc.color == "white") {
		downLeft = checkAttack(ckc, 3, 6, 1, 1, 7, downLeft);
		downRight = checkAttack(ckc, 6, 6, -1, 1, 9, downRight);
	}
	else {
		upRight = checkAttack(ckc, 6, 3, -1, -1, -7, upRight);
		upLeft = checkAttack(ckc, 3, 3, 1, -1, -9, upLeft);
	}

	if (ckc.color == "black" && (upRight || upLeft || downLeft || downRight)) {
		var p = upLeft;
		upLeft = downLeft;
		downLeft = p;

		p = upRight;
		upRight = downRight;
		downRight = p;

		p = downLeft;
		downLeft = downRight;
		downRight = p;

		p = upRight;
		upRight = upLeft;
		upLeft = p;
	}
	if (upLeft != undefined || upRight != undefined || downRight != undefined || downLeft != undefined) {
		return true;

	}
	return false;
}

function changeTurns(ckc) {
	if (ckc.color == "white") {
		history_checker = "black_checker"
		the_checker = b_checker;

		isWhitePlayerMove = false;
		setMoveIndicatorColour(false);
		setMoveIndicatorText();
	}
	else {
		history_checker = "white_checker";
		the_checker = w_checker;

		
		isWhitePlayerMove = true;
		setMoveIndicatorColour(true);
		setMoveIndicatorText();
	}


}
function getPlayerColour() {
	let whiteId = getCookie("whiteId");
	let blackId = getCookie("blackId");
	let userId = getCookie("userId");
	if (userId == whiteId) {
		isWhitePlayer = false;
	} else if (userId == blackId) {
		isWhitePlayer = true;
	} else {
		alert("Cheater")
	}
}
function setMoveIndicatorColour(isWhite) {
	if (isWhite == true) {
		$("#moveDisplay").removeClass("move_black_checker");
		$("#moveDisplay").addClass("move_white_checker");
	} else {
		$("#moveDisplay").removeClass("move_white_checker");
		$("#moveDisplay").addClass("move_black_checker");
	}
}

function setMoveIndicatorText() {
	if (isWhitePlayerMove == isWhitePlayer) {
		$("#playerMoveText").text("Your move");
		
	} else {
		$("#playerMoveText").text("Oponent move");
		
	}
}

function checkIfLost() {
	var i;
	for (i = 1; i <= 12; i++)
		if (the_checker[i].alive)
			return false;
	return true;
}

function checkWinAfterMove() {
	gameOver = checkIfLost();
	if (gameOver) { setTimeout(declareWinner(), 3000); return false };
	gameOver = checkForMoves();
	if (gameOver) { setTimeout(declareWinner(), 3000); return false };
}

function checkForMoves() {
	var i;
	for (i = 1; i <= 12; i++)
		if (the_checker[i].alive && showMoves(the_checker[i].id)) {
			erase_roads(0);
			return false;
		}
	return true;
}

function declareWinner() {
	//playSound(winSound);
	/*black_background.style.display = "inline";*/
	let isWhiteWin = false;

	score.style.display = "block";
	
	if (the_checker[1].color == "white") {
		score.innerHTML = "Black wins";
		isWhiteWin = false;
	}
	else {
		score.innerHTML = "Red wins";
		isWhiteWin = true;
	}

	$.ajax({
		url: `/api/setWinner`,
		method: 'POST',
		dataType: 'json',
		data: JSON.stringify({
			sessionId: sessionId,
			isWhiteWin: isWhiteWin
		}),
		contentType: 'application/json',
		
		success: function(data) {
			console.log(data);
		},
		error: function (xhr, status, error) {
			alert('Request failed with status: ' + status);
		}
	});

	$("#exitButton").removeClass("hidden");
}

function declareWinnerOnLeave(isWhiteLeave) {
	if (isWhiteLeave == true) {
		score.innerHTML = "Black wins";
	}
	else {
		score.innerHTML = "Red wins";
	}
}

function playSound(sound) {
	if (sound) sound.play();
}


function getDimension() {
	contor++;
	windowHeight = window.innerHeight
		|| document.documentElement.clientHeight
		|| document.body.clientHeight;;
	windowWidth = window.innerWidth
		|| document.documentElement.clientWidth
		|| document.body.clientWidth;
}


function decipherHistory(history) {
	if (history == null) 
		return;

	const moves = history.split(' ');

	for (const move of moves) {
		isHistory = true;
		// Split each move based on colons to separate player and move
		const [id, moveDetails] = move.split(':');

		// Split moveDetails based on the dash symbol to get begin and last positions
		const [begin, last] = moveDetails.split('-');

		// Extract individual coordinates
		const [beginX, beginY] = [begin[0], begin[1]];
		const [lastX, lastY] = [last[0], last[1]];


		console.log(`Player: ${id}, BeginX: ${beginX}, BeginY: ${beginY}, LastX: ${lastX}, LastY: ${lastY}`);
		let nSquare = ((lastY - 1) * 8) + parseInt(lastX);

		let checker = document.getElementsByClassName(history_checker)[id];
		showMoves(checker);
		makeMove(nSquare);
		checkWinAfterMove();
		isHistory = false;

	}
}

function selectSide() {
	let whiteId = getCookie("whiteId");
	let blackId = getCookie("blackId");
	let userId = getCookie("userId");

	if (userId == whiteId) {
		b_checker.forEach(x => {
			x.id.onclick = null
		})
		isWhitePlayer = true;
	} else if (userId == blackId) { 
		w_checker.forEach(x => {
			x.id.onclick = null
		})
		isWhitePlayer = false;
	} else {
		alert("Cheater")
		return;
	}

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

function setUpConnection(sessionId) {
	
	connection.invoke("JoinGameGroup", sessionId, connectionId)
		.then(() => {

			// Приєднання до групи успішно
			console.log("Joined the game group");
		})
		.catch((error) => {
			console.error("Error joining the game group:", error);
		});
}


function getIdFromUrl() {
	const nUrl = new URL(window.location.href);
	const path = nUrl.pathname; // Get the pathname component of the URL
	const parts = path.split('/'); // Split the pathname by '/'
	const value = parts.pop();

	return value;
}


document.getElementsByTagName("BODY")[0].onresize = function () {

	getDimension();
	var cpy_bigScreen = bigScreen;

	if (windowWidth < 650) {
		moveLength = 50;
		moveDeviation = 6;
		if (bigScreen == 1) bigScreen = -1;
	}
	if (windowWidth > 650) {
		moveLength = 80;
		moveDeviation = 10;
		if (bigScreen == -1) bigScreen = 1;
	}

	if (bigScreen != cpy_bigScreen) {
		for (var i = 1; i <= 12; i++) {
			b_checker[i].setCoord(0, 0);
			w_checker[i].setCoord(0, 0);
		}
	}
}

$(document).ready(function () {
	
	$.ajax({
		url: `/api/sessions?sessionId=${sessionId}`,
		method: 'GET',
		dataType: 'json',

		success: function (data) {
			if (data == ``) {
				data = null;
			}
			decipherHistory(data);
		},
		error: function (xhr, status, error) {
			alert('Request failed with status: ' + status);
		}
	});

	selectSide();
	setMoveIndicatorColour(true);
	setMoveIndicatorText();
});

window.addEventListener("beforeunload", function (e) {
	connection.invoke("LeaveGameGroup", getIdFromUrl());	

	/*$.ajax({
		url: `/api/setWinner`,
		method: 'POST',
		dataType: 'json',
		data: JSON.stringify({
			sessionId: sessionId,
			isWhiteWin: !isWhitePlayer
		}),
		contentType: 'application/json',
	});*/


	declareWinnerOnLeave(isWhitePlayer);
});


connection.start().then(() => {

	connection.on("ReceiveMessage", (user, move) => {
		console.log(`${user} made the move: ${move}`);
		if (user != connectionId) {
			decipherHistory(move);
		}
	});

	setUpConnection(sessionId)
});



