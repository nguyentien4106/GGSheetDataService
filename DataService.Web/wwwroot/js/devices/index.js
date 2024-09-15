import { showError, showSuccess, isNullOrEmpty } from "../common.js";
$("#loading").hide();
$(".connect").click(function () {
    const id = $(this).attr("device-id");
    $("#loading").show();

    $.post("https://localhost:7058/Devices/Connect/" + id)
      .then((res) => {
        console.log(res);

        if (res.isSuccess) {
          showSuccess("Connected Successfully!");
          $(this).closest("tr").find('td:nth-child(4)').html("Connected")
        //   $(this).closest("tr").find('td:nth-child(7) a').html("Disconnect")
          $(this).removeClass("connect")
          $(this).addClass("disconnect")
          $(this).text("Disconnect")
        } else {
          showError(res.message);
        }
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
        console.log(res);

        if (res.isSuccess) {
          showSuccess("Disconnected Successfully!");
          $(this).closest("tr").find('td:nth-child(4)').html("Disconnected")
          $(this).removeClass("disconnect")
          $(this).addClass("connect")
          $(this).text("Connect")

        } else {
          showError(res.message);
        }
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

        if (res.isSuccess) {
          showSuccess("Delete Successfully!");
          //location.reload();
          window.location.href = res.message;
        } else {
          showError(res.message);
        }
      })
      .catch((e) => {
        showError(e.message);
      })
      .always(() => {
        $("#loading").hide();
      });
  }
});
