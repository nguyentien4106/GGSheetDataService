// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
export const showError = (text) => {
    Toastify({
        text: text,
        duration: 5000,
        newWindow: true,
        close: true,
        gravity: "top", // `top` or `bottom`
        position: "center", // `left`, `center` or `right`
        stopOnFocus: true, // Prevents dismissing of toast on hover
        style: {
            background: "#ffd4d4",
            "border-color": "red",
            color: "black"
        },
    }).showToast();
}

export const showSuccess = (text) => {
    Toastify({
        text: text ? text : "Successfully !",
        duration: 5000,
        newWindow: true,
        close: true,
        gravity: "top", // `top` or `bottom`
        position: "center", // `left`, `center` or `right`
        stopOnFocus: true, // Prevents dismissing of toast on hover
        style: {
            background: "#green",
            "border-color": "gren",
            color: "black"
        },
    }).showToast();
}

export const isNullOrEmpty = (text) => text === null || text === "" || text.trim() === "";