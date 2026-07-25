import { zodResolver } from "@hookform/resolvers/zod";
import { Fingerprint, ShieldCheck } from "lucide-react";
import { useForm } from "react-hook-form";
import { useLocation, useNavigate } from "react-router-dom";
import { z } from "zod";
import { useAuth } from "../auth/auth";
import { Button, Panel, Spinner } from "../components/ui/primitives";

const schema = z.object({
  username: z.string().min(1, "Informe o operador."),
  password: z.string().min(1, "Informe a senha."),
});
type FormData = z.infer<typeof schema>;

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const from = (location.state as { from?: string } | null)?.from ?? "/anomalias";

  const { register, handleSubmit, setError, formState: { errors, isSubmitting } } =
    useForm<FormData>({ resolver: zodResolver(schema), defaultValues: { username: "", password: "" } });

  const onSubmit = handleSubmit(async (data) => {
    try {
      await login(data.username, data.password);
      navigate(from, { replace: true });
    } catch (e) {
      setError("root", { message: e instanceof Error ? e.message : "Falha na autenticação." });
    }
  });

  return (
    <div className="login">
      <div className="login__card">
        <div className="login__logo">
          <div className="brand-orb" style={{ width: 52, height: 52 }}><ShieldCheck size={26} /></div>
          <div className="login__title">D e C C o</div>
          <div className="hud-label">Terminal de Contenção · Acesso Restrito</div>
        </div>

        <Panel title={<><Fingerprint size={14} /> Autenticação de Operador</>}>
          <form onSubmit={onSubmit} className="col" style={{ gap: 14 }} noValidate>
            <div className="field">
              <label className="hud-label" htmlFor="u">Operador</label>
              <input id="u" className="input" placeholder="ex.: vance" autoComplete="username" {...register("username")} />
              {errors.username && <span className="field__error">{errors.username.message}</span>}
            </div>
            <div className="field">
              <label className="hud-label" htmlFor="p">Senha</label>
              <input id="p" type="password" className="input" placeholder="••••••" autoComplete="current-password" {...register("password")} />
              {errors.password && <span className="field__error">{errors.password.message}</span>}
            </div>

            {errors.root && (
              <div className="toast toast--error" style={{ position: "static" }}>
                <div className="toast__msg">{errors.root.message}</div>
              </div>
            )}

            <Button type="submit" variant="primary" disabled={isSubmitting} style={{ justifyContent: "center" }}>
              {isSubmitting ? <><Spinner size={14} /> Verificando…</> : "Autenticar"}
            </Button>
          </form>

          <div className="login__hint">
            <b className="accent">DEMO · operadores</b><br />
            <code>vance</code> (Pesquisador, Sítio-19) · <code>thoth</code> (Diretor, Sítio-64) · <code>o5</code> (O5, acesso total)<br />
            senha para todos: <code>decco</code>
          </div>
        </Panel>
      </div>
    </div>
  );
}
