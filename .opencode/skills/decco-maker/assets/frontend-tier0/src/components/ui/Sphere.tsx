import { useEffect, useRef } from "react";

/* Esfera de partículas (canvas 2D, zero dependências) — aproxima o orbe DeCCO
   do GIF de referência: casca esférica de pontos ciano, rotação + ondulação.
   NOTA (FE Tier 3): para a versão "de verdade" com shaders/curl-noise, migrar para
   react-three-fiber + three.js (Points + vertex shader). Aqui é o placeholder fiel do visual. */

export function ParticleSphere({ size = 260, count = 900, color = "#34d3e6" }: {
  size?: number; count?: number; color?: string;
}) {
  const ref = useRef<HTMLCanvasElement | null>(null);

  useEffect(() => {
    const canvas = ref.current;
    if (!canvas) return;
    const ctx = canvas.getContext("2d");
    if (!ctx) return;

    const dpr = Math.min(window.devicePixelRatio || 1, 2);
    canvas.width = size * dpr;
    canvas.height = size * dpr;
    ctx.scale(dpr, dpr);

    const N = count;
    const R = size * 0.36;
    const cx = size / 2;
    const cy = size / 2;
    const golden = Math.PI * (3 - Math.sqrt(5));

    // direções unitárias na esfera (fibonacci) + fase para a ondulação
    const pts = Array.from({ length: N }, (_, i) => {
      const y = 1 - (i / (N - 1)) * 2;
      const r = Math.sqrt(1 - y * y);
      const th = i * golden;
      return { x: Math.cos(th) * r, y, z: Math.sin(th) * r, phase: th };
    });

    let raf = 0;
    let t = 0;
    let running = true;

    const frame = () => {
      if (!running) return;
      t += 0.012;
      ctx.clearRect(0, 0, size, size);

      // núcleo/reator ao fundo
      const coreR = R * 0.42;
      const grad = ctx.createRadialGradient(cx, cy, 0, cx, cy, coreR);
      grad.addColorStop(0, "rgba(52,211,230,0.30)");
      grad.addColorStop(1, "rgba(52,211,230,0)");
      ctx.fillStyle = grad;
      ctx.beginPath(); ctx.arc(cx, cy, coreR, 0, Math.PI * 2); ctx.fill();

      const cosA = Math.cos(t * 0.5), sinA = Math.sin(t * 0.5);
      const tilt = 0.42, cosT = Math.cos(tilt), sinT = Math.sin(tilt);

      for (const p of pts) {
        // ondulação da casca (turbulência do GIF)
        const wob = 1 + 0.14 * Math.sin(3 * p.phase + t * 1.6) + 0.05 * Math.sin(t * 2 + p.y * 6);
        let x = p.x * wob, y = p.y * wob, z = p.z * wob;
        // rotação em Y
        const x1 = x * cosA - z * sinA;
        const z1 = x * sinA + z * cosA;
        // tilt em X
        const y1 = y * cosT - z1 * sinT;
        const z2 = y * sinT + z1 * cosT;

        const persp = 1.9 / (1.9 + z2);           // profundidade
        const sx = cx + x1 * R * persp;
        const sy = cy + y1 * R * persp;
        const depth = (z2 + 1) / 2;                 // 0 (fundo) .. 1 (frente)
        const alpha = 0.15 + depth * 0.85;
        const dot = (0.6 + depth * 1.6) * persp;

        ctx.beginPath();
        ctx.arc(sx, sy, dot, 0, Math.PI * 2);
        ctx.fillStyle = color;
        ctx.globalAlpha = alpha;
        ctx.fill();
      }
      ctx.globalAlpha = 1;
      raf = requestAnimationFrame(frame);
    };
    raf = requestAnimationFrame(frame);

    return () => { running = false; cancelAnimationFrame(raf); };
  }, [size, count, color]);

  return (
    <div className="sphere-wrap" style={{ width: size, height: size }}>
      <canvas ref={ref} style={{ width: size, height: size }} />
    </div>
  );
}
