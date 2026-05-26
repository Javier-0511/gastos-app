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
