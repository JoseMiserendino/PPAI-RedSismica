
-- Red Sismica - Esquema mínimo para Registrar Resultados Manuales (SQL Server)

/* =========================
   Limpieza (DROP en orden de FK)
   ========================= */
IF OBJECT_ID('dbo.Muestra','U') IS NOT NULL DROP TABLE dbo.Muestra;
IF OBJECT_ID('dbo.SerieTemporal','U') IS NOT NULL DROP TABLE dbo.SerieTemporal;
IF OBJECT_ID('dbo.EventoEstadoHistorial','U') IS NOT NULL DROP TABLE dbo.EventoEstadoHistorial;
IF OBJECT_ID('dbo.EventoSismico','U') IS NOT NULL DROP TABLE dbo.EventoSismico;
IF OBJECT_ID('dbo.Empleado','U') IS NOT NULL DROP TABLE dbo.Empleado;
IF OBJECT_ID('dbo.Estado','U') IS NOT NULL DROP TABLE dbo.Estado;
IF OBJECT_ID('dbo.ClasificacionSismo','U') IS NOT NULL DROP TABLE dbo.ClasificacionSismo;
IF OBJECT_ID('dbo.OrigenGeneracion','U') IS NOT NULL DROP TABLE dbo.OrigenGeneracion;
IF OBJECT_ID('dbo.AlcanceSismo','U') IS NOT NULL DROP TABLE dbo.AlcanceSismo;
IF OBJECT_ID('dbo.TipoDatoSerie','U') IS NOT NULL DROP TABLE dbo.TipoDatoSerie;

GO

/* =========================
   Catálogos
   ========================= */
CREATE TABLE dbo.Estado(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,      -- Ej: No Revisado, Bloqueado, Revisado, Publicado, Anulado
    EsFinal BIT NOT NULL DEFAULT(0)
);

CREATE TABLE dbo.ClasificacionSismo(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE       -- Ej: Sismo, Evento Antropogénico, Ruido, Microtemblor
);

CREATE TABLE dbo.OrigenGeneracion(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE       -- Ej: Natural, Inducido, Explosión, Otro
);

CREATE TABLE dbo.AlcanceSismo(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE       -- Ej: Local, Regional, Lejano
);

CREATE TABLE dbo.TipoDatoSerie(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,      -- Ej: Aceleración, Velocidad, Desplazamiento
    Unidad NVARCHAR(20) NOT NULL              -- Ej: m/s^2, mm/s, nm
);

GO

/* =========================
   Núcleo
   ========================= */
CREATE TABLE dbo.Empleado(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Legajo NVARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(120) NOT NULL,
    Email NVARCHAR(256) NULL,
    Activo BIT NOT NULL DEFAULT(1)
);

CREATE TABLE dbo.EventoSismico(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(30) NOT NULL UNIQUE,        -- Código propio o de red
    FechaHoraUtc DATETIME2(3) NOT NULL,
    Latitud DECIMAL(9,6) NULL,
    Longitud DECIMAL(9,6) NULL,
    ProfundidadKm DECIMAL(6,2) NULL,
    AutoDetectado BIT NOT NULL DEFAULT(1),
    EstadoActualId INT NOT NULL,
    EmpleadoBloqueoId INT NULL,
    FechaHoraBloqueo DATETIME2(3) NULL,
    -- Resultados manuales
    ClasificacionId INT NULL,
    OrigenId INT NULL,
    AlcanceId INT NULL,
    MagnitudValor DECIMAL(4,2) NULL,            -- Ej: 4.5
    MagnitudEscala NVARCHAR(20) NULL,           -- Ej: ML, Mw, Mb
    Observaciones NVARCHAR(MAX) NULL,

    CONSTRAINT FK_Evento_EstadoActual FOREIGN KEY(EstadoActualId) REFERENCES dbo.Estado(Id),
    CONSTRAINT FK_Evento_EmpleadoBloqueo FOREIGN KEY(EmpleadoBloqueoId) REFERENCES dbo.Empleado(Id),
    CONSTRAINT FK_Evento_Clasificacion FOREIGN KEY(ClasificacionId) REFERENCES dbo.ClasificacionSismo(Id),
    CONSTRAINT FK_Evento_Origen FOREIGN KEY(OrigenId) REFERENCES dbo.OrigenGeneracion(Id),
    CONSTRAINT FK_Evento_Alcance FOREIGN KEY(AlcanceId) REFERENCES dbo.AlcanceSismo(Id)
);

-- Historial de cambios de estado (auditoría del flujo)
CREATE TABLE dbo.EventoEstadoHistorial(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    EventoId INT NOT NULL,
    EstadoId INT NOT NULL,
    EmpleadoId INT NULL,                -- quién realizó el cambio
    FechaHoraUtc DATETIME2(3) NOT NULL DEFAULT(SYSUTCDATETIME()),
    Motivo NVARCHAR(300) NULL,
    CONSTRAINT FK_Hist_Evento FOREIGN KEY(EventoId) REFERENCES dbo.EventoSismico(Id),
    CONSTRAINT FK_Hist_Estado FOREIGN KEY(EstadoId) REFERENCES dbo.Estado(Id),
    CONSTRAINT FK_Hist_Empleado FOREIGN KEY(EmpleadoId) REFERENCES dbo.Empleado(Id)
);

-- Series temporales (sismogramas)
CREATE TABLE dbo.SerieTemporal(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    EventoId INT NOT NULL,
    EstacionCodigo NVARCHAR(30) NOT NULL,     -- estación/red
    Canal NVARCHAR(10) NULL,                  -- BHZ, EHZ, etc.
    TipoDatoId INT NOT NULL,                  -- FK a TipoDatoSerie
    FrecuenciaMuestreoHz DECIMAL(10,3) NOT NULL,
    FechaInicioUtc DATETIME2(3) NOT NULL,
    MuestrasCount INT NOT NULL,
    -- Si preferís almacenar también binario crudo:
    DatosCrudos VARBINARY(MAX) NULL,

    CONSTRAINT FK_Serie_Evento FOREIGN KEY(EventoId) REFERENCES dbo.EventoSismico(Id),
    CONSTRAINT FK_Serie_TipoDato FOREIGN KEY(TipoDatoId) REFERENCES dbo.TipoDatoSerie(Id)
);

-- Muestras de la serie (opcional si no usás DatosCrudos)
CREATE TABLE dbo.Muestra(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    SerieId BIGINT NOT NULL,
    -- offset temporal respecto a FechaInicioUtc de la serie
    OffsetMs INT NOT NULL,
    Valor FLOAT NOT NULL,
    CONSTRAINT FK_Muestra_Serie FOREIGN KEY(SerieId) REFERENCES dbo.SerieTemporal(Id)
);

GO

/* =========================
   Índices
   ========================= */
CREATE INDEX IX_Evento_Fecha ON dbo.EventoSismico(FechaHoraUtc DESC);
CREATE INDEX IX_Evento_Estado ON dbo.EventoSismico(EstadoActualId);
CREATE INDEX IX_Hist_EventoFecha ON dbo.EventoEstadoHistorial(EventoId, FechaHoraUtc);
CREATE INDEX IX_Serie_Evento ON dbo.SerieTemporal(EventoId, EstacionCodigo);
CREATE INDEX IX_Muestra_SerieOffset ON dbo.Muestra(SerieId, OffsetMs);

GO

/* =========================
   Datos semilla
   ========================= */
INSERT INTO dbo.Estado(Nombre, EsFinal) VALUES
('No Revisado', 0),
('Bloqueado', 0),
('Revisado', 0),
('Publicado', 1),
('Anulado', 1);

INSERT INTO dbo.ClasificacionSismo(Nombre) VALUES
('Sismo'), ('Evento Antropogénico'), ('Ruido'), ('Microtemblor');

INSERT INTO dbo.OrigenGeneracion(Nombre) VALUES
('Natural'), ('Inducido'), ('Explosión'), ('Otro');

INSERT INTO dbo.AlcanceSismo(Nombre) VALUES
('Local'), ('Regional'), ('Lejano');

INSERT INTO dbo.TipoDatoSerie(Nombre, Unidad) VALUES
('Aceleración', 'm/s^2'), ('Velocidad','mm/s'), ('Desplazamiento','nm');

INSERT INTO dbo.Empleado(Legajo, Nombre, Email) VALUES
('E001','Operador A','operador.a@redsismica.local'),
('E002','Operador B','operador.b@redsismica.local');

-- Evento de ejemplo
DECLARE @idEstadoNoRev INT = (SELECT Id FROM dbo.Estado WHERE Nombre='No Revisado');
INSERT INTO dbo.EventoSismico(Codigo, FechaHoraUtc, Latitud, Longitud, ProfundidadKm, AutoDetectado, EstadoActualId)
VALUES ('EVT-0001', SYSUTCDATETIME(), -31.424, -64.183, 12.5, 1, @idEstadoNoRev);

DECLARE @idEvento INT = SCOPE_IDENTITY();
INSERT INTO dbo.EventoEstadoHistorial(EventoId, EstadoId, EmpleadoId, Motivo) 
VALUES(@idEvento, @idEstadoNoRev, NULL, 'Creación por autodetección');

-- Serie mínima + 3 muestras
DECLARE @tipoVel INT = (SELECT Id FROM dbo.TipoDatoSerie WHERE Nombre='Velocidad');
INSERT INTO dbo.SerieTemporal(EventoId, EstacionCodigo, Canal, TipoDatoId, FrecuenciaMuestreoHz, FechaInicioUtc, MuestrasCount)
VALUES(@idEvento, 'CBA01','BHZ', @tipoVel, 100.0, DATEADD(SECOND,-10,SYSUTCDATETIME()), 3);

DECLARE @serie BIGINT = SCOPE_IDENTITY();
INSERT INTO dbo.Muestra(SerieId, OffsetMs, Valor) VALUES
(@serie, 0, 0.12),
(@serie, 10, 0.08),
(@serie, 20, 0.05);
