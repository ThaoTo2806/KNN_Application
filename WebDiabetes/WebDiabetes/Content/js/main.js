
document.querySelector('#logo_home').onclick = function () {
    sessionStorage.setItem("index", 0);
}
var nav = document.querySelectorAll('#header .nav-item.nav-link, #header .nav-item.dropdown>a')
var vtShow = sessionStorage.getItem("index")
console.log(vtShow)

if (vtShow != null) {
    var itemDelete = document.querySelector('.nav-item.nav-link.active')
    if (itemDelete != null) {
        itemDelete.classList.toggle('active')
    }
    nav[vtShow].classList.toggle('active')
}
else {
    nav[0].classList.toggle('active')
}


for (var i = 0; i < nav.length; i++) {
    nav[i].onclick = function () {
        var index = 0;
        for (; index < nav.length; index++) {
            if (nav[index] == this) {
                break;
            }
        }
        sessionStorage.setItem("index", index);
    }
}

var menuDiabetes = document.querySelector('.nav-item.dropdown')
menuDiabetes.classList.remove('show')
menuDiabetes.onclick = function () {
    this.querySelector('.dropdown-menu').classList.add('show')
}