console.log('test')

$(".delete").click(function (){
    const ip = $(this).attr("ip")
    $.get("https://localhost:7058/Devices/Delete?ip=" + ip).then(res => console.log(res))
})
