import { showError, showSuccess, isNullOrEmpty, show } from "../common.js";
$("#loading").hide();
$(".connect").click(function () {
    const id = $(this).attr("device-id");
    $("#loading").show();

    $.post("https://localhost:7058/Devices/Connect/" + id)
        .then((res) => {
            show(res)
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

    $.post("https://localhost:7058/Devices/Disconnect/" + id)
      .then((res) => {
        show(res)
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

    $.post("https://localhost:7058/Devices/Delete/" + id)
      .then((res) => {
        console.log(res);

        show(res)
      })
      .catch((e) => {
        showError(e.message);
      })
      .always(() => {
        $("#loading").hide();
      });
  }
});
