import * as Dialog from "@radix-ui/react-dialog";
import { X } from "lucide-react";
import type { ReactNode } from "react";

/* Modal do Decco sobre @radix-ui/react-dialog.
   Por que Radix (e não só <dialog> nativo): focus-trap robusto, ESC, scroll-lock,
   portal, ARIA (role/aria-modal/labelledby/describedby) e composição — o padrão
   corporativo acessível e escalável. Aqui só vestimos a estética DeCCO por cima. */

export function Modal({ open, onOpenChange, title, description, children, footer, className }: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  title: ReactNode;
  description?: ReactNode;
  children?: ReactNode;
  footer?: ReactNode;
  className?: string;
}) {
  return (
    <Dialog.Root open={open} onOpenChange={onOpenChange}>
      <Dialog.Portal>
        <Dialog.Overlay className="modal__overlay" />
        <Dialog.Content className={["modal__content", className].filter(Boolean).join(" ")}>
          <div className="modal__head">
            <Dialog.Title className="hud-label" style={{ fontSize: 13, color: "var(--text-0)" }}>
              {title}
            </Dialog.Title>
            <Dialog.Close asChild>
              <button className="modal__close" aria-label="Fechar"><X size={16} /></button>
            </Dialog.Close>
          </div>
          {description && (
            <Dialog.Description className="muted" style={{ marginTop: -6, marginBottom: 12, fontSize: 12 }}>
              {description}
            </Dialog.Description>
          )}
          {children}
          {footer && <div className="modal__foot">{footer}</div>}
        </Dialog.Content>
      </Dialog.Portal>
    </Dialog.Root>
  );
}
