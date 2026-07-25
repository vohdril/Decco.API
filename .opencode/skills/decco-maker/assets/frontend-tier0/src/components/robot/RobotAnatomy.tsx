import type { SVGProps } from "react";

/**
 * RobotDashAnatomy
 *
 * Decomposição didática do SVG robot-dash em partes individuais nomeadas.
 *
 * ── Nota mental sobre manipulação de SVG ──
 * SVG puro (inline no HTML) é declarativo e estático. No React, cada elemento
 * vira um componente JSX — o que permite: props dinâmicas, animação via estado,
 * substituição de partes isoladas, composição conditional, styled-components,
 * motion (framer-motion/GSAP). A mesma lógica se aplica a qualquer framework
 * reativo (Vue SFC, Svelte, Solid). Fora do ecossistema web, engines como
 * Skia (Flutter, Compose) e SceneKit (SwiftUI) tratam gráficos vetoriais de
 * forma análoga: nodos nomeados, transforms, animação por código.
 * O pulo do gatilho é tratar o SVG como árvore de componentes, não como
 * blob de path único.
 * ──────────────────────────────────────────
 *
 * Cada sub-path do `d` original foi extraído e nomeado por região anatômica.
 *
 * Uso: import e monte apenas as peças que quiser editar/animar.
 *   <RobotOlhoEsquerdo />
 *   <RobotOlhoDireito />
 *   <RobotCore />
 *
 * viewBox: "0 0 458.49 458.49"
 * O esqueleto completo (robot-skeleton) usa fill="var(--bg-2)" stroke="var(--text-0)" strokeWidth="1".
 * Os brilhos (olho-esquerdo, olho-direito, interior-tronco) usam fill="var(--accent-strong)" / "var(--core)".
 */

/* ========================================================================
   CABEÇA
   ======================================================================== */

/** Domo superior do crânio (y ≈ 0–57, cx ≈ 229) */
export function RobotCabeca(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="cabeca"
      d="M159.694,33.705c0-13.071,10.634-23.705,23.705-23.705h91.483
c13.07,0,23.704,10.634,23.704,23.705S287.953,57.41,274.883,57.41H248.23v-9.615c0-10.524-8.563-19.086-19.087-19.086
s-19.087,8.562-19.087,19.086v9.615h-26.657C170.328,57.41,159.694,46.776,159.694,33.705z M220.057,57.41v-9.615
c0-5.01,4.076-9.086,9.087-9.086s9.087,4.076,9.087,9.086v9.615H220.057z"
      {...props}
    />
  );
}

/** Placa frontal / testa (y ≈ 67–146) */
export function RobotTesta(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="testa"
      d="M193.094,67.41h16.963v0.002h38.174V67.41h16.958
c-2.05,10.711-10.551,19.153-21.292,21.117c-2.407-5.781-8.112-9.858-14.756-9.858c-6.643,0-12.348,4.077-14.755,9.858
C203.645,86.563,195.144,78.121,193.094,67.41z"
      {...props}
    />
  );
}

/** Ponte nasal / entre-olhos (y ≈ 95–146) */
export function RobotPonteNasal(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="ponte-nasal"
      d="M266.983,146.5H245.12v-21.863C256.187,126.692,264.927,135.433,266.983,146.5z M235.12,146.5h-11.958V94.647
c0-3.296,2.682-5.979,5.979-5.979s5.979,2.682,5.979,5.979V146.5z M213.162,124.637V146.5H191.3
C193.355,135.433,202.096,126.692,213.162,124.637z"
      {...props}
    />
  );
}

/** Órbita / soquete do olho esquerdo (centro ≈ 146.6, 160.5, r≈9) */
export function RobotOrbitaEsquerda(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="orbita-esquerda"
      d="M146.625,160.467L146.625,160.467c-2.396,2.396-5.58,3.715-8.967,3.715c-3.388,0-6.572-1.319-8.968-3.715
c-2.395-2.395-3.714-5.58-3.714-8.967s1.319-6.572,3.714-8.967c2.396-2.395,5.58-3.714,8.968-3.714
c3.387,0,6.571,1.319,8.967,3.714c2.395,2.395,3.714,5.58,3.714,8.967S149.02,158.072,146.625,160.467z"
      {...props}
    />
  );
}

/** Órbita / soquete do olho direito (centro ≈ 311.9, 160.5, r≈9) */
export function RobotOrbitaDireita(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="orbita-direita"
      d="M311.864,160.467c-2.395-2.395-3.714-5.58-3.714-8.967s1.319-6.572,3.714-8.967
c2.473-2.473,5.72-3.708,8.968-3.708c3.247,0,6.495,1.236,8.968,3.708c2.395,2.395,3.714,5.58,3.714,8.967
s-1.319,6.572-3.714,8.967C324.854,165.412,316.81,165.412,311.864,160.467z"
      {...props}
    />
  );
}

/** Região da boca / abertura facial inferior (y ≈ 157–183) */
export function RobotBoca(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="boca"
      d="M283.734,183.278c0,10.053-8.179,18.231-18.231,18.231h-72.724c-10.053,0-18.231-8.179-18.231-18.231V156.5h109.187V183.278z"
      {...props}
    />
  );
}

/** Detalhe central da boca / grelha */
export function RobotGradeBoca(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="grade-boca"
      d="M199.874,211.509h7.507v7.77C203.909,217.868,201.172,215.037,199.874,211.509z M240.901,240.627h-23.521v-28.704h23.521V240.627z M250.901,219.279v-7.77h7.507C257.11,215.037,254.373,217.868,250.901,219.279z"
      {...props}
    />
  );
}

/* ========================================================================
   TRONCO / PEITO
   ======================================================================== */

/** Abertura frontal do tronco (onde o core laranja aparece, y ≈ 250–265) */
export function RobotAberturaTronco(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="abertura-tronco"
      d="M267.297,264.995h-76.313c2.21-8.264,9.762-14.368,18.713-14.368h38.887C257.535,250.627,265.087,256.731,267.297,264.995z"
      {...props}
    />
  );
}

/** Placa peitoral superior */
export function RobotPeitoral(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="peitoral"
      d="M191.974,296.941v24.212c0,3-2.441,5.441-5.441,5.441h-2.403c-3,0-5.441-2.441-5.441-5.441v-24.212c0-3,2.441-5.441,5.441-5.441h2.403C189.532,291.5,191.974,293.941,191.974,296.941z M279.595,296.941v24.212c0,3-2.441,5.441-5.441,5.441h-2.403c-3,0-5.441-2.441-5.441-5.441v-24.212c0-3,2.441-5.441,5.441-5.441h2.403C277.153,291.5,279.595,293.941,279.595,296.941z"
      {...props}
    />
  );
}

/* ========================================================================
   BRAÇOS
   ======================================================================== */

/** Braço esquerdo completo (y ≈ 144–260) */
export function RobotBracoEsquerdo(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="braco-esquerdo"
      d="M83.012,144.407c5.32-5.321,12.395-8.251,19.919-8.251c5.366,0,10.499,1.498,14.935,4.283c-1.875,3.344-2.889,7.122-2.889,11.062c0,4.353,1.236,8.51,3.511,12.099l-4.947,4.947c-2.652-1.472-5.643-2.265-8.76-2.265c-4.834,0-9.379,1.882-12.797,5.3l-10.728,10.729C72.069,171.254,72.652,154.767,83.012,144.407z M108.435,260.621c1.227,0,2.381,0.478,3.249,1.346l6.185,6.185l-7.131,7.13l-6.185-6.185c-0.867-0.868-1.346-2.021-1.346-3.249s0.479-2.381,1.346-3.249l0.634-0.633C106.054,261.098,107.207,260.621,108.435,260.621z M110.506,190.103l-18.518,18.518c-1.53,1.53-3.563,2.372-5.727,2.372s-4.195-0.842-5.725-2.372l-0.001,0c-1.529-1.529-2.371-3.563-2.371-5.726c0-2.163,0.842-4.196,2.371-5.726l18.518-18.518c1.53-1.529,3.563-2.372,5.727-2.372s4.196,0.842,5.726,2.372s2.372,3.563,2.372,5.726S112.035,188.574,110.506,190.103z"
      {...props}
    />
  );
}

/** Braço direito completo (y ≈ 144–269) */
export function RobotBracoDireito(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="braco-direito"
      d="M375.479,144.407c10.36,10.36,10.942,26.847,1.757,37.903l-10.728-10.728c-3.418-3.418-7.963-5.301-12.797-5.301c-3.117,0-6.109,0.793-8.761,2.266l-4.947-4.947c2.275-3.589,3.511-7.746,3.511-12.099c0-3.939-1.013-7.718-2.889-11.062c4.435-2.785,9.568-4.282,14.935-4.282C363.083,136.156,370.157,139.086,375.479,144.407z M353.938,269.096l-6.186,6.185l-7.13-7.13l6.186-6.185c0.867-0.868,2.021-1.345,3.248-1.345c1.227,0,2.381,0.478,3.249,1.346l0.633,0.633c0.867,0.868,1.346,2.021,1.346,3.249S354.805,268.229,353.938,269.096z M366.502,208.621l-18.519-18.518c-1.529-1.529-2.371-3.563-2.371-5.726s0.842-4.196,2.371-5.726c1.53-1.529,3.563-2.372,5.727-2.372s4.196,0.842,5.726,2.372l18.518,18.518c3.157,3.157,3.157,8.295,0,11.452c-1.529,1.529-3.563,2.372-5.726,2.372S368.031,210.151,366.502,208.621z"
      {...props}
    />
  );
}

/* ========================================================================
   PERNAS
   ======================================================================== */

/** Perna esquerda (y ≈ 296–434) */
export function RobotPernaEsquerda(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="perna-esquerda"
      d="M153.319,331.595c0-9.503,6.551-17.484,15.368-19.722v9.28c0,7.202,4.962,13.25,11.643,14.949v6.005c-4.885,1.243-8.839,4.811-10.616,9.46C160.379,349.714,153.319,341.466,153.319,331.595z M191.974,357.058c0,3-2.441,5.442-5.441,5.442h-2.403c-3,0-5.441-2.441-5.441-5.442s2.441-5.442,5.441-5.442h2.403C189.532,351.616,191.974,354.058,191.974,357.058z M191.974,392.69v6.907c0,3-2.441,5.441-5.441,5.441h-2.403c-3,0-5.441-2.441-5.441-5.441v-6.907c0-3,2.441-5.442,5.441-5.442h2.403C189.532,387.248,191.974,389.689,191.974,392.69z"
      {...props}
    />
  );
}

/** Perna direita (y ≈ 296–434) */
export function RobotPernaDireita(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="perna-direita"
      d="M304.963,331.595c0,9.871-7.06,18.119-16.395,19.973c-1.777-4.65-5.732-8.217-10.617-9.46v-6.005c6.681-1.699,11.644-7.747,11.644-14.949v-9.28C298.412,314.111,304.963,322.092,304.963,331.595z M274.153,362.5h-2.403c-3,0-5.441-2.441-5.441-5.442s2.441-5.442,5.441-5.442h2.403c3,0,5.441,2.441,5.441,5.442S277.153,362.5,274.153,362.5z M279.595,392.69v6.907c0,3-2.441,5.441-5.441,5.441h-2.403c-3,0-5.441-2.441-5.441-5.441v-6.907c0-3,2.441-5.442,5.441-5.442h2.403C277.153,387.248,279.595,389.689,279.595,392.69z"
      {...props}
    />
  );
}

/** Pé esquerdo (y ≈ 429–448) */
export function RobotPeEsquerdo(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="pe-esquerdo"
      d="M194.142,433.877l3.746,14.613h-25.113l3.745-14.613c0.617-2.408,2.784-4.09,5.271-4.09h7.081C191.357,429.787,193.524,431.469,194.142,433.877z"
      {...props}
    />
  );
}

/** Pé direito (y ≈ 429–448) */
export function RobotPeDireito(props: SVGProps<SVGPathElement>) {
  return (
    <path
      name="pe-direito"
      d="M281.763,433.877l3.746,14.613h-25.113l3.745-14.613c0.617-2.408,2.784-4.09,5.271-4.09h7.081C278.979,429.787,281.146,431.469,281.763,433.877z"
      {...props}
    />
  );
}

/* ========================================================================
   GLOW / EFEITOS LUMINOSOS (atrás do esqueleto)
   ======================================================================== */

/** Brilho azul do olho esquerdo (renderizar ANTES do esqueleto) */
export function RobotGlowOlhoEsquerdo() {
  return (
    <circle name="olho-esquerdo" cx="183.1" cy="33.7" r="14" fill="var(--accent-strong)" filter="url(#glow-eye)">
      <animate attributeName="opacity" values="0.5;1;0.5" dur="1.8s" repeatCount="indefinite" />
    </circle>
  );
}

/** Brilho azul do olho direito */
export function RobotGlowOlhoDireito() {
  return (
    <circle name="olho-direito" cx="274.4" cy="33.7" r="14" fill="var(--accent-strong)" filter="url(#glow-eye)">
      <animate attributeName="opacity" values="0.5;1;0.5" dur="1.8s" repeatCount="indefinite" />
    </circle>
  );
}

/** Esfera laranja do core (renderizar DEPOIS do esqueleto) */
export function RobotGlowCore({ onCoreClick }: { onCoreClick?: () => void }) {
  return (
    <>
      <circle name="interior-tronco" cx="229" cy="180" r="14" fill="none" stroke="var(--core)" strokeWidth="2" opacity={0.5} />
      <circle name="interior-tronco" cx="229" cy="180" r="10" fill="var(--core)" opacity={0.15} />
      <circle name="interior-tronco" cx="229" cy="180" r="6" fill="var(--core)" filter="url(#glow-core)">
        <animate attributeName="opacity" values="0.4;1;0.4" dur="2.5s" repeatCount="indefinite" />
      </circle>
      {onCoreClick && (
        <circle cx="229" cy="180" r="18" fill="transparent"
          onClick={onCoreClick} style={{ cursor: "pointer" }} />
      )}
    </>
  );
}

/* ========================================================================
   MONTAGEM COMPLETA (igual ao original, mas com partes nomeadas)
   ======================================================================== */

const GLOW_FILTERS = (
  <defs>
    <filter id="glow-eye">
      <feGaussianBlur stdDeviation="3" result="b" />
      <feMerge><feMergeNode in="b" /><feMergeNode in="SourceGraphic" /></feMerge>
    </filter>
    <filter id="glow-core">
      <feGaussianBlur stdDeviation="3" result="b" />
      <feMerge><feMergeNode in="b" /><feMergeNode in="SourceGraphic" /></feMerge>
    </filter>
  </defs>
);

export function RobotDashCompleto({ onCoreClick, glowBoost }: {
  onCoreClick?: () => void; glowBoost?: boolean;
}) {
  return (
    <svg viewBox="0 0 458.49 458.49" className="robot-dash" aria-label="Android DeCCO">
      {GLOW_FILTERS}
      <RobotGlowOlhoEsquerdo />
      <RobotGlowOlhoDireito />
      <g fill="var(--bg-2)" stroke={glowBoost ? "var(--accent-strong)" : "var(--text-0)"}
         strokeWidth={glowBoost ? 1.5 : 1} name="robot-skeleton"
         style={glowBoost ? { filter: "drop-shadow(0 0 10px var(--accent-glow))" } : undefined}>
        <RobotCabeca />
        <RobotTesta />
        <RobotPonteNasal />
        <RobotOrbitaEsquerda />
        <RobotOrbitaDireita />
        <RobotBoca />
        <RobotGradeBoca />
        <RobotAberturaTronco />
        <RobotPeitoral />
        <RobotBracoEsquerdo />
        <RobotBracoDireito />
        <RobotPernaEsquerda />
        <RobotPernaDireita />
        <RobotPeEsquerdo />
        <RobotPeDireito />
      </g>
      <RobotGlowCore onCoreClick={onCoreClick} />
    </svg>
  );
}
