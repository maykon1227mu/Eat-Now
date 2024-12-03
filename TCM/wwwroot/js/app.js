import * as THREE from 'https://cdn.skypack.dev/three@0.129.0/build/three.module.js';
import { GLTFLoader } from 'https://cdn.skypack.dev/three@0.129.0/examples/jsm/loaders/GLTFLoader.js';
import { gsap } from 'https://cdn.skypack.dev/gsap';
import { OrbitControls } from 'https://cdn.skypack.dev/three@0.129.0/examples/jsm/controls/OrbitControls';

const camera = new THREE.PerspectiveCamera(
    10,
    window.innerWidth / window.innerHeight,
    0.1,
    1000
);
camera.position.z = 40;



const scene = new THREE.Scene();
let soda;
let mixer;
const loader = new GLTFLoader();
loader.load('./src/js/objects/soda.glb',
    function (gltf) {
        soda = gltf.scene;
        scene.add(soda);
        
        soda.rotation.set(-0.1, Math.PI / 1, 0.2)
        soda.position.set( Math.PI / 1, -1.5, 0)
        soda.scale.set(1, 1, 1); 

        mixer = new THREE.AnimationMixer(soda);
        mixer.clipAction(gltf.animations[0]).play();
        modelMove();
    },
    function (xhr) {},
    function (error) {}
);
const renderer = new THREE.WebGLRenderer({alpha: true});
renderer.setSize(window.innerWidth, window.innerHeight);
document.getElementById('container3D').appendChild(renderer.domElement);

// light
const ambientLight = new THREE.AmbientLight(0xffffff, 1);
scene.add(ambientLight);

const topLight = new THREE.DirectionalLight(0xffffff, 50);
topLight.position.set(200, 1000, 1000);
scene.add(topLight);
const leftLight = new THREE.DirectionalLight(0xffffff, 100);
topLight.position.set(-500, -200, 10);
scene.add(leftLight);
const bottomLight = new THREE.DirectionalLight(0xffffff, 20);
topLight.position.set(1000, -1000, 1000);
scene.add(bottomLight);




// Variáveis para armazenar a posição normalizada do mouse
let mouseX = 0;
let mouseY = 0;

// Captura o movimento do mouse e normaliza os valores
window.addEventListener('mousemove', (event) => {
    mouseX = (event.clientX / window.innerWidth) * -30 - 30; // Normalizado entre -1 e 1
    mouseY = -(event.clientY / window.innerHeight) * 2 + 1; // Invertido para alinhar com a rotação
});

// Atualize a rotação do modelo no render loop
const reRender3D = () => {
    requestAnimationFrame(reRender3D);
    renderer.render(scene, camera);

    if (soda) {
        // Ajusta a rotação do modelo com base nos valores normalizados do mouse
        soda.rotation.y = mouseX * Math.PI * 0.01; // Multiplicador para controlar a sensibilidade
        soda.rotation.x = mouseY * Math.PI * 0.01;
    }

    if (mixer) mixer.update(0.02);
};
reRender3D();


let arrPositionModel = [
    
    {
        id: 'banner',
        position: {x: -5, y: -1, z: 0},
        rotation: {x: 0, y: 1.5, z: 0},
        
    },
    {
        id: 'intro',
        position: {x:Math.PI / 1 , y: -1.5 , z: 0},
        rotation: {x: -0.1, y: Math.PI / 1, z: 0.2},
    },
    {
        id: "description",
        position: { x: -1, y: -1, z: -5 },
        rotation: { x: 0, y: 0.5, z: 0 },
    },
    {
        id: "box",
        position: { x: 0, y: -1, z: 0 },
        rotation: { x: 0.3, y: -0.5, z: 0 },
    },
    { id: "box-2", 
        position: { x: 0, y: -1, z: 0 },
        rotation: { x: 0.1, y: -20, z: 0 },
    }
];
let currentModelPath = './src/js/objects/soda.glb'; // Caminho inicial do modelo



const modelMove = () => {
    const sections = document.querySelectorAll('.section');
    let currentSection;
    sections.forEach((section) => {
        const rect = section.getBoundingClientRect();
        if (rect.top <= window.innerHeight / 3) {
            currentSection = section.id;
        }
    });

    let position_active = arrPositionModel.findIndex(
        (val) => val.id === currentSection
    );

    if (position_active >= 0) {
        let new_coordinates = arrPositionModel[position_active];

        // Verificar se o modelo precisa ser trocado
        if (currentModelPath !== new_coordinates.modelPath) {
            currentModelPath = new_coordinates.modelPath;

            // Carregar novo modelo
            loader.load(
                currentModelPath,
                function (gltf) {
                    scene.remove(soda); // Remove o modelo atual
                    soda = gltf.scene; // Substitui pelo novo modelo
                    soda.position.set(
                        new_coordinates.position.x,
                        new_coordinates.position.y,
                        new_coordinates.position.z
                    );
                    soda.rotation.set(
                        new_coordinates.rotation.x,
                        new_coordinates.rotation.y,
                        new_coordinates.rotation.z
                    );
                    scene.add(soda);

                    mixer = new THREE.AnimationMixer(soda);
                    if (gltf.animations.length > 0) {
                        mixer.clipAction(gltf.animations[0]).play();
                    }
                },
                undefined,
                function (error) {
                    console.error('Erro ao carregar o modelo:', error);
                }
            );
        } else {
            // Atualizar posição e rotação do modelo atual
            gsap.to(soda.position, {
                x: new_coordinates.position.x,
                y: new_coordinates.position.y,
                z: new_coordinates.position.z,
                duration: 3,
                ease: 'power1.out',
            });
            gsap.to(soda.rotation, {
                x: new_coordinates.rotation.x,
                y: new_coordinates.rotation.y,
                z: new_coordinates.rotation.z,
                duration: 10,
                ease: 'power1.out',
            });
        }
    }
};





window.addEventListener('scroll', () => {
    if (soda) {
        modelMove();
    }
})
window.addEventListener('resize', () => {
    renderer.setSize(window.innerWidth, window.innerHeight);
    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();
})


