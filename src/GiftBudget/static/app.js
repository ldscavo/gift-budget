var openModal = () => {    
    setTimeout(() => {
        document.getElementById("modal").classList.add("is-active");
    }, 10);
};

var closeModal = () => {
    document.getElementById("modal").classList.remove("is-active");
};

var addListeners = () => {
    document.querySelectorAll(".modal-link").forEach(modalLink => {
        modalLink.addEventListener('htmx:afterOnLoad', openModal);
    });

    document.querySelectorAll(".exit-modal").forEach(closeModalLink => {
        closeModalLink.addEventListener("click", closeModal);
    });

    document.getElementById("modal-background").addEventListener("click", closeModal);

    document.addEventListener("keyup", e => {
        if (e.key === "Escape") {
            closeModal();
        }
    });
};

document.addEventListener("htmx:afterOnLoad", addListeners);

addListeners();