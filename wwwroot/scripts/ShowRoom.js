var el1;
var el2;
var el3;
var settingsV;
var elframeV;
var outputV;
var historyV;
var gap = 6;
var set1w;
var set1h;
var set2w;
var set2h;
var set3w;
var set3h;
var image;
function PageLoaded() {
	return new Promise(resolve => {
		if (document.querySelector("#settings")) {
			return resolve(document.querySelector("#settings"));
		}
		const observer = new MutationObserver(mutations => {
			if (document.querySelector("#settings")) {
				observer.disconnect();
				resolve(document.querySelector("#settings"));
			}
		});
		observer.observe(document.body, {
			childList: true,
			subtree: true
		});
	});
}
async function showPage() {
	await PageLoaded();

	settingsV = document.querySelector("#settings");
	elframeV = document.querySelector("#elframe");
	outputV = document.querySelector("#output");
	historyV = document.querySelector("#history");
	el1 = document.querySelector("#el1");
	el2 = document.querySelector("#el2");
	el3 = document.querySelector("#el3");
	image = document.querySelector("#image");

	set1w = gap + settingsV.offsetWidth + gap + elframeV.offsetWidth + gap + outputV.offsetWidth + gap;
	set1h = gap + elframeV.offsetHeight + gap;
	set2w = gap + settingsV.offsetWidth + gap + elframeV.offsetWidth + gap;
	set2h = gap + settingsV.offsetHeight + gap + historyV.offsetHeight + gap + outputV.offsetHeight + gap;
	set3w = gap + settingsV.offsetWidth + gap;
	set3h = gap + settingsV.offsetHeight + gap + historyV.offsetHeight + gap + elframeV.offsetHeight + gap + outputV.offsetHeight + gap;

	document.addEventListener("mousemove", (event) => { el3.innerHTML = `Mouse X: ${event.clientX}, Mouse Y: ${event.clientY}`; });
	//window.addEventListener("resize", (event) => { resizePage(); });
	window.removeEventListener("resize", resizeLayout);
	window.addEventListener("resize", resizePage);

	//resizeLayout();
	resizePage();
	
	//pageV.style.animation = "appear 0.5s";
	//pageV.style.opacity = "1";

	console.log("js showPage");
}
function resizePage() {
	resizeLayout();
	let wiw = pageV.clientWidth;
	if (wiw >= set1w) set1();
	else if (wiw >= set2w) set2();
	else set3();
	console.log("js resizePage");
	resizeLayout();
	function set1() {
		let off = (wiw - set1w) / 2;
		pageV.style.height = `${set1h}px`;
		settingsV.style.left = `${gap + off}px`;
		settingsV.style.top = `${gap}px`;
		historyV.style.left = `${gap + off}px`;
		historyV.style.top = `${gap + settingsV.offsetHeight + gap}px`;
		elframeV.style.left = `${gap + settingsV.offsetWidth + gap + off}px`;
		elframeV.style.top = `${gap}px`;
		outputV.style.left = `${gap + settingsV.offsetWidth + gap + elframeV.offsetWidth + gap + off}px`;
		outputV.style.top = `${gap}px`;
	}
	function set2() {
		let off = (wiw - set2w) / 2;
		pageV.style.height = `${set2h}px`;
		settingsV.style.left = `${gap + off}px`;
		settingsV.style.top = `${gap}px`;
		historyV.style.left = `${gap + off}px`;
		historyV.style.top = `${gap + settingsV.offsetHeight + gap}px`;
		elframeV.style.left = `${gap + settingsV.offsetWidth + gap + off}px`;
		elframeV.style.top = `${gap}px`;
		outputV.style.left = `${wiw / 2 - outputV.offsetWidth / 2}px`;
		outputV.style.top = `${gap + elframeV.offsetHeight + gap}px`;
	}
	function set3() {
		let off = (wiw - set3w) / 2;
		pageV.style.height = `${set3h}px`;
		settingsV.style.left = `${gap + off}px`;
		settingsV.style.top = `${gap}px`;
		historyV.style.left = `${gap + off}px`;
		historyV.style.top = `${gap + settingsV.offsetHeight + gap}px`;
		elframeV.style.left = `${gap + off}px`;
		elframeV.style.top = `${gap + settingsV.offsetHeight + gap + historyV.offsetHeight + gap}px`;
		outputV.style.left = `${gap + off}px`;
		outputV.style.top = `${gap + settingsV.offsetHeight + gap + historyV.offsetHeight + gap + elframeV.offsetHeight + gap}px`;
	}
}
function randomise(min, max, centered) {
	min = parseFloat(min);
	max = parseFloat(max);
	centered = typeof centered == "undefined" ? 10 : Math.round(centered);
	let ret = 0;
	for (let i = 0; i < centered; i++)
		ret += Math.random() * (max - min) + min;
	ret /= centered;
	return ret;
}
function rndall() {
	rnd(1); rnd(2); rnd(3);
}
function log(message) {
	console.log(message);
}
function disablebutton() {
	let button = document.querySelector("#calc");
	button.disabled = true;
}
function enablebutton() {
	let button = document.querySelector("#calc");
	button.disabled = false;
}
function calc() {
	let b1 = 1; if (document.querySelector("#none1").checked) b1 = 0; else if (document.querySelector("#minus1").checked) b1 = -1;
	let b2 = 1; if (document.querySelector("#none2").checked) b2 = 0; else if (document.querySelector("#minus2").checked) b2 = -1;
	let b3 = 1; if (document.querySelector("#none3").checked) b3 = 0; else if (document.querySelector("#minus3").checked) b3 = -1;
	if (b1 != 1 && b2 != 1 && b3 != 1) {
		alert("Please select at least one PLUS in the output!"); console.log("AKJFKAJFLANFLN"); return null;
	}
	let gif1 = document.querySelector("#gif1");
	gif1.style.animation = 'none';
	gif1.offsetHeight;
	gif1.style.animation = null;
	gif1.style.animation = "sature 5.04s";
	//let txt = document.querySelector("#calctime");
	//txt.style.animation = 'none';
	//txt.offsetHeight;
	//txt.style.animation = null;
	//txt.style.animation = "fade 1s";
	//txt.innerText = "";
	let p1 = getTransformedPoints(el1);
	let p2 = getTransformedPoints(el2);
	let p3 = getTransformedPoints(el3);
	//return JSON.stringify([[b1, p1], [b2, p2], [b3, p3]]);
	return [[b1, p1], [b2, p2], [b3, p3]];
}
function draw(img, sw, sh) {
	image.style.width = `${sw}px`;
	image.style.height = `${sh}px`;
	image.style.opacity = "0";
	image.src = img;
	let sx, sy;
	if (sw > 600 || sh > 600)
		if (sw > sh) {
			let scale = 600 / sw;
			sw *= scale;
			sh *= scale;
			sx = 0; sy = (600 - sh) / 2;
		}
		else {
			let scale = 600 / sh;
			sw *= scale;
			sh *= scale;
			sy = 0; sx = (600 - sw) / 2;
		}
	else {
		sx = (600 - sw) / 2;
		sy = (600 - sh) / 2;
	}
	image.style.width = `${sw}px`;
	image.style.height = `${sh}px`;
	image.style.left = `${sx}px`;
	image.style.top = `${sy}px`;
	image.style.opacity = "1";
}
function addhistory(img, sw, sh) {
	let spread = document.querySelector("#spread");
	let child = document.createElement("img");
	child.src = img;
	child.style.width = "121px";
	child.style.height = "121px";
	child.style.margin = "2px"
	spread.appendChild(child);
}
function resetrad() {
	let plus1 = document.querySelector("#plus1");
	let plus2 = document.querySelector("#plus2");
	let plus3 = document.querySelector("#plus3");
	let none1 = document.querySelector("#none1");
	let none2 = document.querySelector("#none2");
	let none3 = document.querySelector("#none3");
	let minus1 = document.querySelector("#minus1");
	let minus2 = document.querySelector("#minus2");
	let minus3 = document.querySelector("#minus3");
	let rad1 = [plus1, none1, minus1];
	let rad2 = [plus2, none2, minus2];
	let rad3 = [plus3, none3, minus3];
	rad1[0].checked = true; rad1[1].checked = false; rad1[2].checked = false;
	rad2[0].checked = false; rad2[1].checked = false; rad2[2].checked = true;
	rad3[0].checked = false; rad3[1].checked = false; rad3[2].checked = true;
}
function rndrad() {
	let plus1 = document.querySelector("#plus1");
	let plus2 = document.querySelector("#plus2");
	let plus3 = document.querySelector("#plus3");
	let none1 = document.querySelector("#none1");
	let none2 = document.querySelector("#none2");
	let none3 = document.querySelector("#none3");
	let minus1 = document.querySelector("#minus1");
	let minus2 = document.querySelector("#minus2");
	let minus3 = document.querySelector("#minus3");
	let rad1 = [plus1, none1, minus1];
	let rad2 = [plus2, none2, minus2];
	let rad3 = [plus3, none3, minus3];
	let all = [rad1, rad2, rad3];
	for (let i = 0; i < all.length; i++) {
		let index = Math.floor(Math.random() * 3);
		all[i][index].checked = true;
		for (let j = 0; j < all[i].length; j++)
			if (j != index)
				all[i][j].checked = false;
	}
	if (!rad1[0].checked && !rad2[0].checked && !rad3[0].checked) {
		let index = Math.floor(Math.random() * 3);
		all[index][0].checked = true;
		for (let j = 1; j < all[index].length; j++)
			all[index][j].checked = false;
	}
}
function tr(n) {
	let trX = document.querySelector(`#tr${n}x`);
	let trY = document.querySelector(`#tr${n}y`);
	let trZ = document.querySelector(`#tr${n}z`);
	let rotX = document.querySelector(`#rot${n}x`);
	let rotY = document.querySelector(`#rot${n}y`);
	let rotZ = document.querySelector(`#rot${n}z`);
	let skewX = document.querySelector(`#skew${n}x`);
	let skewY = document.querySelector(`#skew${n}y`);
	let scaleX = document.querySelector(`#scale${n}x`);
	let scaleY = document.querySelector(`#scale${n}y`);
	let scaleZ = document.querySelector(`#scale${n}z`);
	let el = document.querySelector(`#el${n}`);
	let str = `translate3d(${trX.value}px, ${trY.value}px, ${trZ.value}px) scale3d(${scaleX.value}, ${scaleY.value}, ${scaleZ.value}) rotateX(${rotX.value}deg) rotateY(${rotY.value}deg) rotateZ(${rotZ.value}deg) skew(${skewX.value}deg, ${skewY.value}deg)`;
	el.style.transform = str;
	if (n != 1) {
		let style3d = document.querySelector(`#el${n}3d`).checked;
		el.parentElement.style.transformStyle = style3d ? "preserve-3d" : "flat";
	}
	resizePage();
}
function res(n) {
	let trX = document.querySelector(`#tr${n}x`); trX.value = 0;
	let trY = document.querySelector(`#tr${n}y`); trY.value = 0;
	let trZ = document.querySelector(`#tr${n}z`); trZ.value = 0;
	let rotX = document.querySelector(`#rot${n}x`); rotX.value = 0;
	let rotY = document.querySelector(`#rot${n}y`); rotY.value = 0;
	let rotZ = document.querySelector(`#rot${n}z`); rotZ.value = 0;
	let skewX = document.querySelector(`#skew${n}x`); skewX.value = 0;
	let skewY = document.querySelector(`#skew${n}y`); skewY.value = 0;
	let scaleX = document.querySelector(`#scale${n}x`); scaleX.value = 1;
	let scaleY = document.querySelector(`#scale${n}y`); scaleY.value = 1;
	let scaleZ = document.querySelector(`#scale${n}z`); scaleZ.value = 1;
	if (n != 1) {
		let style3d = document.querySelector(`#el${n}3d`); style3d.checked = true;
	}
	tr(n);
}
function rnd(n) {
	let trX = document.querySelector(`#tr${n}x`); trX.value = randomise(trX.min, trX.max);
	let trY = document.querySelector(`#tr${n}y`); trY.value = randomise(trY.min, trY.max);
	let trZ = document.querySelector(`#tr${n}z`); trZ.value = randomise(trZ.min, trZ.max);
	let rotX = document.querySelector(`#rot${n}x`); rotX.value = randomise(rotX.min, rotX.max);
	let rotY = document.querySelector(`#rot${n}y`); rotY.value = randomise(rotY.min, rotY.max);
	let rotZ = document.querySelector(`#rot${n}z`); rotZ.value = randomise(rotZ.min, rotZ.max);
	let skewX = document.querySelector(`#skew${n}x`); skewX.value = randomise(skewX.min, skewX.max);
	let skewY = document.querySelector(`#skew${n}y`); skewY.value = randomise(skewY.min, skewY.max);
	let scaleX = document.querySelector(`#scale${n}x`); scaleX.value = randomise(0.2, 1.8);
	let scaleY = document.querySelector(`#scale${n}y`); scaleY.value = randomise(0.2, 1.8);
	let scaleZ = document.querySelector(`#scale${n}z`); scaleZ.value = randomise(0.2, 1.8);
	if (n != 1) {
		let style3d = document.querySelector(`#el${n}3d`); if (Math.random() > 0.5) style3d.checked = true; else style3d.checked = false;
	}
	tr(n);
}
function getTransformedPoints(el) {
	const all = getElementChain(el);
	let els = new Array(0);
	let m = new Array(0);
	let p = new Array(0);
	let o = new Array(0);
	let s = new Array(0);
	// get all start variables
	for (let i = 0; i < all.length; i++) {
		const values = getTransformValues(all[i], (i == all.length - 1));
		if (values != null) {
			els.push(all[i]);
			m.push(values[0]);
			p.push(values[1]);
			o.push(values[2]);
			s.push(values[3]);
		}
	}
	const len = els.length;
	const id = len - 1;
	// apply transformation
	for (let i = len - 1; i >= 0; i--) {// matrix, style and origin I
		for (let k = 0; k < 4; k++)
			p[id][k] = getTransformedPoint(m[i], p[id][k], o[i], s[i]);
	}
	// set in rectangle
	let left = p[id][0].x; let top = p[id][0].y; let right = p[id][0].x; let bottom = p[id][0].y;
	for (let j = 0; j < 4; j++) {
		left = Math.min(left, p[id][j].x);
		top = Math.min(top, p[id][j].y);
		right = Math.max(right, p[id][j].x);
		bottom = Math.max(bottom, p[id][j].y);
	}
	const elrect = els[id].getBoundingClientRect();
	const scalex = elrect.width / (right - left); const scaley = elrect.height / (bottom - top);
	for (let j = 0; j < 4; j++) {
		p[id][j].x *= scalex;
		p[id][j].y *= scaley;
	}
	left = p[id][0].x; top = p[id][0].y;
	for (let j = 0; j < 4; j++) {
		left = Math.min(left, p[id][j].x);
		top = Math.min(top, p[id][j].y);
	}
	const diffx = elrect.left - left;
	for (let j = 0; j < 4; j++) p[id][j].x += diffx;
	const diffy = elrect.top - top;
	for (let j = 0; j < 4; j++) p[id][j].y += diffy;
	return p.at(-1);
}
function getElementChain(el) {
	let els = [];
	els.push(el);
	while (true) {
		let nextPar = (els.at(-1)).parentElement;
		if (nextPar)
			els.push(nextPar);
		else break;
	}
	return els.reverse();
}
function matrixIdentity(m) {
	let id = [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1];
	for (let i = 0; i < 16; i++) {
		if (m[i] != id[i])
			return false;
	}
	return true;
}
function getTransformValues(el) { return getTransformValues(el, false); }
function getTransformValues(el, anyway) {
	const css = window.getComputedStyle(el);
	const hasmat = css.transform.includes("matrix");
	const hasstyle = css.transformStyle == "preserve-3d";
	if (hasmat || hasstyle || anyway) {
		const m = [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1];
		if (hasmat) {
			const matstr = css.transform.split('(')[1].split(')')[0].split(',');
			if (matstr.length == 16) {
				for (let i = 0; i < matstr.length; i++)
					m[i] = parseFloat(matstr[i]);
			}
			else if (matstr.length == 6) {
				m[0] = parseFloat(matstr[0]);
				m[1] = parseFloat(matstr[1]);
				m[4] = parseFloat(matstr[2]);
				m[5] = parseFloat(matstr[3]);
				m[12] = parseFloat(matstr[4]);
				m[13] = parseFloat(matstr[5]);
			}
		}
		if (!matrixIdentity(m) || hasstyle || anyway) {
			const width = el.offsetWidth; const height = el.offsetHeight;
			const offset = getOffset(el);
			const left = offset[0]; const top = offset[1];
			const p = [{ x: left, y: top, z: 0 }, { x: left, y: height + top, z: 0 }, { x: width + left, y: height + top, z: 0 }, { x: width + left, y: top, z: 0 }];
			const origstr = css.transformOrigin.split(' ');
			const ofx = origstr.length > 0 ? parseFloat(origstr[0].replace(/[^\d.-]/g, '')) : 0;
			const ofy = origstr.length > 1 ? parseFloat(origstr[1].replace(/[^\d.-]/g, '')) : 0;
			const ofz = origstr.length > 2 ? parseFloat(origstr[2].replace(/[^\d.-]/g, '')) : 0;
			const o = { x: left + ofx, y: top + ofy, z: ofz };
			const s = css.transformStyle;
			return [m, p, o, s];
		}
	}
	return null;
}
function getTransformedPoint(m, p, o, s) {
	const x = p.x - o.x;
	const y = p.y - o.y;
	const z = s == "preserve-3d" ? p.z - o.z : - o.z;
	const k = m[3] * x + m[7] * y + m[11] * z + m[15];
	const u = (m[0] * x + m[4] * y + m[8] * z + m[12]) / k;
	const v = (m[1] * x + m[5] * y + m[9] * z + m[13]) / k;
	const w = (m[2] * x + m[6] * y + m[10] * z + m[14]) / k;
	return { x: u + o.x, y: v + o.y, z: w + o.z };
}
function getOffset(el) {
	let x = 0;
	let y = 0;
	let first = true;
	while (el) {
		const ox = isNaN(el.offsetLeft) ? 0 : el.offsetLeft;
		const oy = isNaN(el.offsetTop) ? 0 : el.offsetTop;
		const sx = isNaN(el.scrollLeft) ? 0 : el.scrollLeft;
		const sy = isNaN(el.scrollTop) ? 0 : el.scrollTop;
		x += ox - sx;
		y += oy - sy;
		if (!first) {
			const cx = isNaN(el.clientLeft) ? 0 : el.clientLeft;
			const cy = isNaN(el.clientTop) ? 0 : el.clientTop;
			x += cx;
			y += cy;
		}
		el = el.offsetParent;
		first = false;
	}
	return [x, y];
}