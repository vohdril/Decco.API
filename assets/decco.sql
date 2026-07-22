-- =============================================
-- SISTEMA DE CATALOGAÇÃO ONTOLÓGICA DE ANOMALIAS
-- Refatoração completa com relacionamentos 1:N,
-- Forças Fundamentais, Perícias e Desvios.
-- Inclui stored procedures para CRUD completo.
-- =============================================

USE master;
GO

CREATE DATABASE DeccoDB;
GO

USE DeccoDB;
GO

-- =============================================
-- SEÇÃO 1: TABELAS DE CATÁLOGO FUNDAMENTAL
-- =============================================

-- CAT0: CLASSES DE OBJETO (Protocolos de Contenção)
CREATE TABLE Cat_ClasseObjeto (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Codigo VARCHAR(10) NOT NULL UNIQUE,
    Nome VARCHAR(50) NOT NULL,
    TipoClasse VARCHAR(50) NOT NULL, --PRIMÁRIA, SECUNDÁRIA,....
	ClasseACS VARCHAR (40) NULL,
    Descricao TEXT NULL,
    NivelAcessoMinimo INT NOT NULL DEFAULT 1,
    CorAlerta VARCHAR(7) NULL DEFAULT '#FFFFFF',
    DataCriacao DATETIME NOT NULL DEFAULT GETDATE(),
    Ativo BIT NOT NULL DEFAULT 1
);
GO

-- Inserção das classes padrão SCP
INSERT INTO Cat_ClasseObjeto (Codigo, Nome, TipoClasse, ClasseACS, Descricao, NivelAcessoMinimo, CorAlerta) VALUES
('PACATO','Pacato','Primária', 'SAFE', 'Anomalia contida de forma confiável com procedimentos padrão.', 1, '#4CAF50'),
('YAGUARA','Yaguara','Primária','EUCLID','Anomalia imprevisível que requer atenção constante.', 2, '#FFC107'),
('ABAPORU','Abaporu','Primária','KETER','Extremamente difícil de conter, risco catastrófico.', 3, '#F44336'),
('UKAR','Ukar','Primária','THAUMIEL', 'Usada para conter outras anomalias. O segredo dos segredos.', 4, '#9C27B0')
GO

-- CAT1: FORÇAS FUNDAMENTAIS (A fonte primária da anomalia)
CREATE TABLE Cat_ForcaFundamental (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Simbolo CHAR(10) NOT NULL UNIQUE,
    Nome VARCHAR(50) NOT NULL UNIQUE,
    Descricao NVARCHAR(MAX) NOT NULL,
    ParticulaPortadora VARCHAR(50) NULL
);
GO

-- CAT2: CAMADAS ONTOLÓGICAS (Onde a anomalia 'vive')
CREATE TABLE Cat_CamadaOntologica (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Simbolo CHAR(10) NOT NULL UNIQUE,
    Nome VARCHAR(50) NOT NULL,
    Descricao NVARCHAR(MAX) NOT NULL,
    ForcaFundamentalId INT NULL FOREIGN KEY REFERENCES Cat_ForcaFundamental(Id),
    Prioridade INT NOT NULL DEFAULT 1
);
GO

-- CAT3: TIPOS DE MATÉRIA
CREATE TABLE Cat_TipoMateria (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome VARCHAR(50) NOT NULL UNIQUE,
    Descricao NVARCHAR(MAX) NOT NULL,
    IsResistenteSupressores BIT NOT NULL DEFAULT 0
);
GO

-- CAT4: MECANISMOS DE INTERAÇÃO (Como a anomalia interage)
CREATE TABLE Cat_MecanismoInteracao (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Codigo VARCHAR(10) NOT NULL UNIQUE,
    Nome VARCHAR(100) NOT NULL,
    Descricao NVARCHAR(MAX) NOT NULL,
    CamadaOntologicaId INT NOT NULL FOREIGN KEY REFERENCES Cat_CamadaOntologica(Id),
    EhSubnatureza BIT NOT NULL DEFAULT 0
);
GO

-- CAT5: MANIFESTAÇÕES ESPECÍFICAS (Efeitos observáveis)
CREATE TABLE Cat_ManifestacaoEspecifica (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Codigo VARCHAR(20) NOT NULL UNIQUE,
    Nome NVARCHAR(100) NOT NULL,
    Descricao NVARCHAR(MAX) NOT NULL
);
GO

-- CAT6: COGNIÇÃO APARENTE (SE/SA/IN/AA)
CREATE TABLE Cat_CognicaoAparente (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Codigo VARCHAR(5) NOT NULL UNIQUE,
    Nome VARCHAR(50) NOT NULL,
    Descricao NVARCHAR(MAX) NOT NULL
);
GO

INSERT INTO Cat_CognicaoAparente (Codigo, Nome, Descricao) VALUES
('SE', 'Sensciente', 'Possui nível de inteligência e cognição aferidos.'),
('SA', 'Sapiente', 'Possui nível de sapiencia aferido, independente de manifestação cultural.'),
('IN', 'Inanimado', 'Sem vontade própria aferida.'),
('AA', 'Autômato Anômalo', 'Entidade com aparente autonomia e comportamento autômato.');
GO

-- CAT7: PERICULOSIDADE (Mínimo → Máximo, 9 níveis)
CREATE TABLE Cat_Periculosidade (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nivel INT NOT NULL UNIQUE CHECK (Nivel BETWEEN 1 AND 9),
    Nome VARCHAR(50) NOT NULL,
    Descricao NVARCHAR(MAX) NOT NULL,
    CorAlerta VARCHAR(7) NULL DEFAULT '#FFFFFF'
);
GO

INSERT INTO Cat_Periculosidade (Nivel, Nome, Descricao, CorAlerta) VALUES
(1, 'Mínimo', 'Nenhum perigo em quase todas as manifestações.', '#4CAF50'),
(2, 'Muito Baixo', 'Leve perigo se não conduzido corretamente.', '#8BC34A'),
(3, 'Baixo I (Atividade)', 'Perigo leve ou moderado por definição ontológica.', '#CDDC39'),
(4, 'Baixo II (Recursos)', 'Perigo leve ou moderado em condições geográficas específicas.', '#FFEB3B'),
(5, 'Médio', 'Perigo moderado. Requer treinamento mínimo.', '#FFC107'),
(6, 'Alto I (Atividade)', 'Alto perigo pela existência ou comportamento da anomalia.', '#FF9800'),
(7, 'Alto II (Recursos)', 'Alto risco mediante condições materiais/geográficas.', '#FF5722'),
(8, 'Muito Alto', 'Risco de romper o Véu/Esquadria. Requer protocolo especial.', '#F44336'),
(9, 'Máximo', 'Nenhum protocolo convencional é esperado funcionar.', '#D32F2F');
GO

-- =============================================
-- SEÇÃO 2: TABELA PRINCIPAL DE ANOMALIAS
-- =============================================

CREATE TABLE Anomalia (
    Id INT PRIMARY KEY IDENTITY(1000,1),
    CodigoSCP VARCHAR(50) NOT NULL UNIQUE,
    NomeComum NVARCHAR(255) NOT NULL,
    Descricao NVARCHAR(MAX) NOT NULL,
    
    -- Classificação Básica
    ClasseObjetoId INT NOT NULL FOREIGN KEY REFERENCES Cat_ClasseObjeto(Id),
    CamadaOntologicaId INT NOT NULL FOREIGN KEY REFERENCES Cat_CamadaOntologica(Id),
    TipoMateriaId INT NOT NULL FOREIGN KEY REFERENCES Cat_TipoMateria(Id),
    
    -- Classificação Brasileira (Sistema OA)
    CognicaoAparenteId INT NULL FOREIGN KEY REFERENCES Cat_CognicaoAparente(Id),
    PericulosidadeId INT NULL FOREIGN KEY REFERENCES Cat_Periculosidade(Id),
    
    -- Mecanismos Padrão
    MecanismoPrimarioId INT NOT NULL FOREIGN KEY REFERENCES Cat_MecanismoInteracao(Id),
    MecanismoSecundarioId INT NULL FOREIGN KEY REFERENCES Cat_MecanismoInteracao(Id),
    
    -- Marcadores Theta
    IEIA_D_Base DECIMAL(8,4) NULL,
    FatorCoerenciaSpin VARCHAR(20) NULL,
    
    -- Status
    Status VARCHAR(20) DEFAULT 'ATIVA',
    SitioContencao NVARCHAR(100) NULL,
    ResponsavelPesquisa NVARCHAR(255) NULL,
    
    -- Auditoria
    DataCriacao DATETIME DEFAULT GETDATE(),
    DataAtualizacao DATETIME DEFAULT GETDATE(),
    UsuarioCriacao NVARCHAR(128) DEFAULT SYSTEM_USER,
    UsuarioAtualizacao NVARCHAR(128) DEFAULT SYSTEM_USER
);
GO
GO

-- =============================================
-- SEÇÃO 3: SUBTABELAS 1:N (INSTÂNCIAS)
-- =============================================

CREATE TABLE EntidadeViva (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AnomaliaId INT NOT NULL FOREIGN KEY REFERENCES Anomalia(Id) ON DELETE CASCADE,
    Identificacao NVARCHAR(100) NOT NULL,
    Especie NVARCHAR(150) NOT NULL,
    Biologia NVARCHAR(255) NULL,
    OrigemPoder NVARCHAR(100) NULL,
    DataNascimento DATE NULL,
    IsConsciente BIT DEFAULT 1,
    NivelInteligencia INT NULL,
    Dieta NVARCHAR(100) NULL,
    Observacoes NVARCHAR(MAX) NULL
);
GO

CREATE TABLE Artefato (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AnomaliaId INT NOT NULL FOREIGN KEY REFERENCES Anomalia(Id) ON DELETE CASCADE,
    Identificacao NVARCHAR(100) NOT NULL,
    Material NVARCHAR(255) NULL,
    DataFabricacao DATE NULL,
    LocalOrigem NVARCHAR(255) NULL,
    PropriedadeSpin VARCHAR(100) NULL,
    Peso_Kg DECIMAL(10,2) NULL,
    Dimensoes VARCHAR(100) NULL,
    ModoUsar NVARCHAR(MAX) NULL
);
GO

CREATE TABLE Localidade (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AnomaliaId INT NOT NULL FOREIGN KEY REFERENCES Anomalia(Id) ON DELETE CASCADE,
    Nome NVARCHAR(255) NOT NULL,
    Coordenadas GEOGRAPHY NULL,
    RaioEfeitoMetros INT NULL,
    IEIA_D_Ambiente DECIMAL(8,4) NULL,
    IsGeograficamenteLimitada BIT DEFAULT 1,
    TipoTerreno VARCHAR(100) NULL,
    ClimaAnomalo VARCHAR(100) NULL
);
GO

CREATE TABLE Evento (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AnomaliaId INT NOT NULL FOREIGN KEY REFERENCES Anomalia(Id) ON DELETE CASCADE,
    Nome NVARCHAR(255) NOT NULL,
    DataHoraInicio DATETIME NOT NULL,
    DataHoraFim DATETIME NULL,
    Periodicidade NVARCHAR(100) NULL,
    ZonaAfetada NVARCHAR(255) NULL,
    DuracaoMedia TIME NULL,
    PreCondicoes NVARCHAR(MAX) NULL
);
GO

-- =============================================
-- SEÇÃO 3B: NOVAS ENTIDADES (LABORATÓRIO, PROTOCOLO, NOTIFICAÇÃO)
-- =============================================

CREATE TABLE Laboratorio (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Codigo VARCHAR(20) NOT NULL UNIQUE,
    Nome NVARCHAR(255) NOT NULL,
    Descricao NVARCHAR(MAX) NULL,
    Sitio NVARCHAR(100) NOT NULL,
    Responsavel NVARCHAR(255) NULL,
    Especialidade VARCHAR(50) NULL,
    NivelAcessoMinimo INT NOT NULL DEFAULT 1,
    Status VARCHAR(20) DEFAULT 'ATIVO',
    DataCriacao DATETIME DEFAULT GETDATE(),
    DataAtualizacao DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE ProtocoloContencao (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Codigo VARCHAR(20) NOT NULL UNIQUE,
    Titulo NVARCHAR(255) NOT NULL,
    Descricao NVARCHAR(MAX) NOT NULL,
    NivelUrgencia INT NOT NULL CHECK (NivelUrgencia BETWEEN 1 AND 5),
    ClassesAplicaveis VARCHAR(100) NULL,
    Passos NVARCHAR(MAX) NOT NULL,
    RecursosNecessarios NVARCHAR(MAX) NULL,
    DataCriacao DATETIME DEFAULT GETDATE(),
    DataAtualizacao DATETIME DEFAULT GETDATE()
);
GO

-- N:N Protocolo ↔ Anomalia (quais protocolos se aplicam a quais anomalias)
CREATE TABLE Protocolo_AplicadoEm (
    ProtocoloId INT NOT NULL FOREIGN KEY REFERENCES ProtocoloContencao(Id) ON DELETE CASCADE,
    AnomaliaId INT NOT NULL FOREIGN KEY REFERENCES Anomalia(Id) ON DELETE CASCADE,
    DataInicio DATETIME NULL,
    DataFim DATETIME NULL,
    Status VARCHAR(20) DEFAULT 'ATIVO',
    Observacoes NVARCHAR(MAX) NULL,
    PRIMARY KEY (ProtocoloId, AnomaliaId)
);
GO

CREATE TABLE NotificacaoAnomalia (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Titulo NVARCHAR(255) NOT NULL,
    Descricao NVARCHAR(MAX) NOT NULL,
    LocalIdentificado NVARCHAR(255) NOT NULL,
    DataHora DATETIME DEFAULT GETDATE(),
    Status VARCHAR(20) DEFAULT 'PENDENTE',
    NivelPrioridade INT NOT NULL DEFAULT 3 CHECK (NivelPrioridade BETWEEN 1 AND 5),
    Relator NVARCHAR(255) NULL,
    AnomaliaId INT NULL FOREIGN KEY REFERENCES Anomalia(Id),
    DataResolucao DATETIME NULL
);
GO

CREATE INDEX IX_Protocolo_AplicadoEm_Anomalia ON Protocolo_AplicadoEm(AnomaliaId);
CREATE INDEX IX_NotificacaoAnomalia_Status ON NotificacaoAnomalia(Status);
CREATE INDEX IX_NotificacaoAnomalia_Data ON NotificacaoAnomalia(DataHora);
GO

-- =============================================
-- SEÇÃO 4: SISTEMA DE PERÍCIAS E DESVIOS
-- =============================================

CREATE TABLE PericiaAnomalia (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AnomaliaId INT NOT NULL FOREIGN KEY REFERENCES Anomalia(Id) ON DELETE CASCADE,
    Nome NVARCHAR(100) NOT NULL,
    Descricao NVARCHAR(MAX) NULL,
    MecanismoPrimarioId INT NOT NULL FOREIGN KEY REFERENCES Cat_MecanismoInteracao(Id),
    MecanismoSecundarioId INT NULL FOREIGN KEY REFERENCES Cat_MecanismoInteracao(Id),
    Nivel INT DEFAULT 1,
    Custo NVARCHAR(100) NULL,
    
    CONSTRAINT UQ_Pericia_Anomalia_Nome UNIQUE (AnomaliaId, Nome)
);
GO

CREATE TABLE Instancia_PericiaDesviante (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TipoInstancia VARCHAR(20) NOT NULL CHECK (TipoInstancia IN ('ENTIDADE', 'ARTEFATO', 'LOCALIDADE', 'EVENTO')),
    InstanciaId INT NOT NULL,
    PericiaDesvianteId INT NOT NULL FOREIGN KEY REFERENCES PericiaAnomalia(Id) ON DELETE CASCADE,
    DataDescoberta DATE DEFAULT GETDATE(),
    Intensidade VARCHAR(20) NULL,
    Observacoes NVARCHAR(MAX) NULL
);
GO

-- =============================================
-- SEÇÃO 5: TABELAS DE RELACIONAMENTO N:N
-- =============================================

CREATE TABLE Pericia_Manifestacao (
    PericiaAnomaliaId INT NOT NULL FOREIGN KEY REFERENCES PericiaAnomalia(Id) ON DELETE CASCADE,
    ManifestacaoEspecificaId INT NOT NULL FOREIGN KEY REFERENCES Cat_ManifestacaoEspecifica(Id),
    Intensidade VARCHAR(20) NULL,
    Observacoes NVARCHAR(MAX) NULL,
    
    PRIMARY KEY (PericiaAnomaliaId, ManifestacaoEspecificaId)
);
GO

CREATE TABLE Incidente (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AnomaliaId INT NOT NULL FOREIGN KEY REFERENCES Anomalia(Id),
    DataHora DATETIME DEFAULT GETDATE(),
    Tipo VARCHAR(50) NOT NULL,
    Titulo NVARCHAR(255) NOT NULL,
    Relatorio NVARCHAR(MAX) NOT NULL,
    NivelSeguranca VARCHAR(20) NOT NULL,
    IsEventoSigma BIT DEFAULT 0,
    Mortes INT DEFAULT 0,
    Feridos INT DEFAULT 0,
    DanoMaterial NVARCHAR(255) NULL
);
GO

-- =============================================
-- SEÇÃO 6: INSERTS INICIAIS (DADOS BASE)
-- =============================================

-- Forças Fundamentais
INSERT INTO Cat_ForcaFundamental (Simbolo, Nome, Descricao, ParticulaPortadora) VALUES
('Kappa', 'Kappa', 'Campo de Coerência Informacional. Força que permite a manifestação de narrativas e conceitos abstratos na realidade física.', 'Áxion'),
('Lambda', 'Lambda', 'Substrato Não-Bariônico Consciente. A "matéria escura" consciente que fundamenta as outras forças.', 'Não-bariônico');
GO

-- Camadas Ontológicas
INSERT INTO Cat_CamadaOntologica (Simbolo, Nome, Descricao, ForcaFundamentalId, Prioridade) VALUES
('THETA', 'THETA', 'Física Anômala. 95% dos casos. Acessa a "API" da realidade via spin coerente e deutério.', 
 (SELECT Id FROM Cat_ForcaFundamental WHERE Simbolo = 'Kappa'), 1),
('PSI', 'PSI', 'Narrativo/Informacional. Opera através de símbolos, histórias e crenças.',
 (SELECT Id FROM Cat_ForcaFundamental WHERE Simbolo = 'Kappa'), 1),
('PHI', 'PHI', 'Consciência/Digital. A mente diretamente interagindo com a realidade.',
 (SELECT Id FROM Cat_ForcaFundamental WHERE Simbolo = 'Kappa'), 1),
('OMEGA', 'OMEGA', 'Substrato Não-Bariônico. Acesso à "matéria escura" consciente.',
 (SELECT Id FROM Cat_ForcaFundamental WHERE Simbolo = 'Lambda'), 2);
GO

-- Tipos de Matéria
INSERT INTO Cat_TipoMateria (Nome, Descricao, IsResistenteSupressores) VALUES
('Bariônica Anômala', 'Átomos normais com propriedades anômalas. Pode ser suprimida com tecnologia Theta.', 0),
('Mista', 'Parte bariônica, parte não-bariônica. Comportamento imprevisível.', 0),
('Não-Bariônica', 'Não é feita de átomos. Sua forma é um "avatar" do substrato Σ.', 1),
('Indefinido', 'Não sabemos ainda. Geralmente em pesquisa ativa.', 0);
GO

-- Mecanismos de Interação
INSERT INTO Cat_MecanismoInteracao (Codigo, Nome, Descricao, CamadaOntologicaId, EhSubnatureza) VALUES
-- THETA
('THETA-A', 'Theta-Ativo', 'Usa ativamente a Força de Anomalia, alterando fisicamente a realidade (Consome deutério). Pirocinese, Crioscinese, fortificação exótica, telecinese são casos comuns', 1, 0),
('THETA-B', 'Theta-Passivo', 'Propriedade intrínseca. "Spin Congelado" na matéria. Materiais inorganicos, ligas anomalas ou fatores de coerencia concetrados em codificação genética são alguns casos', 1, 0),
('THETA-C', 'Theta-Condicional', 'Ativa sob condições específicas.', 1, 0),
-- PSI
('PSI-A', 'Psi-Ativo', 'Força uma narrativa ou conceito sobre a realidade.', 2, 0),
('PSI-B', 'Psi-Passivo', 'Propriedade intrínseca de um conceito ou informação.', 2, 0),
('PSI-C', 'Psi-Condicional', 'Ativa-se sob condições narrativas/informacionais.', 2, 1),
-- PHI
('PHI-A', 'Phi-Ativo', 'Uso ativo da consciência para interagir com a realidade.', 3, 0),
('PHI-B', 'Phi-Passivo', 'Estado consciente que produz efeitos constantes.', 3, 0),
('PHI-C', 'Phi-Condicional', 'Protocolo consciente ativado por gatilho informacional.', 3, 1),
-- OMEGA
('OMEGA-A', 'Omega-Ativo', 'Acesso direto ao substrato Σ para reescrever regras.', 4, 0),
('OMEGA-B', 'Omega-Passivo', 'Emana o substrato Σ passivamente.', 4, 1),
('OMEGA-C', 'Omega-Condicional', 'Acesso limitado a Σ através de gatilho.', 4, 1);
GO

-- Manifestações Específicas
INSERT INTO Cat_ManifestacaoEspecifica (Codigo, Nome, Descricao) VALUES
('METAMORFOSE', 'Metamorfose', 'Capacidade de alterar forma física'),
('REGENERACAO', 'Regeneração', 'Capacidade de regenerar tecidos danificados'),
('TELEPATIA', 'Telepatia', 'Comunicação direta mente-a-mente'),
('DISTORCAO_ST', 'Distorção Espaço-Temporal', 'Manipulação do espaço e tempo local'),
('IGN-01', 'Ignifagia', 'Criação e controle de chamas'),
('CRYO-05', 'Criogênese', 'Redução drástica de temperatura'),
('TELE-03', 'Telecinese', 'Movimento de objetos com a mente'),
('MEM-07', 'Manipulação de Memória', 'Alteração ou apagamento de memórias');
GO

-- =============================================
-- SEÇÃO 7: ÍNDICES PARA PERFORMANCE
-- =============================================

CREATE INDEX IX_EntidadeViva_AnomaliaId ON EntidadeViva(AnomaliaId);
CREATE INDEX IX_Artefato_AnomaliaId ON Artefato(AnomaliaId);
CREATE INDEX IX_Localidade_AnomaliaId ON Localidade(AnomaliaId);
CREATE INDEX IX_Evento_AnomaliaId ON Evento(AnomaliaId);

CREATE INDEX IX_Instancia_PericiaDesviante_Instancia ON Instancia_PericiaDesviante(TipoInstancia, InstanciaId);
CREATE INDEX IX_Instancia_PericiaDesviante_Pericia ON Instancia_PericiaDesviante(PericiaDesvianteId);

CREATE INDEX IX_Anomalia_CodigoSCP ON Anomalia(CodigoSCP);
CREATE INDEX IX_Anomalia_Status ON Anomalia(Status);
CREATE INDEX IX_Incidente_Anomalia ON Incidente(AnomaliaId);
GO

-- =============================================
-- SEÇÃO 8: TRIGGERS DE AUDITORIA E INTEGRIDADE
-- =============================================

-- Trigger para atualizar DataAtualizacao automaticamente
CREATE TRIGGER TR_Anomalia_Update_Date
ON Anomalia
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Anomalia
    SET DataAtualizacao = GETDATE(),
        UsuarioAtualizacao = SYSTEM_USER
    FROM Anomalia a
    INNER JOIN inserted i ON a.Id = i.Id;
END;
GO

-- Trigger para validar mecanismos secundários
CREATE TRIGGER TR_Anomalia_Validar_Mecanismos
ON Anomalia
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar se mecanismo secundário é realmente uma subnatureza
    IF EXISTS (
        SELECT 1 
        FROM inserted i
        INNER JOIN Cat_MecanismoInteracao sec ON i.MecanismoSecundarioId = sec.Id
        WHERE sec.EhSubnatureza = 0 AND i.MecanismoSecundarioId IS NOT NULL
    )
    BEGIN
        RAISERROR('Mecanismo secundário deve ter EhSubnatureza = 1', 16, 1);
        RETURN;
    END
    
    -- Verificar consistência de camadas
    IF EXISTS (
        SELECT 1 
        FROM inserted i
        INNER JOIN Cat_MecanismoInteracao prim ON i.MecanismoPrimarioId = prim.Id
        INNER JOIN Cat_MecanismoInteracao sec ON i.MecanismoSecundarioId = sec.Id
        INNER JOIN Cat_CamadaOntologica cp ON prim.CamadaOntologicaId = cp.Id
        INNER JOIN Cat_CamadaOntologica cs ON sec.CamadaOntologicaId = cs.Id
        WHERE cp.Simbolo = 'OMEGA' AND cs.Simbolo = 'THETA'
    )
    BEGIN
        RAISERROR('Anomalia OMEGA não pode ter subnatureza THETA', 16, 1);
        RETURN;
    END
END;
GO

-- =============================================
-- SEÇÃO 9: STORED PROCEDURES PARA CRUD COMPLETO
-- =============================================

-- Procedure para inserir uma nova anomalia
CREATE PROCEDURE sp_Anomalia_Inserir
    @CodigoSCP VARCHAR(50),
    @NomeComum NVARCHAR(255),
    @Descricao NVARCHAR(MAX),
    @ClasseObjetoId INT,
    @CamadaOntologicaId INT,
    @TipoMateriaId INT,
    @CognicaoAparenteId INT = NULL,
    @PericulosidadeId INT = NULL,
    @MecanismoPrimarioId INT,
    @MecanismoSecundarioId INT = NULL,
    @IEIA_D_Base DECIMAL(8,4) = NULL,
    @FatorCoerenciaSpin VARCHAR(20) = NULL,
    @SitioContencao NVARCHAR(100) = NULL,
    @ResponsavelPesquisa NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        
        INSERT INTO Anomalia (
            CodigoSCP, NomeComum, Descricao,
            ClasseObjetoId, CamadaOntologicaId, TipoMateriaId,
            CognicaoAparenteId, PericulosidadeId,
            MecanismoPrimarioId, MecanismoSecundarioId,
            IEIA_D_Base, FatorCoerenciaSpin,
            SitioContencao, ResponsavelPesquisa
        ) VALUES (
            @CodigoSCP, @NomeComum, @Descricao,
            @ClasseObjetoId, @CamadaOntologicaId, @TipoMateriaId,
            @CognicaoAparenteId, @PericulosidadeId,
            @MecanismoPrimarioId, @MecanismoSecundarioId,
            @IEIA_D_Base, @FatorCoerenciaSpin,
            @SitioContencao, @ResponsavelPesquisa
        );
        
        DECLARE @NovaAnomaliaId INT = SCOPE_IDENTITY();
        
        SELECT @NovaAnomaliaId as NovoId, @CodigoSCP as CodigoFormatado;
        
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- Procedure para atualizar uma anomalia
CREATE PROCEDURE sp_Anomalia_Atualizar
    @Id INT,
    @NomeComum NVARCHAR(255) = NULL,
    @Descricao NVARCHAR(MAX) = NULL,
    @ClasseObjetoId INT = NULL,
    @CamadaOntologicaId INT = NULL,
    @TipoMateriaId INT = NULL,
    @MecanismoPrimarioId INT = NULL,
    @MecanismoSecundarioId INT = NULL,
    @IEIA_D_Base DECIMAL(8,4) = NULL,
    @FatorCoerenciaSpin VARCHAR(20) = NULL,
    @Status VARCHAR(20) = NULL,
    @SitioContencao NVARCHAR(100) = NULL,
    @ResponsavelPesquisa NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        
        UPDATE Anomalia SET
            NomeComum = ISNULL(@NomeComum, NomeComum),
            Descricao = ISNULL(@Descricao, Descricao),
            ClasseObjetoId = ISNULL(@ClasseObjetoId, ClasseObjetoId),
            CamadaOntologicaId = ISNULL(@CamadaOntologicaId, CamadaOntologicaId),
            TipoMateriaId = ISNULL(@TipoMateriaId, TipoMateriaId),
            MecanismoPrimarioId = ISNULL(@MecanismoPrimarioId, MecanismoPrimarioId),
            MecanismoSecundarioId = @MecanismoSecundarioId,
            IEIA_D_Base = ISNULL(@IEIA_D_Base, IEIA_D_Base),
            FatorCoerenciaSpin = ISNULL(@FatorCoerenciaSpin, FatorCoerenciaSpin),
            Status = ISNULL(@Status, Status),
            SitioContencao = ISNULL(@SitioContencao, SitioContencao),
            ResponsavelPesquisa = ISNULL(@ResponsavelPesquisa, ResponsavelPesquisa)
        WHERE Id = @Id;
        
        IF @@ROWCOUNT = 0
            RAISERROR('Anomalia não encontrada', 16, 1);
        
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- Procedure para buscar anomalias com filtros complexos
CREATE PROCEDURE sp_Anomalia_Buscar
    @CodigoSCP VARCHAR(50) = NULL,
    @ClasseObjetoId INT = NULL,
    @CamadaOntologicaId INT = NULL,
    @TipoMateriaId INT = NULL,
    @MecanismoPrimarioId INT = NULL,
    @Status VARCHAR(20) = NULL,
    @ApenasSigma BIT = 0,
    @Pagina INT = 1,
    @ItensPorPagina INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@Pagina - 1) * @ItensPorPagina;
    
    SELECT 
        a.Id,
        a.CodigoSCP,
        a.NomeComum,
        a.Descricao,
        co.Nome as ClasseObjeto,
        ca.Nome as CamadaOntologica,
        tm.Nome as TipoMateria,
        mp.Nome as MecanismoPrimario,
        ms.Nome as MecanismoSecundario,
        a.IEIA_D_Base,
        a.FatorCoerenciaSpin,
        a.Status,
        a.SitioContencao,
        a.DataCriacao,
        a.DataAtualizacao,
        
        -- Manifestações como string agregada (via perícias)
        STUFF((
            SELECT DISTINCT ', ' + cm.Nome
            FROM PericiaAnomalia pa
            INNER JOIN Pericia_Manifestacao pm ON pa.Id = pm.PericiaAnomaliaId
            INNER JOIN Cat_ManifestacaoEspecifica cm ON pm.ManifestacaoEspecificaId = cm.Id
            WHERE pa.AnomaliaId = a.Id
            FOR XML PATH(''), TYPE
        ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') as Manifestacoes,
        
        -- Contador de incidentes
        (SELECT COUNT(*) FROM Incidente i WHERE i.AnomaliaId = a.Id) as TotalIncidentes,
        
        -- Contador de incidentes Sigma
        (SELECT COUNT(*) FROM Incidente i WHERE i.AnomaliaId = a.Id AND i.IsEventoSigma = 1) as IncidentesSigma,
        
        -- Contadores de instâncias
        (SELECT COUNT(*) FROM EntidadeViva ev WHERE ev.AnomaliaId = a.Id) as QtdEntidades,
        (SELECT COUNT(*) FROM Artefato ar WHERE ar.AnomaliaId = a.Id) as QtdArtefatos
        
    FROM Anomalia a
    INNER JOIN Cat_ClasseObjeto co ON a.ClasseObjetoId = co.Id
    INNER JOIN Cat_CamadaOntologica ca ON a.CamadaOntologicaId = ca.Id
    INNER JOIN Cat_TipoMateria tm ON a.TipoMateriaId = tm.Id
    INNER JOIN Cat_MecanismoInteracao mp ON a.MecanismoPrimarioId = mp.Id
    LEFT JOIN Cat_MecanismoInteracao ms ON a.MecanismoSecundarioId = ms.Id
    WHERE (@CodigoSCP IS NULL OR a.CodigoSCP LIKE '%' + @CodigoSCP + '%')
      AND (@ClasseObjetoId IS NULL OR a.ClasseObjetoId = @ClasseObjetoId)
      AND (@CamadaOntologicaId IS NULL OR a.CamadaOntologicaId = @CamadaOntologicaId)
      AND (@TipoMateriaId IS NULL OR a.TipoMateriaId = @TipoMateriaId)
      AND (@MecanismoPrimarioId IS NULL OR a.MecanismoPrimarioId = @MecanismoPrimarioId)
      AND (@Status IS NULL OR a.Status = @Status)
      AND (@ApenasSigma = 0 OR ca.Simbolo = 'OMEGA' OR tm.IsResistenteSupressores = 1)
    ORDER BY a.CodigoSCP
    OFFSET @Offset ROWS
    FETCH NEXT @ItensPorPagina ROWS ONLY;
    
    -- Retornar também o total de registros para paginação
    SELECT COUNT(*) as TotalRegistros
    FROM Anomalia a
    INNER JOIN Cat_CamadaOntologica ca ON a.CamadaOntologicaId = ca.Id
    INNER JOIN Cat_TipoMateria tm ON a.TipoMateriaId = tm.Id
    WHERE (@CodigoSCP IS NULL OR a.CodigoSCP LIKE '%' + @CodigoSCP + '%')
      AND (@ClasseObjetoId IS NULL OR a.ClasseObjetoId = @ClasseObjetoId)
      AND (@CamadaOntologicaId IS NULL OR a.CamadaOntologicaId = @CamadaOntologicaId)
      AND (@TipoMateriaId IS NULL OR a.TipoMateriaId = @TipoMateriaId)
      AND (@MecanismoPrimarioId IS NULL OR a.MecanismoPrimarioId = @MecanismoPrimarioId)
      AND (@Status IS NULL OR a.Status = @Status)
      AND (@ApenasSigma = 0 OR ca.Simbolo = 'OMEGA' OR tm.IsResistenteSupressores = 1);
END;
GO

-- Procedure para adicionar uma perícia a uma anomalia
CREATE PROCEDURE sp_Anomalia_AdicionarPericia
    @AnomaliaId INT,
    @Nome NVARCHAR(100),
    @Descricao NVARCHAR(MAX) = NULL,
    @MecanismoPrimarioId INT,
    @MecanismoSecundarioId INT = NULL,
    @Nivel INT = 1,
    @Custo NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Anomalia WHERE Id = @AnomaliaId)
            RAISERROR('Anomalia não encontrada', 16, 1);
            
        INSERT INTO PericiaAnomalia (
            AnomaliaId, Nome, Descricao, 
            MecanismoPrimarioId, MecanismoSecundarioId,
            Nivel, Custo
        ) VALUES (
            @AnomaliaId, @Nome, @Descricao,
            @MecanismoPrimarioId, @MecanismoSecundarioId,
            @Nivel, @Custo
        );
        
        DECLARE @NovaPericiaId INT = SCOPE_IDENTITY();
        
        -- Retornar de forma padronizada com outras procedures
        SELECT @NovaPericiaId as NovoId, @Nome as NomePericia;
        
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- Procedure para adicionar manifestação a uma perícia
CREATE PROCEDURE sp_Pericia_AdicionarManifestacao
    @PericiaAnomaliaId INT,
    @ManifestacaoEspecificaId INT,
    @Intensidade VARCHAR(20) = NULL,
    @Observacoes NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM PericiaAnomalia WHERE Id = @PericiaAnomaliaId)
            RAISERROR('Perícia não encontrada', 16, 1);
            
        IF NOT EXISTS (SELECT 1 FROM Cat_ManifestacaoEspecifica WHERE Id = @ManifestacaoEspecificaId)
            RAISERROR('Manifestação específica não encontrada', 16, 1);
        
        INSERT INTO Pericia_Manifestacao (PericiaAnomaliaId, ManifestacaoEspecificaId, Intensidade, Observacoes)
        VALUES (@PericiaAnomaliaId, @ManifestacaoEspecificaId, @Intensidade, @Observacoes);
        
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- Procedure para registrar um incidente
CREATE PROCEDURE sp_Incidente_Registrar
    @AnomaliaId INT,
    @Tipo VARCHAR(50),
    @Titulo NVARCHAR(255),
    @Relatorio NVARCHAR(MAX),
    @NivelSeguranca VARCHAR(20),
    @IsEventoSigma BIT = 0,
    @Mortes INT = 0,
    @Feridos INT = 0,
    @DanoMaterial NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Anomalia WHERE Id = @AnomaliaId)
            RAISERROR('Anomalia não encontrada', 16, 1);
        
        INSERT INTO Incidente (AnomaliaId, Tipo, Titulo, Relatorio, NivelSeguranca, IsEventoSigma, Mortes, Feridos, DanoMaterial)
        VALUES (@AnomaliaId, @Tipo, @Titulo, @Relatorio, @NivelSeguranca, @IsEventoSigma, @Mortes, @Feridos, @DanoMaterial);
        
        DECLARE @NovoIncidenteId INT = SCOPE_IDENTITY();
        
        SELECT @NovoIncidenteId as IncidenteId;
        
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- Procedure para inserir uma entidade viva
CREATE PROCEDURE sp_EntidadeViva_Inserir
    @AnomaliaId INT,
    @Identificacao NVARCHAR(100),
    @Especie NVARCHAR(150),
    @Biologia NVARCHAR(255) = NULL,
    @OrigemPoder NVARCHAR(100) = NULL,
    @DataNascimento DATE = NULL,
    @IsConsciente BIT = 1,
    @NivelInteligencia INT = NULL,
    @Dieta NVARCHAR(100) = NULL,
    @Observacoes NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Anomalia WHERE Id = @AnomaliaId)
            RAISERROR('Anomalia não encontrada', 16, 1);
            
        INSERT INTO EntidadeViva (
            AnomaliaId, Identificacao, Especie, Biologia, OrigemPoder,
            DataNascimento, IsConsciente, NivelInteligencia, Dieta, Observacoes
        ) VALUES (
            @AnomaliaId, @Identificacao, @Especie, @Biologia, @OrigemPoder,
            @DataNascimento, @IsConsciente, @NivelInteligencia, @Dieta, @Observacoes
        );
        
        SELECT SCOPE_IDENTITY() as NovaEntidadeId;
        
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- Procedure para inserir um artefato
CREATE PROCEDURE sp_Artefato_Inserir
    @AnomaliaId INT,
    @Identificacao NVARCHAR(100),
    @Material NVARCHAR(255) = NULL,
    @DataFabricacao DATE = NULL,
    @LocalOrigem NVARCHAR(255) = NULL,
    @PropriedadeSpin VARCHAR(100) = NULL,
    @Peso_Kg DECIMAL(10,2) = NULL,
    @Dimensoes VARCHAR(100) = NULL,
    @ModoUsar NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Anomalia WHERE Id = @AnomaliaId)
            RAISERROR('Anomalia não encontrada', 16, 1);
            
        INSERT INTO Artefato (
            AnomaliaId, Identificacao, Material, DataFabricacao, LocalOrigem,
            PropriedadeSpin, Peso_Kg, Dimensoes, ModoUsar
        ) VALUES (
            @AnomaliaId, @Identificacao, @Material, @DataFabricacao, @LocalOrigem,
            @PropriedadeSpin, @Peso_Kg, @Dimensoes, @ModoUsar
        );
        
        SELECT SCOPE_IDENTITY() as NovoArtefatoId;
        
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO



-- Procedure para obter o perfil completo de uma anomalia
CREATE PROCEDURE sp_Anomalia_ObterPerfilCompleto
    @AnomaliaId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Informações básicas da anomalia
    SELECT 
        a.Id,
        a.CodigoSCP,
        a.NomeComum,
        a.Descricao,
        co.Nome as ClasseObjeto,
        co.CorAlerta,
        ca.Nome as CamadaOntologica,
        ff.Nome as ForcaFundamental,
        tm.Nome as TipoMateria,
        mp.Nome as MecanismoPrimario,
        ms.Nome as MecanismoSecundario,
        a.IEIA_D_Base,
        a.FatorCoerenciaSpin,
        a.Status,
        a.SitioContencao,
        a.ResponsavelPesquisa,
        a.DataCriacao,
        a.DataAtualizacao
    FROM Anomalia a
    INNER JOIN Cat_ClasseObjeto co ON a.ClasseObjetoId = co.Id
    INNER JOIN Cat_CamadaOntologica ca ON a.CamadaOntologicaId = ca.Id
    INNER JOIN Cat_ForcaFundamental ff ON ca.ForcaFundamentalId = ff.Id
    INNER JOIN Cat_TipoMateria tm ON a.TipoMateriaId = tm.Id
    INNER JOIN Cat_MecanismoInteracao mp ON a.MecanismoPrimarioId = mp.Id
    LEFT JOIN Cat_MecanismoInteracao ms ON a.MecanismoSecundarioId = ms.Id
    WHERE a.Id = @AnomaliaId;
    
    -- Entidades vivas associadas
    SELECT * FROM EntidadeViva WHERE AnomaliaId = @AnomaliaId;
    
    -- Artefatos associados
    SELECT * FROM Artefato WHERE AnomaliaId = @AnomaliaId;
    
    -- Localidades associadas
    SELECT * FROM Localidade WHERE AnomaliaId = @AnomaliaId;
    
    -- Eventos associados
    SELECT * FROM Evento WHERE AnomaliaId = @AnomaliaId;
    
    -- Perícias da anomalia
    SELECT 
        pa.*,
        mp.Nome as MecanismoPrimarioNome,
        ms.Nome as MecanismoSecundarioNome
    FROM PericiaAnomalia pa
    INNER JOIN Cat_MecanismoInteracao mp ON pa.MecanismoPrimarioId = mp.Id
    LEFT JOIN Cat_MecanismoInteracao ms ON pa.MecanismoSecundarioId = ms.Id
    WHERE pa.AnomaliaId = @AnomaliaId;
    
    -- Manifestações (via perícias)
    SELECT DISTINCT
        cm.Codigo,
        cm.Nome,
        cm.Descricao
    FROM PericiaAnomalia pa
    INNER JOIN Pericia_Manifestacao pm ON pa.Id = pm.PericiaAnomaliaId
    INNER JOIN Cat_ManifestacaoEspecifica cm ON pm.ManifestacaoEspecificaId = cm.Id
    WHERE pa.AnomaliaId = @AnomaliaId;
    
    -- Incidentes registrados
    SELECT * FROM Incidente 
    WHERE AnomaliaId = @AnomaliaId 
    ORDER BY DataHora DESC;
END;
GO

-- =============================================
-- SEÇÃO 9B: STORED PROCEDURES PARA NOVAS ENTIDADES
-- =============================================

-- Procedure para criar laboratório
CREATE PROCEDURE sp_Laboratorio_Inserir
    @Codigo VARCHAR(20),
    @Nome NVARCHAR(255),
    @Descricao NVARCHAR(MAX) = NULL,
    @Sitio NVARCHAR(100),
    @Responsavel NVARCHAR(255) = NULL,
    @Especialidade VARCHAR(50) = NULL,
    @NivelAcessoMinimo INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Laboratorio (Codigo, Nome, Descricao, Sitio, Responsavel, Especialidade, NivelAcessoMinimo)
    VALUES (@Codigo, @Nome, @Descricao, @Sitio, @Responsavel, @Especialidade, @NivelAcessoMinimo);
    SELECT SCOPE_IDENTITY() as NovoId;
END;
GO

-- Procedure para criar protocolo de contenção
CREATE PROCEDURE sp_ProtocoloContencao_Inserir
    @Codigo VARCHAR(20),
    @Titulo NVARCHAR(255),
    @Descricao NVARCHAR(MAX),
    @NivelUrgencia INT,
    @ClassesAplicaveis VARCHAR(100) = NULL,
    @Passos NVARCHAR(MAX),
    @RecursosNecessarios NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO ProtocoloContencao (Codigo, Titulo, Descricao, NivelUrgencia, ClassesAplicaveis, Passos, RecursosNecessarios)
    VALUES (@Codigo, @Titulo, @Descricao, @NivelUrgencia, @ClassesAplicaveis, @Passos, @RecursosNecessarios);
    SELECT SCOPE_IDENTITY() as NovoId;
END;
GO

-- Procedure para vincular protocolo a anomalia
CREATE PROCEDURE sp_Protocolo_AplicarEmAnomalia
    @ProtocoloId INT,
    @AnomaliaId INT,
    @Observacoes NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM ProtocoloContencao WHERE Id = @ProtocoloId)
        RAISERROR('Protocolo não encontrado', 16, 1);
    IF NOT EXISTS (SELECT 1 FROM Anomalia WHERE Id = @AnomaliaId)
        RAISERROR('Anomalia não encontrada', 16, 1);
    INSERT INTO Protocolo_AplicadoEm (ProtocoloId, AnomaliaId, DataInicio, Status, Observacoes)
    VALUES (@ProtocoloId, @AnomaliaId, GETDATE(), 'ATIVO', @Observacoes);
END;
GO

-- Procedure para registrar notificação de anomalia
CREATE PROCEDURE sp_NotificacaoAnomalia_Inserir
    @Titulo NVARCHAR(255),
    @Descricao NVARCHAR(MAX),
    @LocalIdentificado NVARCHAR(255),
    @NivelPrioridade INT = 3,
    @Relator NVARCHAR(255) = NULL,
    @AnomaliaId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO NotificacaoAnomalia (Titulo, Descricao, LocalIdentificado, NivelPrioridade, Relator, AnomaliaId)
    VALUES (@Titulo, @Descricao, @LocalIdentificado, @NivelPrioridade, @Relator, @AnomaliaId);
    SELECT SCOPE_IDENTITY() as NotificacaoId;
END;
GO

-- =============================================
-- SEÇÃO 10: VIEWS PARA DASHBOARD
-- =============================================

CREATE VIEW vw_Dashboard_Anomalias AS
SELECT 
    a.Id,
    a.CodigoSCP,
    a.NomeComum,
    co.Nome as ClasseObjeto,
    co.CorAlerta,
    ca.Simbolo as Camada,
    ca.Nome as CamadaOntologica,
    tm.Nome as TipoMateria,
    mp.Nome as MecanismoPrimario,
    ms.Nome as MecanismoSecundario,
    a.IEIA_D_Base,
    a.FatorCoerenciaSpin,
    a.Status,
    a.SitioContencao,
    
    -- Indicadores de risco
    CASE 
        WHEN co.Codigo = 'KETER' THEN 3
        WHEN co.Codigo = 'EUCLID' THEN 2
        WHEN co.Codigo = 'SAFE' THEN 1
        ELSE 0
    END as NivelRisco,
    
    CASE 
        WHEN ca.Simbolo = 'OMEGA' OR tm.IsResistenteSupressores = 1 THEN 'SIGMA-ALERTA'
        WHEN ca.Simbolo = 'THETA' AND a.IEIA_D_Base > 0.5 THEN 'THETA-ALTO'
        WHEN ca.Simbolo = 'THETA' AND a.IEIA_D_Base > 0.1 THEN 'THETA-MEDIO'
        ELSE 'THETA-BAIXO'
    END as StatusTheta,
    
    -- Contadores
    (SELECT COUNT(*) FROM Incidente i WHERE i.AnomaliaId = a.Id AND i.IsEventoSigma = 1) as ContagemSigma,
    (SELECT COUNT(*) FROM EntidadeViva ev WHERE ev.AnomaliaId = a.Id) as QtdEntidades,
    (SELECT COUNT(*) FROM Artefato ar WHERE ar.AnomaliaId = a.Id) as QtdArtefatos
    
FROM Anomalia a
INNER JOIN Cat_ClasseObjeto co ON a.ClasseObjetoId = co.Id
INNER JOIN Cat_CamadaOntologica ca ON a.CamadaOntologicaId = ca.Id
INNER JOIN Cat_TipoMateria tm ON a.TipoMateriaId = tm.Id
INNER JOIN Cat_MecanismoInteracao mp ON a.MecanismoPrimarioId = mp.Id
LEFT JOIN Cat_MecanismoInteracao ms ON a.MecanismoSecundarioId = ms.Id
WHERE a.Status = 'ATIVA';
GO

CREATE VIEW vw_Relatorio_Sigma AS
SELECT 
    a.CodigoSCP,
    a.NomeComum,
    co.Nome as Classe,
    ca.Nome as Camada,
    tm.IsResistenteSupressores,
    COUNT(i.Id) as TotalIncidentes,
    SUM(CASE WHEN i.IsEventoSigma = 1 THEN 1 ELSE 0 END) as IncidentesSigma,
    MAX(i.DataHora) as UltimoIncidente,
    a.SitioContencao,
    a.ResponsavelPesquisa
FROM Anomalia a
INNER JOIN Cat_ClasseObjeto co ON a.ClasseObjetoId = co.Id
INNER JOIN Cat_CamadaOntologica ca ON a.CamadaOntologicaId = ca.Id
INNER JOIN Cat_TipoMateria tm ON a.TipoMateriaId = tm.Id
LEFT JOIN Incidente i ON a.Id = i.AnomaliaId
WHERE ca.Simbolo = 'OMEGA' 
   OR tm.IsResistenteSupressores = 1
   OR co.Codigo = 'APOT'
GROUP BY a.Id, a.CodigoSCP, a.NomeComum, co.Nome, ca.Nome, 
         tm.IsResistenteSupressores, a.SitioContencao, a.ResponsavelPesquisa;
GO

CREATE VIEW vw_Estatisticas_Anomalias AS
SELECT 
    -- Totais
    COUNT(*) as TotalAnomalias,
    SUM(CASE WHEN Status = 'ATIVA' THEN 1 ELSE 0 END) as Ativas,
    SUM(CASE WHEN Status = 'NEUTRALIZADA' THEN 1 ELSE 0 END) as Neutralizadas,
    
    -- Por Classe
    SUM(CASE WHEN co.Codigo = 'SAFE' THEN 1 ELSE 0 END) as Classe_Safe,
    SUM(CASE WHEN co.Codigo = 'EUCLID' THEN 1 ELSE 0 END) as Classe_Euclid,
    SUM(CASE WHEN co.Codigo = 'KETER' THEN 1 ELSE 0 END) as Classe_Keter,
    SUM(CASE WHEN co.Codigo = 'THAUMIEL' THEN 1 ELSE 0 END) as Classe_Thaumiel,
    SUM(CASE WHEN co.Codigo = 'APOT' THEN 1 ELSE 0 END) as Classe_Apotheosis,
    
    -- Por Camada
    SUM(CASE WHEN ca.Simbolo = 'THETA' THEN 1 ELSE 0 END) as Camada_Theta,
    SUM(CASE WHEN ca.Simbolo = 'PSI' THEN 1 ELSE 0 END) as Camada_Psi,
    SUM(CASE WHEN ca.Simbolo = 'PHI' THEN 1 ELSE 0 END) as Camada_Phi,
    SUM(CASE WHEN ca.Simbolo = 'OMEGA' THEN 1 ELSE 0 END) as Camada_Omega,
    
    -- Por Tipo de Matéria
    SUM(CASE WHEN tm.Nome = 'Bariônica Anômala' THEN 1 ELSE 0 END) as Materia_Barionica,
    SUM(CASE WHEN tm.Nome = 'Não-Bariônica' THEN 1 ELSE 0 END) as Materia_NaoBarionica,
    SUM(CASE WHEN tm.Nome = 'Mista' THEN 1 ELSE 0 END) as Materia_Mista,
    
    -- Estatísticas Theta
    AVG(ISNULL(a.IEIA_D_Base, 0)) as IEIA_D_Medio,
    SUM(CASE WHEN tm.IsResistenteSupressores = 1 THEN 1 ELSE 0 END) as ResistenteSupressao
    
FROM Anomalia a
INNER JOIN Cat_ClasseObjeto co ON a.ClasseObjetoId = co.Id
INNER JOIN Cat_CamadaOntologica ca ON a.CamadaOntologicaId = ca.Id
INNER JOIN Cat_TipoMateria tm ON a.TipoMateriaId = tm.Id;
GO

-- =============================================
-- SEÇÃO 11: INSERÇÃO DE DADOS DE EXEMPLO
-- =============================================

-- =============================================
-- SEÇÃO 11A: DADOS DE EXEMPLO — LABORATÓRIOS
-- =============================================
EXEC sp_Laboratorio_Inserir @Codigo='LAB-BIO-19', @Nome='Setor de Biologia Anômala', @Sitio='Sítio-19', @Responsavel='Dra. Elara Vance', @Especialidade='Biologia Anômala';
EXEC sp_Laboratorio_Inserir @Codigo='LAB-SPIN-64', @Nome='Laboratório de Spin Coerente', @Sitio='Sítio-64', @Responsavel='Dr. Aris Thoth', @Especialidade='Física Quântica';
EXEC sp_Laboratorio_Inserir @Codigo='LAB-PSI-07', @Nome='Câmara de Ressonância Psi', @Sitio='Sítio-07', @Responsavel='Dr. Marcus Bell', @Especialidade='Narratologia';
GO

-- =============================================
-- SEÇÃO 11B: DADOS DE EXEMPLO — PROTOCOLOS
-- =============================================
INSERT INTO ProtocoloContencao (Codigo, Titulo, Descricao, NivelUrgencia, ClassesAplicaveis, Passos, RecursosNecessarios) VALUES
('PROT-BIO-STD', 'Protocolo Padrão de Conteção Biológica', 'Procedimentos padrão para entidades biológicas anômalas de baixo risco.', 2, 'PACATO,YAGUARA',
 '1. Isolar o perímetro de 50m.\n2. Equipe de contenção nível 2.\n3. Conter com rede de spin bloqueador.\n4. Transporte em container climatizado.\n5. Avaliação psiquiátrica pós-contenção.',
 'Rede de spin bloqueador, container classe II, tranqüilizante B-47'),
('PROT-OMEGA-CRIT', 'Protocolo Sigma — Contenção de Emergência OMEGA', 'Procedimento de contenção para anomalias de camada OMEGA em estado crítico.', 5, 'UKAR,ABAPORU',
 '1. Evacuar raio de 5km.\n2. Acionar Equipe Theta-9.\n3. Ativar geradores de campo Kappa.\n4. Estabelecer perímetro de segurança nível 5.\n5. Contato com Conselho O5 imediato.',
 'Gerador de campo Kappa, equipe Theta-9, autorização nível 5'),
('PROT-PSI-PESQ', 'Protocolo de Pesquisa Narrativa', 'Procedimentos para interação com anomalias de causalidade narrativa.', 3, 'YAGUARA,ABAPORU',
 '1. Estabelecer barreira informacional.\n2. Apenas pesquisador designado pode interagir.\n3. Registrar toda interação em diário ontológico.\n4. Nunca revelar o nome verdadeiro da anomalia.\n5. Sessões limitadas a 30 min.',
 'Diário ontológico, gravador de campo Psi, bloqueador de memória');
GO

-- =============================================
-- SEÇÃO 11C: DADOS DE EXEMPLO — NOTIFICAÇÕES
-- =============================================
EXEC sp_NotificacaoAnomalia_Inserir @Titulo='Flutuação Theta no Sítio-19', @Descricao='Sensor IEIA-D detectou flutuação acima de 0.3% no perímetro oeste. Possível nova anomalia não catalogada.', @LocalIdentificado='Sítio-19, Perímetro Oeste', @NivelPrioridade=3, @Relator='Sistema Automático';
EXEC sp_NotificacaoAnomalia_Inserir @Titulo='Evento Psi não identificado', @Descricao='Relato de sonhos compartilhados entre 12 funcionários do Sítio-64. Possível entidade onírica em formação.', @LocalIdentificado='Sítio-64, Alojamento Funcionários', @NivelPrioridade=4, @Relator='Dr. Aris Thoth';
GO

-- Exemplo 1: Metamorfo Complexo
DECLARE @MetamorfoId INT;
DECLARE @PericiaMetamorfoseId INT;
DECLARE @ClasseObjetoId_EUCLID INT, @CamadaOntologicaId_THETA INT, @TipoMateriaId_BARIONICA INT;
DECLARE @CognicaoAparenteId_SA INT, @PericulosidadeId_MEDIO INT;
DECLARE @MecanismoPrimarioId_THETA_C INT, @MecanismoSecundarioId_PSI_C INT;
DECLARE @ManifestacaoId_METAMORFOSE INT, @ManifestacaoId_REGENERACAO INT;

-- Obter os IDs necessários
SET @ClasseObjetoId_EUCLID = (SELECT Id FROM Cat_ClasseObjeto WHERE ClasseACS = 'EUCLID');
SET @CamadaOntologicaId_THETA = (SELECT Id FROM Cat_CamadaOntologica WHERE Simbolo = 'THETA');
SET @TipoMateriaId_BARIONICA = (SELECT Id FROM Cat_TipoMateria WHERE Nome = 'Bariônica Anômala');
SET @CognicaoAparenteId_SA = (SELECT Id FROM Cat_CognicaoAparente WHERE Codigo = 'SA');
SET @PericulosidadeId_MEDIO = (SELECT Id FROM Cat_Periculosidade WHERE Nivel = 5);
SET @MecanismoPrimarioId_THETA_C = (SELECT Id FROM Cat_MecanismoInteracao WHERE Codigo = 'THETA-C');
SET @MecanismoSecundarioId_PSI_C = (SELECT Id FROM Cat_MecanismoInteracao WHERE Codigo = 'PSI-C');
SET @ManifestacaoId_METAMORFOSE = (SELECT Id FROM Cat_ManifestacaoEspecifica WHERE Codigo = 'METAMORFOSE');
SET @ManifestacaoId_REGENERACAO = (SELECT Id FROM Cat_ManifestacaoEspecifica WHERE Codigo = 'REGENERACAO');

-- Criar tabela temporária para capturar o resultado
DECLARE @Resultado TABLE (NovoId INT, CodigoFormatado VARCHAR(50));

-- Inserir a anomalia
INSERT INTO @Resultado
EXEC sp_Anomalia_Inserir 
    @CodigoSCP = 'SCP-1001',
    @NomeComum = 'Proteu - O Metamorfo Complexo',
    @Descricao = 'Entidade humanoide capaz de se transformar em múltiplas formas animais. Massa varia até +/- 60% da forma base. Formas são anatomicamente perfeitas.',
    @ClasseObjetoId = @ClasseObjetoId_EUCLID,
    @CamadaOntologicaId = @CamadaOntologicaId_THETA,
    @TipoMateriaId = @TipoMateriaId_BARIONICA,
    @CognicaoAparenteId = @CognicaoAparenteId_SA,
    @PericulosidadeId = @PericulosidadeId_MEDIO,
    @MecanismoPrimarioId = @MecanismoPrimarioId_THETA_C,
    @MecanismoSecundarioId = @MecanismoSecundarioId_PSI_C,
    @IEIA_D_Base = 0.5,
    @FatorCoerenciaSpin = 'Alto',
    @SitioContencao = 'Sítio-19, Setor de Biologia Anômala',
    @ResponsavelPesquisa = 'Dra. Elara Vance';

-- Obter o ID da anomalia criada
SELECT @MetamorfoId = NovoId FROM @Resultado;

-- Limpar a tabela temporária
DELETE FROM @Resultado;

-- Adicionar como Entidade Viva
EXEC sp_EntidadeViva_Inserir 
    @AnomaliaId = @MetamorfoId,
    @Identificacao = 'Indivíduo-Alpha',
    @Especie = 'Homo sapiens metamorfo',
    @Biologia = 'Bariônica Modificada',
    @OrigemPoder = 'Catalisador + Acesso PSI',
    @NivelInteligencia = 8;

-- Adicionar perícia e manifestações
INSERT INTO @Resultado
EXEC sp_Anomalia_AdicionarPericia
    @AnomaliaId = @MetamorfoId,
    @Nome = 'Metamorfose Complexa',
    @Descricao = 'Capacidade de transformação em múltiplas formas animais',
    @MecanismoPrimarioId = @MecanismoPrimarioId_THETA_C,
    @MecanismoSecundarioId = @MecanismoSecundarioId_PSI_C,
    @Nivel = 3,
	@Custo = 'Nenhum'

-- Obter o ID da Pericia de Anomalia criada
SELECT @PericiaMetamorfoseId = NovoId FROM @Resultado;

-- Limpar a tabela temporária
DELETE FROM @Resultado;

EXEC sp_Pericia_AdicionarManifestacao
    @PericiaAnomaliaId = @PericiaMetamorfoseId,
    @ManifestacaoEspecificaId = @ManifestacaoId_METAMORFOSE,
    @Intensidade = 'Alta';

EXEC sp_Pericia_AdicionarManifestacao
    @PericiaAnomaliaId = @PericiaMetamorfoseId,
    @ManifestacaoEspecificaId = @ManifestacaoId_REGENERACAO,
    @Intensidade = 'Média';

-- Registrar um incidente
EXEC sp_Incidente_Registrar
    @AnomaliaId = @MetamorfoId,
    @Tipo = 'Teste de Pesquisa',
    @Titulo = 'Teste de Limites de Transformação',
    @Relatorio = 'Sujeito transformou-se sequencialmente em lobo, urso e corvo dentro de 5 minutos.',
    @NivelSeguranca = 'Alto',
    @IsEventoSigma = 0,
    @Feridos = 0;



-- Exemplo 2: Grimório de Spin Congelado
DECLARE @GrimorioId INT;
DECLARE @PericiaGrimorioId INT;
DECLARE @ClasseObjetoId_KETER INT, @CamadaOntologicaId_OMEGA INT, @TipoMateriaId_MISTA INT;
DECLARE @CognicaoAparenteId_IN INT, @PericulosidadeId_ALTO INT;
DECLARE @MecanismoPrimarioId_OMEGA_A INT, @MecanismoSecundarioId_THETA_C INT;

SET @ClasseObjetoId_KETER = (SELECT Id FROM Cat_ClasseObjeto WHERE ClasseACS = 'KETER');
SET @CamadaOntologicaId_OMEGA = (SELECT Id FROM Cat_CamadaOntologica WHERE Simbolo = 'OMEGA');
SET @TipoMateriaId_MISTA = (SELECT Id FROM Cat_TipoMateria WHERE Nome = 'Mista');
SET @CognicaoAparenteId_IN = (SELECT Id FROM Cat_CognicaoAparente WHERE Codigo = 'IN');
SET @PericulosidadeId_ALTO = (SELECT Id FROM Cat_Periculosidade WHERE Nivel = 6);
SET @MecanismoPrimarioId_OMEGA_A = (SELECT Id FROM Cat_MecanismoInteracao WHERE Codigo = 'THETA-A');
SET @MecanismoSecundarioId_THETA_C = (SELECT Id FROM Cat_MecanismoInteracao WHERE Codigo = 'OMEGA-C');


-- Inserir a anomalia
INSERT INTO @Resultado
EXEC sp_Anomalia_Inserir 
    @CodigoSCP = 'SCP-1002',
    @NomeComum = 'Codex de Realidades - Grimório SIGMA',
    @Descricao = 'Tomo antigo com padrões de spin coerente "congelados" no pergaminho.',
    @ClasseObjetoId = @ClasseObjetoId_KETER,
    @CamadaOntologicaId = @CamadaOntologicaId_OMEGA,
    @TipoMateriaId = @TipoMateriaId_MISTA,
    @CognicaoAparenteId = @CognicaoAparenteId_IN,
    @PericulosidadeId = @PericulosidadeId_ALTO,
    @MecanismoPrimarioId = @MecanismoPrimarioId_OMEGA_A,
    @MecanismoSecundarioId = @MecanismoSecundarioId_THETA_C,
    @IEIA_D_Base = 0.05,
    @FatorCoerenciaSpin = 'Crítico',
    @SitioContencao = 'Sítio-64, Biblioteca Proibida',
    @ResponsavelPesquisa = 'Dr. Aris Thoth';

-- Obter o ID da anomalia criada
SELECT @GrimorioId = NovoId FROM @Resultado;


-- Limpar a tabela temporária
DELETE FROM @Resultado;

-- Adicionar como Artefato
EXEC sp_Artefato_Inserir
    @AnomaliaId = @GrimorioId,
    @Identificacao = 'Codex-Primus',
    @Material = 'Pergaminho/Pele Anômala',
    @PropriedadeSpin = 'Spin Congelado com acoplamento OMEGA',
    @Peso_Kg = 3.5,
    @ModoUsar = 'Ritual de ativação requer pronúncia precisa e gestos específicos.';

-- Adicionar perícia para o artefato

INSERT INTO @Resultado
EXEC sp_Anomalia_AdicionarPericia
    @AnomaliaId = @GrimorioId,
    @Nome = 'Manipulação da Realidade',
    @Descricao = 'Capacidade de alterar regras locais da realidade através de padrões de spin',
    @MecanismoPrimarioId = @MecanismoPrimarioId_OMEGA_A,
    @MecanismoSecundarioId = @MecanismoSecundarioId_THETA_C,
    @Nivel = 5,
    @Custo = 'Deutério e ritual específico';

-- Obter o ID da anomalia criada
SELECT @PericiaGrimorioId = NovoId FROM @Resultado;


-- Limpar a tabela temporária
DELETE FROM @Resultado;

-- Adicionar manifestações à perícia do grimório
DECLARE @ManifestacaoId_DISTORCAO_ST INT, @ManifestacaoId_TELEPATIA INT;

SET @ManifestacaoId_DISTORCAO_ST = (SELECT Id FROM Cat_ManifestacaoEspecifica WHERE Codigo = 'DISTORCAO_ST');
SET @ManifestacaoId_TELEPATIA = (SELECT Id FROM Cat_ManifestacaoEspecifica WHERE Codigo = 'TELEPATIA');

EXEC sp_Pericia_AdicionarManifestacao
    @PericiaAnomaliaId = @PericiaGrimorioId,
    @ManifestacaoEspecificaId = @ManifestacaoId_DISTORCAO_ST,
    @Intensidade = 'Variável';

EXEC sp_Pericia_AdicionarManifestacao
    @PericiaAnomaliaId = @PericiaGrimorioId,
    @ManifestacaoEspecificaId = @ManifestacaoId_TELEPATIA,
    @Intensidade = 'Média';

-- Registrar incidente Sigma
EXEC sp_Incidente_Registrar
    @AnomaliaId = @GrimorioId,
    @Tipo = 'Evento SIGMA',
    @Titulo = 'Manifestação Não-Autorizada',
    @Relatorio = 'D-Class não-treinado tentou ler o Codex. Padrão de spin foi ativado, criando uma zona de realidade instável.',
    @NivelSeguranca = 'Nível 4',
    @IsEventoSigma = 1,
    @Mortes = 1,
    @DanoMaterial = 'Sala de teste completamente desestruturada';

PRINT '✅ Sistema de catalogação completo instalado com sucesso!';
PRINT 'Estrutura:';
PRINT '- 7 tabelas de catálogo (ClasseObjeto, ForcaFundamental, CamadaOntologica, TipoMateria, MecanismoInteracao, ManifestacaoEspecifica, CognicaoAparente, Periculosidade)';
PRINT '- 1 tabela principal (Anomalia)';
PRINT '- 4 subtabelas 1:N (EntidadeViva, Artefato, Localidade, Evento)';
PRINT '- 2 tabelas de perícias (PericiaAnomalia, Instancia_PericiaDesviante)';
PRINT '- 2 tabelas de relacionamento N:N (Pericia_Manifestacao, Protocolo_AplicadoEm)';
PRINT '- 1 tabela de histórico (Incidente)';
PRINT '- 3 novas entidades (Laboratorio, ProtocoloContencao, NotificacaoAnomalia)';
PRINT '- 2 triggers de integridade';
PRINT '- 12 stored procedures de CRUD';
PRINT '- 3 views para dashboard';
PRINT '- 2 exemplos de anomalias inseridas';
PRINT '- 3 laboratórios, 3 protocolos, 2 notificações';
GO

