-- Políticas RLS de la app gastos-app
-- Ejecutar en el SQL Editor de Supabase tras crear las tablas.
-- Re-ejecutable de forma segura: cada policy se DROP+CREATE.

-- =====================================================
-- accounts
-- =====================================================

-- INSERT: un usuario autenticado solo puede crear cuentas a su nombre.
DROP POLICY IF EXISTS "Users can create their own accounts" ON public.accounts;
CREATE POLICY "Users can create their own accounts"
ON public.accounts
FOR INSERT
TO authenticated
WITH CHECK (owner_id = auth.uid());

-- SELECT: un usuario ve una cuenta si es el owner o si está en account_members.
-- ¡Ojo! El `owner_id = auth.uid()` es necesario aunque luego se cree el member,
-- porque PostgreSQL aplica esta USING como check adicional durante un INSERT ... RETURNING
-- y el cliente Supabase envía `Prefer: return=representation` por defecto. Sin esta
-- cláusula, el primer INSERT en accounts falla con 42501 antes de poder crear el member.
DROP POLICY IF EXISTS "Users can view their accounts" ON public.accounts;
CREATE POLICY "Users can view their accounts"
ON public.accounts
FOR SELECT
TO authenticated
USING (
    owner_id = auth.uid()
    OR id IN (
        SELECT account_id FROM public.account_members
        WHERE user_id = auth.uid()
    )
);

-- =====================================================
-- account_members
-- =====================================================

-- INSERT: un usuario solo puede añadirse a sí mismo como miembro.
DROP POLICY IF EXISTS "Users can add themselves as members" ON public.account_members;
CREATE POLICY "Users can add themselves as members"
ON public.account_members
FOR INSERT
TO authenticated
WITH CHECK (user_id = auth.uid());

-- SELECT: un usuario ve sus propias membresías.
DROP POLICY IF EXISTS "Users can view memberships of their accounts" ON public.account_members;
CREATE POLICY "Users can view memberships of their accounts"
ON public.account_members
FOR SELECT
TO authenticated
USING (user_id = auth.uid());
