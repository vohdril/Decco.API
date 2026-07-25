import { AlertTriangle } from "lucide-react";
import { useEffect, useState } from "react";

interface DeviationWarningProps {
  /** O conceito que está sendo usado fora da ordem */
  concept: string;
  /** O tier em que este conceito deveria entrar */
  belongsTo: string;
  /** O tier atual do projeto */
  currentTier: string;
  /** O que fazer para voltar aos trilhos */
  getBackOnTrack: string;
}

const dismissed = new Set<string>();

export function DeviationWarning({ concept, belongsTo, currentTier, getBackOnTrack }: DeviationWarningProps) {
  const [visible, setVisible] = useState(!dismissed.has(concept));

  useEffect(() => {
    if (dismissed.has(concept)) setVisible(false);
  }, [concept]);

  if (!visible) return null;

  return (
    <div className="deviation">
      <div className="deviation__header">
        <AlertTriangle size={16} />
        <span className="deviation__title">DESVIO DE ROTA — Workflow Não-Destrutivo</span>
      </div>
      <p className="deviation__body">
        <b>"{concept}"</b> pertence ao <b>{belongsTo}</b> mas está sendo usado agora (Tier <b>{currentTier}</b>).
        Não é proibido — o sandbox é livre — mas sai do percurso didático idealizado.
      </p>
      <div className="deviation__action">
        <span className="deviation__track">🛤️ Voltar aos trilhos:</span>
        <span className="deviation__advice">{getBackOnTrack}</span>
      </div>
      <p className="deviation__foot">
        <button className="deviation__dismiss" onClick={() => { dismissed.add(concept); setVisible(false); }}>
          Dispensar este aviso (não mostrar novamente nesta sessão)
        </button>
      </p>
    </div>
  );
}

export function useDeviationLog() {
  return {
    DeviationWarning,
    /**
     * Use <DeviationWarning> em qualquer lugar onde um conceito de tier superior
     * for usado antes da hora. Exemplo:
     *
     *   <DeviationWarning
     *     concept="Chamada HTTP direta ao back sem gateway"
     *     belongsTo="FE1 — Conexão ao Back"
     *     currentTier="FE0 — Console Mock"
     *     getBackOnTrack="Use o gateway (data/index.ts) com VITE_DATA_SOURCE=live. Se não há back, deixe em mock."
     *   />
     */
  };
}
