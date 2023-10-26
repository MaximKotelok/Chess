$(document).ready(function () {
    let avatar = $("#avatar");
    let fileInput = $("#file");

    fileInput.on("change", function () {
        if (this.files && this.files[0]) {
            let reader = new FileReader();

            reader.onload = function (e) {
                avatar.attr("src", e.target.result);
            }

            reader.readAsDataURL(this.files[0]);
        }
    });
});