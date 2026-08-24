-- =====================================================================
-- Sistema de Gestión de Licitaciones — Modelo de Datos PostgreSQL
-- Motor: PostgreSQL 16+
-- Convenciones:
--   * PKs en UUID (gen_random_uuid) salvo catálogos pequeños (smallserial).
--   * Montos monetarios: numeric(18,2). Tipo de cambio: numeric(18,6).
--   * Fechas: timestamptz (almacenadas/comparadas en UTC; la presentación
--     en la aplicación usa America/Costa_Rica).
--   * Borrado lógico: columna deleted_at (NULL = activo).
--   * Concurrencia optimista: columna version (incrementada por la app/EF Core).
--   * created_at / updated_at gestionados por trigger.
-- =====================================================================

BEGIN;

-- ---------------------------------------------------------------------
-- 0. Extensiones requeridas
-- ---------------------------------------------------------------------
CREATE EXTENSION IF NOT EXISTS pgcrypto;   -- gen_random_uuid()
CREATE EXTENSION IF NOT EXISTS citext;     -- comparaciones case-insensitive
CREATE EXTENSION IF NOT EXISTS btree_gist; -- EXCLUDE USING gist con igualdad/rango

-- ---------------------------------------------------------------------
-- 0.1 Función utilitaria: actualizar updated_at automáticamente
-- ---------------------------------------------------------------------
CREATE OR REPLACE FUNCTION fn_set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at := now();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- =====================================================================
-- 1. CATÁLOGO: estados_licitacion
--    Tabla parametrizable en lugar de un enum embebido en código,
--    facilita auditoría y evita cadenas if/else fijas.
-- =====================================================================
CREATE TABLE estados_licitacion (
    id          smallint      PRIMARY KEY,
    nombre      varchar(30)   NOT NULL,
    descripcion varchar(200),
    CONSTRAINT ux_estados_licitacion_nombre UNIQUE (nombre)
);

COMMENT ON TABLE estados_licitacion IS
  'Catálogo de estados del ciclo de vida de una licitación.';

-- =====================================================================
-- 2. proveedores
-- =====================================================================
CREATE TABLE proveedores (
    id                  uuid            PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre              varchar(200)    NOT NULL,
    nombre_normalizado  varchar(200)    NOT NULL,
    activo              boolean         NOT NULL DEFAULT true,
    created_at          timestamptz     NOT NULL DEFAULT now(),
    updated_at          timestamptz     NOT NULL DEFAULT now(),
    deleted_at          timestamptz,
    version             integer         NOT NULL DEFAULT 1,
    CONSTRAINT ck_proveedores_nombre_no_vacio
        CHECK (btrim(nombre) <> ''),
    -- Caracteres permitidos: letras (incl. acentos/ñ), números, espacios, . , ( )
    CONSTRAINT ck_proveedores_nombre_caracteres
        CHECK (nombre ~ '^[A-Za-zÁÉÍÓÚÑáéíóúñÜü0-9 .,()]+$')
);

COMMENT ON COLUMN proveedores.nombre_normalizado IS
  'nombre tras Trim + colapso de espacios + normalización Unicode (NFKC) + mayúsculas, usado solo para validar unicidad.';

-- Unicidad de proveedor activo por nombre normalizado
CREATE UNIQUE INDEX ux_proveedores_nombre_normalizado
    ON proveedores (nombre_normalizado)
    WHERE deleted_at IS NULL;

CREATE TRIGGER trg_proveedores_updated_at
    BEFORE UPDATE ON proveedores
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- =====================================================================
-- 3. licitaciones
-- =====================================================================
CREATE TABLE licitaciones (
    id                  uuid            PRIMARY KEY DEFAULT gen_random_uuid(),
    codigo              varchar(50)     NOT NULL,
    codigo_normalizado  varchar(50)     NOT NULL,
    titulo              varchar(200)    NOT NULL,
    descripcion         text,
    presupuesto         numeric(18,2)   NOT NULL,
    fecha_cierre        timestamptz     NOT NULL,
    estado_id           smallint        NOT NULL REFERENCES estados_licitacion(id),
    created_at          timestamptz     NOT NULL DEFAULT now(),
    updated_at          timestamptz     NOT NULL DEFAULT now(),
    deleted_at          timestamptz,
    version             integer         NOT NULL DEFAULT 1,
    CONSTRAINT ck_licitaciones_presupuesto_positivo
        CHECK (presupuesto > 0),
    CONSTRAINT ck_licitaciones_codigo_no_vacio
        CHECK (btrim(codigo) <> '')
);

COMMENT ON COLUMN licitaciones.codigo_normalizado IS
  'codigo tras Trim + mayúsculas, usado solo para validar unicidad case-insensitive.';
COMMENT ON COLUMN licitaciones.fecha_cierre IS
  'Fecha/hora de cierre almacenada en UTC (timestamptz); comparaciones internas siempre en UTC.';

CREATE UNIQUE INDEX ux_licitaciones_codigo_normalizado
    ON licitaciones (codigo_normalizado)
    WHERE deleted_at IS NULL;

CREATE INDEX ix_licitaciones_estado ON licitaciones (estado_id);
CREATE INDEX ix_licitaciones_fecha_cierre ON licitaciones (fecha_cierre);

CREATE TRIGGER trg_licitaciones_updated_at
    BEFORE UPDATE ON licitaciones
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- ---------------------------------------------------------------------
-- 3.1 licitacion_transiciones
--     Evidencia auditable de cada cambio de estado (histórico inmutable).
-- ---------------------------------------------------------------------
CREATE TABLE licitacion_transiciones (
    id                  bigserial       PRIMARY KEY,
    licitacion_id       uuid            NOT NULL REFERENCES licitaciones(id) ON DELETE CASCADE,
    estado_anterior_id  smallint        REFERENCES estados_licitacion(id),
    estado_nuevo_id     smallint        NOT NULL REFERENCES estados_licitacion(id),
    fecha               timestamptz     NOT NULL DEFAULT now(),
    comentario          varchar(300)
);

CREATE INDEX ix_licitacion_transiciones_licitacion
    ON licitacion_transiciones (licitacion_id, fecha DESC);

-- =====================================================================
-- 4. ofertas
-- =====================================================================
CREATE TABLE ofertas (
    id                  uuid            PRIMARY KEY DEFAULT gen_random_uuid(),
    licitacion_id       uuid            NOT NULL REFERENCES licitaciones(id),
    proveedor_id        uuid            NOT NULL REFERENCES proveedores(id),
    monto               numeric(18,2)   NOT NULL,
    fecha_registro      timestamptz     NOT NULL DEFAULT now(),
    rechazada           boolean         NOT NULL DEFAULT false,
    motivo_rechazo      varchar(200),
    created_at          timestamptz     NOT NULL DEFAULT now(),
    updated_at          timestamptz     NOT NULL DEFAULT now(),
    deleted_at          timestamptz,
    version             integer         NOT NULL DEFAULT 1,
    CONSTRAINT ck_ofertas_monto_positivo
        CHECK (monto > 0)
);

COMMENT ON TABLE ofertas IS
  'Las ofertas cerradas (de licitaciones cerradas) se conservan como evidencia y no deben alterarse desde la capa de aplicación.';

-- Un proveedor no puede tener más de una oferta activa por licitación
CREATE UNIQUE INDEX ux_ofertas_licitacion_proveedor
    ON ofertas (licitacion_id, proveedor_id)
    WHERE deleted_at IS NULL AND rechazada = false;

CREATE INDEX ix_ofertas_licitacion ON ofertas (licitacion_id);
CREATE INDEX ix_ofertas_proveedor ON ofertas (proveedor_id);

CREATE TRIGGER trg_ofertas_updated_at
    BEFORE UPDATE ON ofertas
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- =====================================================================
-- 5. niveles_aprobacion
--    Tabla parametrizable: el aprobador se resuelve por consulta,
--    nunca por condicionales fijos en código.
-- =====================================================================
CREATE TABLE niveles_aprobacion (
    id              smallserial     PRIMARY KEY,
    nombre          varchar(100)    NOT NULL,
    monto_minimo    numeric(18,2)   NOT NULL,
    monto_maximo    numeric(18,2),  -- NULL = rango abierto (sin máximo)
    aprobador       varchar(150)    NOT NULL,
    orden           smallint        NOT NULL,
    activo          boolean         NOT NULL DEFAULT true,
    created_at      timestamptz     NOT NULL DEFAULT now(),
    updated_at      timestamptz     NOT NULL DEFAULT now(),
    CONSTRAINT ck_niveles_aprobacion_minimo_no_negativo
        CHECK (monto_minimo >= 0),
    CONSTRAINT ck_niveles_aprobacion_maximo_mayor_minimo
        CHECK (monto_maximo IS NULL OR monto_maximo > monto_minimo)
);

-- Evita rangos traslapados entre niveles activos (requiere btree_gist).
-- Se usa 'infinity' cuando el máximo es NULL (rango abierto).
ALTER TABLE niveles_aprobacion
    ADD CONSTRAINT ex_niveles_aprobacion_rango_sin_traslape
    EXCLUDE USING gist (
        numrange(monto_minimo, COALESCE(monto_maximo, 'infinity'::numeric), '[]') WITH &&
    )
    WHERE (activo);

-- Solo puede existir un rango abierto (monto_maximo NULL) activo a la vez
CREATE UNIQUE INDEX ux_niveles_aprobacion_unico_rango_abierto
    ON niveles_aprobacion ((monto_maximo IS NULL))
    WHERE activo AND monto_maximo IS NULL;

CREATE TRIGGER trg_niveles_aprobacion_updated_at
    BEFORE UPDATE ON niveles_aprobacion
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- =====================================================================
-- 6. tipos_cambio
--    Solo un tipo de cambio activo a la vez (fuente de verdad para
--    la conversión referencial CRC -> USD, calculada solo en presentación).
-- =====================================================================
CREATE TABLE tipos_cambio (
    id              uuid            PRIMARY KEY DEFAULT gen_random_uuid(),
    valor           numeric(18,6)   NOT NULL,
    fecha_vigencia  timestamptz     NOT NULL DEFAULT now(),
    activo          boolean         NOT NULL DEFAULT false,
    fuente          varchar(100),
    created_at      timestamptz     NOT NULL DEFAULT now(),
    updated_at      timestamptz     NOT NULL DEFAULT now(),
    CONSTRAINT ck_tipos_cambio_valor_positivo
        CHECK (valor > 0)
);

COMMENT ON TABLE tipos_cambio IS
  'Valores oficiales de licitaciones/ofertas permanecen en CRC; este tipo de cambio solo se usa para representación en USD.';

-- Garantiza que solo exista un tipo de cambio activo en toda la tabla
CREATE UNIQUE INDEX ux_tipos_cambio_unico_activo
    ON tipos_cambio ((activo))
    WHERE activo = true;

CREATE TRIGGER trg_tipos_cambio_updated_at
    BEFORE UPDATE ON tipos_cambio
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- =====================================================================
-- 7. DATOS SEMILLA (seed data)
-- =====================================================================

-- 7.1 Estados de licitación
INSERT INTO estados_licitacion (id, nombre, descripcion) VALUES
    (1, 'Borrador',   'Licitación creada, aún no visible para proveedores'),
    (2, 'Publicada',  'Licitación publicada y habilitada para recibir ofertas'),
    (3, 'Cerrada',    'Licitación cerrada, ya no admite nuevas ofertas'),
    (4, 'Adjudicada', 'Licitación adjudicada a la mejor oferta'),
    (5, 'Cancelada',  'Licitación cancelada por la administración');

-- 7.2 Niveles de aprobación iniciales (rangos en CRC, sin traslape)
INSERT INTO niveles_aprobacion (nombre, monto_minimo, monto_maximo, aprobador, orden, activo) VALUES
    ('Nivel 1 - Jefatura',      0,           5000000,   'Jefatura de Área',            1, true),
    ('Nivel 2 - Gerencia',      5000000,     25000000,  'Gerencia General',            2, true),
    ('Nivel 3 - Junta Directiva', 25000000,  NULL,      'Junta Directiva',             3, true);

-- 7.3 Tipo de cambio inicial (referencial, administrable localmente)
INSERT INTO tipos_cambio (valor, fecha_vigencia, activo, fuente) VALUES
    (520.00, now(), true, 'Valor semilla administrado localmente');

COMMIT;

-- =====================================================================
-- 8. NOTAS DE USO PARA LA CAPA DE APLICACIÓN (EF Core / servicios)
-- =====================================================================
-- * Normalización de nombre de proveedor antes de insertar/comparar:
--     TRIM -> colapsar espacios repetidos -> NORMALIZE(NFKC) -> UPPER
--   y guardar el resultado en nombre_normalizado.
-- * Normalización de código de licitación: TRIM -> UPPER -> codigo_normalizado.
-- * "Cierre funcional" (licitación con fecha_cierre alcanzada pero estado_id
--   aún en 2/Publicada) se resuelve en la capa de aplicación comparando
--   fecha_cierre contra IClock.UtcNow(); no se materializa aquí para evitar
--   jobs obligatorios, aunque puede añadirse un job/trigger opcional que
--   transicione a estado 3/Cerrada de forma asíncrona.
-- * Resolución de aprobador: SELECT ... FROM niveles_aprobacion
--     WHERE activo AND :monto >= monto_minimo
--       AND (monto_maximo IS NULL OR :monto <= monto_maximo)
--     ORDER BY orden LIMIT 1;
-- * Concurrencia optimista: mapear la columna "version" como concurrency
--   token en EF Core (Property(x => x.Version).IsConcurrencyToken()),
--   incrementándola en cada UPDATE desde la aplicación.
-- =====================================================================
