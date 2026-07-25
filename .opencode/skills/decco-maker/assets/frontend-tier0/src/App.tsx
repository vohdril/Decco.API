import { Navigate, Route, Routes } from "react-router-dom";
import { ProtectedRoute } from "./auth/auth";
import { AnomaliasPage } from "./pages/AnomaliasPage";
import DashboardPage from "./pages/DashboardPage";
import GlossarioPage from "./pages/GlossarioPage";
import InstanciaDeviantePage from "./pages/InstanciaDeviantePage";
import LaboratorioPage from "./pages/LaboratorioPage";
import { LoginPage } from "./pages/LoginPage";
import ManifestacaoEspecificaPage from "./pages/ManifestacaoEspecificaPage";
import MecanismoInteracaoPage from "./pages/MecanismoInteracaoPage";
import PericiaAnomaliaPage from "./pages/PericiaAnomaliaPage";
import ProtocoloPage from "./pages/ProtocoloPage";

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/" element={<ProtectedRoute><DashboardPage /></ProtectedRoute>} />
      <Route path="/anomalias" element={<ProtectedRoute><AnomaliasPage /></ProtectedRoute>} />
      <Route path="/glossario" element={<ProtectedRoute><GlossarioPage /></ProtectedRoute>} />
      <Route path="/config/mecanismo-interacao" element={<ProtectedRoute><MecanismoInteracaoPage /></ProtectedRoute>} />
      <Route path="/config/pericia-anomalia" element={<ProtectedRoute><PericiaAnomaliaPage /></ProtectedRoute>} />
      <Route path="/config/manifestacao-especifica" element={<ProtectedRoute><ManifestacaoEspecificaPage /></ProtectedRoute>} />
      <Route path="/config/instancia-deviante" element={<ProtectedRoute><InstanciaDeviantePage /></ProtectedRoute>} />
      <Route path="/config/laboratorio" element={<ProtectedRoute><LaboratorioPage /></ProtectedRoute>} />
      <Route path="/config/protocolo" element={<ProtectedRoute><ProtocoloPage /></ProtectedRoute>} />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
