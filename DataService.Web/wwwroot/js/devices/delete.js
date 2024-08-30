console.log('test')

$(".delete").click(function (){
    console.log('delete')
    console.log()
    const ip = $(this).attr("ip")
    $.get("https://localhost:7058/Devices/Delete?ip=" + ip).then(res => console.log(res))
})
