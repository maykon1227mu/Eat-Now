import * as THREE from 'https://cdnjs.cloudflare.com/ajax/libs/three.js/r128/three.module.js';

// Seleciona a div onde o cubo será renderizado
const container = document.getElementById('container3d');

// Configurações iniciais da cena
const cena = new THREE.Scene();
const camera = new THREE.PerspectiveCamera(
    75,
    container.clientWidth / container.clientHeight,
    0.1,
    1000
);
const renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
renderer.setSize(container.clientWidth, container.clientHeight);
container.appendChild(renderer.domElement);
cena.background = null;

// Geometria do cubo
const geometria = new THREE.BoxGeometry();
const material = new THREE.MeshBasicMaterial({ color: 0x00ff00 });
const cubo = new THREE.Mesh(geometria, material);

cena.add(cubo);

camera.position.z = 5;

// Responsividade: ajusta o tamanho ao redimensionar a janela
window.addEventListener('resize', () => {
    camera.aspect = container.clientWidth / container.clientHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(container.clientWidth, container.clientHeight);
});

// Função de animação
function animate() {
    requestAnimationFrame(animate);

    // Rotaciona o cubo
    cubo.rotation.x += 0.01;
    cubo.rotation.y += 0.01;

    renderer.render(cena, camera);
}

animate();