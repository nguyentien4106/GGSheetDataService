$('#loading').hide() 

const notification = $("#notification")
const options24Hour = {
    year: 'numeric',
    month: 'numeric',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false // 24-hour format
};

function createNotification(item) {
    const date = new Date(item.createAt);

    const noti = `<div class="notification ${item.success ? "success-item" : "fail-item"}">
                    <p>${item.message}</p>
                    <p class="notification__time">${date.toLocaleString('en-US', options24Hour) }</p>
                </div>`
    return noti
}

notification.click(function () {
    const panel = $('#notificationPanel');
    $.get("https://localhost:7058/api/notifications").then(res => {
        console.log(res.data)
        const notifications = res.data.map(createNotification).join("")
        panel.html(notifications)
        panel.toggleClass('active');

    })
})