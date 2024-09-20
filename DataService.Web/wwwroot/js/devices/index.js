import { showError, showSuccess, isNullOrEmpty, show } from "../common.js";
$("#loading").hide();
$(".connect").click(function () {
    const id = $(this).attr("device-id");
    $("#loading").show();

    $.post("/Devices/Connect/" + id)
        .then(async (res) => {
            show(res);
            await new Promise((resolve) => setTimeout(resolve, 5000));
        })
        .catch((e) => {
            showError(e.message);
        })
        .always(() => {
            $("#loading").hide();
        });
});

$(".disconnect").click(function () {
    const id = $(this).attr("device-id");
    $("#loading").show();

    $.post("/Devices/Disconnect/" + id)
        .then((res) => {
            show(res);
        })
        .catch((e) => {
            showError(e.message);
        })
        .always(() => {
            $("#loading").hide();
        });
});
$(".delete").click(function () {
    const result = confirm("Do you confirm to delete this device");
    if (result) {
        const id = $(this).attr("device-id");
        $("#loading").show();

        $.post("/Devices/Delete/" + id)
            .then((res) => {
                console.log(res);

                show(res);
            })
            .catch((e) => {
                showError(e.message);
            })
            .always(() => {
                $("#loading").hide();
            });
    }
});
