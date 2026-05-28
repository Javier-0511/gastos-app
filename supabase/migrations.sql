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
