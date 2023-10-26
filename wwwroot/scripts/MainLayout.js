var menuV;
var mainV;
var showV;
var aboutV;
var chosenV;
var pageV;
function LayoutLoaded() {
	return new Promise(resolve => {
		if (document.querySelector("#page")) {
			return resolve(document.querySelector("#page"));
		}
		const observer = new MutationObserver(mutations => {
			if (document.querySelector("#page")) {
				observer.disconnect();
				resolve(document.querySelector("#page"));
			}
		});
		observer.observe(document.body, {
			childList: true,
			subtree: true
		});
	});
}
async function showLayout() {
	await LayoutLoaded();

	menuV = document.querySelector("#menu");
	mainV = document.querySelector("#main");
	showV = document.querySelector("#show");
	aboutV = document.querySelector("#about");
	pageV = document.querySelector("#page");

	let site = location.href;
	if (site.toLowerCase().includes("/showroom")) {
		showV.style.background = "#00c1ff"; chosenV = showV;
	}
	else if (site.toLowerCase().includes("/about")) {
		aboutV.style.background = "#00c1ff"; chosenV = aboutV;
	}
	else { mainV.style.background = "#00c1ff"; chosenV = mainV; }

	//window.addEventListener("resize", (event) => { resizeLayout(); });
	window.addEventListener("resize", resizeLayout);

	resizeLayout();

	document.querySelector("body").style.opacity = "1";

	pageV.style.animation = "appear 0.5s";
	pageV.style.opacity = "1";

	console.log("js showLayout");
}
function light(id) {
	let el = document.querySelector(id);
	if (el !== chosenV) {
		if (el == null || el === null || el == "undefined" || el === "undefined")
			return;
		el.style.background = "white";
	}
}
function dim(id) {
	let el = document.querySelector(id);
	if (el == null || el === null || el == "undefined" || el === "undefined")
		return;
	if (el !== chosenV)
		el.style.background = "#ff00c8bf";
}
function resizeLayout() {
	let wiw = document.documentElement.clientWidth;
	menuV.style.width = `${wiw - parseInt((menuV.offsetLeft + menuV.clientLeft) * 2)}px`;
	let width = mainV.offsetWidth;
	let between = 20;
	let offx = (wiw - mainV.clientLeft - 2 * between - 3 * width) / 2.0;
	mainV.style.left = `${parseInt(offx)}px`;
	showV.style.left = `${parseInt(offx + width + between)}px`;
	aboutV.style.left = `${parseInt(offx + 2.0 * width + 2.0 * between)}px`;
	pageV.style.width = `${menuV.offsetWidth - pageV.clientLeft * 2}px`;
	console.log("js resizeLayout");
}
function loadJs(sourceUrl) {
	if (sourceUrl.Length == 0) {
		console.error("Invalid source URL");
		return;
	}
	let js = document.querySelector("#PageScript");
	if (js.src !== sourceUrl)
		js.src = sourceUrl;
}
function errorpage(message) {
	alert(message);
	location.href = "/Error/";
}