// =============================================
// ProfsMarkov Hub — Dark Tech Theme JS
// =============================================

document.addEventListener('DOMContentLoaded', function () {

    // ---- AOS Init ----
    if (typeof AOS !== 'undefined') {
        AOS.init({
            duration: 800,
            easing: 'ease-out-cubic',
            once: true,
            offset: 60
        });
    }

    // ---- Navbar Scroll Effect ----
    const nav = document.getElementById('mainNav');
    if (nav) {
        const onScroll = () => {
            if (window.scrollY > 50) {
                nav.classList.add('scrolled');
            } else {
                nav.classList.remove('scrolled');
            }
        };
        window.addEventListener('scroll', onScroll, { passive: true });
        onScroll(); // initial check
    }

    // ---- Particles.js Init ----
    const particlesEl = document.getElementById('particles-js');
    if (particlesEl && typeof particlesJS !== 'undefined') {
        particlesJS('particles-js', {
            particles: {
                number: { value: 50, density: { enable: true, value_area: 900 } },
                color: { value: '#8ab4f8' },
                shape: { type: 'circle' },
                opacity: {
                    value: 0.25,
                    random: true,
                    anim: { enable: true, speed: 0.5, opacity_min: 0.08, sync: false }
                },
                size: {
                    value: 2.5,
                    random: true,
                    anim: { enable: true, speed: 1, size_min: 0.5, sync: false }
                },
                line_linked: {
                    enable: true,
                    distance: 140,
                    color: '#8ab4f8',
                    opacity: 0.08,
                    width: 1
                },
                move: {
                    enable: true,
                    speed: 0.6,
                    direction: 'none',
                    random: true,
                    straight: false,
                    out_mode: 'out',
                    bounce: false
                }
            },
            interactivity: {
                detect_on: 'canvas',
                events: {
                    onhover: { enable: false },
                    onclick: { enable: false },
                    resize: true
                }
            },
            retina_detect: true
        });
    }

    // ---- Typed.js Init ----
    const typedEl = document.getElementById('typed-tagline');
    if (typedEl && typeof Typed !== 'undefined') {
        new Typed('#typed-tagline', {
            strings: [
                'Gaming \u2022 Tech \u2022 Code',
                'Streams \u2022 Reviews \u2022 Deep Dives',
                'Horror \u2022 Indie \u2022 WoW',
                'By ProfsMarkov'
            ],
            typeSpeed: 45,
            backSpeed: 25,
            backDelay: 2000,
            startDelay: 500,
            loop: true,
            showCursor: true,
            cursorChar: '|'
        });
    }

    // ---- Reading Progress Bar ----
    const progressBar = document.getElementById('reading-progress');
    const articleContent = document.querySelector('.blog-article, .blog-content');
    if (progressBar && articleContent) {
        progressBar.style.display = 'block';
        window.addEventListener('scroll', function () {
            const docHeight = document.documentElement.scrollHeight - window.innerHeight;
            const scrolled = (window.scrollY / docHeight) * 100;
            progressBar.style.width = Math.min(100, Math.max(0, scrolled)) + '%';
        }, { passive: true });
    }
});
