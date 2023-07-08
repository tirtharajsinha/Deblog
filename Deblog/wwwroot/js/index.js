
let menustate = false;
document.querySelector(".dpbutton").addEventListener("click", (e) => {
    document.querySelector(".navmenu").classList.toggle("showmenu");
    menustate = !menustate;
})

document.querySelector(".content").addEventListener("click", (e) => {
    if (menustate) {
        document.querySelector(".navmenu").classList.remove("showmenu");
        menustate = false;
    }
    
})

