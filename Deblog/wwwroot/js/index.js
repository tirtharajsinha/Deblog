document.addEventListener("scroll", (event) => {
    lastKnownScrollPosition = window.scrollY;
    let accentcolor = getComputedStyle(document.documentElement)
        .getPropertyValue('--accent-color');
    if (lastKnownScrollPosition > 400) {
        document.querySelector("nav").style.background = "#ffffff";
        document.querySelector(".nav-active").style.background = accentcolor;
    } else {
        document.querySelector("nav").style.background = accentcolor;
        document.querySelector(".nav-active").style.background = "#ffffff";
    }
});



let trend = document.querySelector(".trends").innerHTML

document.querySelector(".trends").innerHTML = ``;
for (let i = 1; i <= 6; i++) {
    document.querySelector(".trends").innerHTML += trend.replace("{1}", `${i}`)
}

let blog = document.querySelector(".blogs").innerHTML

document.querySelector(".blogs").innerHTML = ``;
for (let i = 1; i <= 20; i++) {
    document.querySelector(".blogs").innerHTML += blog
}

let topics = ["Programming",
    "Data Science",
    "Technology",
    "Self Improvement",
    "Writing",
    "Relationships",
    "Machine Learning",
    "Productivity",
    "Politics"]

topics.forEach(element => {
    document.querySelector(".topics").innerHTML += `<a href="#">${element}</a>`
});



