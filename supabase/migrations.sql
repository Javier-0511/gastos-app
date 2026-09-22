-- Migraciones de schema de la app gastos-app.
-- Ejecutar en el SQL Editor de Supabase en orden cronológico.
-- Cada bloque es idempotente (re-ejecutable sin romper).

-- =====================================================
-- 2026-05-24 — Saldos y nómina en monthly_budgets
-- =====================================================
-- Añadimos columnas para modelar:
-- - opening_balance: dinero al inicio del mes en esa cuenta (manual primera
--   vez; después se sugiere a partir del saldo final del mes anterior).
-- - income: nómina del mes (solo aplica a cuentas personales; en compartida
--   el "income" se deriva de la previsión = aporte conjunto).
--
-- La columna `amount` existente sigue siendo la previsión. En la cuenta
-- compartida, previsión = aporte conjunto al banco (entre los dos miembros).

ALTER TABLE public.monthly_budgets
    ADD COLUMN IF NOT EXISTS opening_balance NUMERIC,
    ADD COLUMN IF NOT EXISTS income NUMERIC;

-- =====================================================
-- 2026-05-25 — Bloques para la cuenta personal
-- =====================================================
-- Hasta ahora la personal solo tenía el bloque 'individual'. Añadimos
-- 'ocio' e 'inversion' para poder estructurarla (Fijos / Ocio / Variable /
-- Inversiones; 'fijo' y 'variable' ya existían de la compartida).
--
-- Mantenemos 'individual' en el CHECK para NO invalidar las categorías
-- personales ya existentes. Quedará como legacy hasta que se reasignen.

ALTER TABLE public.categories DROP CONSTRAINT IF EXISTS categories_block_check;
ALTER TABLE public.categories
    ADD CONSTRAINT categories_block_check
    CHECK (block IN ('fijo', 'comida', 'variable', 'minicompra', 'individual', 'ocio', 'inversion'));

-- =====================================================
-- 2026-05-25 — Proteger gastos al borrar categorías
-- =====================================================
-- La FK expenses.category_id estaba como ON DELETE SET NULL: al borrar una
-- categoría, sus gastos quedaban huérfanos (category_id = null). Lo
-- cambiamos a ON DELETE RESTRICT: la BBDD impide borrar una categoría que
-- tenga gastos. Hay que reasignarlos o borrarlos primero.
--
-- OJO: si ya hay gastos con category_id null (de borrados anteriores),
-- arréglalos antes (reasignar o borrar) — ver nota más abajo.

ALTER TABLE public.expenses DROP CONSTRAINT IF EXISTS expenses_category_id_fkey;
ALTER TABLE public.expenses
    ADD CONSTRAINT expenses_category_id_fkey
    FOREIGN KEY (category_id) REFERENCES public.categories(id) ON DELETE RESTRICT;

-- Para localizar gastos huérfanos (category_id null) ya existentes:
--   SELECT * FROM public.expenses WHERE category_id IS NULL;
-- Puedes borrarlos:   DELETE FROM public.expenses WHERE category_id IS NULL;
-- o editarlos desde la app para asignarles una categoría nueva.

-- =====================================================
-- 2026-09-22 — Adiós al bloque 'minicompra'
-- =====================================================
-- Reorganización de bloques para que las categorías caigan donde tocan:
--
--   Compartida: el bloque 'minicompra' desaparece. Sus dos categorías
--   (Chino, Otros) pasan a 'variable', que queda con: Ocio, Restaurante,
--   Viaje, Chino, Otros.
--
--   Personal: el bloque 'fijo' contenía cosas que no son fijas. Moto y
--   Peluquería pasan a 'variable'; 'fijo' se queda con Gasolina, Padel y
--   Suscripciones. La categoría "Clases padel" se fusiona en "Padel"
--   (sus gastos se reasignan antes de borrarla, porque la FK está en
--   ON DELETE RESTRICT y si no la BBDD rechaza el DELETE).
--
-- Los movimientos de datos ya se ejecutaron el 2026-09-22. Se dejan aquí
-- documentados por si hubiera que rehacerlos en otro entorno:
--
--   UPDATE public.categories SET block = 'variable'
--    WHERE id IN ('7016a0bb-3cba-439d-abd3-41567ed0f520',   -- Chino
--                 '39c373e9-9dfe-4753-a2d4-261914e12762');  -- Otros
--
--   UPDATE public.expenses
--      SET category_id = 'f9c90c82-82e8-4336-b94a-f658898d5868'    -- Padel
--    WHERE category_id = '82c72aa3-3171-432f-ad2e-be68b2d95762';   -- Clases padel
--   DELETE FROM public.categories
--    WHERE id = '82c72aa3-3171-432f-ad2e-be68b2d95762';
--
--   UPDATE public.categories SET block = 'variable'
--    WHERE id IN ('4f6903d5-323b-456e-a90c-1d52b5266215',   -- Moto
--                 '7f09866f-79c7-4c58-80c4-f30b549ca468');  -- Peluquería

-- Cerramos la puerta: 'minicompra' deja de ser un bloque válido. De paso
-- retiramos 'individual', el bloque legacy de la personal que quedó de la
-- migración de 2026-05-25: ya no lo usa ninguna categoría (comprobado).
--
-- Si quedara alguna categoría en 'minicompra' o 'individual', este ALTER
-- falla — que es justo lo que queremos (avisa en vez de dejar datos
-- inconsistentes). Para comprobarlo antes:
--   SELECT name, block FROM public.categories
--    WHERE block IN ('minicompra', 'individual');
ALTER TABLE public.categories DROP CONSTRAINT IF EXISTS categories_block_check;
ALTER TABLE public.categories
    ADD CONSTRAINT categories_block_check
    CHECK (block IN ('fijo', 'comida', 'variable', 'ocio', 'inversion'));
