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

-- =====================================================
-- categories
-- =====================================================
-- Reglas: un usuario solo puede ver/crear/editar/borrar categorías
-- de cuentas en las que es miembro (vía account_members).
-- Igual que en accounts, la USING de SELECT se aplica también durante
-- INSERT ... RETURNING (Prefer: return=representation del cliente .NET),
-- así que aquí basta con la membresía: la categoría se crea para una
-- cuenta donde el usuario YA es miembro, no hay chicken-and-egg.

-- INSERT
DROP POLICY IF EXISTS "Users can create categories in their accounts" ON public.categories;
CREATE POLICY "Users can create categories in their accounts"
ON public.categories
FOR INSERT
TO authenticated
WITH CHECK (
    account_id IN (
        SELECT account_id FROM public.account_members
        WHERE user_id = auth.uid()
    )
);

-- SELECT
DROP POLICY IF EXISTS "Users can view categories of their accounts" ON public.categories;
CREATE POLICY "Users can view categories of their accounts"
ON public.categories
FOR SELECT
TO authenticated
USING (
    account_id IN (
        SELECT account_id FROM public.account_members
        WHERE user_id = auth.uid()
    )
);

-- UPDATE
DROP POLICY IF EXISTS "Users can update categories of their accounts" ON public.categories;
CREATE POLICY "Users can update categories of their accounts"
ON public.categories
FOR UPDATE
TO authenticated
USING (
    account_id IN (
        SELECT account_id FROM public.account_members
        WHERE user_id = auth.uid()
    )
)
WITH CHECK (
    account_id IN (
        SELECT account_id FROM public.account_members
        WHERE user_id = auth.uid()
    )
);

-- DELETE
DROP POLICY IF EXISTS "Users can delete categories of their accounts" ON public.categories;
CREATE POLICY "Users can delete categories of their accounts"
ON public.categories
FOR DELETE
TO authenticated
USING (
    account_id IN (
        SELECT account_id FROM public.account_members
        WHERE user_id = auth.uid()
    )
);

-- =====================================================
-- expenses
-- =====================================================
-- Reglas: ver/crear/editar/borrar gastos solo en cuentas donde soy miembro.
-- Además, en INSERT exigimos que `paid_by = auth.uid()` para que un usuario
-- no pueda marcar a otro miembro como pagador. Cuando entre Marta y queramos
-- soportar "registrar un gasto pagado por el otro" habrá que relajar esto.

-- INSERT
DROP POLICY IF EXISTS "Users can create expenses in their accounts" ON public.expenses;
CREATE POLICY "Users can create expenses in their accounts"
ON public.expenses
FOR INSERT
TO authenticated
WITH CHECK (
    paid_by = auth.uid()
    AND account_id IN (
        SELECT account_id FROM public.account_members
        WHERE user_id = auth.uid()
    )
);

-- SELECT
DROP POLICY IF EXISTS "Users can view expenses of their accounts" ON public.expenses;
CREATE POLICY "Users can view expenses of their accounts"
ON public.expenses
FOR SELECT
TO authenticated
USING (
    account_id IN (
        SELECT account_id FROM public.account_members
        WHERE user_id = auth.uid()
    )
);

-- UPDATE
DROP POLICY IF EXISTS "Users can update expenses of their accounts" ON public.expenses;
CREATE POLICY "Users can update expenses of their accounts"
ON public.expenses
FOR UPDATE
TO authenticated
USING (
    account_id IN (
        SELECT account_id FROM public.account_members
        WHERE user_id = auth.uid()
    )
)
WITH CHECK (
    account_id IN (
        SELECT account_id FROM public.account_members
        WHERE user_id = auth.uid()
    )
);

-- DELETE
DROP POLICY IF EXISTS "Users can delete expenses of their accounts" ON public.expenses;
CREATE POLICY "Users can delete expenses of their accounts"
ON public.expenses
FOR DELETE
TO authenticated
USING (
    account_id IN (
        SELECT account_id FROM public.account_members
        WHERE user_id = auth.uid()
    )
);
