import { useCallback, useEffect, useRef, useState } from "react";

/* ───── inline particle sphere ───── */
function ParticleSphere({ size = 180 }: { size?: number }) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const animRef = useRef<number>(0);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext("2d");
    if (!ctx) return;

    const cx = size / 2;
    const cy = size / 2;
    const radius = size * 0.38;
    const dots: { theta: number; phi: number; speed: number }[] = [];
    const count = 300;

    for (let i = 0; i < count; i++) {
      dots.push({
        theta: Math.random() * Math.PI * 2,
        phi: Math.acos(2 * Math.random() - 1),
        speed: 0.002 + Math.random() * 0.004,
      });
    }

    let frame = 0;
    function draw() {
      frame++;
      ctx!.clearRect(0, 0, size, size);

      for (const d of dots) {
        d.theta += d.speed;
        d.phi += d.speed * 0.3;
        const x = cx + radius * Math.sin(d.phi) * Math.cos(d.theta);
        const y = cy + radius * Math.cos(d.phi);
        const z = radius * Math.sin(d.phi) * Math.sin(d.theta);
        const zNorm = (z / radius + 1) / 2;
        const alpha = 0.2 + zNorm * 0.6;
        const dotSize = 1 + zNorm * 2;

        ctx!.beginPath();
        ctx!.arc(x, y, dotSize, 0, Math.PI * 2);
        ctx!.fillStyle = `rgba(52, 211, 230, ${alpha})`;
        ctx!.fill();
      }

      animRef.current = requestAnimationFrame(draw);
    }
    draw();

    return () => cancelAnimationFrame(animRef.current);
  }, [size]);

  return (
    <canvas
      ref={canvasRef}
      width={size}
      height={size}
      style={{
        display: "block",
        margin: "0 auto",
        borderRadius: "50%",
        background: "radial-gradient(circle, rgba(52,211,230,0.06) 0%, transparent 70%)",
      }}
    />
  );
}

/* ───── widget ───── */
export default function Widget() {
  const [step, setStep] = useState(0);

  const handleClick = useCallback(() => {
    setStep((s) => (s < 3 ? s + 1 : 0));
  }, []);

  const messages = [
    { text: "SINAL DETECTADO", sub: "Frequência desconhecida…" },
    { text: "DECODIFICANDO…", sub: "Protocolo não reconhecido" },
    { text: "ACESSO NEGADO", sub: "Planos além do seu nível de clearance" },
    {
      text: "VOCÊ AINDA NÃO ESTÁ PRONTO",
      sub: "para se comunicar com outros planos, mas chegará lá.",
    },
  ];

  const m = messages[step];

  return (
    <div
      onClick={handleClick}
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        gap: 10,
        padding: "28px 20px",
        cursor: "pointer",
        userSelect: "none",
        fontFamily: "'Segoe UI', system-ui, sans-serif",
      }}
    >
      <ParticleSphere size={180} />
      <div style={{ textAlign: "center" }}>
        <div
          style={{
            fontSize: 16,
            fontWeight: 700,
            color: "#34d3e6",
            letterSpacing: ".1em",
            textTransform: "uppercase",
            textShadow: "0 0 12px rgba(52,211,230,0.3)",
          }}
        >
          {m.text}
        </div>
        <div
          style={{
            marginTop: 4,
            fontSize: 12,
            color: "#888baa",
            letterSpacing: ".04em",
          }}
        >
          {m.sub}
        </div>
      </div>
      <div
        style={{
          fontSize: 10,
          color: "#555770",
          letterSpacing: ".06em",
          marginTop: 6,
        }}
      >
        {">"} clique para avançar o sinal {">"}
      </div>
    </div>
  );
}
